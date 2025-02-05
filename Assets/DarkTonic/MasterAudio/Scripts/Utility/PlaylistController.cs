using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_5_0
using UnityEngine.Audio;
#endif

/// <summary>
/// This class is used to host and play Playlists. Contains cross-fading, ducking and more!
/// </summary>
[RequireComponent(typeof(AudioSource))]
// ReSharper disable once CheckNamespace
public class PlaylistController : MonoBehaviour {
    private const string NotReadyMessage =
        "Playlist Controller is not initialized yet. It must call its own Awake & Start method before any other methods are called. If you have a script with an Awake or Start event that needs to call it, make sure PlaylistController.cs is set to execute first (Script Execution Order window in Unity).";

    // ReSharper disable InconsistentNaming
    public bool startPlaylistOnAwake = true;
    public bool isShuffle = false;
    public bool isAutoAdvance = true;
    public bool loopPlaylist = true;
    public float _playlistVolume = 1f;
    public bool isMuted;
    public string startPlaylistName = string.Empty;
    public int syncGroupNum = -1;

#if UNITY_5_0
    public AudioMixerGroup mixerChannel;
    public MasterAudio.ItemSpatialBlendType spatialBlendType = MasterAudio.ItemSpatialBlendType.ForceTo2D;
    public float spatialBlend = MasterAudio.SpatialBlend_2DValue;
#endif

    public bool songChangedEventExpanded = false;
    public string songChangedCustomEvent = string.Empty;
    public bool songEndedEventExpanded = false;
    public string songEndedCustomEvent = string.Empty;
    // ReSharper restore InconsistentNaming

    private AudioSource _activeAudio;
    private AudioSource _transitioningAudio;
    private float _activeAudioEndVolume;
    private float _transitioningAudioStartVolume;
    private float _crossFadeStartTime;
    private readonly List<int> _clipsRemaining = new List<int>(10);
    private int _currentSequentialClipIndex;
    private AudioDuckingMode _duckingMode;
    private float _timeToStartUnducking;
    private float _timeToFinishUnducking;
    private float _originalMusicVolume;
    private float _initialDuckVolume;
    private float _duckRange;
    private MusicSetting _currentSong;
    private GameObject _go;
    private string _name;
    private FadeMode _curFadeMode = FadeMode.None;
    private float _slowFadeTargetVolume;
    private float _slowFadeVolStep;
    private MasterAudio.Playlist _currentPlaylist;
    private float _lastTimeMissingPlaylistLogged = -5f;
    private Action _fadeCompleteCallback;
    private readonly List<MusicSetting> _queuedSongs = new List<MusicSetting>(5);
    private bool _lostFocus;

    private AudioSource _audioClip;
    private AudioSource _transClip;
    private MusicSetting _newSongSetting;
    private bool _nextSongRequested;
    private bool _nextSongScheduled;
    private int _lastRandomClipIndex = -1;
    private readonly Dictionary<AudioSource, double> _scheduledSongsByAudioSource = new Dictionary<AudioSource, double>(2);

    public delegate void SongChangedEventHandler(string newSongName);
    public delegate void SongEndedEventHandler(string songName);

    /// <summary>
    /// This event will notify you when the Playlist song changes.
    /// </summary>
    public event SongChangedEventHandler SongChanged;

    /// <summary>
    /// This event will notify you when the Playlist song ends.
    /// </summary>
    public event SongEndedEventHandler SongEnded;

    private static List<PlaylistController> _instances;
    private static int _songsPlayedFromPlaylist;
    private AudioSource _audio1;
    private AudioSource _audio2;

    private Transform _trans;

    public enum AudioPlayType {
        PlayNow,
        Schedule,
        AlreadyScheduled
    }

    public enum PlaylistStates {
        NotInScene,
        Stopped,
        Playing,
        Paused,
        Crossfading
    }

    public enum FadeMode {
        None,
        GradualFade
    }

    public enum AudioDuckingMode {
        NotDucking,
        SetToDuck,
        Ducked
    }

    #region Monobehavior events
    // ReSharper disable once UnusedMember.Local
    void Awake() {
        ControllerIsReady = false;

        // check for "extra" Playlist Controllers of the same name.
        // ReSharper disable once ArrangeStaticMemberQualifier
        var controllers = (PlaylistController[])GameObject.FindObjectsOfType(typeof(PlaylistController));
        var sameNameCount = 0;

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < controllers.Length; i++) {
            if (controllers[i].ControllerName == ControllerName) {
                sameNameCount++;
            }
        }

        if (sameNameCount > 1) {
            Destroy(gameObject);
            Debug.Log("More than one Playlist Controller prefab exists in this Scene with the same name. Destroying the one called '" + ControllerName + "'. You may wish to set up a Bootstrapper Scene so this does not occur.");
            return;
        }
        // end check

        useGUILayout = false;
        _duckingMode = AudioDuckingMode.NotDucking;
        _currentSong = null;
        _songsPlayedFromPlaylist = 0;

        var audios = GetComponents<AudioSource>();
        if (audios.Length < 2) {
            Debug.LogError("This prefab should have exactly two Audio Source components. Please revert it.");
            return;
        }

        _audio1 = audios[0];
        _audio2 = audios[1];

        _audio1.clip = null;
        _audio2.clip = null;

        if (_audio1.playOnAwake || _audio2.playOnAwake) {
            Debug.LogWarning("One or more 'Play on Awake' checkboxes in the Audio Sources on Playlist Controller '" + name + "' are checked. These are not used in Master Audio. Make sure to uncheck them before hitting Play next time. For Playlist Controllers, use the similarly named checkbox 'Start Playlist on Awake' in the Playlist Controller's Inspector.");
        }

        _activeAudio = _audio1;
        _transitioningAudio = _audio2;

#if UNITY_5_0
        _audio1.outputAudioMixerGroup = mixerChannel;
        _audio2.outputAudioMixerGroup = mixerChannel;

        SetSpatialBlend();
#endif

        _curFadeMode = FadeMode.None;
        _fadeCompleteCallback = null;
        _lostFocus = false;
    }

#if UNITY_5_0
    public void SetSpatialBlend() {
        switch (MasterAudio.Instance.musicSpatialBlendType) {
            case MasterAudio.AllMusicSpatialBlendType.ForceAllTo2D:
                SetAudioSpatialBlend(MasterAudio.SpatialBlend_2DValue);
                break;
            case MasterAudio.AllMusicSpatialBlendType.ForceAllTo3D:
                SetAudioSpatialBlend(MasterAudio.SpatialBlend_3DValue);
                break;
            case MasterAudio.AllMusicSpatialBlendType.ForceAllToCustom:
                SetAudioSpatialBlend(MasterAudio.Instance.musicSpatialBlend);
                break;
            case MasterAudio.AllMusicSpatialBlendType.AllowDifferentPerController:
                switch (spatialBlendType) {
                    case MasterAudio.ItemSpatialBlendType.ForceTo2D:
                        SetAudioSpatialBlend(MasterAudio.SpatialBlend_2DValue);
                        break;
                    case MasterAudio.ItemSpatialBlendType.ForceTo3D:
                        SetAudioSpatialBlend(MasterAudio.SpatialBlend_3DValue);
                        break;
                    case MasterAudio.ItemSpatialBlendType.ForceToCustom:
                        SetAudioSpatialBlend(spatialBlend);
                        break;
                }

                break;
        }
    }

    private void SetAudioSpatialBlend(float blend) {
        _audio1.spatialBlend = blend;
        _audio2.spatialBlend = blend;
    }
#endif

    // Use this for initialization 
    // ReSharper disable once UnusedMember.Local
    void Start() {
        if (IsMuted) {
            MutePlaylist();
        }

        if (!string.IsNullOrEmpty(startPlaylistName) && _currentPlaylist == null) {
            // fill up randomizer
            InitializePlaylist();
        }

        if (_currentPlaylist != null && startPlaylistOnAwake) {
            PlayNextOrRandom(AudioPlayType.PlayNow);
        }

        StartCoroutine(CoUpdate());

        ControllerIsReady = true;
    }

    IEnumerator CoUpdate() {
        while (true) {
            if (MasterAudio.IgnoreTimeScale) {
                yield return StartCoroutine(CoroutineHelper.WaitForActualSeconds(MasterAudio.InnerLoopCheckInterval));
            } else {
                yield return MasterAudio.InnerLoopDelay;
            }

            // fix scheduled track play time if it has changed (it changes constantly).
            if (CanSchedule) {
                if (_scheduledSongsByAudioSource.Count > 0 && _scheduledSongsByAudioSource.ContainsKey(_audioClip)) {
                    var newStartTime = CalculateNextTrackStartTime();
                    var existingStartTime = _scheduledSongsByAudioSource[_audioClip];

                    if (newStartTime != existingStartTime) {
                        _audioClip.Stop(); // stop the previous scheduled play
                        ScheduleClipPlay(newStartTime);
                    }
                }
            }

            // gradual fade code
            if (_curFadeMode != FadeMode.GradualFade) {
                continue;
            }

            if (_activeAudio == null) {
                continue; // paused or error in setup
            }

            var newVolume = _playlistVolume + _slowFadeVolStep;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (_slowFadeVolStep > 0f) {
                newVolume = Math.Min(newVolume, _slowFadeTargetVolume);
            } else {
                newVolume = Math.Max(newVolume, _slowFadeTargetVolume);
            }

            _playlistVolume = newVolume;

            UpdateMasterVolume();

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (newVolume != _slowFadeTargetVolume) {
                continue;
            }
            if (MasterAudio.Instance.stopZeroVolumePlaylists && _slowFadeTargetVolume == 0f) {
                StopPlaylist();
            }

            if (_fadeCompleteCallback != null) {
                _fadeCompleteCallback();
                _fadeCompleteCallback = null;
            }
            _curFadeMode = FadeMode.None;
        }
        // ReSharper disable once FunctionNeverReturns
    }

    // ReSharper disable once UnusedMember.Local
    void OnEnable() {
        _instances = null; // in case you have a new Controller in the next Scene, we need to uncache the list.
    }

    // ReSharper disable once UnusedMember.Local
    void OnDisable() {
        _instances = null; // in case you have a new Controller in the next Scene, we need to uncache the list.
    }

    // ReSharper disable once UnusedMember.Local
    void OnApplicationPause(bool pauseStatus) {
        _lostFocus = pauseStatus;
    }

    // ReSharper disable once UnusedMember.Local
    void Update() {
        if (_lostFocus) {
            return; // don't accidentally stop the song below if we just lost focus.
        }

        if (IsCrossFading) {
            // cross-fade code
            if (_activeAudio.volume >= _activeAudioEndVolume) {
                CeaseAudioSource(_transitioningAudio);
                IsCrossFading = false;
                if (CanSchedule && !_nextSongScheduled) { // this needs to run if using crossfading > 0 seconds, because it will not schedule during cross fading (it would kill the crossfade).
                    PlayNextOrRandom(AudioPlayType.Schedule);
                }
                SetDuckProperties(); // they now should read from a new audio source
            }

            var workingCrossFade = Math.Max(CrossFadeTime, .001f);
            var ratioPassed = (Time.realtimeSinceStartup - _crossFadeStartTime) / workingCrossFade;

            _activeAudio.volume = ratioPassed * _activeAudioEndVolume;
            _transitioningAudio.volume = _transitioningAudioStartVolume * (1 - ratioPassed);
            // end cross-fading code
        }

        if (!_activeAudio.loop && _activeAudio.clip != null) {
            if (!IsAutoAdvance && !_activeAudio.isPlaying) {
                CeaseAudioSource(_activeAudio); // this will release the resources if not auto-advance
                return;
            }

            if (AudioUtil.IsAudioPaused(_activeAudio)) {
                // do not auto-advance if the audio is paused.
                goto AfterAutoAdvance;
            }

            bool shouldAdvance;

            if (!_activeAudio.isPlaying) {
                shouldAdvance = true; // this will advance even if the code below didn't and the clip stopped due to excessive lag.
            } else {
                var currentClipTime = _activeAudio.clip.length - _activeAudio.time - (CrossFadeTime * _activeAudio.pitch);
                var clipFadeStartTime = Time.deltaTime * MasterAudio.FramesEarlyToTrigger * _activeAudio.pitch;
                shouldAdvance = currentClipTime <= clipFadeStartTime;
            }

            if (shouldAdvance) { // time to cross fade or fade out
                if (_currentPlaylist.fadeOutLastSong) {
                    if (isShuffle) {
                        if (_clipsRemaining.Count == 0 || !IsAutoAdvance) {
                            FadeOutPlaylist();
                            return;
                        }
                    } else {
                        if (_currentSequentialClipIndex >= _currentPlaylist.MusicSettings.Count || _currentPlaylist.MusicSettings.Count == 1 || !IsAutoAdvance) {
                            FadeOutPlaylist();
                            return;
                        }
                    }
                }

                if (IsAutoAdvance && !_nextSongRequested) {
                    if (CanSchedule) {
                        FadeInScheduledSong();
                    } else {
                        PlayNextOrRandom(AudioPlayType.PlayNow);
                    }
                }
            }
        }

    AfterAutoAdvance:

        if (IsCrossFading) {
            return;
        }

        AudioDucking();
    }
    #endregion

    #region public methods

    /// <summary>
    /// This method returns a reference to the PlaylistController whose name you specify. This is necessary when you have more than one.
    /// </summary>
    /// <param name="playlistControllerName">Name of Playlist Controller</param>
    /// <param name="errorIfNotFound">Defaults to true. Pass false if you don't want an error in the console when not found.</param>
    /// <returns></returns>
    public static PlaylistController InstanceByName(string playlistControllerName, bool errorIfNotFound = true) {
        var match = Instances.Find(delegate(PlaylistController obj) {
            return obj != null && obj.ControllerName == playlistControllerName;
        });

        if (match != null) {
            return match;
        }

        if (errorIfNotFound) {
            Debug.LogError("Could not find Playlist Controller '" + playlistControllerName + "'.");
        }
        return null;
    }

    /// <summary>
    /// Call this method to clear all songs out of the queued songs list.
    /// </summary>
    public void ClearQueue() {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        _queuedSongs.Clear();
    }

    /// <summary>
    /// This method mutes the Playlist if it's not muted, and vice versa.
    /// </summary>
    public void ToggleMutePlaylist() {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        if (IsMuted) {
            UnmutePlaylist();
        } else {
            MutePlaylist();
        }
    }

    /// <summary>
    /// This method mutes the Playlist.
    /// </summary>
    public void MutePlaylist() {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        PlaylistIsMuted = true;
    }

    /// <summary>
    /// This method unmutes the Playlist.
    /// </summary>
    public void UnmutePlaylist() {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        PlaylistIsMuted = false;
    }

    /// <summary>
    /// This method is used by Master Audio to update conditions based on the Ducked Volume Multiplier changing.
    /// </summary>
    public void UpdateDuckedVolumeMultiplier() {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        if (Application.isPlaying) {
            SetDuckProperties();
        }
    }

    /// <summary>
    /// This method will pause the Playlist.
    /// </summary>
    public void PausePlaylist() {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        if (_activeAudio == null || _transitioningAudio == null) {
            return;
        }

        _activeAudio.Pause();
        _transitioningAudio.Pause();
    }

    /// <summary>
    /// This method will unpause the Playlist.
    /// </summary>
    public bool ResumePlaylist() {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return false;
        }

        if (_activeAudio == null || _transitioningAudio == null) {
            return false;
        }

        if (_activeAudio.clip == null) {
            return false;
        }

        _activeAudio.Play();
        _transitioningAudio.Play();
        return true;
    }

    /// <summary>
    /// This method will Stop the Playlist. 
    /// </summary>
    public void StopPlaylist(bool onlyFadingClip = false) {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        if (!Application.isPlaying) {
            return;
        }

        _currentSong = null;
        if (!onlyFadingClip) {
            CeaseAudioSource(_activeAudio);
        }

        CeaseAudioSource(_transitioningAudio);
    }

    /// <summary>
    /// This method allows you to fade the Playlist to a specified volume over X seconds.
    /// </summary>
    /// <param name="targetVolume">The volume to fade to.</param>
    /// <param name="fadeTime">The amount of time to fully fade to the target volume.</param>
    /// <param name="callback">Optional callback method</param>
    public void FadeToVolume(float targetVolume, float fadeTime, Action callback = null) {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        if (fadeTime <= MasterAudio.InnerLoopCheckInterval) {
            _playlistVolume = targetVolume;
            UpdateMasterVolume();
            _curFadeMode = FadeMode.None; // in case another fade is happening, stop it!
            return;
        }

        _curFadeMode = FadeMode.GradualFade;
        _slowFadeTargetVolume = targetVolume;
        _slowFadeVolStep = (_slowFadeTargetVolume - _playlistVolume) / (fadeTime / MasterAudio.InnerLoopCheckInterval);

        _fadeCompleteCallback = callback;
    }

    /// <summary>
    /// This method will play a random song in the current Playlist.
    /// </summary>
    public void PlayRandomSong() {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        PlayARandomSong(AudioPlayType.PlayNow, false);
    }

    public void PlayARandomSong(AudioPlayType playType, bool isMidsong) {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        if (_clipsRemaining.Count == 0) {
            Debug.LogWarning("There are no clips left in this Playlist. Turn on Loop Playlist if you want to loop the entire song selection.");
            return;
        }

        if (IsCrossFading && playType == AudioPlayType.Schedule) {
            return; // this will kill the crossfade, so abort
        }

        if (isMidsong) {
            _nextSongScheduled = false;
        }

        var randIndex = UnityEngine.Random.Range(0, _clipsRemaining.Count);
        var clipIndex = _clipsRemaining[randIndex];

        switch (playType) {
            case AudioPlayType.PlayNow:
                RemoveRandomClip(randIndex);
                break;
            case AudioPlayType.Schedule:
                _lastRandomClipIndex = randIndex;
                break;
            case AudioPlayType.AlreadyScheduled:
                if (_lastRandomClipIndex >= 0) {
                    RemoveRandomClip(_lastRandomClipIndex);
                }
                break;
        }

        PlaySong(_currentPlaylist.MusicSettings[clipIndex], playType);
    }

    private void RemoveRandomClip(int randIndex) {
        _clipsRemaining.RemoveAt(randIndex);
        if (loopPlaylist && _clipsRemaining.Count == 0) {
            FillClips();
        }
    }

    private void PlayFirstQueuedSong(AudioPlayType playType) {
        if (_queuedSongs.Count == 0) {
            Debug.LogWarning("There are zero queued songs in PlaylistController '" + ControllerName + "'. Cannot play first queued song.");
            return;
        }

        var oldestQueued = _queuedSongs[0];
        _queuedSongs.RemoveAt(0); // remove before playing so the queued song can loop.

        _currentSequentialClipIndex = oldestQueued.songIndex; // keep track of which song we're playing so we don't loop playlist if it's not supposed to.
        PlaySong(oldestQueued, playType);
    }

    /// <summary>
    /// This method will play the next song in the current Playlist.
    /// </summary>
    public void PlayNextSong() {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        PlayTheNextSong(AudioPlayType.PlayNow, false);
    }

    public void PlayTheNextSong(AudioPlayType playType, bool isMidsong) {
        if (_currentPlaylist == null) {
            return;
        }

        if (IsCrossFading && playType == AudioPlayType.Schedule) {
            return; // this will kill the crossfade, so abort
        }

        //Debug.Log(nextSongScheduled);
        if (playType != AudioPlayType.AlreadyScheduled && _songsPlayedFromPlaylist > 0 && !_nextSongScheduled) {
            //Debug.LogError("advance!");
            AdvanceSongCounter();
        }

        if (_currentSequentialClipIndex >= _currentPlaylist.MusicSettings.Count) {
            Debug.LogWarning("There are no clips left in this Playlist. Turn on Loop Playlist if you want to loop the entire song selection.");
            return;
        }

        if (isMidsong) {
            _nextSongScheduled = false;
        }

        PlaySong(_currentPlaylist.MusicSettings[_currentSequentialClipIndex], playType);
    }

    private void AdvanceSongCounter() {
        _currentSequentialClipIndex++;

        if (_currentSequentialClipIndex < _currentPlaylist.MusicSettings.Count) {
            return;
        }
        if (loopPlaylist) {
            _currentSequentialClipIndex = 0;
        }
    }

    /// <summary>
    /// This method will play the song in the current Playlist whose name you specify as soon as the currently playing song is done. The current song, if looping, will have loop turned off by this call. This requires auto-advance to work.
    /// </summary>
    /// <param name="clipName">The name of the song to play.</param>
    public void QueuePlaylistClip(string clipName) {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        if (_currentPlaylist == null) {
            MasterAudio.LogNoPlaylist(ControllerName, "QueuePlaylistClip");
            return;
        }

        if (!_activeAudio.isPlaying) {
            TriggerPlaylistClip(clipName);
            return;
        }

        var setting = _currentPlaylist.MusicSettings.Find(delegate(MusicSetting obj) {
            if (obj.audLocation == MasterAudio.AudioLocation.Clip) {
                return obj.clip != null && obj.clip.name == clipName;
            } else { // resource file!
                return obj.resourceFileName == clipName;
            }
        });

        if (setting == null) {
            Debug.LogWarning("Could not find clip '" + clipName + "' in current Playlist in '" + ControllerName + "'.");
            return;
        }

        // turn off loop if it's on.
        _activeAudio.loop = false;
        // add to queue.
        _queuedSongs.Add(setting);
    }

    /// <summary>
    /// This method will play the song in the current Playlist whose name you specify.
    /// </summary>
    /// <param name="clipName">The name of the song to play.</param>
    /// <returns>bool - whether the song was played or not</returns>
    public bool TriggerPlaylistClip(string clipName) {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return false;
        }

        if (_currentPlaylist == null) {
            MasterAudio.LogNoPlaylist(ControllerName, "TriggerPlaylistClip");
            return false;
        }

        var setting = _currentPlaylist.MusicSettings.Find(delegate(MusicSetting obj) {
            if (obj.audLocation == MasterAudio.AudioLocation.Clip) {
                return obj.clip != null && obj.clip.name == clipName;
            } else { // resource file!
                return obj.resourceFileName == clipName || obj.songName == clipName;
            }
        });

        if (setting == null) {
            Debug.LogWarning("Could not find clip '" + clipName + "' in current Playlist in '" + ControllerName + "'.");
            return false;
        }

        _currentSequentialClipIndex = setting.songIndex; // keep track of which song we're playing so we don't loop playlist if it's not supposed to.

        AdvanceSongCounter();

        PlaySong(setting, AudioPlayType.PlayNow);

        return true;
    }

    public void DuckMusicForTime(float duckLength, float pitch, float duckedTimePercentage) {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        if (IsCrossFading) {
            return; // no ducking during cross-fading, it screws up calculations.
        }

        var rangedDuck = duckLength / pitch;

        _duckingMode = AudioDuckingMode.SetToDuck;
        _timeToStartUnducking = Time.realtimeSinceStartup + (rangedDuck * duckedTimePercentage);
        _timeToFinishUnducking = Math.Max(Time.realtimeSinceStartup + rangedDuck, _timeToStartUnducking);
    }

    /// <summary>
    /// This method is used to update state based on the Playlist Master Volume.
    /// </summary>
    public void UpdateMasterVolume() {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        if (!Application.isPlaying) {
            return;
        }

        if (_activeAudio != null && _currentSong != null && !IsCrossFading) {
            _activeAudio.volume = _currentSong.volume * PlaylistVolume;
        }

        if (_currentSong != null) {
            _activeAudioEndVolume = _currentSong.volume * PlaylistVolume;
        }

        SetDuckProperties();
    }

    /// <summary>
    /// This method is used to start a Playlist whether it's already loaded and playing or not.
    /// </summary>
    /// <param name="playlistName">The name of the Playlist to start</param>
    public void StartPlaylist(string playlistName) {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        if (_currentPlaylist != null && _currentPlaylist.playlistName == playlistName) {
            RestartPlaylist();
        } else {
            ChangePlaylist(playlistName);
        }
    }

    /// <summary>
    /// This method is used to change the current Playlist to a new one, and optionally start it playing.
    /// </summary>
    /// <param name="playlistName">The name of the Playlist to start</param>
    /// <param name="playFirstClip">Defaults to true. Whether to start the first song or not.</param>
    public void ChangePlaylist(string playlistName, bool playFirstClip = true) {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        if (_currentPlaylist != null && _currentPlaylist.playlistName == playlistName) {
            Debug.LogWarning("The Playlist '" + playlistName + "' is already loaded. Ignoring Change Playlist request.");
            return;
        }

        startPlaylistName = playlistName;

        FinishPlaylistInit(playFirstClip);
    }

    private void FinishPlaylistInit(bool playFirstClip = true) {
        if (IsCrossFading) {
            StopPlaylist(true);
        }

        InitializePlaylist();

        if (!Application.isPlaying) {
            return;
        }

        _queuedSongs.Clear();

        if (playFirstClip) {
            PlayNextOrRandom(AudioPlayType.PlayNow);
        }
    }

    /// <summary>
    /// This method can be called to restart the current Playlist
    /// </summary>
    public void RestartPlaylist() {
        if (!ControllerIsReady) {
            Debug.LogError(NotReadyMessage);
            return;
        }

        FinishPlaylistInit();
    }

    #endregion

    #region Helper methods
    private void FadeOutPlaylist() {
        if (_curFadeMode == FadeMode.GradualFade) {
            return;
        }

        var volumeBeforeFade = _playlistVolume;

        FadeToVolume(0f, CrossFadeTime, delegate {
            StopPlaylist();
            _playlistVolume = volumeBeforeFade;
        });
    }

    private void InitializePlaylist() {
        FillClips();
        _songsPlayedFromPlaylist = 0;
        _currentSequentialClipIndex = 0;
        _nextSongScheduled = false;
        _lastRandomClipIndex = -1;
    }

    private void PlayNextOrRandom(AudioPlayType playType) {
        _nextSongRequested = true;

        if (_queuedSongs.Count > 0) {
            PlayFirstQueuedSong(playType);
        } else if (!isShuffle) {
            PlayTheNextSong(playType, false);
        } else {
            PlayARandomSong(playType, false);
        }
    }

    private void FillClips() {
        _clipsRemaining.Clear();

        // add clips from named playlist.
        if (startPlaylistName == MasterAudio.NoPlaylistName) {
            return;
        }

        _currentPlaylist = MasterAudio.GrabPlaylist(startPlaylistName);

        if (_currentPlaylist == null) {
            return;
        }

        for (var i = 0; i < _currentPlaylist.MusicSettings.Count; i++) {
            var aSong = _currentPlaylist.MusicSettings[i];
            aSong.songIndex = i;

            if (aSong.audLocation != MasterAudio.AudioLocation.ResourceFile) {
                if (aSong.clip == null) {
                    continue;
                }
            } else { // resource file!
                if (string.IsNullOrEmpty(aSong.resourceFileName)) {
                    continue;
                }
            }

            _clipsRemaining.Add(i);
        }
    }

    private void PlaySong(MusicSetting setting, AudioPlayType playType) {
        //Debug.Log("play: " + playType);

        _newSongSetting = setting;

        if (_activeAudio == null) {
            Debug.LogError("PlaylistController prefab is not in your scene. Cannot play a song.");
            return;
        }

        AudioClip clipToPlay = null;

        var clipWillBeAudibleNow = playType == AudioPlayType.PlayNow || playType == AudioPlayType.AlreadyScheduled;
        if (clipWillBeAudibleNow && _currentSong != null && !CanSchedule) {
            if (_currentSong.songChangedCustomEvent != string.Empty && _currentSong.songChangedCustomEvent != MasterAudio.NoGroupName) {
                MasterAudio.FireCustomEvent(_currentSong.songChangedCustomEvent, Trans.position);
            }
        }

        if (playType != AudioPlayType.AlreadyScheduled) {
            if (_activeAudio.clip != null) {
                var newSongName = string.Empty;
                switch (setting.audLocation) {
                    case MasterAudio.AudioLocation.Clip:
                        if (setting.clip != null) {
                            newSongName = setting.clip.name;
                        }
                        break;
                    case MasterAudio.AudioLocation.ResourceFile:
                        newSongName = setting.resourceFileName;
                        break;
                }

                if (string.IsNullOrEmpty(newSongName)) {
                    Debug.LogWarning("The next song has no clip or Resource file assigned. Please fix  Ignoring song change request.");
                    return;
                }
            }

            if (_activeAudio.clip == null) {
                _audioClip = _activeAudio;
                _transClip = _transitioningAudio;
            } else if (_transitioningAudio.clip == null) {
                _audioClip = _transitioningAudio;
                _transClip = _activeAudio;
            } else {
                // both are busy!
                _audioClip = _transitioningAudio;
                _transClip = _activeAudio;
            }

            if (setting.clip != null) {
                _audioClip.clip = setting.clip;
                _audioClip.pitch = setting.pitch;
            }

            _audioClip.loop = SongShouldLoop(setting);

            switch (setting.audLocation) {
                case MasterAudio.AudioLocation.Clip:
                    if (setting.clip == null) {
                        MasterAudio.LogWarning("MasterAudio will not play empty Playlist clip for PlaylistController '" + ControllerName + "'.");
                        return;
                    }

                    clipToPlay = setting.clip;
                    break;
                case MasterAudio.AudioLocation.ResourceFile:
                    if (MasterAudio.HasAsyncResourceLoaderFeature() && ShouldLoadAsync) {
                        StartCoroutine(AudioResourceOptimizer.PopulateResourceSongToPlaylistControllerAsync(setting.resourceFileName, CurrentPlaylist.playlistName, this, playType));
                    } else {
                        clipToPlay = AudioResourceOptimizer.PopulateResourceSongToPlaylistController(ControllerName, setting.resourceFileName, CurrentPlaylist.playlistName);
                        if (clipToPlay == null) {
                            return;
                        }
                    }

                    break;
            }
        } else {
            FinishLoadingNewSong(null, AudioPlayType.AlreadyScheduled);
        }

        if (clipToPlay != null) {
            FinishLoadingNewSong(clipToPlay, playType);
        }
    }

    public void FinishLoadingNewSong(AudioClip clipToPlay, AudioPlayType playType) {
        _nextSongRequested = false;

        var shouldPopulateClip = playType == AudioPlayType.PlayNow || playType == AudioPlayType.Schedule;
        var clipWillBeAudibleNow = playType == AudioPlayType.PlayNow || playType == AudioPlayType.AlreadyScheduled;

        if (shouldPopulateClip) {
            _audioClip.clip = clipToPlay;
            _audioClip.pitch = _newSongSetting.pitch;
        }

        // set last known time for current song.
        if (_currentSong != null) {
            _currentSong.lastKnownTimePoint = _activeAudio.timeSamples;
        }

        if (clipWillBeAudibleNow) {
            if (CrossFadeTime == 0 || _transClip.clip == null) {
                CeaseAudioSource(_transClip);
                _audioClip.volume = _newSongSetting.volume * PlaylistVolume;

                if (!ActiveAudioSource.isPlaying && _currentPlaylist != null && _currentPlaylist.fadeInFirstSong) {
                    CrossFadeNow(_audioClip);
                }
            } else {
                CrossFadeNow(_audioClip);
            }

            SetDuckProperties();
        }

        switch (playType) {
            case AudioPlayType.AlreadyScheduled:
                // start crossfading now	
                _nextSongScheduled = false;
                RemoveScheduledClip();
                break;
            case AudioPlayType.PlayNow:
                _audioClip.Play(); // need to play before setting time or it sometimes resets back to zero.
                _songsPlayedFromPlaylist++;
                break;
            case AudioPlayType.Schedule:
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
					Debug.LogError("Master Audio cannot do gapless song transition on Unity 3. Please report this bug to Dark Tonic as it should not happen.");
					return;
#else
                var scheduledPlayTime = CalculateNextTrackStartTime();

                ScheduleClipPlay(scheduledPlayTime);

                _nextSongScheduled = true;
                _songsPlayedFromPlaylist++;
                break;
#endif
        }

        var songTimeChanged = false;

        // ReSharper disable once PossibleNullReferenceException
        if (syncGroupNum > 0 && _currentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.SynchronizeClips) {
            var firstMatchingGroupController = Instances.Find(delegate(PlaylistController obj) {
                return obj != this && obj.syncGroupNum == syncGroupNum && obj.ActiveAudioSource.isPlaying;
            });

            if (firstMatchingGroupController != null) {
                _audioClip.timeSamples = firstMatchingGroupController._activeAudio.timeSamples;
                songTimeChanged = true;
            }
        }

        // this code will adjust the starting position of a song, but shouldn't do so when you first change Playlists.
        if (_currentPlaylist != null) {
            if (_songsPlayedFromPlaylist <= 1 && !songTimeChanged) {
                _audioClip.timeSamples = 0; // reset pointer so a new Playlist always starts at the beginning, but don't do it for synchronized! We need that first song to use the sync group.
            } else {
                switch (_currentPlaylist.songTransitionType) {
                    case MasterAudio.SongFadeInPosition.SynchronizeClips:
                        if (!songTimeChanged) { // otherwise the sync group code above will get defeated.
                            _transitioningAudio.timeSamples = _activeAudio.timeSamples;
                        }
                        break;
                    case MasterAudio.SongFadeInPosition.NewClipFromLastKnownPosition:
                        var thisSongInPlaylist = _currentPlaylist.MusicSettings.Find(delegate(MusicSetting obj) {
                            return obj == _newSongSetting;
                        });

                        if (thisSongInPlaylist != null) {
                            _transitioningAudio.timeSamples = thisSongInPlaylist.lastKnownTimePoint;
                        }
                        break;
                    case MasterAudio.SongFadeInPosition.NewClipFromBeginning:
                        _audioClip.timeSamples = 0; // new song will start at beginning
                        break;
                }
            }

            // account for custom start time.
            if (_currentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.NewClipFromBeginning && _newSongSetting.customStartTime > 0f) {
                _audioClip.timeSamples = (int)(_newSongSetting.customStartTime * _audioClip.clip.frequency);
            }
        }

        if (clipWillBeAudibleNow) {
            _activeAudio = _audioClip;
            _transitioningAudio = _transClip;

            // song changed
            if (songChangedCustomEvent != string.Empty && songChangedCustomEvent != MasterAudio.NoGroupName) {
                MasterAudio.FireCustomEvent(songChangedCustomEvent, Trans.position);
            }

            if (SongChanged != null) {
                var clipName = String.Empty;
                if (_audioClip != null) {
                    clipName = _audioClip.clip.name;
                }
                SongChanged(clipName);
            }
            // song changed end
        }

        _activeAudioEndVolume = _newSongSetting.volume * PlaylistVolume;
        var transStartVol = _transitioningAudio.volume;
        if (_currentSong != null) {
            transStartVol = _currentSong.volume;
        }

        _transitioningAudioStartVolume = transStartVol * PlaylistVolume;
        _currentSong = _newSongSetting;

        if (clipWillBeAudibleNow && _currentSong.songStartedCustomEvent != string.Empty && _currentSong.songStartedCustomEvent != MasterAudio.NoGroupName) {
            MasterAudio.FireCustomEvent(_currentSong.songStartedCustomEvent, Trans.position);
        }

        if (CanSchedule && playType != AudioPlayType.Schedule) {
            ScheduleNextSong();
        }
    }

    private void RemoveScheduledClip() {
        if (_audioClip != null) {
            _scheduledSongsByAudioSource.Remove(_audioClip);
        }
    }

#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0
        // ReSharper disable once UnusedParameter.Local
		private static void ScheduleClipPlay(double scheduledPlayTime) {
			// no support
		}
	
		private static double CalculateNextTrackStartTime() {
			return 0;
		}	
	
		// can't schedule
		private static void ScheduleNextSong() {
		}
		
		private static void FadeInScheduledSong() {

		}
#else
    private void ScheduleNextSong() {
        PlayNextOrRandom(AudioPlayType.Schedule);
    }

    private void FadeInScheduledSong() {
        PlayNextOrRandom(AudioPlayType.AlreadyScheduled);
    }

    private double CalculateNextTrackStartTime() {
        var timeRemainingOnMainClip = (_activeAudio.clip.length - _activeAudio.time) / _activeAudio.pitch - CrossFadeTime;
        return AudioSettings.dspTime + timeRemainingOnMainClip;
    }

    private void ScheduleClipPlay(double scheduledPlayTime) {
        _audioClip.PlayScheduled(scheduledPlayTime);

        RemoveScheduledClip();

        _scheduledSongsByAudioSource.Add(_audioClip, scheduledPlayTime);
    }
#endif

    private void CrossFadeNow(AudioSource audioClip) {
        audioClip.volume = 0f;
        IsCrossFading = true;
        _duckingMode = AudioDuckingMode.NotDucking;
        _crossFadeStartTime = Time.realtimeSinceStartup;
    }

    private void CeaseAudioSource(AudioSource source) {
        if (source == null) {
            return;
        }

        var songName = source.clip == null ? string.Empty : source.clip.name;
        source.Stop();
        AudioResourceOptimizer.UnloadPlaylistSongIfUnused(ControllerName, source.clip);
        source.clip = null;
        RemoveScheduledClip();

        // song ended start
        if (songEndedCustomEvent != string.Empty && songEndedCustomEvent != MasterAudio.NoGroupName) {
            MasterAudio.FireCustomEvent(songEndedCustomEvent, Trans.position);
        }

        if (SongEnded != null && !string.IsNullOrEmpty(songName)) {
            SongEnded(songName);
        }
        // song ended end
    }

    private void SetDuckProperties() {
        _originalMusicVolume = _activeAudio == null ? 1 : _activeAudio.volume;

        if (_currentSong != null) {
            _originalMusicVolume = _currentSong.volume * PlaylistVolume;
        }

        _initialDuckVolume = MasterAudio.Instance.DuckedVolumeMultiplier * _originalMusicVolume;
        _duckRange = _originalMusicVolume - MasterAudio.Instance.DuckedVolumeMultiplier;

        _duckingMode = AudioDuckingMode.NotDucking; // cancel any ducking
    }

    private void AudioDucking() {
        switch (_duckingMode) {
            case AudioDuckingMode.NotDucking:
                break;
            case AudioDuckingMode.SetToDuck:
                _activeAudio.volume = _initialDuckVolume;
                _duckingMode = AudioDuckingMode.Ducked;
                break;
            case AudioDuckingMode.Ducked:
                if (Time.realtimeSinceStartup >= _timeToFinishUnducking) {
                    _activeAudio.volume = _originalMusicVolume;
                    _duckingMode = AudioDuckingMode.NotDucking;
                } else if (Time.realtimeSinceStartup >= _timeToStartUnducking) {
                    _activeAudio.volume = _initialDuckVolume + (Time.realtimeSinceStartup - _timeToStartUnducking) / (_timeToFinishUnducking - _timeToStartUnducking) * _duckRange;
                }
                break;
        }
    }

    private bool SongShouldLoop(MusicSetting setting) {
        if (_queuedSongs.Count > 0) {
            return false;
        }

        if (CurrentPlaylist != null && CurrentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.SynchronizeClips) {
            return true;
        }

        return setting.isLoop;
    }

    #endregion

    #region Properties
    private bool ShouldLoadAsync {
        get {
            if (MasterAudio.Instance.resourceClipsAllLoadAsync) {
                return true;
            }

            return CurrentPlaylist.resourceClipsAllLoadAsync;
        }
    }

    /// <summary>
    /// This property returns true if the Playlist Controller has already run its Awake method. You should not call any PlaylistController method until it has done so.
    /// </summary>
    public bool ControllerIsReady { get; private set; }

    /// <summary>
    /// This property returns the current state of the Playlist. Choices are: NotInScene, Stopped, Playing, Paused, Crossfading
    /// </summary>
    public PlaylistStates PlaylistState {
        get {
            if (_activeAudio == null || _transitioningAudio == null) {
                return PlaylistStates.NotInScene;
            }

            if (!ActiveAudioSource.isPlaying) {
                if (ActiveAudioSource.time != 0f) {
                    return PlaylistStates.Paused;
                } else {
                    return PlaylistStates.Stopped;
                }
            }

            if (IsCrossFading) {
                return PlaylistStates.Crossfading;
            }

            return PlaylistStates.Playing;
        }
    }

    /// <summary>
    /// This property returns the active audio source for the PlaylistControllers in the Scene. During cross-fading, the one fading in is returned, not the one fading out.
    /// </summary>
    public AudioSource ActiveAudioSource {
        get {
            if (_activeAudio.clip == null) {
                return _transitioningAudio;
            } else {
                return _activeAudio;
            }
        }
    }

    /// <summary>
    /// This property returns all the PlaylistControllers in the Scene.
    /// </summary>
    public static List<PlaylistController> Instances {
        get {
            if (_instances != null) {
                return _instances;
            }
            _instances = new List<PlaylistController>();

            var controllers = FindObjectsOfType(typeof(PlaylistController));
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < controllers.Length; i++) {
                _instances.Add(controllers[i] as PlaylistController);
            }

            return _instances;
        }
        set {
            // only for non-caching.
            _instances = value;
        }
    }

    /// <summary>
    /// This property returns the GameObject for the PlaylistController's GameObject.
    /// </summary>
    public GameObject PlaylistControllerGameObject {
        get {
            return _go;
        }
    }

    /// <summary>
    ///  This property returns the current Audio Source for the current Playlist song that is playing.
    /// </summary>
    // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    public AudioSource CurrentPlaylistSource {
        get {
            return _activeAudio;
        }
    }

    /// <summary>
    ///  This property returns the current Audio Clip for the current Playlist song that is playing.
    /// </summary>
    public AudioClip CurrentPlaylistClip {
        get {
            if (_activeAudio == null) {
                return null;
            }

            return _activeAudio.clip;
        }
    }

    /// <summary>
    /// This property returns the currently fading out Audio Clip for the Playlist (null if not during cross-fading).
    /// </summary>
    public AudioClip FadingPlaylistClip {
        get {
            if (!IsCrossFading) {
                return null;
            }

            if (_transitioningAudio == null) {
                return null;
            }

            return _transitioningAudio.clip;
        }
    }

    /// <summary>
    /// This property returns the currently fading out Audio Source for the Playlist (null if not during cross-fading).
    /// </summary>
    public AudioSource FadingSource {
        get {
            if (!IsCrossFading) {
                return null;
            }

            return _transitioningAudio;
        }
    }

    /// <summary>
    /// This property returns whether or not the Playlist is currently cross-fading.
    /// </summary>
    public bool IsCrossFading { get; private set; }

    /// <summary>
    /// This property returns whether or not the Playlist is currently cross-fading or doing another fade.
    /// </summary>
    public bool IsFading {
        get {
            return IsCrossFading || _curFadeMode != FadeMode.None;
        }
    }

    /// <summary>
    /// This property gets and sets the volume of the Playlist Controller with Master Playlist Volume taken into account.
    /// </summary>
    public float PlaylistVolume {
        get {
            return MasterAudio.PlaylistMasterVolume * _playlistVolume;
        }
        set {
            _playlistVolume = value;
            UpdateMasterVolume();
        }
    }

#if UNITY_5_0
    public void RouteToMixerChannel(AudioMixerGroup group) {
        _activeAudio.outputAudioMixerGroup = group;
        _transitioningAudio.outputAudioMixerGroup = group;
    }
#endif

    /// <summary>
    /// This property returns the current Playlist
    /// </summary>
    public MasterAudio.Playlist CurrentPlaylist {
        get {
            if (_currentPlaylist != null || !(Time.realtimeSinceStartup - _lastTimeMissingPlaylistLogged > 2f)) {
                return _currentPlaylist;
            }

            Debug.LogWarning("Current Playlist is NULL. Subsequent calls will fail.");
            _lastTimeMissingPlaylistLogged = Time.realtimeSinceStartup;
            return _currentPlaylist;
        }
    }

    /// <summary>
    /// This property returns whether you have a Playlist assigned to this controller or not.
    /// </summary>
    public bool HasPlaylist {
        get {
            return _currentPlaylist != null;
        }
    }

    /// <summary>
    /// This property returns the name of the current Playlist
    /// </summary>
    public string PlaylistName {
        get {
            if (CurrentPlaylist == null) {
                return string.Empty;
            }

            return CurrentPlaylist.playlistName;
        }
    }

    private bool IsMuted {
        get {
            return isMuted;
        }
    }

    /// <summary>
    /// This property returns whether the current Playlist is muted or not
    /// </summary>
    private bool PlaylistIsMuted {
        set {
            isMuted = value;

            if (Application.isPlaying) {
                if (_activeAudio != null) {
                    _activeAudio.mute = value;
                }

                if (_transitioningAudio != null) {
                    _transitioningAudio.mute = value;
                }
            } else {
                var audios = GetComponents<AudioSource>();
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < audios.Length; i++) {
                    audios[i].mute = value;
                }
            }
        }
    }

    private float CrossFadeTime {
        get {
            if (_currentPlaylist != null) {
                return _currentPlaylist.crossfadeMode == MasterAudio.Playlist.CrossfadeTimeMode.UseMasterSetting ? MasterAudio.Instance.MasterCrossFadeTime : _currentPlaylist.crossFadeTime;
            }

            return MasterAudio.Instance.MasterCrossFadeTime;
        }
    }

    private bool IsAutoAdvance {
        get {
            if (_currentPlaylist != null && _currentPlaylist.songTransitionType == MasterAudio.SongFadeInPosition.SynchronizeClips) {
                return false;
            }

            return isAutoAdvance;
        }
    }

    public GameObject GameObj {
        get {
            if (_go != null) {
                return _go;
            }

            _go = gameObject;

            return _go;
        }
    }

    public string ControllerName {
        get {
            if (_name != null) {
                return _name;
            }
            _name = GameObj.name;

            return _name;
        }
    }

    public bool CanSchedule {
        get {
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
				return false;
#else
            return MasterAudio.Instance.useGaplessPlaylists && IsAutoAdvance;
#endif
        }
    }

    private Transform Trans {
        get {
            if (_trans != null) {
                return _trans;
            }
            _trans = transform;

            return _trans;
        }
    }

    #endregion
}
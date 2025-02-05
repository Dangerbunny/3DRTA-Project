using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_4_6 || UNITY_5_0
// ReSharper disable once RedundantUsingDirective
using UnityEngine.UI;
using UnityEngine.EventSystems;
#endif

#if UNITY_5_0
using UnityEngine.Audio;
#endif

[AddComponentMenu("Dark Tonic/Master Audio/Event Sounds")]
// ReSharper disable once CheckNamespace
public class EventSounds : MonoBehaviour, ICustomEventReceiver
#if UNITY_4_6 || UNITY_5_0
, IPointerDownHandler
    , IDragHandler
    , IPointerUpHandler
    , IPointerEnterHandler
    , IPointerExitHandler
    , IDropHandler
    , IScrollHandler
    , IUpdateSelectedHandler
    , ISelectHandler, IDeselectHandler, IMoveHandler, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, ISubmitHandler, ICancelHandler
#endif
 {
    // ReSharper disable InconsistentNaming
    public bool showGizmo = true;
    public MasterAudio.SoundSpawnLocationMode soundSpawnMode = MasterAudio.SoundSpawnLocationMode.AttachToCaller;
    public bool disableSounds = false;
    public bool showPoolManager = false;
    public bool showNGUI = false;

#if UNITY_4_6 || UNITY_5_0
    public UnityUIVersion unityUIMode = UnityUIVersion.uGUI;
#else
	public UnityUIVersion unityUIMode = UnityUIVersion.Legacy;
#endif

    public bool logMissingEvents = true;
    // ReSharper restore InconsistentNaming

    public enum UnityUIVersion {
        Legacy
#if UNITY_4_6 || UNITY_5_0
            // ReSharper disable once InconsistentNaming
        , uGUI
#endif
    }

    public enum EventType {
        OnStart,
        OnVisible,
        OnInvisible,
        OnCollision,
        OnTriggerEnter,
        OnTriggerExit,
        OnMouseEnter,
        OnMouseClick,
        OnSpawned,
        OnDespawned,
        OnEnable,
        OnDisable,
        OnCollision2D,
        OnTriggerEnter2D,
        OnTriggerExit2D,
        OnParticleCollision,
        UserDefinedEvent,
        OnCollisionExit,
        OnCollisionExit2D,
        OnMouseUp,
        OnMouseExit,
        OnMouseDrag,
        NGUIOnClick,
        NGUIMouseDown,
        NGUIMouseUp,
        NGUIMouseEnter,
        NGUIMouseExit,
        MechanimStateChanged,
        UnitySliderChanged,
        UnityButtonClicked,
        UnityPointerDown,
        UnityPointerUp,
        UnityPointerEnter,
        UnityPointerExit,
        UnityDrag,
        UnityDrop,
        UnityScroll,
        UnityUpdateSelected,
        UnitySelect,
        UnityDeselect,
        UnityMove,
        UnityInitializePotentialDrag,
        UnityBeginDrag,
        UnityEndDrag,
        UnitySubmit,
        UnityCancel
    }

    public enum VariationType {
        PlaySpecific,
        PlayRandom
    }

    public enum PreviousSoundStopMode {
        None,
        Stop,
        FadeOut
    }

    public enum RetriggerLimMode {
        None,
        FrameBased,
        TimeBased
    }

    public static List<string> LayerTagFilterEvents = new List<string>() {
		EventType.OnCollision.ToString(),
		EventType.OnTriggerEnter.ToString(),
		EventType.OnTriggerExit.ToString(),
		EventType.OnCollision2D.ToString(),
		EventType.OnTriggerEnter2D.ToString(),
		EventType.OnTriggerExit2D.ToString(),
		EventType.OnParticleCollision.ToString(),
		EventType.OnCollisionExit.ToString(),
		EventType.OnCollisionExit2D.ToString()
	};

    public static List<MasterAudio.PlaylistCommand> PlaylistCommandsWithAll = new List<MasterAudio.PlaylistCommand>() {
		MasterAudio.PlaylistCommand.FadeToVolume,
		MasterAudio.PlaylistCommand.Pause,
		MasterAudio.PlaylistCommand.PlayNextSong,
		MasterAudio.PlaylistCommand.PlayRandomSong,
		MasterAudio.PlaylistCommand.Resume,
		MasterAudio.PlaylistCommand.Stop,
		MasterAudio.PlaylistCommand.Mute,
		MasterAudio.PlaylistCommand.Unmute,
		MasterAudio.PlaylistCommand.ToggleMute,
        MasterAudio.PlaylistCommand.Restart
	};

    // ReSharper disable InconsistentNaming
    public AudioEventGroup startSound;
    public AudioEventGroup visibleSound;
    public AudioEventGroup invisibleSound;
    public AudioEventGroup collisionSound;
    public AudioEventGroup collisionExitSound;
    public AudioEventGroup triggerSound;
    public AudioEventGroup triggerExitSound;
    public AudioEventGroup mouseEnterSound;
    public AudioEventGroup mouseExitSound;
    public AudioEventGroup mouseClickSound;
    public AudioEventGroup mouseUpSound;
    public AudioEventGroup mouseDragSound;
    public AudioEventGroup spawnedSound;
    public AudioEventGroup despawnedSound;
    public AudioEventGroup enableSound;
    public AudioEventGroup disableSound;
    public AudioEventGroup collision2dSound;
    public AudioEventGroup collisionExit2dSound;
    public AudioEventGroup triggerEnter2dSound;
    public AudioEventGroup triggerExit2dSound;
    public AudioEventGroup particleCollisionSound;
    public AudioEventGroup nguiOnClickSound;
    public AudioEventGroup nguiMouseDownSound;
    public AudioEventGroup nguiMouseUpSound;
    public AudioEventGroup nguiMouseEnterSound;
    public AudioEventGroup nguiMouseExitSound;

    public AudioEventGroup unitySliderChangedSound;
    public AudioEventGroup unityButtonClickedSound;
    public AudioEventGroup unityPointerDownSound;
    public AudioEventGroup unityDragSound;
    public AudioEventGroup unityPointerUpSound;
    public AudioEventGroup unityPointerEnterSound;
    public AudioEventGroup unityPointerExitSound;
    public AudioEventGroup unityDropSound;
    public AudioEventGroup unityScrollSound;
    public AudioEventGroup unityUpdateSelectedSound;
    public AudioEventGroup unitySelectSound;
    public AudioEventGroup unityDeselectSound;
    public AudioEventGroup unityMoveSound;
    public AudioEventGroup unityInitializePotentialDragSound;
    public AudioEventGroup unityBeginDragSound;
    public AudioEventGroup unityEndDragSound;
    public AudioEventGroup unitySubmitSound;
    public AudioEventGroup unityCancelSound;

    public List<AudioEventGroup> userDefinedSounds = new List<AudioEventGroup>();
    public List<AudioEventGroup> mechanimStateChangedSounds = new List<AudioEventGroup>();

    public bool useStartSound = false;
    public bool useVisibleSound = false;
    public bool useInvisibleSound = false;
    public bool useCollisionSound = false;
    public bool useCollisionExitSound = false;
    public bool useTriggerEnterSound = false;
    public bool useTriggerExitSound = false;
    public bool useMouseEnterSound = false;
    public bool useMouseExitSound = false;
    public bool useMouseClickSound = false;
    public bool useMouseUpSound = false;
    public bool useMouseDragSound = false;
    public bool useSpawnedSound = false;
    public bool useDespawnedSound = false;
    public bool useEnableSound = false;
    public bool useDisableSound = false;
    public bool useCollision2dSound = false;
    public bool useCollisionExit2dSound = false;
    public bool useTriggerEnter2dSound = false;
    public bool useTriggerExit2dSound = false;
    public bool useParticleCollisionSound = false;

    public bool useNguiOnClickSound = false;
    public bool useNguiMouseDownSound = false;
    public bool useNguiMouseUpSound = false;
    public bool useNguiMouseEnterSound = false;
    public bool useNguiMouseExitSound = false;

    public bool useUnitySliderChangedSound = false;
    public bool useUnityButtonClickedSound = false;
    public bool useUnityPointerDownSound = false;
    public bool useUnityDragSound = false;
    public bool useUnityPointerUpSound = false;
    public bool useUnityPointerEnterSound = false;
    public bool useUnityPointerExitSound = false;
    public bool useUnityDropSound = false;
    public bool useUnityScrollSound = false;
    public bool useUnityUpdateSelectedSound = false;
    public bool useUnitySelectSound = false;
    public bool useUnityDeselectSound = false;
    public bool useUnityMoveSound = false;
    public bool useUnityInitializePotentialDragSound = false;
    public bool useUnityBeginDragSound = false;
    public bool useUnityEndDragSound = false;
    public bool useUnitySubmitSound = false;
    public bool useUnityCancelSound = false;
    // ReSharper restore InconsistentNaming

#if UNITY_4_6 || UNITY_5_0
    // ReSharper disable RedundantNameQualifier
    private UnityEngine.UI.Slider _slider;
    private UnityEngine.UI.Button _button;
    // ReSharper restore RedundantNameQualifier
#endif

    private bool _isVisible;
    private bool _needsCoroutine;

#if UNITY_IPHONE || UNITY_ANDROID
	// no mouse events!
#else
    private bool _mouseDragSoundPlayed;
    private PlaySoundResult _mouseDragResult;
#endif

    private Transform _trans;
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		// component doesn't exist
#else
    private readonly List<AudioEventGroup> _validMechanimStateChangedSounds = new List<AudioEventGroup>();
    private Animator _anim;
#endif

    // ReSharper disable once UnusedMember.Local
    void Awake() {
        _trans = transform;
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			// component doesn't exist
#else
        _anim = GetComponent<Animator>();
#endif

#if UNITY_4_6 || UNITY_5_0
        // ReSharper disable RedundantNameQualifier
        _slider = GetComponent<UnityEngine.UI.Slider>();
        _button = GetComponent<UnityEngine.UI.Button>();
        // ReSharper restore RedundantNameQualifier
#endif
        SpawnedOrAwake();
    }

    protected virtual void SpawnedOrAwake() {
        _isVisible = false;

#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
			// component doesn't exist
#else
        // check if we need a coroutine for Mechanim stuff
        _validMechanimStateChangedSounds.Clear();
        _needsCoroutine = false;

        if (disableSounds || _anim == null) {
            return;
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < mechanimStateChangedSounds.Count; i++) {
            var state = mechanimStateChangedSounds[i];
            if (!state.mechanimEventActive || string.IsNullOrEmpty(state.mechanimStateName)) {
                continue;
            }
            _needsCoroutine = true;
            _validMechanimStateChangedSounds.Add(state);
        }
#endif
    }

#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		// component doesn't exist, no reason to have this CoRoutine
#else
    IEnumerator CoUpdate() {
        while (true) {
            yield return MasterAudio.EndOfFrameDelay;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _validMechanimStateChangedSounds.Count; i++) {
                var chg = _validMechanimStateChangedSounds[i];

                var matchState = _anim.GetCurrentAnimatorStateInfo(0).IsName(chg.mechanimStateName);
                if (!matchState) {
                    chg.mechEventPlayedForState = false;
                    continue;
                }

                if (chg.mechEventPlayedForState) {
                    continue;
                }

                chg.mechEventPlayedForState = true;
                PlaySounds(chg, EventType.MechanimStateChanged);
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
#endif

    #region Core Monobehavior events
    // ReSharper disable once UnusedMember.Local
    void Start() {
        CheckForIllegalCustomEvents();

        if (useStartSound) {
            PlaySounds(startSound, EventType.OnStart);
        }
    }

    // ReSharper disable once UnusedMember.Local
    void OnBecameVisible() {
        if (!useVisibleSound || _isVisible) {
            return;
        }
        _isVisible = true;
        PlaySounds(visibleSound, EventType.OnVisible);
    }

    // ReSharper disable once UnusedMember.Local
    void OnBecameInvisible() {
        if (!useInvisibleSound) {
            return;
        }
        _isVisible = false;
        PlaySounds(invisibleSound, EventType.OnInvisible);
    }

    // ReSharper disable once UnusedMember.Local
    void OnEnable() {
#if UNITY_4_6 || UNITY_5_0
        if (_slider != null) {
            _slider.onValueChanged.AddListener(SliderChanged);
        }
        if (_button != null) {
            _button.onClick.AddListener(ButtonClicked);
        }
#endif

#if UNITY_IPHONE || UNITY_ANDROID
			// no mouse events!
#else
        _mouseDragResult = null;
#endif

        RegisterReceiver();


#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
		// component doesn't exist, no reason to have this CoRoutine
#else
        // start coroutine if we're doing Mechanim monitoring.
        // ReSharper disable once InvertIf
        if (_needsCoroutine) {
            StopAllCoroutines();
            StartCoroutine(CoUpdate()); 
        }
#endif

        if (useEnableSound) {
            PlaySounds(enableSound, EventType.OnEnable);
        }
        }

    // ReSharper disable once UnusedMember.Local
    void OnDisable() {
#if UNITY_4_6 || UNITY_5_0
        if (_slider != null) {
            _slider.onValueChanged.RemoveListener(SliderChanged);
        }
        if (_button != null) {
            _button.onClick.RemoveListener(ButtonClicked);
        }
#endif

        UnregisterReceiver();

        if (!useDisableSound || MasterAudio.AppIsShuttingDown) {
            return;
        }

        PlaySounds(disableSound, EventType.OnDisable);
    }
    #endregion

    #region Collision and Trigger Events

#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
		// these events don't exist
#else
    // ReSharper disable once UnusedMember.Local
    void OnTriggerEnter2D(Collider2D other) {
        if (!useTriggerEnter2dSound) {
            return;
        }

        // check filters for matches if turned on
        if (triggerEnter2dSound.useLayerFilter && !triggerEnter2dSound.matchingLayers.Contains(other.gameObject.layer)) {
            return;
        }

        if (triggerEnter2dSound.useTagFilter && !triggerEnter2dSound.matchingTags.Contains(other.gameObject.tag)) {
            return;
        }

        PlaySounds(triggerEnter2dSound, EventType.OnTriggerEnter2D);
    }

    // ReSharper disable once UnusedMember.Local
    void OnTriggerExit2D(Collider2D other) {
        if (!useTriggerExit2dSound) {
            return;
        }

        // check filters for matches if turned on
        if (triggerExit2dSound.useLayerFilter && !triggerExit2dSound.matchingLayers.Contains(other.gameObject.layer)) {
            return;
        }

        if (triggerExit2dSound.useTagFilter && !triggerExit2dSound.matchingTags.Contains(other.gameObject.tag)) {
            return;
        }

        PlaySounds(triggerExit2dSound, EventType.OnTriggerExit2D);
    }

    // ReSharper disable once UnusedMember.Local
    void OnCollisionEnter2D(Collision2D collision) {
        if (!useCollision2dSound) {
            return;
        }

        // check filters for matches if turned on
        if (collision2dSound.useLayerFilter && !collision2dSound.matchingLayers.Contains(collision.gameObject.layer)) {
            return;
        }

        if (collision2dSound.useTagFilter && !collision2dSound.matchingTags.Contains(collision.gameObject.tag)) {
            return;
        }

        PlaySounds(collision2dSound, EventType.OnCollision2D);
    }

    // ReSharper disable once UnusedMember.Local
    void OnCollisionExit2D(Collision2D collision) {
        if (!useCollisionExit2dSound) {
            return;
        }

        // check filters for matches if turned on
        if (collisionExit2dSound.useLayerFilter && !collisionExit2dSound.matchingLayers.Contains(collision.gameObject.layer)) {
            return;
        }

        if (collisionExit2dSound.useTagFilter && !collisionExit2dSound.matchingTags.Contains(collision.gameObject.tag)) {
            return;
        }

        PlaySounds(collisionExit2dSound, EventType.OnCollisionExit2D);
    }
#endif

    // ReSharper disable once UnusedMember.Local
    void OnCollisionEnter(Collision collision) {
        if (!useCollisionSound) {
            return;
        }

        // check filters for matches if turned on
        if (collisionSound.useLayerFilter && !collisionSound.matchingLayers.Contains(collision.gameObject.layer)) {
            return;
        }

        if (collisionSound.useTagFilter && !collisionSound.matchingTags.Contains(collision.gameObject.tag)) {
            return;
        }

        PlaySounds(collisionSound, EventType.OnCollision);
    }

    // ReSharper disable once UnusedMember.Local
    void OnCollisionExit(Collision collision) {
        if (!useCollisionExitSound) {
            return;
        }

        // check filters for matches if turned on
        if (collisionExitSound.useLayerFilter && !collisionExitSound.matchingLayers.Contains(collision.gameObject.layer)) {
            return;
        }

        if (collisionExitSound.useTagFilter && !collisionExitSound.matchingTags.Contains(collision.gameObject.tag)) {
            return;
        }

        PlaySounds(collisionExitSound, EventType.OnCollisionExit);
    }

    // ReSharper disable once UnusedMember.Local
    void OnTriggerEnter(Collider other) {
        if (!useTriggerEnterSound) {
            return;
        }

        // check filters for matches if turned on
        if (triggerSound.useLayerFilter && !triggerSound.matchingLayers.Contains(other.gameObject.layer)) {
            return;
        }

        if (triggerSound.useTagFilter && !triggerSound.matchingTags.Contains(other.gameObject.tag)) {
            return;
        }

        PlaySounds(triggerSound, EventType.OnTriggerEnter);
    }

    // ReSharper disable once UnusedMember.Local
    void OnTriggerExit(Collider other) {
        if (!useTriggerExitSound) {
            return;
        }

        // check filters for matches if turned on
        if (triggerExitSound.useLayerFilter && !triggerExitSound.matchingLayers.Contains(other.gameObject.layer)) {
            return;
        }

        if (triggerExitSound.useTagFilter && !triggerExitSound.matchingTags.Contains(other.gameObject.tag)) {
            return;
        }

        PlaySounds(triggerExitSound, EventType.OnTriggerExit);
    }

    // ReSharper disable once UnusedMember.Local
    void OnParticleCollision(GameObject other) {
        if (!useParticleCollisionSound) {
            return;
        }

        // check filters for matches if turned on
        if (particleCollisionSound.useLayerFilter && !particleCollisionSound.matchingLayers.Contains(other.gameObject.layer)) {
            return;
        }

        if (particleCollisionSound.useTagFilter && !particleCollisionSound.matchingTags.Contains(other.gameObject.tag)) {
            return;
        }

        PlaySounds(particleCollisionSound, EventType.OnParticleCollision);
    }
    #endregion

#if UNITY_4_6 || UNITY_5_0
    #region UI Events
    public void OnPointerEnter(PointerEventData data) {
        if (IsSetToUGUI && useUnityPointerEnterSound) {
            PlaySounds(unityPointerEnterSound, EventType.UnityPointerEnter);
        }
    }

    public void OnPointerExit(PointerEventData data) {
        if (IsSetToUGUI && useUnityPointerExitSound) {
            PlaySounds(unityPointerExitSound, EventType.UnityPointerExit);
        }
    }

    public void OnPointerDown(PointerEventData data) {
        if (IsSetToUGUI && useUnityPointerDownSound) {
            PlaySounds(unityPointerDownSound, EventType.UnityPointerDown);
        }
    }

    public void OnPointerUp(PointerEventData data) {
        if (IsSetToUGUI && useUnityPointerUpSound) {
            PlaySounds(unityPointerUpSound, EventType.UnityPointerUp);
        }
    }

    public void OnDrag(PointerEventData data) {
        if (IsSetToUGUI && useUnityDragSound) {
            PlaySounds(unityDragSound, EventType.UnityDrag);
        }
    }

    public void OnDrop(PointerEventData data) {
        if (IsSetToUGUI && useUnityDropSound) {
            PlaySounds(unityDropSound, EventType.UnityDrop);
        }
    }

    public void OnScroll(PointerEventData data) {
        if (IsSetToUGUI && useUnityScrollSound) {
            PlaySounds(unityScrollSound, EventType.UnityScroll);
        }
    }

    public void OnUpdateSelected(BaseEventData data) {
        if (IsSetToUGUI && useUnityUpdateSelectedSound) {
            PlaySounds(unityUpdateSelectedSound, EventType.UnityUpdateSelected);
        }
    }

    public void OnSelect(BaseEventData data) {
        if (IsSetToUGUI && useUnitySelectSound) {
            PlaySounds(unitySelectSound, EventType.UnitySelect);
        }
    }

    public void OnDeselect(BaseEventData data) {
        if (IsSetToUGUI && useUnityDeselectSound) {
            PlaySounds(unityDeselectSound, EventType.UnityDeselect);
        }
    }

    public void OnMove(AxisEventData data) {
        if (IsSetToUGUI && useUnityMoveSound) {
            PlaySounds(unityMoveSound, EventType.UnityMove);
        }
    }

    public void OnInitializePotentialDrag(PointerEventData data) {
        if (IsSetToUGUI && useUnityInitializePotentialDragSound) {
            PlaySounds(unityInitializePotentialDragSound, EventType.UnityInitializePotentialDrag);
        }
    }

    public void OnBeginDrag(PointerEventData data) {
        if (IsSetToUGUI && useUnityBeginDragSound) {
            PlaySounds(unityBeginDragSound, EventType.UnityBeginDrag);
        }
    }

    public void OnEndDrag(PointerEventData data) {
        if (IsSetToUGUI && useUnityEndDragSound) {
            PlaySounds(unityEndDragSound, EventType.UnityEndDrag);
        }
    }

    public void OnSubmit(BaseEventData data) {
        if (IsSetToUGUI && useUnitySubmitSound) {
            PlaySounds(unitySubmitSound, EventType.UnitySubmit);
        }
    }

    public void OnCancel(BaseEventData data) {
        if (IsSetToUGUI && useUnityCancelSound) {
            PlaySounds(unityCancelSound, EventType.UnityCancel);
        }
    }

    #endregion

    #region Unity UI Events (4.6)
    private void SliderChanged(float newValue) {
        if (!useUnitySliderChangedSound) {
            return;
        }

        unitySliderChangedSound.sliderValue = newValue;
        PlaySounds(unitySliderChangedSound, EventType.UnitySliderChanged);
    }

    private void ButtonClicked() {
        if (useUnityButtonClickedSound) {
            PlaySounds(unityButtonClickedSound, EventType.UnityButtonClicked);
        }
    }
    #endregion
#endif

    #region Unity GUI Mouse Events

    // ReSharper disable once UnusedMember.Local
    private bool IsSetToUGUI {
        get {
            return unityUIMode != UnityUIVersion.Legacy;
        }
    }

    private bool IsSetToLegacyUI {
        get {
            return unityUIMode == UnityUIVersion.Legacy;
        }
    }

#if UNITY_IPHONE || UNITY_ANDROID
	// no mouse events!
#else
    // ReSharper disable once UnusedMember.Local
    void OnMouseEnter() {
        if (IsSetToLegacyUI && useMouseEnterSound) {
            PlaySounds(mouseEnterSound, EventType.OnMouseEnter);
        }
    }

    // ReSharper disable once UnusedMember.Local
    void OnMouseExit() {
        if (IsSetToLegacyUI && useMouseExitSound) {
            PlaySounds(mouseExitSound, EventType.OnMouseExit);
        }
    }

    // ReSharper disable once UnusedMember.Local
    void OnMouseDown() {
        if (IsSetToLegacyUI && useMouseClickSound) {
            PlaySounds(mouseClickSound, EventType.OnMouseClick);
        }
    }

    // ReSharper disable once UnusedMember.Local
    void OnMouseUp() {
        if (IsSetToLegacyUI && useMouseUpSound) {
            PlaySounds(mouseUpSound, EventType.OnMouseUp);
        }

        if (useMouseDragSound) {
            switch (mouseUpSound.mouseDragStopMode) {
                case PreviousSoundStopMode.Stop:
                    // stop the drag sound
                    if (_mouseDragResult != null && (_mouseDragResult.SoundPlayed || _mouseDragResult.SoundScheduled)) {
                        _mouseDragResult.ActingVariation.Stop(true);
                    }
                    break;
                case PreviousSoundStopMode.FadeOut:
                    // stop the drag sound
                    if (_mouseDragResult != null && (_mouseDragResult.SoundPlayed || _mouseDragResult.SoundScheduled)) {
                        _mouseDragResult.ActingVariation.FadeToVolume(0f, mouseUpSound.mouseDragFadeOutTime);
                    }
                    break;
            }

            _mouseDragResult = null;
        }

        _mouseDragSoundPlayed = false; // can play drag sound again next time
    }

    // ReSharper disable once UnusedMember.Local
    void OnMouseDrag() {
        if (!IsSetToLegacyUI || !useMouseDragSound) {
            return;
        }
        if (_mouseDragSoundPlayed) {
            return;
        }
        PlaySounds(mouseDragSound, EventType.OnMouseDrag);
        _mouseDragSoundPlayed = true;
    }
#endif

    #endregion

    #region NGUI Events
    // ReSharper disable once UnusedMember.Local
    void OnPress(bool isDown) {
        if (!showNGUI) {
            return;
        }

        if (isDown) {
            if (useNguiMouseDownSound) {
                PlaySounds(nguiMouseDownSound, EventType.NGUIMouseDown);
            }
        } else {
            if (useNguiMouseUpSound) {
                PlaySounds(nguiMouseUpSound, EventType.NGUIMouseUp);
            }
        }
    }

    // ReSharper disable once UnusedMember.Local
    void OnClick() {
        if (showNGUI && useNguiOnClickSound) {
            PlaySounds(nguiOnClickSound, EventType.NGUIOnClick);
        }
    }

    // ReSharper disable once UnusedMember.Local
    void OnHover(bool isOver) {
        if (!showNGUI) {
            return;
        }

        if (isOver) {
            if (useNguiMouseEnterSound) {
                PlaySounds(nguiMouseEnterSound, EventType.NGUIMouseEnter);
            }
        } else {
            if (useNguiMouseExitSound) {
                PlaySounds(nguiMouseExitSound, EventType.NGUIMouseExit);
            }
        }
    }
    #endregion

    #region Pooling Events
    // ReSharper disable once UnusedMember.Local
    void OnSpawned() {
        SpawnedOrAwake();

        if (showPoolManager && useSpawnedSound) {
            PlaySounds(spawnedSound, EventType.OnSpawned);
        }
    }

    // ReSharper disable once UnusedMember.Local
    void OnDespawned() {
        if (showPoolManager && useDespawnedSound) {
            PlaySounds(despawnedSound, EventType.OnDespawned);
        }
    }
    #endregion

    // ReSharper disable once UnusedMember.Local
    void OnDrawGizmos() {
        if (showGizmo) {
            Gizmos.DrawIcon(transform.position, MasterAudio.GizmoFileName, true);
        }
    }

    private IEnumerator TryPlayStartSound(AudioEventGroup grp, AudioEvent aEvent) {
        for (var i = 0; i < 3; i++) {
            if (MasterAudio.IgnoreTimeScale) {
                yield return StartCoroutine(CoroutineHelper.WaitForActualSeconds(MasterAudio.InnerLoopCheckInterval));
            } else {
                yield return MasterAudio.InnerLoopDelay;
            }

            var result = PerformSingleAction(grp, aEvent, EventType.OnStart, false);
            if (result != null && result.SoundPlayed) {
                break;
            }
        }
    }

    private static bool CheckForRetriggerLimit(AudioEventGroup grp) {
        // check for limiting restraints
        switch (grp.retriggerLimitMode) {
            case RetriggerLimMode.FrameBased:
                if (grp.triggeredLastFrame > 0 && Time.frameCount - grp.triggeredLastFrame < grp.limitPerXFrm) {
                    return false;
                }
                break;
            case RetriggerLimMode.TimeBased:
                if (grp.triggeredLastTime > 0 && Time.time - grp.triggeredLastTime < grp.limitPerXSec) {
                    return false;
                }
                break;
        }

        return true;
    }

    public void PlaySounds(AudioEventGroup eventGrp, EventType eType) {
        if (!CheckForRetriggerLimit(eventGrp)) {
            return;
        }

        if (MasterAudio.SafeInstance == null) {
            return;
        }

        // set the last triggered time or frame
        switch (eventGrp.retriggerLimitMode) {
            case RetriggerLimMode.FrameBased:
                eventGrp.triggeredLastFrame = Time.frameCount;
                break;
            case RetriggerLimMode.TimeBased:
                eventGrp.triggeredLastTime = Time.time;
                break;
        }

        // Pre-warm event sounds!
        if (!MasterAudio.AppIsShuttingDown && MasterAudio.IsWarming) {
            var evt = new AudioEvent();
            PerformSingleAction(eventGrp, evt, eType);
            return;
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < eventGrp.SoundEvents.Count; i++) {
            PerformSingleAction(eventGrp, eventGrp.SoundEvents[i], eType);
        }
    }

    // ReSharper disable once FunctionComplexityOverflow
    private PlaySoundResult PerformSingleAction(AudioEventGroup grp, AudioEvent aEvent, EventType eType, bool isFirstTry = true) {
        if (disableSounds || MasterAudio.AppIsShuttingDown || MasterAudio.SafeInstance == null) {
            return null;
        }

        var useSliderValue = (eType == EventType.UnitySliderChanged && aEvent.targetVolMode == AudioEvent.TargetVolumeMode.UseSliderValue);
        var volume = aEvent.volume;
        var sType = aEvent.soundType;
        float? pitch = aEvent.pitch;
        if (!aEvent.useFixedPitch) {
            pitch = null;
        }

        PlaySoundResult soundPlayed = null;

        var soundSpawnModeToUse = soundSpawnMode;

        if (eType == EventType.OnDisable || eType == EventType.OnDespawned) {
            soundSpawnModeToUse = MasterAudio.SoundSpawnLocationMode.CallerLocation;
        }

        // these events need a PlaySoundResult, the rest do not. Save on allocation!
        var needsResult = (eType == EventType.OnStart && !isFirstTry) || eType == EventType.OnMouseDrag;

        switch (aEvent.currentSoundFunctionType) {
            case MasterAudio.EventSoundFunctionType.PlaySound:
                string variationName = null;
                if (aEvent.variationType == VariationType.PlaySpecific) {
                    variationName = aEvent.variationName;
                }

                if (eType == EventType.OnStart && isFirstTry && !MasterAudio.SoundGroupExists(sType)) {
                    // don't try to play sound yet.
                } else {
                    if (useSliderValue) {
                        volume = grp.sliderValue;
                    }

                    switch (soundSpawnModeToUse) {
                        case MasterAudio.SoundSpawnLocationMode.CallerLocation:
                            if (needsResult) {
                                soundPlayed = MasterAudio.PlaySound3DAtTransform(sType, _trans, volume, pitch, aEvent.delaySound, variationName);
                            } else {
                                MasterAudio.PlaySound3DAtTransformAndForget(sType, _trans, volume, pitch, aEvent.delaySound, variationName);
                            }
                            break;
                        case MasterAudio.SoundSpawnLocationMode.AttachToCaller:
                            if (needsResult) {
                                soundPlayed = MasterAudio.PlaySound3DFollowTransform(sType, _trans, volume, pitch, aEvent.delaySound, variationName);
                            } else {
                                MasterAudio.PlaySound3DFollowTransformAndForget(sType, _trans, volume, pitch, aEvent.delaySound, variationName);
                            }
                            break;
                        case MasterAudio.SoundSpawnLocationMode.MasterAudioLocation:
                            if (needsResult) {
                                soundPlayed = MasterAudio.PlaySound(sType, volume, pitch, aEvent.delaySound, variationName);
                            } else {
                                MasterAudio.PlaySoundAndForget(sType, volume, pitch, aEvent.delaySound, variationName);
                            }
                            break;
                    }
                }

#if UNITY_IPHONE || UNITY_ANDROID
				// no mouse events!
#else
                if (eType == EventType.OnMouseDrag) {
                    _mouseDragResult = soundPlayed;
                }
#endif

                if (soundPlayed == null || !soundPlayed.SoundPlayed) {
                    if (eType == EventType.OnStart && isFirstTry) {
                        // race condition met. So try to play it a few more times.
                        StartCoroutine(TryPlayStartSound(grp, aEvent));
                    }
                    return soundPlayed;
                }
                break;
            case MasterAudio.EventSoundFunctionType.PlaylistControl:
                soundPlayed = new PlaySoundResult() {
                    ActingVariation = null,
                    SoundPlayed = true,
                    SoundScheduled = false
                };

                if (string.IsNullOrEmpty(aEvent.playlistControllerName)) {
                    aEvent.playlistControllerName = MasterAudio.OnlyPlaylistControllerName;
                }

                if (MasterAudio.PlaylistCommandsThatFailOnStart.Contains(aEvent.currentPlaylistCommand) && eType == EventType.OnStart && isFirstTry) {
                    StartCoroutine(TryPlayStartSound(grp, aEvent));
                } else {
                    switch (aEvent.currentPlaylistCommand) {
                        case MasterAudio.PlaylistCommand.None:
                            soundPlayed.SoundPlayed = false;
                            break;
                        case MasterAudio.PlaylistCommand.Restart:
                            if (aEvent.allPlaylistControllersForGroupCmd) {
                                MasterAudio.RestartAllPlaylists();
                            } else if (aEvent.playlistControllerName == MasterAudio.NoGroupName) {
                                // don't play	
                            } else {
                                MasterAudio.RestartPlaylist(aEvent.playlistControllerName);
                            }
                            break;
                        case MasterAudio.PlaylistCommand.Start:
                            if (aEvent.playlistControllerName == MasterAudio.NoGroupName || aEvent.playlistName == MasterAudio.NoGroupName) {
                                // don't play	
                            } else {
                                MasterAudio.StartPlaylist(aEvent.playlistControllerName, aEvent.playlistName);
                            }
                            break;
                        case MasterAudio.PlaylistCommand.ChangePlaylist:
                            if (string.IsNullOrEmpty(aEvent.playlistName)) {
                                Debug.Log("You have not specified a Playlist name for Event Sounds on '" + _trans.name + "'.");
                                soundPlayed.SoundPlayed = false;
                            } else {
                                if (aEvent.playlistControllerName == MasterAudio.NoGroupName) {
                                    // don't play	
                                } else {
                                    MasterAudio.ChangePlaylistByName(aEvent.playlistControllerName, aEvent.playlistName, aEvent.startPlaylist);
                                }
                            }

                            break;
                        case MasterAudio.PlaylistCommand.FadeToVolume:
                            var targetVol = useSliderValue ? grp.sliderValue : aEvent.fadeVolume;

                            if (aEvent.allPlaylistControllersForGroupCmd) {
                                MasterAudio.FadeAllPlaylistsToVolume(targetVol, aEvent.fadeTime);
                            } else if (aEvent.playlistControllerName == MasterAudio.NoGroupName) {
                                // don't play	
                            } else {
                                MasterAudio.FadePlaylistToVolume(aEvent.playlistControllerName, targetVol, aEvent.fadeTime);
                            }
                            break;
                        case MasterAudio.PlaylistCommand.Mute:
                            if (aEvent.allPlaylistControllersForGroupCmd) {
                                MasterAudio.MuteAllPlaylists();
                            } else if (aEvent.playlistControllerName == MasterAudio.NoGroupName) {
                                // don't play	
                            } else {
                                MasterAudio.MutePlaylist(aEvent.playlistControllerName);
                            }
                            break;
                        case MasterAudio.PlaylistCommand.Unmute:
                            if (aEvent.allPlaylistControllersForGroupCmd) {
                                MasterAudio.UnmuteAllPlaylists();
                            } else if (aEvent.playlistControllerName == MasterAudio.NoGroupName) {
                                // don't play	
                            } else {
                                MasterAudio.UnmutePlaylist(aEvent.playlistControllerName);
                            }
                            break;
                        case MasterAudio.PlaylistCommand.ToggleMute:
                            if (aEvent.allPlaylistControllersForGroupCmd) {
                                MasterAudio.ToggleMuteAllPlaylists();
                            } else if (aEvent.playlistControllerName == MasterAudio.NoGroupName) {
                                // don't play	
                            } else {
                                MasterAudio.ToggleMutePlaylist(aEvent.playlistControllerName);
                            }
                            break;
                        case MasterAudio.PlaylistCommand.PlayClip:
                            if (string.IsNullOrEmpty(aEvent.clipName)) {
                                Debug.Log("You have not specified a clip name for Event Sounds on '" + _trans.name + "'.");
                                soundPlayed.SoundPlayed = false;
                            } else {
                                if (aEvent.playlistControllerName == MasterAudio.NoGroupName) {
                                    // don't play	
                                } else {
                                    if (!MasterAudio.TriggerPlaylistClip(aEvent.playlistControllerName, aEvent.clipName)) {
                                        soundPlayed.SoundPlayed = false;
                                    }
                                }
                            }

                            break;
                        case MasterAudio.PlaylistCommand.PlayRandomSong:
                            if (aEvent.allPlaylistControllersForGroupCmd) {
                                MasterAudio.TriggerRandomClipAllPlaylists();
                            } else if (aEvent.playlistControllerName == MasterAudio.NoGroupName) {
                                // don't play	
                            } else {
                                MasterAudio.TriggerRandomPlaylistClip(aEvent.playlistControllerName);
                            }
                            break;
                        case MasterAudio.PlaylistCommand.PlayNextSong:
                            if (aEvent.allPlaylistControllersForGroupCmd) {
                                MasterAudio.TriggerNextClipAllPlaylists();
                            } else if (aEvent.playlistControllerName == MasterAudio.NoGroupName) {
                                // don't play	
                            } else {
                                MasterAudio.TriggerNextPlaylistClip(aEvent.playlistControllerName);
                            }
                            break;
                        case MasterAudio.PlaylistCommand.Pause:
                            if (aEvent.allPlaylistControllersForGroupCmd) {
                                MasterAudio.PauseAllPlaylists();
                            } else if (aEvent.playlistControllerName == MasterAudio.NoGroupName) {
                                // don't play	
                            } else {
                                MasterAudio.PausePlaylist(aEvent.playlistControllerName);
                            }
                            break;
                        case MasterAudio.PlaylistCommand.Stop:
                            if (aEvent.allPlaylistControllersForGroupCmd) {
                                MasterAudio.StopAllPlaylists();
                            } else if (aEvent.playlistControllerName == MasterAudio.NoGroupName) {
                                // don't play	
                            } else {
                                MasterAudio.StopPlaylist(aEvent.playlistControllerName);
                            }
                            break;
                        case MasterAudio.PlaylistCommand.Resume:
                            if (aEvent.allPlaylistControllersForGroupCmd) {
                                MasterAudio.ResumeAllPlaylists();
                            } else if (aEvent.playlistControllerName == MasterAudio.NoGroupName) {
                                // don't play	
                            } else {
                                MasterAudio.ResumePlaylist(aEvent.playlistControllerName);
                            }
                            break;
                    }
                }
                break;
            case MasterAudio.EventSoundFunctionType.GroupControl:
                soundPlayed = new PlaySoundResult() {
                    ActingVariation = null,
                    SoundPlayed = true,
                    SoundScheduled = false
                };

                var soundTypesForCmd = new List<string>();
                if (!aEvent.allSoundTypesForGroupCmd || MasterAudio.GroupCommandsWithNoAllGroupSelector.Contains(aEvent.currentSoundGroupCommand)) {
                    soundTypesForCmd.Add(aEvent.soundType);
                } else {
                    soundTypesForCmd.AddRange(MasterAudio.RuntimeSoundGroupNames);
                }

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < soundTypesForCmd.Count; i++) {
                    var soundType = soundTypesForCmd[i];

                    switch (aEvent.currentSoundGroupCommand) {
                        case MasterAudio.SoundGroupCommand.None:
                            soundPlayed.SoundPlayed = false;
                            break;
                        case MasterAudio.SoundGroupCommand.RefillSoundGroupPool:
                            MasterAudio.RefillSoundGroupPool(soundType);
                            break;
                        case MasterAudio.SoundGroupCommand.FadeToVolume:
                            var targetVol = useSliderValue ? grp.sliderValue : aEvent.fadeVolume;
                            MasterAudio.FadeSoundGroupToVolume(soundType, targetVol, aEvent.fadeTime);
                            break;
                        case MasterAudio.SoundGroupCommand.FadeOutAllOfSound:
                            MasterAudio.FadeOutAllOfSound(soundType, aEvent.fadeTime);
                            break;
                        case MasterAudio.SoundGroupCommand.Mute:
                            MasterAudio.MuteGroup(soundType);
                            break;
                        case MasterAudio.SoundGroupCommand.Pause:
                            MasterAudio.PauseSoundGroup(soundType);
                            break;
                        case MasterAudio.SoundGroupCommand.Solo:
                            MasterAudio.SoloGroup(soundType);
                            break;
                        case MasterAudio.SoundGroupCommand.StopAllOfSound:
                            MasterAudio.StopAllOfSound(soundType);
                            break;
                        case MasterAudio.SoundGroupCommand.Unmute:
                            MasterAudio.UnmuteGroup(soundType);
                            break;
                        case MasterAudio.SoundGroupCommand.Unpause:
                            MasterAudio.UnpauseSoundGroup(soundType);
                            break;
                        case MasterAudio.SoundGroupCommand.Unsolo:
                            MasterAudio.UnsoloGroup(soundType);
                            break;
                        case MasterAudio.SoundGroupCommand.StopAllSoundsOfTransform:
                            MasterAudio.StopAllSoundsOfTransform(_trans);
                            break;
                        case MasterAudio.SoundGroupCommand.StopSoundGroupOfTransform:
                            MasterAudio.StopSoundGroupOfTransform(_trans, soundType);
                            break;
                        case MasterAudio.SoundGroupCommand.PauseAllSoundsOfTransform:
                            MasterAudio.PauseAllSoundsOfTransform(_trans);
                            break;
                        case MasterAudio.SoundGroupCommand.PauseSoundGroupOfTransform:
                            MasterAudio.PauseSoundGroupOfTransform(_trans, soundType);
                            break;
                        case MasterAudio.SoundGroupCommand.UnpauseAllSoundsOfTransform:
                            MasterAudio.UnpauseAllSoundsOfTransform(_trans);
                            break;
                        case MasterAudio.SoundGroupCommand.UnpauseSoundGroupOfTransform:
                            MasterAudio.UnpauseSoundGroupOfTransform(_trans, soundType);
                            break;
                        case MasterAudio.SoundGroupCommand.FadeOutSoundGroupOfTransform:
                            MasterAudio.FadeOutSoundGroupOfTransform(_trans, soundType, aEvent.fadeTime);
                            break;
                    }
                }

                break;
            case MasterAudio.EventSoundFunctionType.BusControl:
                soundPlayed = new PlaySoundResult() {
                    ActingVariation = null,
                    SoundPlayed = true,
                    SoundScheduled = false
                };

                var busesForCmd = new List<string>();
                if (!aEvent.allSoundTypesForBusCmd) {
                    busesForCmd.Add(aEvent.busName);
                } else {
                    busesForCmd.AddRange(MasterAudio.RuntimeBusNames);
                }

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < busesForCmd.Count; i++) {
                    var busName = busesForCmd[i];

                    switch (aEvent.currentBusCommand) {
                        case MasterAudio.BusCommand.None:
                            soundPlayed.SoundPlayed = false;
                            break;
                        case MasterAudio.BusCommand.FadeToVolume:
                            var targetVol = useSliderValue ? grp.sliderValue : aEvent.fadeVolume;
                            MasterAudio.FadeBusToVolume(busName, targetVol, aEvent.fadeTime);
                            break;
                        case MasterAudio.BusCommand.Pause:
                            MasterAudio.PauseBus(busName);
                            break;
                        case MasterAudio.BusCommand.Stop:
                            MasterAudio.StopBus(busName);
                            break;
                        case MasterAudio.BusCommand.Unpause:
                            MasterAudio.UnpauseBus(busName);
                            break;
                        case MasterAudio.BusCommand.Mute:
                            MasterAudio.MuteBus(busName);
                            break;
                        case MasterAudio.BusCommand.Unmute:
                            MasterAudio.UnmuteBus(busName);
                            break;
                        case MasterAudio.BusCommand.Solo:
                            MasterAudio.SoloBus(busName);
                            break;
                        case MasterAudio.BusCommand.Unsolo:
                            MasterAudio.UnsoloBus(busName);
                            break;
                        case MasterAudio.BusCommand.ChangeBusPitch:
                            MasterAudio.ChangeBusPitch(busName, aEvent.pitch);
                            break;
                    }
                }

                break;
            case MasterAudio.EventSoundFunctionType.CustomEventControl:
                soundPlayed = new PlaySoundResult() {
                    ActingVariation = null,
                    SoundPlayed = false,
                    SoundScheduled = false
                };

                if (eType == EventType.UserDefinedEvent) {
                    Debug.LogError("Custom Event Receivers cannot fire events. Occured in Transform '" + name + "'.");
                    break;
                }
                switch (aEvent.currentCustomEventCommand) {
                    case MasterAudio.CustomEventCommand.FireEvent:
                        MasterAudio.FireCustomEvent(aEvent.theCustomEventName, _trans.position);
                        break;
                }
                break;
            case MasterAudio.EventSoundFunctionType.GlobalControl:
                switch (aEvent.currentGlobalCommand) {
                    case MasterAudio.GlobalCommand.SetMasterMixerVolume:
                        var targetVol = useSliderValue ? grp.sliderValue : aEvent.volume;
                        MasterAudio.MasterVolumeLevel = targetVol;
                        break;
                    case MasterAudio.GlobalCommand.SetMasterPlaylistVolume:
                        var tgtVol = useSliderValue ? grp.sliderValue : aEvent.volume;
                        MasterAudio.PlaylistMasterVolume = tgtVol;
                        break;
                    case MasterAudio.GlobalCommand.PauseMixer:
                        MasterAudio.PauseMixer();
                        break;
                    case MasterAudio.GlobalCommand.UnpauseMixer:
                        MasterAudio.UnpauseMixer();
                        break;
                    case MasterAudio.GlobalCommand.StopMixer:
                        MasterAudio.StopMixer();
                        break;
                    case MasterAudio.GlobalCommand.MuteEverything:
                        MasterAudio.MuteEverything();
                        break;
                    case MasterAudio.GlobalCommand.UnmuteEverything:
                        MasterAudio.UnmuteEverything();
                        break;
                    case MasterAudio.GlobalCommand.PauseEverything:
                        MasterAudio.PauseEverything();
                        break;
                    case MasterAudio.GlobalCommand.UnpauseEverything:
                        MasterAudio.UnpauseEverything();
                        break;
                    case MasterAudio.GlobalCommand.StopEverything:
                        MasterAudio.StopEverything();
                        break;
                }
                break;
#if UNITY_5_0
            case MasterAudio.EventSoundFunctionType.UnityMixerControl:
                switch (aEvent.currentMixerCommand) {
                    case MasterAudio.UnityMixerCommand.TransitionToSnapshot:
                        var snapshot = aEvent.snapshotToTransitionTo;
                        if (snapshot != null) {
                            snapshot.audioMixer.TransitionToSnapshots(
                                new[] { snapshot },
                                new float[] { 1f },
                                aEvent.snapshotTransitionTime);
                        }
                        break;
                    case MasterAudio.UnityMixerCommand.TransitionToSnapshotBlend:
                        var snapshots = new List<AudioMixerSnapshot>();
                        var weights = new List<float>();
                        AudioMixer theMixer = null;

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < aEvent.snapshotsToBlend.Count; i++) {
                            var aSnap = aEvent.snapshotsToBlend[i];
                            if (aSnap.snapshot == null) {
                                continue;
                            }

                            if (theMixer == null) {
                                theMixer = aSnap.snapshot.audioMixer;
                            } else if (theMixer != aSnap.snapshot.audioMixer) {
                                Debug.LogError("Snapshot '" + aSnap.snapshot.name + "' isn't in the same Audio Mixer as the previous snapshot in EventSounds on GameObject '" + name + "'. Please make sure all the Snapshots to blend are on the same mixer.");
                                break;
                            }

                            snapshots.Add(aSnap.snapshot);
                            weights.Add(aSnap.weight);
                        }

                        if (snapshots.Count > 0) {
                            Debug.Log("trans");
                            // ReSharper disable once PossibleNullReferenceException
                            theMixer.TransitionToSnapshots(snapshots.ToArray(), weights.ToArray(), aEvent.snapshotTransitionTime);
                        }

                        break;
                }
                break;
#endif
            case MasterAudio.EventSoundFunctionType.PersistentSettingsControl:
                switch (aEvent.currentPersistentSettingsCommand) {
                    case MasterAudio.PersistentSettingsCommand.SetBusVolume:
                        var busesForCommand = new List<string>();
                        if (!aEvent.allSoundTypesForBusCmd) {
                            busesForCommand.Add(aEvent.busName);
                        } else {
                            busesForCommand.AddRange(MasterAudio.RuntimeBusNames);
                        }

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < busesForCommand.Count; i++) {
                            var aBusName = busesForCommand[i];
                            var tgtVol = useSliderValue ? grp.sliderValue : aEvent.volume;
                            PersistentAudioSettings.SetBusVolume(aBusName, tgtVol);
                        }
                        break;
                    case MasterAudio.PersistentSettingsCommand.SetGroupVolume:
                        var groupsForCommand = new List<string>();
                        if (!aEvent.allSoundTypesForGroupCmd) {
                            groupsForCommand.Add(aEvent.soundType);
                        } else {
                            groupsForCommand.AddRange(MasterAudio.RuntimeSoundGroupNames);
                        }

                        // ReSharper disable once ForCanBeConvertedToForeach
                        for (var i = 0; i < groupsForCommand.Count; i++) {
                            var aGroupName = groupsForCommand[i];
                            var tgtVol = useSliderValue ? grp.sliderValue : aEvent.volume;
                            PersistentAudioSettings.SetGroupVolume(aGroupName, tgtVol);
                        }
                        break;
                    case MasterAudio.PersistentSettingsCommand.SetMixerVolume:
                        var targetVol = useSliderValue ? grp.sliderValue : aEvent.volume;
                        PersistentAudioSettings.MixerVolume = targetVol;
                        break;
                    case MasterAudio.PersistentSettingsCommand.SetMusicVolume:
                        var targVol = useSliderValue ? grp.sliderValue : aEvent.volume;
                        PersistentAudioSettings.MusicVolume = targVol;
                        break;
                }
                break;
        }

        if (aEvent.emitParticles && soundPlayed != null && (soundPlayed.SoundPlayed || soundPlayed.SoundScheduled)) {
            MasterAudio.TriggerParticleEmission(_trans, aEvent.particleCountToEmit);
        }

        return soundPlayed;
    }

    private void LogIfCustomEventMissing(AudioEventGroup eventGroup) {
        if (!logMissingEvents) {
            return;
        }

        if (eventGroup.isCustomEvent) {
            if (!eventGroup.customSoundActive || string.IsNullOrEmpty(eventGroup.customEventName)) {
                return;
            }
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < eventGroup.SoundEvents.Count; i++) {
            var aEvent = eventGroup.SoundEvents[i];

            if (aEvent.currentSoundFunctionType != MasterAudio.EventSoundFunctionType.CustomEventControl) {
                continue;
            }

            var customEventName = aEvent.theCustomEventName;
            if (!MasterAudio.CustomEventExists(customEventName)) {
                MasterAudio.LogWarning("Transform '" + name + "' is set up to receive or fire Custom Event '" + customEventName + "', which does not exist in Master Audio.");
            }
        }
    }

    #region ICustomEventReceiver methods
    public void CheckForIllegalCustomEvents() {
        if (useStartSound) {
            LogIfCustomEventMissing(startSound);
        }
        if (useVisibleSound) {
            LogIfCustomEventMissing(visibleSound);
        }
        if (useInvisibleSound) {
            LogIfCustomEventMissing(invisibleSound);
        }
        if (useCollisionSound) {
            LogIfCustomEventMissing(collisionSound);
        }
        if (useCollisionExitSound) {
            LogIfCustomEventMissing(collisionExitSound);
        }
        if (useTriggerEnterSound) {
            LogIfCustomEventMissing(triggerSound);
        }
        if (useTriggerExitSound) {
            LogIfCustomEventMissing(triggerExitSound);
        }
        if (useMouseEnterSound) {
            LogIfCustomEventMissing(mouseEnterSound);
        }
        if (useMouseExitSound) {
            LogIfCustomEventMissing(mouseExitSound);
        }
        if (useMouseClickSound) {
            LogIfCustomEventMissing(mouseClickSound);
        }
        if (useMouseDragSound) {
            LogIfCustomEventMissing(mouseDragSound);
        }
        if (useMouseUpSound) {
            LogIfCustomEventMissing(mouseUpSound);
        }
        if (useNguiMouseDownSound) {
            LogIfCustomEventMissing(nguiMouseDownSound);
        }
        if (useNguiMouseUpSound) {
            LogIfCustomEventMissing(nguiMouseUpSound);
        }
        if (useNguiOnClickSound) {
            LogIfCustomEventMissing(nguiOnClickSound);
        }
        if (useNguiMouseEnterSound) {
            LogIfCustomEventMissing(nguiMouseEnterSound);
        }
        if (useNguiMouseExitSound) {
            LogIfCustomEventMissing(nguiMouseExitSound);
        }
        if (useSpawnedSound) {
            LogIfCustomEventMissing(spawnedSound);
        }
        if (useDespawnedSound) {
            LogIfCustomEventMissing(despawnedSound);
        }
        if (useEnableSound) {
            LogIfCustomEventMissing(enableSound);
        }
        if (useDisableSound) {
            LogIfCustomEventMissing(disableSound);
        }
        if (useCollision2dSound) {
            LogIfCustomEventMissing(collision2dSound);
        }
        if (useCollisionExit2dSound) {
            LogIfCustomEventMissing(collisionExit2dSound);
        }
        if (useTriggerEnter2dSound) {
            LogIfCustomEventMissing(triggerEnter2dSound);
        }
        if (useTriggerExit2dSound) {
            LogIfCustomEventMissing(triggerExit2dSound);
        }
        if (useParticleCollisionSound) {
            LogIfCustomEventMissing(particleCollisionSound);
        }

        if (useUnitySliderChangedSound) {
            LogIfCustomEventMissing(unitySliderChangedSound);
        }
        if (useUnityButtonClickedSound) {
            LogIfCustomEventMissing(unityButtonClickedSound);
        }
        if (useUnityPointerDownSound) {
            LogIfCustomEventMissing(unityPointerDownSound);
        }
        if (useUnityDragSound) {
            LogIfCustomEventMissing(unityDragSound);
        }
        if (useUnityDropSound) {
            LogIfCustomEventMissing(unityDropSound);
        }
        if (useUnityPointerUpSound) {
            LogIfCustomEventMissing(unityPointerUpSound);
        }
        if (useUnityPointerEnterSound) {
            LogIfCustomEventMissing(unityPointerEnterSound);
        }
        if (useUnityPointerExitSound) {
            LogIfCustomEventMissing(unityPointerExitSound);
        }
        if (useUnityScrollSound) {
            LogIfCustomEventMissing(unityScrollSound);
        }
        if (useUnityUpdateSelectedSound) {
            LogIfCustomEventMissing(unityUpdateSelectedSound);
        }
        if (useUnitySelectSound) {
            LogIfCustomEventMissing(unitySelectSound);
        }
        if (useUnityDeselectSound) {
            LogIfCustomEventMissing(unityDeselectSound);
        }
        if (useUnityMoveSound) {
            LogIfCustomEventMissing(unityMoveSound);
        }
        if (useUnityInitializePotentialDragSound) {
            LogIfCustomEventMissing(unityInitializePotentialDragSound);
        }
        if (useUnityBeginDragSound) {
            LogIfCustomEventMissing(unityBeginDragSound);
        }
        if (useUnityEndDragSound) {
            LogIfCustomEventMissing(unityEndDragSound);
        }
        if (useUnitySubmitSound) {
            LogIfCustomEventMissing(unitySubmitSound);
        }
        if (useUnityCancelSound) {
            LogIfCustomEventMissing(unityCancelSound);
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < userDefinedSounds.Count; i++) {
            var custEvent = userDefinedSounds[i];

            LogIfCustomEventMissing(custEvent);
        }
    }

    public void ReceiveEvent(string customEventName, Vector3 originPoint) {
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < userDefinedSounds.Count; i++) {
            var userDefGroup = userDefinedSounds[i];

            if (!userDefGroup.customSoundActive || string.IsNullOrEmpty(userDefGroup.customEventName)) {
                continue;
            }

            if (!userDefGroup.customEventName.Equals(customEventName)) {
                continue;
            }

            PlaySounds(userDefGroup, EventType.UserDefinedEvent);
        }
    }

    public bool SubscribesToEvent(string customEventName) {
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < userDefinedSounds.Count; i++) {
            var customGrp = userDefinedSounds[i];

            if (customGrp.customSoundActive && !string.IsNullOrEmpty(customGrp.customEventName) && customGrp.customEventName.Equals(customEventName)) {
                return true;
            }
        }

        return false;
    }

    public void RegisterReceiver() {
        if (userDefinedSounds.Count > 0) {
            MasterAudio.AddCustomEventReceiver(this, _trans);
        }
    }

    public void UnregisterReceiver() {
        if (userDefinedSounds.Count > 0) {
            MasterAudio.RemoveCustomEventReceiver(this);
        }
    }
    #endregion
}

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_5_0
	using UnityEngine.Audio;
#endif

[CustomEditor(typeof(PlaylistController))]
// ReSharper disable once CheckNamespace
public class PlaylistControllerInspector : Editor {
	private List<string> _customEventNames;

    // ReSharper disable once FunctionComplexityOverflow
	public override void OnInspectorGUI() {

		EditorGUIUtility.LookLikeControls();
        EditorGUI.indentLevel = 0;

        var controller = (PlaylistController)target;

        MasterAudio.Instance = null;

        var ma = MasterAudio.Instance;
        var maInScene = ma != null;

        if (maInScene) {
            DTGUIHelper.ShowHeaderTexture(MasterAudioInspectorResources.LogoTexture);
			_customEventNames = ma.CustomEventNames;
		}
		
		var isDirty = false;

		var newVol = DTGUIHelper.DisplayVolumeField(controller._playlistVolume, DTGUIHelper.VolumeFieldType.None, MasterAudio.MixerWidthMode.Normal, 0f, true, "Playlist Volume");
	    // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (newVol != controller._playlistVolume) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "change Playlist Volume");
            controller.PlaylistVolume = newVol;
        }

        if (maInScene) {
            // ReSharper disable once PossibleNullReferenceException
            var plNames = MasterAudio.Instance.PlaylistNames;

            var existingIndex = plNames.IndexOf(controller.startPlaylistName);

            int? groupIndex = null;

            var noPl = false;
            var noMatch = false;

            if (existingIndex >= 1) {
                groupIndex = EditorGUILayout.Popup("Initial Playlist", existingIndex, plNames.ToArray());
                if (existingIndex == 1) {
                    noPl = true;
                }
            } else if (existingIndex == -1 && controller.startPlaylistName == MasterAudio.NoGroupName) {
                groupIndex = EditorGUILayout.Popup("Initial Playlist", existingIndex, plNames.ToArray());
            } else { // non-match
                noMatch = true;
                var newPlaylist = EditorGUILayout.TextField("Initial Playlist", controller.startPlaylistName);
                if (newPlaylist != controller.startPlaylistName) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "change Initial Playlist");
                    controller.startPlaylistName = newPlaylist;
                }

                var newIndex = EditorGUILayout.Popup("All Playlists", -1, plNames.ToArray());
                if (newIndex >= 0) {
                    groupIndex = newIndex;
                }
            }

            if (noPl) {
                DTGUIHelper.ShowRedError("Initial Playlist not specified. No music will play.");
            } else if (noMatch) {
                DTGUIHelper.ShowRedError("Initial Playlist found no match. Type in or choose one from 'All Playlists'.");
            }

            if (groupIndex.HasValue) {
                if (existingIndex != groupIndex.Value) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "change Initial Playlist");
                }
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (groupIndex.Value == -1) {
                    controller.startPlaylistName = MasterAudio.NoGroupName;
                } else {
                    controller.startPlaylistName = plNames[groupIndex.Value];
                }
            }
        }


        var syncGroupList = new List<string>();
        for (var i = 0; i < 4; i++) {
            syncGroupList.Add((i + 1).ToString());
        }
        syncGroupList.Insert(0, MasterAudio.NoGroupName);

        var syncIndex = syncGroupList.IndexOf(controller.syncGroupNum.ToString());
        if (syncIndex == -1) {
            syncIndex = 0;
        }
        var newSync = EditorGUILayout.Popup("Controller Sync Group", syncIndex, syncGroupList.ToArray());
        if (newSync != syncIndex) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "change Controller Sync Group");
            controller.syncGroupNum = newSync;
        }

        EditorGUI.indentLevel = 0;

		#if UNITY_5_0
			var newChan = (AudioMixerGroup) EditorGUILayout.ObjectField("Mixer Group", controller.mixerChannel, typeof(AudioMixerGroup), false);	
			if (newChan != controller.mixerChannel) {
				UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "change Unity Mixer Group");
				controller.mixerChannel = newChan;

				if (Application.isPlaying) {
					controller.RouteToMixerChannel(newChan);
				}
			}

			if (!maInScene || ma.musicSpatialBlendType == MasterAudio.AllMusicSpatialBlendType.AllowDifferentPerController) {
				var newMusicSpatialType = (MasterAudio.ItemSpatialBlendType) EditorGUILayout.EnumPopup("Spatial Blend Rule", controller.spatialBlendType);
				if (newMusicSpatialType != controller.spatialBlendType) {
					UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "change Spatial Blend Rule");
					controller.spatialBlendType = newMusicSpatialType;
					if (Application.isPlaying) {
						controller.SetSpatialBlend();
					}
				}
				
				switch (controller.spatialBlendType) {
					case MasterAudio.ItemSpatialBlendType.ForceToCustom:
						EditorGUI.indentLevel = 1;
						DTGUIHelper.ShowLargeBarAlert(MasterAudioInspector.SpatialBlendSliderText);
						var newMusic3D = EditorGUILayout.Slider("Spatial Blend", controller.spatialBlend, 0f, 1f);
				        // ReSharper disable once CompareOfFloatsByEqualityOperator
						if (newMusic3D != controller.spatialBlend) {
							UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "change Spatial Blend");
							controller.spatialBlend = newMusic3D;

							if (Application.isPlaying) {
								controller.SetSpatialBlend();
							}
						}
						break;				
				}
			} else {
				DTGUIHelper.ShowLargeBarAlert("Spatial Blend is currently controlled globally in the Master Audio prefab.");
			}	

		#endif

		EditorGUI.indentLevel = 0;
		var newAwake = EditorGUILayout.Toggle("Start Playlist on Awake?", controller.startPlaylistOnAwake);
        if (newAwake != controller.startPlaylistOnAwake) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "toggle Start Playlist on Awake");
            controller.startPlaylistOnAwake = newAwake;
        }

        var newShuffle = EditorGUILayout.Toggle("Shuffle Mode", controller.isShuffle);
        if (newShuffle != controller.isShuffle) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "toggle Shuffle Mode");
            controller.isShuffle = newShuffle;
        }

        var newLoop = EditorGUILayout.Toggle("Loop Playlists", controller.loopPlaylist);
        if (newLoop != controller.loopPlaylist) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "toggle Loop Playlists");
            controller.loopPlaylist = newLoop;
        }

        var newAuto = EditorGUILayout.Toggle("Auto advance clips", controller.isAutoAdvance);
        if (newAuto != controller.isAutoAdvance) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "toggle Auto advance clips");
            controller.isAutoAdvance = newAuto;
        }

        DTGUIHelper.ShowColorWarning("Note: auto advance will not advance past a looped track.");

        DTGUIHelper.StartGroupHeader();

        var newUse = EditorGUILayout.BeginToggleGroup(" Song Changed Event", controller.songChangedEventExpanded);
        if (newUse != controller.songChangedEventExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "toggle expand Song Changed Event");
            controller.songChangedEventExpanded = newUse;
        }
        DTGUIHelper.EndGroupHeader();
        
		GUI.color = Color.white;

		if (controller.songChangedEventExpanded) {
			DTGUIHelper.ShowColorWarning("When song changes, fire Custom Event below.");

			if (maInScene) {
				var existingIndex = _customEventNames.IndexOf(controller.songChangedCustomEvent);
				
				int? customEventIndex = null;
				
				EditorGUI.indentLevel = 0;
				
				var noEvent = false;
				var noMatch = false;
				
				if (existingIndex >= 1) {
					customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, _customEventNames.ToArray());
					if (existingIndex == 1) {
						noEvent = true;
					}
				} else if (existingIndex == -1 && controller.songChangedCustomEvent == MasterAudio.NoGroupName) {
					customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, _customEventNames.ToArray());
				} else { // non-match
					noMatch = true;
					var newEventName = EditorGUILayout.TextField("Custom Event Name", controller.songChangedCustomEvent);
					if (newEventName != controller.songChangedCustomEvent) {
						UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "change Custom Event Name");
						controller.songChangedCustomEvent = newEventName;
					}
					
					var newIndex = EditorGUILayout.Popup("All Custom Events", -1, _customEventNames.ToArray());
					if (newIndex >= 0) {
						customEventIndex = newIndex;
					}
				}
				
				if (noEvent) {
					DTGUIHelper.ShowRedError("No Custom Event specified. This section will do nothing.");
				} else if (noMatch) {
					DTGUIHelper.ShowRedError("Custom Event found no match. Type in or choose one.");
				}
				
				if (customEventIndex.HasValue) {
					if (existingIndex != customEventIndex.Value) {
						UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "change Custom Event");
					}
				    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
					if (customEventIndex.Value == -1) {
						controller.songChangedCustomEvent = MasterAudio.NoGroupName;
					} else {
						controller.songChangedCustomEvent = _customEventNames[customEventIndex.Value];
					}
				}
			} else {
				var newCustomEvent = EditorGUILayout.TextField("Custom Event Name", controller.songChangedCustomEvent);
				if (newCustomEvent != controller.songChangedCustomEvent) {
					UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "Custom Event Name");
					controller.songChangedCustomEvent = newCustomEvent;
				}
			}
		}

        EditorGUILayout.EndToggleGroup();
        DTGUIHelper.AddSpaceForNonU5(2);

        DTGUIHelper.StartGroupHeader();

        newUse = EditorGUILayout.BeginToggleGroup(" Song Ended Event", controller.songEndedEventExpanded);
        if (newUse != controller.songEndedEventExpanded) {
            UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "toggle expand Song Ended Event");
            controller.songEndedEventExpanded = newUse;
        }
        DTGUIHelper.EndGroupHeader();

        GUI.color = Color.white;

        if (controller.songEndedEventExpanded) {
            DTGUIHelper.ShowColorWarning("When song ends, fire Custom Event below.");

            if (maInScene) {
                var existingIndex = _customEventNames.IndexOf(controller.songEndedCustomEvent);

                int? customEventIndex = null;

                EditorGUI.indentLevel = 0;

                var noEvent = false;
                var noMatch = false;

                if (existingIndex >= 1) {
                    customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, _customEventNames.ToArray());
                    if (existingIndex == 1) {
                        noEvent = true;
                    }
                } else if (existingIndex == -1 && controller.songEndedCustomEvent == MasterAudio.NoGroupName) {
                    customEventIndex = EditorGUILayout.Popup("Custom Event Name", existingIndex, _customEventNames.ToArray());
                } else { // non-match
                    noMatch = true;
                    var newEventName = EditorGUILayout.TextField("Custom Event Name", controller.songEndedCustomEvent);
                    if (newEventName != controller.songEndedCustomEvent) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "change Custom Event Name");
                        controller.songEndedCustomEvent = newEventName;
                    }

                    var newIndex = EditorGUILayout.Popup("All Custom Events", -1, _customEventNames.ToArray());
                    if (newIndex >= 0) {
                        customEventIndex = newIndex;
                    }
                }

                if (noEvent) {
                    DTGUIHelper.ShowRedError("No Custom Event specified. This section will do nothing.");
                } else if (noMatch) {
                    DTGUIHelper.ShowRedError("Custom Event found no match. Type in or choose one.");
                }

                if (customEventIndex.HasValue) {
                    if (existingIndex != customEventIndex.Value) {
                        UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "change Custom Event");
                    }
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (customEventIndex.Value == -1) {
                        controller.songEndedCustomEvent = MasterAudio.NoGroupName;
                    } else {
                        controller.songEndedCustomEvent = _customEventNames[customEventIndex.Value];
                    }
                }
            } else {
                var newCustomEvent = EditorGUILayout.TextField("Custom Event Name", controller.songEndedCustomEvent);
                if (newCustomEvent != controller.songEndedCustomEvent) {
                    UndoHelper.RecordObjectPropertyForUndo(ref isDirty, controller, "Custom Event Name");
                    controller.songEndedCustomEvent = newCustomEvent;
                }
            }
        }
        EditorGUILayout.EndToggleGroup();

		if (GUI.changed || isDirty) {
			EditorUtility.SetDirty(target);
		}
		
		//DrawDefaultInspector();
	}
}
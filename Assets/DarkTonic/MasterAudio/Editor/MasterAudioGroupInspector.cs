using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MasterAudioGroup))]
// ReSharper disable once CheckNamespace
public class MasterAudioGroupInspector : Editor {
    private static bool _isValid = true;
    private List<string> _groupNames;

    private MasterAudioGroup _group;
    private bool _isDirty;
    private GameObject _previewer;
    private MasterAudio _ma;

    // ReSharper disable once FunctionComplexityOverflow
    public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();

        _isValid = true;

        EditorGUI.indentLevel = 0;

        _ma = MasterAudio.Instance;
        var maInScene = _ma != null;

        if (!maInScene) {
            _isValid = false;
        }

        _group = (MasterAudioGroup)target;
        _group = RescanChildren(_group);

        if (!_isValid) {
            return;
        }

        if (maInScene) {
            _previewer = _ma.gameObject;
            _groupNames = _ma.GroupNames;
            _groupNames.Remove(_group.name);
        }

        var isInProjectView = DTGUIHelper.IsPrefabInProjectView(_group);

        if (MasterAudioInspectorResources.LogoTexture != null) {
            DTGUIHelper.ShowHeaderTexture(MasterAudioInspectorResources.LogoTexture);
        }

        var newVol = DTGUIHelper.DisplayVolumeField(_group.groupMasterVolume, DTGUIHelper.VolumeFieldType.None, MasterAudio.MixerWidthMode.Normal, 0f, true, "Group Master Volume");
        if (newVol != _group.groupMasterVolume) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Group Master Volume");
            _group.groupMasterVolume = newVol;
        }

#if UNITY_5_0
			if (MasterAudio.Instance.mixerSpatialBlendType == MasterAudio.AllMixerSpatialBlendType.AllowDifferentPerGroup) {
				var newSpatialType = (MasterAudio.ItemSpatialBlendType) EditorGUILayout.EnumPopup("Spatial Blend Rule", _group.spatialBlendType);
                if (newSpatialType != _group.spatialBlendType) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Spatial Blend Rule");
                    _group.spatialBlendType = newSpatialType;
					
					if (Application.isPlaying) {
                        MasterAudio.SetGroupSpatialBlend(_group.GameObjectName);
					}
				}

                switch (_group.spatialBlendType) {
					case MasterAudio.ItemSpatialBlendType.ForceToCustom:
						EditorGUI.indentLevel = 1;
						DTGUIHelper.ShowLargeBarAlert(MasterAudioInspector.SpatialBlendSliderText);
                        var newBlend = EditorGUILayout.Slider("Spatial Blend", _group.spatialBlend, 0f, 1f);
                        if (newBlend != _group.spatialBlend) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Spatial Blend");
                            _group.spatialBlend = newBlend;
							
							if (Application.isPlaying) {
                                MasterAudio.SetGroupSpatialBlend(_group.GameObjectName);
							}
						}
						break;				
				}
			}
#endif

        EditorGUI.indentLevel = 0;
        var newTargetGone = (MasterAudioGroup.TargetDespawnedBehavior)EditorGUILayout.EnumPopup("Caller Despawned Mode", _group.targetDespawnedBehavior);
        if (newTargetGone != _group.targetDespawnedBehavior) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "Change Caller Despawned Mode");
            _group.targetDespawnedBehavior = newTargetGone;
        }

        if (_group.targetDespawnedBehavior == MasterAudioGroup.TargetDespawnedBehavior.FadeOut) {
            EditorGUI.indentLevel = 1;
            var newFade = EditorGUILayout.Slider("Fade Out Time", _group.despawnFadeTime, .1f, 20f);
            if (newFade != _group.despawnFadeTime) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "Change Called Despawned Fade Out Time");
                _group.despawnFadeTime = newFade;
            }
        }

        var groupHasResource = false;
        foreach (var t in _group.groupVariations) {
            if (t.audLocation != MasterAudio.AudioLocation.ResourceFile) {
                continue;
            }

            groupHasResource = true;
            break;
        }

        if (MasterAudio.HasAsyncResourceLoaderFeature() && groupHasResource) {
            if (maInScene && !_ma.resourceClipsAllLoadAsync) {
                var newAsync = EditorGUILayout.Toggle(new GUIContent("Load Resources Async", "Checking this means Resource files in this Sound Group will be loaded asynchronously."), _group.resourceClipsAllLoadAsync);
                if (newAsync != _group.resourceClipsAllLoadAsync) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "toggle Load Resources Async");
                    _group.resourceClipsAllLoadAsync = newAsync;
                }
            }
        }

        EditorGUI.indentLevel = 0;
        if (MasterAudio.Instance.prioritizeOnDistance) {
            var newContinual = EditorGUILayout.Toggle("Use Clip Age Priority", _group.useClipAgePriority);
            if (newContinual != _group.useClipAgePriority) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "toggle Use Clip Age Priority");
                _group.useClipAgePriority = newContinual;
            }
        }

        if (MasterAudio.Instance.prioritizeOnDistance) {
            var hiPri = EditorGUILayout.Toggle("Always Highest Priority", _group.alwaysHighestPriority);
            if (hiPri != _group.alwaysHighestPriority) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "toggle Always Highest Priority");
                _group.alwaysHighestPriority = hiPri;
            }
        }

        var newLog = EditorGUILayout.Toggle("Log Sounds", _group.logSound);
        if (newLog != _group.logSound) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "toggle Log Sounds");
            _group.logSound = newLog;
        }

        EditorGUI.indentLevel = 0;

        DTGUIHelper.VerticalSpace(1);
        DTGUIHelper.StartGroupHeader();
        var newVarSequence = (MasterAudioGroup.VariationSequence)EditorGUILayout.EnumPopup("Variation Sequence", _group.curVariationSequence);
        if (newVarSequence != _group.curVariationSequence) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Variation Sequence");
            _group.curVariationSequence = newVarSequence;
        }
        EditorGUILayout.EndVertical();

        if (_group.curVariationSequence == MasterAudioGroup.VariationSequence.TopToBottom) {
            var newUseInactive = EditorGUILayout.BeginToggleGroup(" Refill Variation Pool After Inactive Time", _group.useInactivePeriodPoolRefill);
            if (newUseInactive != _group.useInactivePeriodPoolRefill) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "toggle Inactive Refill");
                _group.useInactivePeriodPoolRefill = newUseInactive;
            }

            EditorGUI.indentLevel = 1;
            var newInactivePeriod = EditorGUILayout.Slider(" Inactive Time (sec)", _group.inactivePeriodSeconds, .2f, 30f);
            if (newInactivePeriod != _group.inactivePeriodSeconds) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Inactive Time");
                _group.inactivePeriodSeconds = newInactivePeriod;
            }

            EditorGUILayout.EndToggleGroup();
        }
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel = 0;
        DTGUIHelper.AddSpaceForNonU5(2);
        DTGUIHelper.StartGroupHeader();
        var newVarMode = (MasterAudioGroup.VariationMode)EditorGUILayout.EnumPopup("Variation Mode", _group.curVariationMode);
        if (newVarMode != _group.curVariationMode) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Variation Mode");
            _group.curVariationMode = newVarMode;
        }
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel = 0;

        switch (_group.curVariationMode) {
            case MasterAudioGroup.VariationMode.Normal:
                var newRetrigger = EditorGUILayout.IntSlider("Retrigger Percentage", _group.retriggerPercentage, 0, 100);
                if (newRetrigger != _group.retriggerPercentage) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Retrigger Percentage");
                    _group.retriggerPercentage = newRetrigger;
                }

                var newLimitPoly = EditorGUILayout.Toggle("Limit Polyphony", _group.limitPolyphony);
                if (newLimitPoly != _group.limitPolyphony) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "toggle Limit Polyphony");
                    _group.limitPolyphony = newLimitPoly;
                }
                if (_group.limitPolyphony) {
                    var maxVoices = 0;
                    foreach (var variation in _group.groupVariations) {
                        maxVoices += variation.weight;
                    }

                    var newVoiceLimit = EditorGUILayout.IntSlider("Polyphony Voice Limit", _group.voiceLimitCount, 1, maxVoices);
                    if (newVoiceLimit != _group.voiceLimitCount) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Polyphony Voice Limit");
                        _group.voiceLimitCount = newVoiceLimit;
                    }
                }

                var newLimitMode = (MasterAudioGroup.LimitMode)EditorGUILayout.EnumPopup("Replay Limit Mode", _group.limitMode);
                if (newLimitMode != _group.limitMode) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Replay Limit Mode");
                    _group.limitMode = newLimitMode;
                }

                switch (_group.limitMode) {
                    case MasterAudioGroup.LimitMode.FrameBased:
                        var newFrameLimit = EditorGUILayout.IntSlider("Min Frames Between", _group.limitPerXFrames, 1, 120);
                        if (newFrameLimit != _group.limitPerXFrames) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Min Frames Between");
                            _group.limitPerXFrames = newFrameLimit;
                        }
                        break;
                    case MasterAudioGroup.LimitMode.TimeBased:
                        var newMinTime = EditorGUILayout.Slider("Min Seconds Between", _group.minimumTimeBetween, 0.1f, 10f);
                        if (newMinTime != _group.minimumTimeBetween) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Min Seconds Between");
                            _group.minimumTimeBetween = newMinTime;
                        }
                        break;
                }
                break;
            case MasterAudioGroup.VariationMode.LoopedChain:
                DTGUIHelper.ShowColorWarning("In this mode, only one Variation can be played at a time.");

                var newLoopMode = (MasterAudioGroup.ChainedLoopLoopMode)EditorGUILayout.EnumPopup("Loop Mode", _group.chainLoopMode);
                if (newLoopMode != _group.chainLoopMode) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Loop Mode");
                    _group.chainLoopMode = newLoopMode;
                }

                if (_group.chainLoopMode == MasterAudioGroup.ChainedLoopLoopMode.NumberOfLoops) {
                    var newLoopCount = EditorGUILayout.IntSlider("Number of Loops", _group.chainLoopNumLoops, 1, 500);
                    if (newLoopCount != _group.chainLoopNumLoops) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Number of Loops");
                        _group.chainLoopNumLoops = newLoopCount;
                    }
                }

                var newDelayMin = EditorGUILayout.Slider("Clip Change Delay Min", _group.chainLoopDelayMin, 0f, 20f);
                if (newDelayMin != _group.chainLoopDelayMin) {
                    if (_group.chainLoopDelayMax < newDelayMin) {
                        _group.chainLoopDelayMax = newDelayMin;
                    }
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Chained Clip Delay Min");
                    _group.chainLoopDelayMin = newDelayMin;
                }

                var newDelayMax = EditorGUILayout.Slider("Clip Change Delay Max", _group.chainLoopDelayMax, 0f, 20f);
                if (newDelayMax != _group.chainLoopDelayMax) {
                    if (newDelayMax < _group.chainLoopDelayMin) {
                        newDelayMax = _group.chainLoopDelayMin;
                    }
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Chained Clip Delay Max");
                    _group.chainLoopDelayMax = newDelayMax;
                }
                break;
            case MasterAudioGroup.VariationMode.Dialog:
                DTGUIHelper.ShowColorWarning("In this mode, only one Variation can be played at a time.");

                var newUseDialog = EditorGUILayout.Toggle("Dialog Custom Fade?", _group.useDialogFadeOut);
                if (newUseDialog != _group.useDialogFadeOut) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "toggle Dialog Custom Fade?");
                    _group.useDialogFadeOut = newUseDialog;
                }

                if (_group.useDialogFadeOut) {
                    var newFadeTime = EditorGUILayout.Slider("Custom Fade Out Time", _group.dialogFadeOutTime, 0.1f, 20f);
                    if (newFadeTime != _group.dialogFadeOutTime) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Custom Fade Out Time");
                        _group.dialogFadeOutTime = newFadeTime;
                    }
                }
                break;
        }

        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel = 0;

        DTGUIHelper.AddSpaceForNonU5(2);
        DTGUIHelper.StartGroupHeader();

        var newChildMode = (MasterAudioGroup.ChildGroupMode)EditorGUILayout.EnumPopup(new GUIContent("Linked Group Mode", "Groups you set up in this section will also get played automatically when this Group plays."), _group.childGroupMode);
        if (newChildMode != _group.childGroupMode) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Linked Group Mode");
            _group.childGroupMode = newChildMode;
        }
        EditorGUILayout.EndVertical();

        if (_group.childGroupMode != MasterAudioGroup.ChildGroupMode.None) {
            if (_group.childSoundGroups.Count == 0) {
                DTGUIHelper.ShowLargeBarAlert("You have no other Groups set up to trigger.");
                EditorGUILayout.Separator();
            }

            EditorGUI.indentLevel = 0;
            for (var i = 0; i < _group.childSoundGroups.Count; i++) {
                var aGroup = _group.childSoundGroups[i];
                if (maInScene) {
                    var existingIndex = _groupNames.IndexOf(aGroup);

                    int? groupIndex = null;

                    EditorGUI.indentLevel = 0;

                    var noGroup = false;
                    var noMatch = false;

                    if (existingIndex >= 1) {
                        groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, _groupNames.ToArray());
                        if (existingIndex == 1) {
                            noGroup = true;
                        }
                    } else if (existingIndex == -1 && aGroup == MasterAudio.NoGroupName) {
                        groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, _groupNames.ToArray());
                    } else { // non-match
                        noMatch = true;
                        var newSound = EditorGUILayout.TextField("Sound Group", aGroup);
                        if (newSound != aGroup) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Sound Group");
                            _group.childSoundGroups[i] = newSound;
                        }

                        var newIndex = EditorGUILayout.Popup("All Sound Groups", -1, _groupNames.ToArray());
                        if (newIndex >= 0) {
                            groupIndex = newIndex;
                        }
                    }

                    if (noGroup) {
                        DTGUIHelper.ShowRedError("No Sound Group specified. Action will do nothing.");
                    } else if (noMatch) {
                        DTGUIHelper.ShowRedError("Sound Group found no match. Type in or choose one.");
                    }

                    if (!groupIndex.HasValue) {
                        continue;
                    }

                    if (existingIndex != groupIndex.Value) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Sound Group");
                    }
                    if (groupIndex.Value == -1) {
                        _group.childSoundGroups[i] = MasterAudio.NoGroupName;
                    } else {
                        _group.childSoundGroups[i] = _groupNames[groupIndex.Value];
                    }
                } else {
                    var newSType = EditorGUILayout.TextField("Sound Group", aGroup);
                    if (newSType == aGroup) {
                        continue;
                    }

                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Sound Group");
                    _group.childSoundGroups[i] = newSType;
                }
            }

            GUI.contentColor = DTGUIHelper.BrightButtonColor;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Add Trigger Group"), EditorStyles.toolbarButton, GUILayout.Width(120))) {
                _group.childSoundGroups.Add(string.Empty);
            }
            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("Delete Trigger Group", "Delete the last Trigger Group"), EditorStyles.toolbarButton, GUILayout.Width(120))) {
                _group.childSoundGroups.RemoveAt(_group.childSoundGroups.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = Color.white;
        }
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel = 0;

        int? deadChildIndex = null;

        var canCopy = false;

        if (!Application.isPlaying) {
            // bulk copy settings

            DTGUIHelper.AddSpaceForNonU5(2);
            DTGUIHelper.StartGroupHeader();


            EditorGUI.indentLevel = 1;
            var newBulk = DTGUIHelper.Foldout(_group.copySettingsExpanded, "Copy Settings");
            if (newBulk != _group.copySettingsExpanded) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "toggle Copy Settings");
                _group.copySettingsExpanded = newBulk;
            }

            EditorGUI.indentLevel = 0;
            EditorGUILayout.EndVertical();
            GUI.color = Color.white;

            if (_group.copySettingsExpanded) {
                if (_group.groupVariations.Count == 0) {
                    DTGUIHelper.ShowLargeBarAlert("You currently have no Variations in this Group.");
                } else if (_group.groupVariations.Count == 1) {
                    DTGUIHelper.ShowLargeBarAlert("You only have a single Variation in this Group. Nothing to copy to.");
                } else {
                    canCopy = true;

                    var varNames = new List<string>(_group.groupVariations.Count);
                    foreach (var t in _group.groupVariations) {
                        varNames.Add(t.name);
                    }

                    if (_group.selectedVariationIndex >= varNames.Count) {
                        _group.selectedVariationIndex = 0;
                    }

                    var newVar = EditorGUILayout.Popup("Source Variation", _group.selectedVariationIndex, varNames.ToArray());
                    if (newVar != _group.selectedVariationIndex) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Source Variation");
                        _group.selectedVariationIndex = newVar;
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Target Variations");

                    GUILayout.Space(44);
                    GUI.contentColor = DTGUIHelper.BrightButtonColor;
                    if (GUILayout.Button("Check All", EditorStyles.toolbarButton, GUILayout.Width(80))) {
                        CheckAll();
                    }
                    GUILayout.Space(10);
                    if (GUILayout.Button("Uncheck All", EditorStyles.toolbarButton, GUILayout.Width(80))) {
                        UncheckAll();
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();

                    DTGUIHelper.ShowColorWarning("Click buttons below to copy from Source to checked Variations.");

                    var hasSelected = GetNonMatchingVariations().Count > 0;
                    if (!hasSelected) {
                        DTGUIHelper.ShowRedError("You have no Variations checked. Please use the checkboxes.");
                        EditorGUILayout.Separator();
                    }

                    var sourceVar = _group.groupVariations[_group.selectedVariationIndex];

                    GUI.contentColor = DTGUIHelper.BrightButtonColor;
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    const int btnWidth = 96;

                    if (GUILayout.Button("Volume", EditorStyles.toolbarButton, GUILayout.Width(btnWidth))) {
                        CopyVolumes(sourceVar);
                        _isDirty = true;
                    }
                    GUILayout.Space(10);
                    if (GUILayout.Button("Pitch", EditorStyles.toolbarButton, GUILayout.Width(btnWidth))) {
                        CopyPitches(sourceVar);
                        _isDirty = true;
                    }
                    GUILayout.Space(10);
                    if (GUILayout.Button("Loop", EditorStyles.toolbarButton, GUILayout.Width(btnWidth))) {
                        CopyLoops(sourceVar);
                        _isDirty = true;
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    DTGUIHelper.VerticalSpace(2);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    if (GUILayout.Button("FX Tail Time", EditorStyles.toolbarButton, GUILayout.Width(btnWidth))) {
                        CopyFxTail(sourceVar);
                        _isDirty = true;
                    }

                    GUILayout.Space(10);
                    if (GUILayout.Button("Rand. Pitch", EditorStyles.toolbarButton, GUILayout.Width(btnWidth))) {
                        CopyRandomPitch(sourceVar);
                        _isDirty = true;
                    }
                    GUILayout.Space(10);
                    if (GUILayout.Button("Rand. Volume", EditorStyles.toolbarButton, GUILayout.Width(btnWidth))) {
                        CopyRandomVolume(sourceVar);
                        _isDirty = true;
                    }
                    EditorGUILayout.EndHorizontal();
                    DTGUIHelper.VerticalSpace(2);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    if (GUILayout.Button("Rand. Delay", EditorStyles.toolbarButton, GUILayout.Width(btnWidth))) {
                        CopyRandomDelay(sourceVar);
                        _isDirty = true;
                    }
                    GUILayout.Space(10);
                    if (GUILayout.Button("Custom Fade", EditorStyles.toolbarButton, GUILayout.Width(btnWidth))) {
                        CopyCustomFade(sourceVar);
                        _isDirty = true;
                    }
                    EditorGUILayout.EndHorizontal();


                    GUI.contentColor = Color.white;
                }
                EditorGUILayout.Separator();
            }
            EditorGUILayout.EndVertical();

            if (!Application.isPlaying) {
                DTGUIHelper.AddSpaceForNonU5(2);
                DTGUIHelper.StartGroupHeader();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Actions", EditorStyles.wordWrappedLabel, GUILayout.Width(50f));
                GUILayout.Space(30);
                GUI.contentColor = DTGUIHelper.BrightButtonColor;

                var buttonText = "Collapse All";
                var allCollapsed = true;

                foreach (var t in _group.groupVariations) {
                    if (!t.isExpanded) {
                        continue;
                    }

                    allCollapsed = false;
                    break;
                }

                if (allCollapsed) {
                    buttonText = "Expand All";
                }

                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent(buttonText), EditorStyles.toolbarButton, GUILayout.Width(80))) {
                    _isDirty = true;
                    ExpandCollapseAll(allCollapsed);
                }
                GUILayout.Space(10);

                if (GUILayout.Button(new GUIContent("Eq. Weights", "Reset Weights to one"), EditorStyles.toolbarButton, GUILayout.Width(80))) {
                    _isDirty = true;
                    EqualizeWeights(_group);
                }

                GUILayout.Space(10);
                if (GUILayout.Button(new GUIContent("Eq. Volumes"), EditorStyles.toolbarButton, GUILayout.Width(80))) {
                    _isDirty = true;
                    EqualizeVariationVolumes(_group.groupVariations);
                }

                GUI.contentColor = Color.white;
                EditorGUILayout.EndHorizontal();
                DTGUIHelper.VerticalSpace(1);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Localization", EditorStyles.wordWrappedLabel, GUILayout.Width(80));
                GUILayout.FlexibleSpace();
                GUI.contentColor = DTGUIHelper.BrightButtonColor;
                if (GUILayout.Button(new GUIContent("All Use Loc.", "Check the 'Use Localized Folder' checkbox for all Variations."), EditorStyles.toolbarButton, GUILayout.Width(125))) {
                    _isDirty = true;
                    BulkUseLocalization(_group.groupVariations, true);
                }

                GUILayout.Space(10);
                if (GUILayout.Button(new GUIContent("None Use Loc.", "Uncheck the 'Use Localized Folder' checkbox for all Variations."), EditorStyles.toolbarButton, GUILayout.Width(125))) {
                    _isDirty = true;
                    BulkUseLocalization(_group.groupVariations, false);
                }

                GUI.contentColor = Color.white;
                EditorGUILayout.EndHorizontal();

                var newBulkMode = (MasterAudio.AudioLocation)EditorGUILayout.EnumPopup("Variation Create Mode", _group.bulkVariationMode);
                if (newBulkMode != _group.bulkVariationMode) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _group, "change Bulk Variation Mode");
                    _group.bulkVariationMode = newBulkMode;
                }
                if (_group.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
                    DTGUIHelper.ShowColorWarning("Resource mode: make sure to drag from Resource folders only.");
                }

                DTGUIHelper.EndGroupHeader();
            }

            DTGUIHelper.VerticalSpace(2);

            // new variation settings
            EditorGUILayout.BeginVertical();
            var anEvent = Event.current;

            GUI.color = DTGUIHelper.BrightTextColor;

            if (isInProjectView) {
                DTGUIHelper.ShowLargeBarAlert("You are in Project View and cannot create Variations.");
            } else {
                var dragArea = GUILayoutUtility.GetRect(0f, 35f, GUILayout.ExpandWidth(true));
                GUI.Box(dragArea, "Drag Audio clips here to create Variations!");

                GUI.color = Color.white;

                switch (anEvent.type) {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (!dragArea.Contains(anEvent.mousePosition)) {
                            break;
                        }

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (anEvent.type == EventType.DragPerform) {
                            DragAndDrop.AcceptDrag();

                            foreach (var dragged in DragAndDrop.objectReferences) {
                                var aClip = dragged as AudioClip;
                                if (aClip == null) {
                                    continue;
                                }

                                CreateVariation(_group, _ma, aClip);
                            }
                        }
                        Event.current.Use();
                        break;
                }
            }
            EditorGUILayout.EndVertical();
            // end new variation settings

            GUI.color = Color.white;
        }

        if (_group.groupVariations.Count == 0) {
            DTGUIHelper.ShowRedError("You currently have no Variations.");
        } else {
            for (var i = 0; i < _group.groupVariations.Count; i++) {
                var variation = _group.groupVariations[i];

                var isNotSource = _group.selectedVariationIndex != i;


				var state = variation.isExpanded;
				var text = variation.name;

				// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
				if (!state) {
					GUI.backgroundColor = DTGUIHelper.InactiveHeaderColor;
				} else {
					GUI.backgroundColor = DTGUIHelper.ActiveHeaderColor;
				}
				
				GUILayout.BeginHorizontal();
				
				#if UNITY_3_5_7
				if (!state) {
					text += " (Click to expand)";
				}
				#else
				text = "<b><size=11>" + text + "</size></b>";
				#endif
				if (state) {
					text = "\u25BC " + text;
				} else {
					text = "\u25BA " + text;
				}
				if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
					state = !state;
				}
				
				GUI.backgroundColor = Color.white;
				if (!state) {
					GUILayout.Space(3f);
				}

                if (state != variation.isExpanded) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, variation, "toggle Expand Variation");
                    variation.isExpanded = state;
                }

				var varIsDirty = false;

                if (canCopy) {
                    if (isNotSource) {
                        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                        var newChecked = EditorGUILayout.Toggle(variation.isChecked, GUILayout.Width(16), GUILayout.Height(16));
                        if (newChecked != variation.isChecked) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, variation, "toggle check Variation");
                            variation.isChecked = newChecked;
                        }
                        EditorGUILayout.EndHorizontal();
                    } else {
                        GUI.contentColor = DTGUIHelper.BrightTextColor;
                        GUILayout.Label("SOURCE", GUILayout.Width(54));
                        GUI.contentColor = Color.white;
                    }
                }

                if (GUILayout.Button(new GUIContent(MasterAudioInspectorResources.GearTexture, "Click to goto Variation"), EditorStyles.toolbarButton, GUILayout.Height(16), GUILayout.Width(40))) {
                    Selection.activeObject = variation;
                }

                if (!Application.isPlaying && !DTGUIHelper.IsPrefabInProjectView(_group)) {
                    if (GUILayout.Button(new GUIContent(MasterAudioInspectorResources.CopyTexture, "Click to clone Variation"), EditorStyles.toolbarButton, GUILayout.Height(16), GUILayout.Width(40))) {
                        CloneVariation(i);
                    }
                }

                var buttonPressed = DTGUIHelper.AddVariationButtons();

                if (!Application.isPlaying && !DTGUIHelper.IsPrefabInProjectView(_group)) {
                    if (GUILayout.Button(new GUIContent(MasterAudioInspectorResources.DeleteTexture, "Click to delete this Variation"), EditorStyles.toolbarButton, GUILayout.Height(16), GUILayout.Width(40))) {
                        deadChildIndex = i;
                        _isDirty = true;
                    }
                }

                switch (buttonPressed) {
                    case DTGUIHelper.DTFunctionButtons.Play:
                        if (Application.isPlaying) {
                            MasterAudio.PlaySoundAndForget(_group.name, 1f, null, 0f, variation.name);
                        } else {
                            var calcVolume = _group.groupMasterVolume * variation.VarAudio.volume;

                            if (variation.audLocation == MasterAudio.AudioLocation.ResourceFile) {
                                StopPreviewer();
                                var fileName = AudioResourceOptimizer.GetLocalizedFileName(variation.useLocalization, variation.resourceFileName);
                                GetPreviewer().PlayOneShot(Resources.Load(fileName) as AudioClip, calcVolume);
                            } else {
                                variation.VarAudio.PlayOneShot(variation.VarAudio.clip, calcVolume);
                            }

                            _isDirty = true;
                        }
                        break;
                    case DTGUIHelper.DTFunctionButtons.Stop:
                        if (Application.isPlaying) {
                            MasterAudio.StopAllOfSound(_group.name);
                        } else {
                            if (variation.audLocation == MasterAudio.AudioLocation.ResourceFile) {
                                StopPreviewer();
                            } else {
                                variation.VarAudio.Stop();
                            }
                        }
                        break;
                }

                GUILayout.Space(4);

                EditorGUILayout.EndHorizontal();

                GUI.backgroundColor = Color.white;

                if (!variation.isExpanded) {
                    DTGUIHelper.VerticalSpace(4);
                    continue;
                }

				DTGUIHelper.BeginGroupedControls();

                if (!Application.isPlaying) {
                    DTGUIHelper.ShowColorWarning(MasterAudio.PreviewText);
                }
                if (variation.VarAudio == null) {
                    DTGUIHelper.ShowRedError(string.Format("The Variation: '{0}' has no Audio Source.", variation.name));
                    break;
                }

                var oldLocation = variation.audLocation;
                var newLocation = (MasterAudio.AudioLocation)EditorGUILayout.EnumPopup("Audio Origin", variation.audLocation);
                if (newLocation != variation.audLocation) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Audio Origin");
                    variation.audLocation = newLocation;
                }

                switch (variation.audLocation) {
                    case MasterAudio.AudioLocation.Clip:
                        var newClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", variation.VarAudio.clip, typeof(AudioClip), false);
                        if (newClip != variation.VarAudio.clip) {
                            UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation.VarAudio, "change Audio Clip");
                            variation.VarAudio.clip = newClip;
                        }
                        break;
                    case MasterAudio.AudioLocation.ResourceFile:
                        if (oldLocation != variation.audLocation) {
                            if (variation.VarAudio.clip != null) {
                                Debug.Log("Audio clip removed to prevent unnecessary memory usage on Resource file Variation.");
                            }
                            variation.VarAudio.clip = null;
                        }

                        EditorGUILayout.BeginVertical();
                        var anEvent = Event.current;

                        GUI.color = DTGUIHelper.DragAreaColor;
                        var dragArea = GUILayoutUtility.GetRect(0f, 20f, GUILayout.ExpandWidth(true));
                        GUI.Box(dragArea, "Drag Resource Audio clip here to use its name!");
                        GUI.color = Color.white;

                        switch (anEvent.type) {
                            case EventType.DragUpdated:
                            case EventType.DragPerform:
                                if (!dragArea.Contains(anEvent.mousePosition)) {
                                    break;
                                }

                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                                if (anEvent.type == EventType.DragPerform) {
                                    DragAndDrop.AcceptDrag();

                                    foreach (var dragged in DragAndDrop.objectReferences) {
                                        var aClip = dragged as AudioClip;
                                        if (aClip == null) {
                                            continue;
                                        }

                                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Resource Filename");
                                        var useLocalization = false;
                                        var resourceFileName = DTGUIHelper.GetResourcePath(aClip, ref useLocalization);
                                        if (string.IsNullOrEmpty(resourceFileName)) {
                                            resourceFileName = aClip.name;
                                        }

                                        variation.resourceFileName = resourceFileName;
                                        variation.useLocalization = useLocalization;
                                    }
                                }
                                Event.current.Use();
                                break;
                        }
                        EditorGUILayout.EndVertical();

                        var newFilename = EditorGUILayout.TextField("Resource Filename", variation.resourceFileName);
                        if (newFilename != variation.resourceFileName) {
                            UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Resource Filename");
                            variation.resourceFileName = newFilename;
                        }

                        EditorGUI.indentLevel = 1;

                        var newLocal = EditorGUILayout.Toggle("Use Localized Folder", variation.useLocalization);
                        if (newLocal != variation.useLocalization) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, variation, "toggle Use Localized Folder");
                            variation.useLocalization = newLocal;
                        }

                        break;
                }

                EditorGUI.indentLevel = 0;

                var newVolume = DTGUIHelper.DisplayVolumeField(variation.VarAudio.volume, DTGUIHelper.VolumeFieldType.None, MasterAudio.MixerWidthMode.Normal, 0f, true);
                if (newVolume != variation.VarAudio.volume) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation.VarAudio, "change Volume");
                    variation.VarAudio.volume = newVolume;
                }

                var newPitch = DTGUIHelper.DisplayPitchField(variation.VarAudio.pitch);
                if (newPitch != variation.VarAudio.pitch) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation.VarAudio, "change Pitch");
                    variation.VarAudio.pitch = newPitch;
                }

                if (_group.curVariationMode == MasterAudioGroup.VariationMode.LoopedChain) {
                    DTGUIHelper.ShowLargeBarAlert("Loop Clip is always OFF for Looped Chain Groups");
                } else {
                    var newLoop = EditorGUILayout.Toggle("Loop Clip", variation.VarAudio.loop);
                    if (newLoop != variation.VarAudio.loop) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation.VarAudio, "toggle Loop Clip");
                        variation.VarAudio.loop = newLoop;
                    }
                }

                var newWeight = EditorGUILayout.IntSlider("Weight (Instances)", variation.weight, 0, 100);
                if (newWeight != variation.weight) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Weight");
                    variation.weight = newWeight;
                }

                var newFxTailTime = EditorGUILayout.Slider("FX Tail Time", variation.fxTailTime, 0f, 10f);
                if (newFxTailTime != variation.fxTailTime) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change FX Tail Time");
                    variation.fxTailTime = newFxTailTime;
                }

                DTGUIHelper.StartGroupHeader();

                var newUseRndPitch = EditorGUILayout.BeginToggleGroup(" Use Random Pitch", variation.useRandomPitch);
                if (newUseRndPitch != variation.useRandomPitch) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "toggle Use Random Pitch");
                    variation.useRandomPitch = newUseRndPitch;
                }
				DTGUIHelper.EndGroupHeader();

                if (variation.useRandomPitch) {
                    var newMode = (SoundGroupVariation.RandomPitchMode)EditorGUILayout.EnumPopup("Pitch Compute Mode", variation.randomPitchMode);
                    if (newMode != variation.randomPitchMode) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Pitch Compute Mode");
                        variation.randomPitchMode = newMode;
                    }

                    var newPitchMin = DTGUIHelper.DisplayPitchField(variation.randomPitchMin, "Random Pitch Min");
                    if (newPitchMin != variation.randomPitchMin) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Random Pitch Min");
                        variation.randomPitchMin = newPitchMin;
                        if (variation.randomPitchMax <= variation.randomPitchMin) {
                            variation.randomPitchMax = variation.randomPitchMin;
                        }
                    }

                    var newPitchMax = DTGUIHelper.DisplayPitchField(variation.randomPitchMax, "Random Pitch Max");
                    if (newPitchMax != variation.randomPitchMax) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Random Pitch Max");
                        variation.randomPitchMax = newPitchMax;
                        if (variation.randomPitchMin > variation.randomPitchMax) {
                            variation.randomPitchMin = variation.randomPitchMax;
                        }
                    }
                }

                EditorGUILayout.EndToggleGroup();
                DTGUIHelper.AddSpaceForNonU5(2);

				DTGUIHelper.StartGroupHeader();
				var newUseRndVol = EditorGUILayout.BeginToggleGroup(" Use Random Volume", variation.useRandomVolume);
                if (newUseRndVol != variation.useRandomVolume) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "toggle Use Random Volume");
                    variation.useRandomVolume = newUseRndVol;
                }
				DTGUIHelper.EndGroupHeader();

                if (variation.useRandomVolume) {
                    var newMode = (SoundGroupVariation.RandomVolumeMode)EditorGUILayout.EnumPopup("Volume Compute Mode", variation.randomVolumeMode);
                    if (newMode != variation.randomVolumeMode) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Volume Compute Mode");
                        variation.randomVolumeMode = newMode;
                    }

                    var volMin = 0f;
                    if (variation.randomVolumeMode == SoundGroupVariation.RandomVolumeMode.AddToClipVolume) {
                        volMin = -1f;
                    }

                    var newVolMin = DTGUIHelper.DisplayVolumeField(variation.randomVolumeMin, DTGUIHelper.VolumeFieldType.None, MasterAudio.MixerWidthMode.Normal, volMin, true, "Random Volume Min");
                    if (newVolMin != variation.randomVolumeMin) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Random Volume Min");
                        variation.randomVolumeMin = newVolMin;
                        if (variation.randomVolumeMax <= variation.randomVolumeMin) {
                            variation.randomVolumeMax = variation.randomVolumeMin;
                        }
                    }

                    var newVolMax = DTGUIHelper.DisplayVolumeField(variation.randomVolumeMax, DTGUIHelper.VolumeFieldType.None, MasterAudio.MixerWidthMode.Normal, volMin, true, "Random Volume Max");
                    if (newVolMax != variation.randomVolumeMax) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Random Volume Max");
                        variation.randomVolumeMax = newVolMax;
                        if (variation.randomVolumeMin > variation.randomVolumeMax) {
                            variation.randomVolumeMin = variation.randomVolumeMax;
                        }
                    }
                }

                EditorGUILayout.EndToggleGroup();
                DTGUIHelper.AddSpaceForNonU5(2);

				DTGUIHelper.StartGroupHeader();

                var newSilence = EditorGUILayout.BeginToggleGroup(" Use Random Delay", variation.useIntroSilence);
                if (newSilence != variation.useIntroSilence) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "toggle Use Random Delay");
                    variation.useIntroSilence = newSilence;
                }
				DTGUIHelper.EndGroupHeader();

                if (variation.useIntroSilence) {
                    var newSilenceMin = EditorGUILayout.Slider("Delay Min (sec)", variation.introSilenceMin, 0f, 100f);
                    if (newSilenceMin != variation.introSilenceMin) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Delay Min (sec)");
                        variation.introSilenceMin = newSilenceMin;
                        if (variation.introSilenceMin > variation.introSilenceMax) {
                            variation.introSilenceMax = newSilenceMin;
                        }
                    }

                    var newSilenceMax = EditorGUILayout.Slider("Delay Max (sec)", variation.introSilenceMax, 0f, 100f);
                    if (newSilenceMax != variation.introSilenceMax) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Delay Max (sec)");
                        variation.introSilenceMax = newSilenceMax;
                        if (variation.introSilenceMax < variation.introSilenceMin) {
                            variation.introSilenceMin = newSilenceMax;
                        }
                    }
                }

                EditorGUILayout.EndToggleGroup();
                DTGUIHelper.AddSpaceForNonU5(2);

				DTGUIHelper.StartGroupHeader();

                var newStart = EditorGUILayout.BeginToggleGroup("Use Random Start Position", variation.useRandomStartTime);
                if (newStart != variation.useRandomStartTime) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "toggle Use Random Start Position");
                    variation.useRandomStartTime = newStart;
                }
				DTGUIHelper.EndGroupHeader();

                if (variation.useRandomStartTime) {
                    var newMin = EditorGUILayout.Slider("Start Min (%)", variation.randomStartMinPercent, 0f, 100f);
                    if (newMin != variation.randomStartMinPercent) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "toggle Start Min (%)");
                        variation.randomStartMinPercent = newMin;
                        if (variation.randomStartMaxPercent <= variation.randomStartMinPercent) {
                            variation.randomStartMaxPercent = variation.randomStartMinPercent;
                        }
                    }

                    var newMax = EditorGUILayout.Slider("Start Max (%)", variation.randomStartMaxPercent, 0f, 100f);
                    if (newMax != variation.randomStartMaxPercent) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "toggle Start Max (%)");
                        variation.randomStartMaxPercent = newMax;
                        if (variation.randomStartMinPercent > variation.randomStartMaxPercent) {
                            variation.randomStartMinPercent = variation.randomStartMaxPercent;
                        }
                    }
                }

                EditorGUILayout.EndToggleGroup();
                DTGUIHelper.AddSpaceForNonU5(2);

				DTGUIHelper.StartGroupHeader();

                var newFades = EditorGUILayout.BeginToggleGroup(" Use Custom Fading", variation.useFades);
                if (newFades != variation.useFades) {
                    UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "toggle Use Custom Fading");
                    variation.useFades = newFades;
                }
				DTGUIHelper.EndGroupHeader();

                if (variation.useFades) {
                    var newFadeIn = EditorGUILayout.Slider("Fade In Time (sec)", variation.fadeInTime, 0f, 10f);
                    if (newFadeIn != variation.fadeInTime) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Fade In Time");
                        variation.fadeInTime = newFadeIn;
                    }

                    var newFadeOut = EditorGUILayout.Slider("Fade Out time (sec)", variation.fadeOutTime, 0f, 10f);
                    if (newFadeOut != variation.fadeOutTime) {
                        UndoHelper.RecordObjectPropertyForUndo(ref varIsDirty, variation, "change Fade Out Time");
                        variation.fadeOutTime = newFadeOut;
                    }
                }
                EditorGUILayout.EndToggleGroup();
				DTGUIHelper.EndGroupedControls();

				DTGUIHelper.VerticalSpace(4);

                if (!varIsDirty) {
                    continue;
                }

                EditorUtility.SetDirty(variation.VarAudio);
                EditorUtility.SetDirty(variation);
            }
        }

        if (deadChildIndex.HasValue) {
            var deadVar = _group.groupVariations[deadChildIndex.Value];

            if (deadVar != null) {
                // delete variation from Hierarchy
                UndoHelper.DestroyForUndo(deadVar.gameObject);
            }

            // delete group.
            _group.groupVariations.RemoveAt(deadChildIndex.Value);
        }

        if (GUI.changed || _isDirty) {
            EditorUtility.SetDirty(target);
        }

        //DrawDefaultInspector();
    }

    public static MasterAudioGroup RescanChildren(MasterAudioGroup group) {
        var newChildren = new List<SoundGroupVariation>();

        var childNames = new List<string>();

        var dupeShown = false;

        for (var i = 0; i < group.transform.childCount; i++) {
            var child = group.transform.GetChild(i);

            if (!Application.isPlaying) {
                if (childNames.Contains(child.name) && !dupeShown) {
                    DTGUIHelper.ShowRedError("You have more than one Variation named: " + child.name + ".");
                    DTGUIHelper.ShowRedError("Please ensure each Variation of this Group has a unique name.");
                    dupeShown = true;
                }
            }

            childNames.Add(child.name);

            var variation = child.GetComponent<SoundGroupVariation>();

            newChildren.Add(variation);
        }

        group.groupVariations = newChildren;
        return group;
    }

    public void EqualizeWeights(MasterAudioGroup grp) {
        var variations = new SoundGroupVariation[grp.groupVariations.Count];

        for (var i = 0; i < grp.groupVariations.Count; i++) {
            var variation = grp.groupVariations[i];
            variations[i] = variation;
        }

        UndoHelper.RecordObjectsForUndo(variations, "Equalize Weights");

        foreach (var vari in variations) {
            vari.weight = 1;
        }
    }

    public void CreateVariation(MasterAudioGroup group, MasterAudio ma, AudioClip clip) {
        var resourceFileName = string.Empty;
        var useLocalization = false;

        if (group.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
            resourceFileName = DTGUIHelper.GetResourcePath(clip, ref useLocalization);
            if (string.IsNullOrEmpty(resourceFileName)) {
                resourceFileName = clip.name;
            }
        }

        var clipName = clip.name;

        if (group.transform.FindChild(clipName) != null) {
            DTGUIHelper.ShowAlert("You already have a Variation for this Group named '" + clipName + "'. \n\nPlease rename these Variations when finished to be unique, or you may not be able to play them by name if you have a need to.");
        }

        var newVar = (GameObject)Instantiate(ma.soundGroupVariationTemplate.gameObject, group.transform.position, Quaternion.identity);
        UndoHelper.CreateObjectForUndo(newVar, "create Variation");

        newVar.transform.name = clipName;
        newVar.transform.parent = group.transform;
        var variation = newVar.GetComponent<SoundGroupVariation>();

        if (group.bulkVariationMode == MasterAudio.AudioLocation.ResourceFile) {
            variation.audLocation = MasterAudio.AudioLocation.ResourceFile;
            variation.resourceFileName = resourceFileName;
            variation.useLocalization = useLocalization;
        } else {
            variation.VarAudio.clip = clip;
        }

        MasterAudioInspector.CopyFromAudioSourceTemplate(variation.VarAudio, false);
    }

    private static void EqualizeVariationVolumes(List<SoundGroupVariation> variations) {
        var clips = new Dictionary<SoundGroupVariation, float>();

        if (variations.Count < 2) {
            DTGUIHelper.ShowAlert("You must have at least 2 Variations to use this function.");
            return;
        }

        var lowestVolume = 1f;

        foreach (var setting in variations) {
            AudioClip ac = null;

            switch (setting.audLocation) {
                case MasterAudio.AudioLocation.Clip:
                    if (setting.VarAudio.clip == null) {
                        continue;
                    }
                    ac = setting.VarAudio.clip;
                    break;
                case MasterAudio.AudioLocation.ResourceFile:
                    if (string.IsNullOrEmpty(setting.resourceFileName)) {
                        continue;
                    }

                    ac = Resources.Load(setting.resourceFileName) as AudioClip;

                    if (ac == null) {
                        continue; // bad resource path
                    }
                    break;
            }

            if (!AudioUtil.IsClipReadyToPlay(ac)) {
                Debug.Log("Clip is not ready to play (streaming?). Skipping '" + setting.name + "'.");
                continue;
            }

            var average = 0f;
            // ReSharper disable once PossibleNullReferenceException
            var buffer = new float[ac.samples];

            Debug.Log("Measuring amplitude of '" + ac.name + "'.");

            ac.GetData(buffer, 0);

            for (var c = 0; c < ac.samples; c++) {
                average += Mathf.Pow(buffer[c], 2);
            }

            average = Mathf.Sqrt(1f / ac.samples * average);

            if (average < lowestVolume) {
                lowestVolume = average;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (average == 0f) {
                // don't factor in.
                continue;
            }
            clips.Add(setting, average);
        }

        if (clips.Count < 2) {
            DTGUIHelper.ShowAlert("You must have at least 2 Variations with non-compressed, non-streaming clips to use this function.");
            return;
        }

        foreach (var kv in clips) {
            if (kv.Value == 0) {
                // skip
                continue;
            }
            var adjustedVol = lowestVolume / kv.Value;
            //set your volume for each Variation in your Sound Group.
            kv.Key.VarAudio.volume = adjustedVol;
        }
    }

    private static void BulkUseLocalization(List<SoundGroupVariation> variations, bool shouldUse) {
        foreach (var setting in variations) {
            if (setting.audLocation != MasterAudio.AudioLocation.ResourceFile) {
                continue;
            }

            setting.useLocalization = shouldUse;
        }
    }

    private List<SoundGroupVariation> GetNonMatchingVariations() {
        var changedVars = new List<SoundGroupVariation>();

        for (var i = 0; i < _group.groupVariations.Count; i++) {
            if (i == _group.selectedVariationIndex) {
                continue;
            }

            var vari = _group.groupVariations[i];
            if (!vari.isChecked) {
                continue;
            }

            changedVars.Add(vari);
        }

        return changedVars;
    }

    private void CopyVolumes(SoundGroupVariation variation) {
        var changed = 0;

        var changedVars = GetNonMatchingVariations();

        if (changedVars.Count > 0) {
            UndoHelper.RecordObjectsForUndo(changedVars.ToArray(), "change Variation Volumes");
        }

        foreach (var aVar in changedVars) {
            aVar.VarAudio.volume = variation.VarAudio.volume;
            changed++;
        }

        Debug.LogWarning(changed + " Variation Volume(s) changed.");
    }

    private void CopyPitches(SoundGroupVariation variation) {
        var changed = 0;

        var changedVars = GetNonMatchingVariations();

        if (changedVars.Count > 0) {
            UndoHelper.RecordObjectsForUndo(changedVars.ToArray(), "change Variation Pitches");
        }

        foreach (var aVar in changedVars) {
            aVar.VarAudio.pitch = variation.VarAudio.pitch;
            changed++;
        }

        Debug.LogWarning(changed + " Variation Pitch(es) changed.");
    }

    private void CopyLoops(SoundGroupVariation variation) {
        var changed = 0;

        var changedVars = GetNonMatchingVariations();

        if (changedVars.Count > 0) {
            UndoHelper.RecordObjectsForUndo(changedVars.ToArray(), "change Variation Loops");
        }

        foreach (var aVar in changedVars) {
            aVar.VarAudio.loop = variation.VarAudio.loop;
            changed++;
        }

        Debug.LogWarning(changed + " Variation Loop(s) changed.");
    }

    private void CopyFxTail(SoundGroupVariation variation) {
        var changed = 0;

        var changedVars = GetNonMatchingVariations();

        if (changedVars.Count > 0) {
            UndoHelper.RecordObjectsForUndo(changedVars.ToArray(), "change Variation Fx Tail");
        }

        foreach (var aVar in changedVars) {
            aVar.fxTailTime = variation.fxTailTime;
            changed++;
        }

        Debug.LogWarning(changed + " Fx Tail(s) changed.");
    }

    private void CopyRandomPitch(SoundGroupVariation variation) {
        var changed = 0;

        var changedVars = GetNonMatchingVariations();

        if (changedVars.Count > 0) {
            UndoHelper.RecordObjectsForUndo(changedVars.ToArray(), "change Variation Random Pitch");
        }

        foreach (var aVar in changedVars) {
            aVar.useRandomPitch = variation.useRandomPitch;
            aVar.randomPitchMode = variation.randomPitchMode;
            aVar.randomPitchMin = variation.randomPitchMin;
            aVar.randomPitchMax = variation.randomPitchMax;
            changed++;
        }

        Debug.LogWarning(changed + " Random Pitch(es) changed.");
    }

    private void CopyRandomVolume(SoundGroupVariation variation) {
        var changed = 0;

        var changedVars = GetNonMatchingVariations();

        if (changedVars.Count > 0) {
            UndoHelper.RecordObjectsForUndo(changedVars.ToArray(), "change Variation Random Volume");
        }

        foreach (var aVar in changedVars) {
            aVar.useRandomVolume = variation.useRandomVolume;
            aVar.randomVolumeMode = variation.randomVolumeMode;
            aVar.randomVolumeMin = variation.randomVolumeMin;
            aVar.randomVolumeMax = variation.randomVolumeMax;
            changed++;
        }

        Debug.LogWarning(changed + " Random Volume(s) changed.");
    }

    private void CopyRandomDelay(SoundGroupVariation variation) {
        var changed = 0;

        var changedVars = GetNonMatchingVariations();

        if (changedVars.Count > 0) {
            UndoHelper.RecordObjectsForUndo(changedVars.ToArray(), "change Variation Random Delay");
        }

        foreach (var aVar in changedVars) {
            aVar.useIntroSilence = variation.useIntroSilence;
            aVar.introSilenceMin = variation.introSilenceMin;
            aVar.introSilenceMax = variation.introSilenceMax;
            changed++;
        }

        Debug.LogWarning(changed + " Random Delay(s) changed.");
    }

    private void CopyCustomFade(SoundGroupVariation variation) {
        var changed = 0;

        var changedVars = GetNonMatchingVariations();

        if (changedVars.Count > 0) {
            UndoHelper.RecordObjectsForUndo(changedVars.ToArray(), "change Variation Custom Fade");
        }

        foreach (var aVar in changedVars) {
            aVar.useFades = variation.useFades;
            aVar.fadeInTime = variation.fadeInTime;
            aVar.fadeOutTime = variation.fadeOutTime;
            changed++;
        }

        Debug.LogWarning(changed + " Random Custom Fade(s) changed.");
    }

    private void ExpandCollapseAll(bool expand) {
        var vars = new List<SoundGroupVariation>();

        foreach (var t in _group.groupVariations) {
            vars.Add(t);
        }

        UndoHelper.RecordObjectsForUndo(vars.ToArray(), "toggle Expand / Collapse Variations");
        foreach (var t in vars) {
            t.isExpanded = expand;
        }
    }

    private void CloneVariation(int index) {
        var gameObj = _group.groupVariations[index].gameObject;

        var dupe = DTGUIHelper.DuplicateGameObject(gameObj, _group.name, _group.groupVariations.Count + 1);

        if (dupe == null) {
            return;
        }

        dupe.transform.parent = _group.transform;
    }

    private void CheckAll() {
        var vars = new List<SoundGroupVariation>();

        for (var i = 0; i < _group.groupVariations.Count; i++) {
            var vari = _group.groupVariations[i];
            if (i == _group.selectedVariationIndex) {
                continue;
            }
            vars.Add(vari);
        }

        UndoHelper.RecordObjectsForUndo(vars.ToArray(), "check Variations");

        foreach (var t in vars) {
            t.isChecked = true;
        }
    }

    private void UncheckAll() {
        var vars = new List<SoundGroupVariation>();

        for (var i = 0; i < _group.groupVariations.Count; i++) {
            var vari = _group.groupVariations[i];
            if (i == _group.selectedVariationIndex) {
                continue;
            }
            vars.Add(vari);
        }

        UndoHelper.RecordObjectsForUndo(vars.ToArray(), "check Variations");

        foreach (var t in vars) {
            t.isChecked = false;
        }
    }

    private void StopPreviewer() {
        GetPreviewer().Stop();
    }

    private AudioSource GetPreviewer() {
        var aud = _previewer.GetComponent<AudioSource>();
        if (aud != null) {
            return aud;
        }

#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0
        _previewer.AddComponent<AudioSource>();
#else
        UnityEditorInternal.ComponentUtility.CopyComponent(_ma.soundGroupVariationTemplate.GetComponent<AudioSource>());
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(_previewer);
#endif

        aud = _previewer.GetComponent<AudioSource>();

        return aud;
    }
}
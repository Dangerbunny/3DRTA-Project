using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FootstepSounds))]
[CanEditMultipleObjects]
// ReSharper disable once CheckNamespace
public class FootstepsSoundsInspector : Editor {
    private bool _isDirty;
    private FootstepSounds _sounds;
    private List<string> _groupNames;

    // ReSharper disable once FunctionComplexityOverflow
    public override void OnInspectorGUI() {
        EditorGUIUtility.LookLikeControls();

        MasterAudio.Instance = null;

        var ma = MasterAudio.Instance;
        var maInScene = ma != null;

        if (maInScene) {
            DTGUIHelper.ShowHeaderTexture(MasterAudioInspectorResources.LogoTexture);
            _groupNames = ma.GroupNames;
        }

        _isDirty = false;

        _sounds = (FootstepSounds)target;

        var newEvent = (FootstepSounds.FootstepTriggerMode)EditorGUILayout.EnumPopup("Event Used", _sounds.footstepEvent);
        if (newEvent != _sounds.footstepEvent) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Event Used");
            _sounds.footstepEvent = newEvent;
        }

        if (_sounds.footstepEvent == FootstepSounds.FootstepTriggerMode.None) {
            DTGUIHelper.ShowRedError("No sound will be made when Event Used is set to None.");
            return;
        }

        DTGUIHelper.VerticalSpace(3);

        EditorGUILayout.BeginHorizontal();
        GUI.contentColor = DTGUIHelper.BrightButtonColor;
        GUILayout.Space(10);
        if (GUILayout.Button("Add Footstep Sound", EditorStyles.toolbarButton, GUILayout.Width(125))) {
            AddFootstepSound();
        }

        if (_sounds.footstepGroups.Count > 0) {
            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("Delete Footstep Sound", "Delete the bottom Footstep Sound"), EditorStyles.toolbarButton, GUILayout.Width(125))) {
                DeleteFootstepSound();
            }
            var buttonText = "Collapse All";
            var allCollapsed = true;

            foreach (var t in _sounds.footstepGroups) {
                if (!t.isExpanded) {
                    continue;
                }

                allCollapsed = false;
                break;
            }

            if (allCollapsed) {
                buttonText = "Expand All";
            }

            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent(buttonText), EditorStyles.toolbarButton, GUILayout.Width(80))) {
                _isDirty = true;
                ExpandCollapseAll(allCollapsed);
            }
        }

        GUI.contentColor = Color.white;
        EditorGUILayout.EndHorizontal();
        DTGUIHelper.VerticalSpace(3);

        DTGUIHelper.StartGroupHeader();
        var newRetrigger = (EventSounds.RetriggerLimMode)EditorGUILayout.EnumPopup("Retrigger Limit Mode", _sounds.retriggerLimitMode);
        if (newRetrigger != _sounds.retriggerLimitMode) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Retrigger Limit Mode");
            _sounds.retriggerLimitMode = newRetrigger;
        }
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel = 0;
        switch (_sounds.retriggerLimitMode) {
            case EventSounds.RetriggerLimMode.FrameBased:
                var newFrm = EditorGUILayout.IntSlider("Min Frames Between", _sounds.limitPerXFrm, 0, 10000);
                if (newFrm != _sounds.limitPerXFrm) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Min Frames Between");
                    _sounds.limitPerXFrm = newFrm;
                }
                break;
            case EventSounds.RetriggerLimMode.TimeBased:
                var newSec = EditorGUILayout.Slider("Min Seconds Between", _sounds.limitPerXSec, 0f, 10000f);
                if (newSec != _sounds.limitPerXSec) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Min Seconds Between");
                    _sounds.limitPerXSec = newSec;
                }
                break;
        }
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel = 0;
        if (_sounds.footstepGroups.Count == 0) {
            DTGUIHelper.ShowRedError("You have no Footstep Sounds configured.");
        }
        for (var f = 0; f < _sounds.footstepGroups.Count; f++) {
            EditorGUI.indentLevel = 1;
            var step = _sounds.footstepGroups[f];


            var state = step.isExpanded;
            var text = "Footstep Sound #" + (f + 1);

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

            if (state != step.isExpanded) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "toggle Expand Variation");
                step.isExpanded = state;
            }

            EditorGUILayout.EndHorizontal();

            if (!step.isExpanded) {
                DTGUIHelper.VerticalSpace(3);
                continue;
            }

            EditorGUI.indentLevel = 0;
            DTGUIHelper.BeginGroupedControls();

            DTGUIHelper.StartGroupHeader();

            var newUseLayers = EditorGUILayout.BeginToggleGroup("Layer filters", step.useLayerFilter);
            if (newUseLayers != step.useLayerFilter) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "toggle Layer filters");
                step.useLayerFilter = newUseLayers;
            }
            DTGUIHelper.EndGroupHeader();

            if (step.useLayerFilter) {
                for (var i = 0; i < step.matchingLayers.Count; i++) {
                    var newLayer = EditorGUILayout.LayerField("Layer Match " + (i + 1), step.matchingLayers[i]);
                    if (newLayer == step.matchingLayers[i]) {
                        continue;
                    }

                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Layer filter");
                    step.matchingLayers[i] = newLayer;
                }
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);

                GUI.contentColor = DTGUIHelper.BrightButtonColor;
                if (GUILayout.Button(new GUIContent("Add", "Click to add a layer match at the end"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "add Layer filter");
                    step.matchingLayers.Add(0);
                }
                if (step.matchingLayers.Count > 1) {
                    GUILayout.Space(10);
                    if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last layer match"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "remove Layer filter");
                        step.matchingLayers.RemoveAt(step.matchingLayers.Count - 1);
                    }
                }
                GUI.contentColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndToggleGroup();

            DTGUIHelper.AddSpaceForNonU5(2);
            DTGUIHelper.StartGroupHeader();
            var newTagFilter = EditorGUILayout.BeginToggleGroup("Tag filter", step.useTagFilter);
            if (newTagFilter != step.useTagFilter) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "toggle Tag filter");
                step.useTagFilter = newTagFilter;
            }
            DTGUIHelper.EndGroupHeader();

            if (step.useTagFilter) {
                for (var i = 0; i < step.matchingTags.Count; i++) {
                    var newTag = EditorGUILayout.TagField("Tag Match " + (i + 1), step.matchingTags[i]);
                    if (newTag == step.matchingTags[i]) {
                        continue;
                    }

                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Tag filter");
                    step.matchingTags[i] = newTag;
                }
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUI.contentColor = DTGUIHelper.BrightButtonColor;
                if (GUILayout.Button(new GUIContent("Add", "Click to add a tag match at the end"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "Add Tag filter");
                    step.matchingTags.Add("Untagged");
                }
                if (step.matchingTags.Count > 1) {
                    GUILayout.Space(10);
                    if (GUILayout.Button(new GUIContent("Remove", "Click to remove the last tag match"), EditorStyles.toolbarButton, GUILayout.Width(60))) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "remove Tag filter");
                        step.matchingTags.RemoveAt(step.matchingLayers.Count - 1);
                    }
                }
                GUI.contentColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndToggleGroup();

            EditorGUI.indentLevel = 0;

            if (maInScene) {
                var existingIndex = _groupNames.IndexOf(step.soundType);

                int? groupIndex = null;

                var noGroup = false;
                var noMatch = false;

                if (existingIndex >= 1) {
                    groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, _groupNames.ToArray());
                    if (existingIndex == 1) {
                        noGroup = true;
                    }
                } else if (existingIndex == -1 && step.soundType == MasterAudio.NoGroupName) {
                    groupIndex = EditorGUILayout.Popup("Sound Group", existingIndex, _groupNames.ToArray());
                } else { // non-match
                    noMatch = true;
                    var newSound = EditorGUILayout.TextField("Sound Group", step.soundType);
                    if (newSound != step.soundType) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Sound Group");
                        step.soundType = newSound;
                    }

                    var newIndex = EditorGUILayout.Popup("All Sound Groups", -1, _groupNames.ToArray());
                    if (newIndex >= 0) {
                        groupIndex = newIndex;
                    }
                }

                if (noGroup) {
                    DTGUIHelper.ShowRedError("No Sound Group specified. Footstep will not sound.");
                } else if (noMatch) {
                    DTGUIHelper.ShowRedError("Sound Group found no match. Type in or choose one.");
                }

                if (groupIndex.HasValue) {
                    if (existingIndex != groupIndex.Value) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Sound Group");
                    }
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (groupIndex.Value == -1) {
                        step.soundType = MasterAudio.NoGroupName;
                    } else {
                        step.soundType = _groupNames[groupIndex.Value];
                    }
                }
            } else {
                var newSType = EditorGUILayout.TextField("Sound Group", step.soundType);
                if (newSType != step.soundType) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Sound Group");
                    step.soundType = newSType;
                }
            }

            var newVarType = (EventSounds.VariationType)EditorGUILayout.EnumPopup("Variation Mode", step.variationType);
            if (newVarType != step.variationType) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Variation Mode");
                step.variationType = newVarType;
            }

            if (step.variationType == EventSounds.VariationType.PlaySpecific) {
                var newVarName = EditorGUILayout.TextField("Variation Name", step.variationName);
                if (newVarName != step.variationName) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Variation Name");
                    step.variationName = newVarName;
                }

                if (string.IsNullOrEmpty(step.variationName)) {
                    DTGUIHelper.ShowRedError("Variation Name is empty. No sound will play.");
                }
            }

            var newVol = DTGUIHelper.DisplayVolumeField(step.volume, DTGUIHelper.VolumeFieldType.None, MasterAudio.MixerWidthMode.Normal, 0f, true);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (newVol != step.volume) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Volume");
                step.volume = newVol;
            }

            var newFixedPitch = EditorGUILayout.Toggle("Override pitch?", step.useFixedPitch);
            if (newFixedPitch != step.useFixedPitch) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "toggle Override pitch");
                step.useFixedPitch = newFixedPitch;
            }
            if (step.useFixedPitch) {
                var newPitch = DTGUIHelper.DisplayPitchField(step.pitch);
                if (newPitch != step.pitch) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Pitch");
                    step.pitch = newPitch;
                }
            }

            var newDelay = EditorGUILayout.Slider("Delay Sound (sec)", step.delaySound, 0f, 10f);
            if (newDelay == step.delaySound) {
                DTGUIHelper.EndGroupedControls();
                continue;
            }

            DTGUIHelper.EndGroupedControls();

            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "change Delay Sound");
            step.delaySound = newDelay;

        }

        if (GUI.changed || _isDirty) {
            EditorUtility.SetDirty(target);
        }

        //DrawDefaultInspector();
    }

    private void AddFootstepSound() {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "Add Footstep Sound");
        _sounds.footstepGroups.Add(new FootstepGroup());
    }

    private void DeleteFootstepSound() {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "Delete Footstep Sound");
        _sounds.footstepGroups.RemoveAt(_sounds.footstepGroups.Count - 1);
    }

    private void ExpandCollapseAll(bool expand) {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _sounds, "toggle Expand / Collapse Footstep Groups");
        foreach (var t in _sounds.footstepGroups) {
            t.isExpanded = expand;
        }
    }
}

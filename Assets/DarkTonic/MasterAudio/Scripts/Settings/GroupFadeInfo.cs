using System;

[Serializable]
// ReSharper disable once CheckNamespace
public class GroupFadeInfo  {
	public MasterAudioGroup ActingGroup;
	public string NameOfGroup;
	public float TargetVolume;
	public float VolumeStep;
	public bool IsActive = true;
    // ReSharper disable once InconsistentNaming
	public Action completionAction;
}

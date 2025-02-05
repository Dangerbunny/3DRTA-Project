﻿using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public static class MasterAudioInspectorResources {
    public const string MasterAudioFolderPath = "MasterAudio";

    public static Texture LogoTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/inspector_header_master_audio.png", MasterAudioFolderPath)) as Texture;
    public static Texture DeleteTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/deleteIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture GearTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/gearIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture MuteOffTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/muteOff.png", MasterAudioFolderPath)) as Texture;
    public static Texture MuteOnTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/muteOn.png", MasterAudioFolderPath)) as Texture;
    public static Texture NextTrackTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/nextTrackIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture PauseTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/pauseIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture PauseOnTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/pauseIconOn.png", MasterAudioFolderPath)) as Texture;
    public static Texture PlaySongTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/playIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture PreviousTrackTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/prevTrackIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture RandomTrackTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/randomIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture SoloOffTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/soloOff.png", MasterAudioFolderPath)) as Texture;
    public static Texture SoloOnTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/soloOn.png", MasterAudioFolderPath)) as Texture;
    public static Texture PreviewTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/speakerIcon.png", MasterAudioFolderPath)) as Texture;
    public static Texture StopTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/stopIcon.png", MasterAudioFolderPath)) as Texture;
	public static Texture CopyTexture = EditorGUIUtility.LoadRequired(string.Format("{0}/copyIcon.png", MasterAudioFolderPath)) as Texture;

    public static Texture[] LedTextures = {
		EditorGUIUtility.LoadRequired(string.Format("{0}/LED5.png", MasterAudioFolderPath)) as Texture,
		EditorGUIUtility.LoadRequired(string.Format("{0}/LED4.png", MasterAudioFolderPath)) as Texture,
		EditorGUIUtility.LoadRequired(string.Format("{0}/LED3.png", MasterAudioFolderPath)) as Texture,
		EditorGUIUtility.LoadRequired(string.Format("{0}/LED2.png", MasterAudioFolderPath)) as Texture,
		EditorGUIUtility.LoadRequired(string.Format("{0}/LED1.png", MasterAudioFolderPath)) as Texture,
		EditorGUIUtility.LoadRequired(string.Format("{0}/LED0.png", MasterAudioFolderPath)) as Texture
	};
}

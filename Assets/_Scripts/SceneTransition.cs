using UnityEngine;
using System.Collections;

public class SceneTransition : MonoBehaviour {

	public int nextLevelNum = 1;

	void OnTriggerEnter(Collider other) { 
		//Application.LoadLevel(nextLevelNum); 
		AutoFade.LoadLevel (nextLevelNum, 2.5f, 1.0f, Color.black);
		MasterAudio.FadePlaylistToVolume (0f, 2.5f);
	}
}

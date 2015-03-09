using UnityEngine;
using System.Collections;

public class AnimationEvent : MonoBehaviour {

	public SceneManager sceneManager;

	/**
	 * This enum is just to help me remember which event is which 
	 */
	const int
		next = 0,
		sc3_elder = 1,
		sc2_dogControllable = 2,
		sc3_swoop = 3,
		sc3_ljEntrance = 4,
		sc3_ljControllable = 5,
		sc1_woflHowl1 = 6,
		sc1_wolfHowl2 = 7,
		sc1_ljNotice = 8,
		nextAndRetain = 9,
		sc2_howl = 10,
		sc2_confrontWolf = 11,
		sc2_nextScene = 12,
		sc3_wdDialogue = 13,
		sc3_ElderDialogue = 14;


	void fireEvent(int eventIndex){
		switch (eventIndex) {
		case next:
			sceneManager.nextCamera();
			break;
		case nextAndRetain:
			sceneManager.nextCameraAndRetain();
			break;
		case sc3_elder:
			sceneManager.enableActor(SceneManager.Actor.elder);
			sceneManager.nextCamera();
			break;
		case sc2_dogControllable:
			sceneManager.nextCamera();
			GameObject dog = sceneManager.getActor(SceneManager.Actor.dog);
			dog.GetComponent<DogController>().setControllable(true);
			dog.GetComponentInParent<MouseLook>().enabled = true;
			dog.GetComponentInParent<CharacterMotor>().enabled = true;
			GameObject.Find("FPS Camera").GetComponent<MouseLook>().enabled = true;
			break;
		case sc3_swoop:
			GameObject sceneSwoop = GameObject.Find("Scene Swoop");
			sceneSwoop.transform.parent = null;
			sceneSwoop.GetComponent<Animator>().SetTrigger("Go");
			break;
		case sc3_ljEntrance:
			sceneManager.nextCamera();
			GameObject lj = sceneManager.getActor(SceneManager.Actor.lumberjack);
			lj.SetActive(true);
			lj.GetComponent<PlayerControl>().enabled = false;
//			lj.GetComponent<FPSInputController>().enabled = false;

			break;
		case sc3_ljControllable:
			GameObject ljGO = sceneManager.getActor(SceneManager.Actor.lumberjack);
//			ljGO.GetComponentInChildren<LumberjackController>().setSpeed(0);
			ljGO.GetComponent<LumberjackController>().setControllable(true);
			ljGO.GetComponent<PlayerControl>().enabled = true;
//			ljGO.GetComponent<FPSInputController>().enabled = true;
			sceneManager.nextCamera();
			sceneManager.getActor(SceneManager.Actor.wolf).GetComponent<WolfController>().setFocus(ljGO);
			break;
		case sc1_woflHowl1:
			MasterAudio.TriggerNextPlaylistClip();
			MasterAudio.FireCustomEvent("Howl", new Vector3(10.13506f, 8.539352f, 16.24728f));
			break;
		case sc1_wolfHowl2:
			MasterAudio.FireCustomEvent("Howl", new Vector3(10.13506f, 8.539352f, 16.24728f));
			break;
		case sc1_ljNotice:
			sceneManager.getActor(SceneManager.Actor.lumberjack).GetComponent<LumberjackController>().setFocus(
				sceneManager.getActor(SceneManager.Actor.dog));
			MasterAudio.FireCustomEvent("LJDialogue", sceneManager.getActor(SceneManager.Actor.lumberjack).transform.position);
			break;
		case sc2_howl:
			MasterAudio.FireCustomEvent("Howl", sceneManager.getActor(SceneManager.Actor.wolf).transform.position);
			break;
		case sc2_confrontWolf:
			sceneManager.nextCamera();
			sceneManager.getActor(SceneManager.Actor.dog).GetComponent<DogController>().StartCoroutine("autoMove");
			break;
		case sc2_nextScene:
			sceneManager.nextCamera();
			AutoFade.LoadLevel (2, 2.5f, 1.0f, Color.black);
			MasterAudio.FadePlaylistToVolume (0f, 2.5f);
			break;
		case sc3_wdDialogue:
			MasterAudio.FireCustomEvent("DWD", Vector3.zero);
			break;
		case sc3_ElderDialogue:
			MasterAudio.FireCustomEvent("ElderD", Vector3.zero);
			sceneManager.nextCamera();
			break;
		}
		
	}
}
using UnityEngine;
using System.Collections;

public class AnimationEvent : MonoBehaviour {

	public SceneManager sceneManager;

	const int //Types of events
		next = 0,
		sc3_elder = 1,
		sc2_dogControllable = 2,
		sc3_swoop = 3,
		sc3_ljEntrance = 4,
		sc3_ljControllable = 5;


	void fireEvent(int eventIndex){
		switch (eventIndex) {
		case next:
			sceneManager.nextCamera();
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
			//dog.transform.parent.gameObject.GetComponentInChildren<MouseLook>().enabled = true;
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
		}
		
	}
}
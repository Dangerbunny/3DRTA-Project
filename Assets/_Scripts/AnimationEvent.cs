using UnityEngine;
using System.Collections;

public class AnimationEvent : MonoBehaviour {

	public SceneManager sceneManager;

	void fireEvent(int eventIndex){
		switch (eventIndex) {
		case 0:
			sceneManager.nextCamera();
			break;
		case 1:
			sceneManager.enableActor(SceneManager.Actor.elder);
			sceneManager.nextCamera();
			break;
		case 2:
			sceneManager.nextCamera();
			GameObject dog = sceneManager.getActor(SceneManager.Actor.dog);
			dog.GetComponent<DogController>().setControllable(true);
			dog.GetComponentInParent<MouseLook>().enabled = true;
			//dog.transform.parent.gameObject.GetComponentInChildren<MouseLook>().enabled = true;
			GameObject.Find("FPS Camera").GetComponent<MouseLook>().enabled = true;
			break;
		case 3:
			GameObject sceneSwoop = GameObject.Find("Scene Swoop");
			sceneSwoop.transform.parent = null;
			sceneSwoop.GetComponent<Animator>().SetTrigger("Go");
			break;
		case 4:
			sceneManager.nextCamera();
			GameObject lj = sceneManager.getActor(SceneManager.Actor.lumberjack);
			lj.SetActive(true);
			lj.GetComponent<MouseLook>().enabled = false;
			lj.GetComponent<FPSInputController>().enabled = false;

			break;
		case 5:
			GameObject ljGO = sceneManager.getActor(SceneManager.Actor.lumberjack);
			ljGO.GetComponentInChildren<LumberjackController>().setSpeed(0);
			ljGO.GetComponentInChildren<LumberjackController>().setControllable(true);
			ljGO.GetComponent<MouseLook>().enabled = true;
			ljGO.GetComponent<FPSInputController>().enabled = true;
			sceneManager.nextCamera();
			break;
		}
		
	}
}
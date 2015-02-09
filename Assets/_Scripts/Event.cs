using UnityEngine;
using System.Collections;

public class Event : MonoBehaviour {

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
		}

	}
}
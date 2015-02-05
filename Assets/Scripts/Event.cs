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
		}
	}
}
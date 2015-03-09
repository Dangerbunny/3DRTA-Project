using UnityEngine;
using System.Collections;

public class TransitionArea : MonoBehaviour {
	
	void OnTriggerEnter(Collider other){
		print ("Collider: " + other);
		if (other.gameObject.layer == 8) { //8 is character layer
			SceneManager manager = GameObject.Find ("Overlord").GetComponent<SceneManager> ();
			manager.nextCamera ();
			MasterAudio.FireCustomEvent ("Conversation", Vector3.zero); //Even though we're re-firing, we can't re trigger
			GameObject dog = manager.getActor(SceneManager.Actor.dog);
			dog.GetComponent<DogController>().setControllable(false);
			dog.GetComponentInParent<MouseLook>().enabled = false;
			dog.GetComponentInParent<CharacterMotor>().enabled = false;
			dog.GetComponent<Animator>().SetInteger("Speed", 0);
			//			Debug.Log("SETTING SPEED TO 0");
			//			GameObject.Find("FPS Camera").GetComponent<MouseLook>().enabled = true;
		} else if (other.gameObject.name == "elder"){
			SceneManager manager = GameObject.Find("Ultimate Overlord").GetComponent<SceneManager>();
			manager.enableActor(SceneManager.Actor.boy);
			manager.nextCamera();
			//			focus = sceneManager.getActor(SceneManager.Actor.boy);
		}
	}
}
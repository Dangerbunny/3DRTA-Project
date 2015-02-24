using UnityEngine;
using System.Collections;

public class WaypointFinal : Waypoint {

	public int effect;

	void OnTriggerEnter(Collider other){
		doEffect (other);
	}

	void doEffect(Collider other){
		print ("OTHER: " + other);
		switch (effect) {
		case 0:
			SceneManager manager = GameObject.Find("Ultimate Overlord").GetComponent<SceneManager>();
			other.transform.gameObject.GetComponentInChildren<LumberjackController>().setSpeed(0);
			manager.nextCamera();
			break;
		}
	}

}

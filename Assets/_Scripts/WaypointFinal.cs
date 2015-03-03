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
			
			LumberjackController ljC= other.transform.gameObject.GetComponentInChildren<LumberjackController>();
			if(ljC != null){
				ljC.setSpeed(0);
				SceneManager manager = GameObject.Find("Ultimate Overlord").GetComponent<SceneManager>();
				manager.nextCamera();
			}
			break;
		}
	}

}

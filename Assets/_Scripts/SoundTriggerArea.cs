using UnityEngine;
using System.Collections;

public class SoundTriggerArea: MonoBehaviour {
	
	void OnTriggerEnter(Collider other){
		//		print ("Collider: " + other);
		if (other.gameObject.name == "lumberjack"){
			MasterAudio.FireCustomEvent("Howl", Vector3.zero);
		}
	}
}

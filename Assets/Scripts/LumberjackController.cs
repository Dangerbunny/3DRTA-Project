using UnityEngine;
using System.Collections;

public class LumberjackController : MonoBehaviour {

	public int sceneNumber;
	public GameObject animal;
	
	// Use this for initialization
	void Start () {
		switch (sceneNumber) {
		case 0:
			print("Lumberjack: Chopping Wood");
			break;
	   	case 1:
			print ("Lumberjack: Searching for dog");
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float distance = Vector3.Distance (animal.transform.position, transform.position);
		switch (sceneNumber) {
		case 0:
			if(distance > 30f && distance < 40f)
				print("Lumberjack: Calling out to dog to come back");
			else if(distance >= 40f)
				print("Lumberjack: Decides to let him go, look for him if not back by dark");
			break;
		case 1:
			print ("Lumberjack: Searching for dog");
			break;
		}
	}
}

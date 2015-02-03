using UnityEngine;
using System.Collections;

public class LumberjackController : MonoBehaviour {

	public int sceneNumber;
	public GameObject animal;

	private Animator animator;
	private Vector3 lastPos;
//	 Use this for initialization
	void Start () {
		switch (sceneNumber) {
		case 1:
			print("Lumberjack: Chopping Wood");
			break;
	   	case 3:
			print ("Lumberjack: Searching for dog");
			lastPos = transform.position;
			animator = GetComponent<Animator>();
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch (sceneNumber) {
		case 1:
			float distance = Vector3.Distance (animal.transform.position, transform.position);
			if(distance > 30f && distance < 40f)
				print("Lumberjack: Calling out to dog to come back");
			else if(distance >= 40f)
				print("Lumberjack: Decides to let him go, look for him if not back by dark");
			break;
		case 3:
//			if (distance < 10)
			Vector3 velocity = transform.position - lastPos;
			lastPos = transform.position;
			animator.SetFloat("Speed", velocity.magnitude);
			print ("Speed: " + velocity.magnitude);
			print("Lumberjack: Kills wolf (after a truly epic battle) and is reunited with dog");
			break;
		}
	}
	
}

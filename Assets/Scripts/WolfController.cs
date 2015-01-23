using UnityEngine;
using System.Collections;

public class WolfController : MonoBehaviour {

	public int sceneNumber;
	public GameObject other;
	public float speed, rotationSpeed;

	NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		switch (sceneNumber) {
		case 1:
			print("Wolf: Howling");
			break;
		case 2:
			agent = GetComponent<NavMeshAgent>();
			print ("Wolf: Talking with dog, waiting for lumberjack");
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float distance = Vector3.Distance (other.transform.position, transform.position);
		switch (sceneNumber) {
		case 1:
			if(distance < 10f)
				print("Wolf: Talking to dog");
			break;
		case 2:
			if(distance < 30f){
				print ("Wolf: Attacking Lumberjack");
				agent.SetDestination(other.transform.position);
			}
			break;
		}
	}
}

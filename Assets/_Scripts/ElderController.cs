using UnityEngine;
using System.Collections;

public class ElderController : MonoBehaviour {

	public Transform[] path_corners;
	public GameObject boy;

	float rotationSpeed = 75;
	NavMeshAgent agent;
	int currentPath = 0;

	void Start(){
		agent = GetComponent<NavMeshAgent> ();
	}

	// Update is called once per frame
	void Update () {
//		float distance = Vector3.Distance(path_corners[currentPath].position, transform.position);
//		if (distance <= 5f) {
//			if(currentPath == path_corners.Length - 1){
//				gameObject.SetActive(false);
//				return;
//			}
//			currentPath++;
//		}
//		agent.SetDestination(path_corners[currentPath].position);

	}

	public void doubleSpeed(){
		agent.speed *= 2;
	}
	public void halfSpeed(){
		agent.speed /= 2;
	}

}

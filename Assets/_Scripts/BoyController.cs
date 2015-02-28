using UnityEngine;
using System.Collections;

public class BoyController : MonoBehaviour {

//	public Transform[] path_corners;

	public Transform elder;
	 Vector3 elders_side_offset;
	 Animator animator;

	NavMeshAgent agent;

	void Awake(){
		elders_side_offset = transform.position - elder.position;
	}

	void Start(){
		animator = GetComponent<Animator> ();
		agent = GetComponent<NavMeshAgent> ();
	}

	// Update is called once per frame
	void Update () {
//		if (elder.gameObject.activeInHierarchy == false){
//			gameObject.SetActive (false);
//			return;
//		}
//		Vector3 elders_side = elder.position+elders_side_offset;
//		float distance = Vector3.Distance(elders_side, transform.position);
//		if (distance <= 2f) {
//			animator.SetBool ("NearElder", true);
//		} else {
//			animator.SetBool("NearElder", false);
//		}
//		
//		agent.SetDestination(elders_side);
	}
}

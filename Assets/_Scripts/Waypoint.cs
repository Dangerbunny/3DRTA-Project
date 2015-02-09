using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {

	WaypointChain owner;

	void Start () {
		owner = GetComponentInParent<WaypointChain> ();
	}

	void OnTriggerEnter(Collider other){
		owner.nextPoint ();
	}

}

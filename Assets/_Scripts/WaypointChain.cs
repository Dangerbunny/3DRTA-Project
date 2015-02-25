using UnityEngine;
using System.Collections;

public class WaypointChain : MonoBehaviour {

	Transform[] waypoints;
	Transform destination;

	int current;

	void Start(){
		current = 1; //First waypoint is the position of the chain object
		waypoints = GetComponentsInChildren<Transform> ();
		print (waypoints.Length);
	}
	public Transform nextPoint(){
		if(current != waypoints.Length)
			current++;
		return waypoints [current];
	}
	public Transform currentPoint(){
		return waypoints [current];
	}

}

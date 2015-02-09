using UnityEngine;
using System.Collections;

public class WaypointChain : MonoBehaviour {

	private Transform[] waypoints;
	private Transform destination;

	private int current;

//	private Quaternion startRotation;
//
//	private float startTime;
//	private float journeyLength;

	void Start(){
		current = 1; //First waypoint is the position of the chain object
		waypoints = GetComponentsInChildren<Transform> ();
		print (waypoints.Length);
	}
	public Transform nextPoint(){
		current++;
		return waypoints [current];
	}
	public Transform currentPoint(){
		return waypoints [current];
	}

//	public void setStart(float time, Transform startPos){
//		startTime = time;
//		journeyLength = (waypoints[current].position - startPos.position).magnitude;
//		startRotation = startPos.rotation;
//	}

//	public Quaternion alignmentRotation(float speed, Quaternion endRotation){
//		float distCovered = (Time.time - startTime) * speed;
//		float fracJourney = distCovered / journeyLength;
//		return Quaternion.Lerp(startRotation, endRotation, fracJourney);
//	}

	//				if (currentPath != path_corners.Length) {
	//					float distCovered = (Time.time - startTime) * speeds[currentPath];
	//					float fracJourney = distCovered / journeyLength;
	//					transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
	//					transform.rotation = Quaternion.Lerp(startMarker.rotation, endMarker.rotation, fracJourney);
	//					if(Vector3.Distance(transform.position, path_corners[currentPath].position) < 2	)
	//						pointReached();
	//				}
}

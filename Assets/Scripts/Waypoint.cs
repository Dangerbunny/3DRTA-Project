using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		print ("Collider: " + other);
		DogController c = other.GetComponentInParent<DogController> ();
		if (c != null)
			c.nextWaypoint();
	}
}

using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		Debug.Log ("Collider: " + other);
		WolfController c = other.GetComponentInParent<WolfController> ();
		if(c != null)
			c.takeDamage ();
	}
}

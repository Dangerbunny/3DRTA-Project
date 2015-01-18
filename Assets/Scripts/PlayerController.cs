using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	Transform camera;

	// Update is called once per frame
	void Update () {
		float hori = Input.GetAxis ("Horizontal");
		float vert = Input.GetAxis ("Vertical");

		transform.position = transform.position + transform.forward;
	}
}

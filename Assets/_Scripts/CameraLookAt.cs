using UnityEngine;
using System.Collections;

public class CameraLookAt : MonoBehaviour {

	public Transform targetPos;

	// Update is called once per frame
	void Update () {
		transform.forward = targetPos.position - transform.position;
	}
}

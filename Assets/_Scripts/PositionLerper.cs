using UnityEngine;
using System.Collections;

public class PositionLerper : MonoBehaviour {

	public Transform startMarker;
	public Transform endMarker;
	public float speed = 1.0F;
	 float startTime;
	 float journeyLength;

	void Start() {
		startTime = Time.time;
		startMarker = transform;
		journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
	}
	void Update() {
		float distCovered = (Time.time - startTime) * speed;
		float fracJourney = distCovered / journeyLength;
		transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
	}
}
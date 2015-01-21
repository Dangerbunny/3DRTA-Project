using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform[] views;
	public float[] transitionDurations;

	public Transform startMarker;
	public Transform endMarker;
	public float speed = 1.0F;
	private float startTime;
	private float journeyLength;
	public float smooth = 5.0F;
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

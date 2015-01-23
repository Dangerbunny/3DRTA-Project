using UnityEngine;
using System.Collections;

public class DogController : MonoBehaviour {

	public GameObject other;

	public int sceneNumber;
	public Transform[] paths;
	public float[] speeds;

	private int currentPath = 0;

	private Transform startMarker;
	private Transform endMarker;

	private float startTime;
	private float journeyLength;

	private bool awake = false;


	IEnumerator Start(){

		switch(sceneNumber){
		case 0:
			print ("Dog: Sleeping");
			yield return new WaitForSeconds (7);
			print ("Dog: Hears Wolf's Howl");
			yield return new WaitForSeconds (4);
			print ("Dog: Resents wolf, decides to do something about it");
			awake = true;

			startTime = Time.time;
			startMarker = transform;
			endMarker = paths [0];
			journeyLength = Vector3.Distance(startMarker.position, endMarker.position);

			break;
		case 1:
			print ("Dog: Looking for wolf");
			break;	
		}


	}

	void Update () {
		switch (sceneNumber) {
		case 0:
			if(awake){
				if (currentPath != paths.Length) {
					float distCovered = (Time.time - startTime) * speeds[currentPath];
					float fracJourney = distCovered / journeyLength;
					transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
					transform.rotation = Quaternion.Lerp(startMarker.rotation, endMarker.rotation, fracJourney);
					if(transform.position == paths[currentPath].position)
						pointReached();
				}
			}
			break;
		case 1:
			float distance = Vector3.Distance (other.transform.position, transform.position);
			if(distance < 10f){
				print ("Dog: Confronting wolf");
			}
			break;
		case 2:
			break;
		}
	}

	void pointReached(){
		currentPath++;
		startMarker = transform;
		endMarker = paths [currentPath];
		startTime = Time.time;
		journeyLength = Vector3.Distance (startMarker.position, endMarker.position);
	}	

}

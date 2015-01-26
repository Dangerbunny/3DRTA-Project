using UnityEngine;
using System.Collections;

public class DogController : MonoBehaviour {
	
	public GameObject other;
	
	public int sceneNumber;
	public Transform[] path_corners;
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
			endMarker = path_corners [0];
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
				if (currentPath != path_corners.Length) {
					float distCovered = (Time.time - startTime) * speeds[currentPath];
					float fracJourney = distCovered / journeyLength;
					transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
					transform.rotation = Quaternion.Lerp(startMarker.rotation, endMarker.rotation, fracJourney);
					if(Vector3.Distance(transform.position, path_corners[currentPath].position) < 2	)
						pointReached();
				}
				
			}
			break;
		case 1:
			float distance = Vector3.Distance (other.transform.position, transform.position);
			if(distance < 10f){
				print("Dog: Confronting wolf");
				AutoFade.LoadLevel(2, 6f, 1.5f, Color.black);
				
			}
			break;
		case 2:
			print ("Dog: Waiting for lumberjack");
			break;
		}
	}
	
	void pointReached(){
		currentPath++;
		if (currentPath >= path_corners.Length) {
			AutoFade.LoadLevel (1, 2.5f, 1.0f, Color.black);
			return;
		} else {
			startMarker = transform;
			endMarker = path_corners [currentPath];
			startTime = Time.time;
			journeyLength = Vector3.Distance (startMarker.position, endMarker.position);
		}
	}	
}

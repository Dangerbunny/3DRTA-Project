using UnityEngine;
using System.Collections;

public class DogController : MonoBehaviour {
	
	public SceneManager sceneManager;
	public int sceneNumber;
	public Transform[] path_corners;
	public float[] speeds;
	
	private int currentPath = 0;
	
	private Transform startMarker;
	private Transform endMarker;
	
	private float startTime;
	private float journeyLength;
	
	private bool awake = false;

	private Animator animator;
	private GameObject focus;

	private NavMeshAgent agent;

	private bool controllable;

	private Transform currentDest;

	IEnumerator Start(){
		animator = GetComponent<Animator> ();
		switch(sceneNumber){
		case 1:
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
		case 2:
			focus = sceneManager.getActor(SceneManager.Actor.wolf);
			agent = GetComponent<NavMeshAgent>();
			currentDest = path_corners[0];
			controllable = false;
			break;	
		}
		
		
	}
	
	void Update () {
		switch (sceneNumber) {
		case 1:
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
		case 2:
			if(controllable){
				float distance = Vector3.Distance (focus.transform.position, transform.position);
				if(distance < 10f){
					AutoFade.LoadLevel(2, 6f, 1.5f, Color.black);
				}
				int move = (int)Input.GetAxisRaw("Vertical");
				bool jump = Input.GetButtonDown("Jump");
				if(move != 0)
					animator.SetInteger("Speed", 1);
				else
					animator.SetInteger("Speed", 0);
				if(jump)
					animator.SetTrigger("Jump");
	//			else
	//				animator.SetTrigger("Jumping");
			} else{
//				Vector3 targetDir = currentDest.position - transform.position;
//				Quaternion rotation = new Quaternion(
//				transform.Rotate(targetDir);
			}
			break;
		case 3:
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

	public void setControllable(bool state){
		controllable = state;
	}

	public void nextWaypoint(){
		print ("Next");
		
		currentDest = path_corners [++currentPath];
	}
}

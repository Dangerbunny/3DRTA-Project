using UnityEngine;
using System.Collections;

public class DogController : MonoBehaviour {
	
	public SceneManager sceneManager;
	public float speed;
	public float rotationSpeed;
	public WaypointChain wpChain;

	Transform destination;

	int sceneNumber;
	int currentPath = 0;

	bool controllable;

	Animator animator;
	GameObject focus;

	bool awake;

	CharacterController controller;

	IEnumerator Start(){
		sceneNumber = sceneManager.getSceneNumber ();
		animator = GetComponent<Animator> ();

		switch(sceneNumber){
		case 1:
			destination = wpChain.currentPoint();
			print (destination.position);
			
			controller = GetComponent<CharacterController>();
			awake = false;
			controllable = false;
			animator.SetBool("Asleep", true);

			const float howlDuration = 4.5f;
			const float speech1Duration = 9.5f;
			const float speech2Duration = 4.1f;

			/**
			 * Waiting for:
			 * 1. First howl to finish,
			 * 2. First speech (while on ground) to finish,
			 * 3. Second howl to finish 
			 */
			yield return new WaitForSeconds (16.6f+howlDuration+speech1Duration+howlDuration);

			//Makes decision and gets up
			animator.SetBool("Asleep", false);
			sceneManager.nextCamera();

			//Narrates decision
			yield return new WaitForSeconds (speech2Duration);

			awake = true;

			animator.SetInteger("Speed", 1);
			sceneManager.nextCamera();

			break;
		case 2:
			focus = sceneManager.getActor(SceneManager.Actor.wolf);
			controller = GetComponentInParent<CharacterController>();
			controllable = false;
			break;	
		case 3:
			focus = sceneManager.getActor(SceneManager.Actor.wolf);
			break;
		}
		
		
	}
	
	void Update () {
		switch (sceneNumber) {
		case 1:
			if(awake){
				Vector3 dir = (wpChain.currentPoint().position - transform.position).normalized;
				dir.y = 0;
				transform.forward = Vector3.Lerp(transform.forward, dir, rotationSpeed * Time.deltaTime);
				controller.SimpleMove (transform.forward * speed * Time.deltaTime);
				
			}
			break;
		case 2:
			if(controllable){
				int move = (int)Input.GetAxisRaw("Vertical");
				bool jump = Input.GetKeyDown(KeyCode.Space);
				if(move != 0)
					animator.SetInteger("Speed", 2);
				else
					animator.SetInteger("Speed", 0);
//				if(jump){
//					animator.SetTrigger("Jump");
//					Debug.Log("JUMP TRIGER");
//				}
			} else{
				float distance = Vector3.Distance (focus.transform.position, transform.position);
				if(distance < 10f){
					Vector3 dir = (focus.transform.position - transform.position).normalized;
					dir.y = 0;
					transform.forward = Vector3.Lerp(transform.forward, dir, rotationSpeed * Time.deltaTime);
				} else{
//					print ("SETTING SPEED");
					animator.SetInteger("Speed", 1);
				}
			}
			break;
		case 3:

			break;
		}
	}

	public IEnumerator autoMove(){
		float distance = Vector3.Distance (focus.transform.position, transform.position);
		while (distance > 3.5) {
			animator.SetInteger("Speed", 1);
			controller.SimpleMove (transform.forward * speed * Time.deltaTime);
			distance = Vector3.Distance (focus.transform.position, transform.position);
			yield return new WaitForSeconds(0);
		}
		animator.SetInteger("Speed", 0);

	}
	public void setControllable(bool state){
		controllable = state;
	}
	
	public GameObject getFocus ()
	{
		return focus;
	}
}

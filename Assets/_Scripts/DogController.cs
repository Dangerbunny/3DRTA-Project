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

			yield return new WaitForSeconds (16.6f);

			//Inner dialogue
			animator.SetBool("Asleep", false);

			yield return new WaitForSeconds (4);

			awake = true;

//			wpChain.setStart(Time.time, transform);
//			print ("Dog: Hears Wolf's Howl");
			animator.SetInteger("Speed", 1);
			sceneManager.nextCamera();
//			print ("Dog: Resents wolf, decides to do something about it");


			break;
		case 2:
			focus = sceneManager.getActor(SceneManager.Actor.wolf);
			controllable = false;
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
				controller.SimpleMove(transform.forward * speed * Time.deltaTime);
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
					animator.SetInteger("Speed", 2);
				else
					animator.SetInteger("Speed", 0);
				if(jump)
					animator.SetTrigger("Jump");
	//			else
	//				animator.SetTrigger("Jumping");
			} else{
				animator.SetInteger("Speed", 1);
//				Vector3 targetDir = currentDest.position - transform.position;
//				Quaternion rotation = new Quaternion(
//				transform.Rotate(targetDir);
			}
			break;
		case 3:
//			print ("Dog: Waiting for lumberjack");
			break;
		}
	}


	public void setControllable(bool state){
		controllable = state;
	}
	
}

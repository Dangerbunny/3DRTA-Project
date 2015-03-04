using UnityEngine;
using System.Collections;

public class LumberjackController : MonoBehaviour {

	public SceneManager sceneManager;
	public WaypointChain wpChain;
	public float rotationSpeed;
	public float autonomousSpeed;

	GameObject focus;
	Animator animator;
	int sceneNumber;
	bool controllable = false;
	CharacterController controller;

	IEnumerator Start () {
		sceneNumber = sceneManager.getSceneNumber ();
		animator = GetComponent<Animator>();
		switch (sceneNumber) {
		case 1:
			focus = sceneManager.getActor(SceneManager.Actor.dog);
			yield return new WaitForSeconds(5.9f);
			animator.SetTrigger("SitDown");
			break;
	   	case 3:
			controller = GetComponentInParent<CharacterController>();
			print ("Lumberjack: Searching for dog");
			break;
		}
	}
	
	void Update () {
		switch (sceneNumber) {
		case 1:
			float distance = Vector3.Distance (focus.transform.position, transform.position);
			if(distance > 32.5f && distance < 40f){
				animator.SetTrigger("GetUp");
				Vector3 dir = (focus.transform.position - transform.position).normalized;
				dir.y = 0;
				transform.forward = Vector3.Lerp(transform.forward, dir, rotationSpeed * Time.deltaTime);
				print("Lumberjack: Calling out to dog to come back");
			}
			else if(distance >= 40f)
				print("Lumberjack: Decides to let him go, look for him if not back by dark");
			break;
		case 3:
			if(controllable){
				float move = Input.GetAxisRaw("Vertical");
				bool sprint = Input.GetKey(KeyCode.LeftShift);
				if(move != 0){
					if(sprint)
						animator.SetInteger("Speed", 2);
					else
						animator.SetInteger("Speed", 1);
				}
				else
					animator.SetInteger("Speed", 0);

				bool attack = Input.GetMouseButtonDown(0);
				if(attack)
					animator.SetTrigger("Attack");
			} else{
				if(autonomousSpeed > 0){
					animator.SetInteger("Speed", 1);
					Transform parentT = transform.parent.transform;
					Vector3 dir = (wpChain.currentPoint().position - parentT.position).normalized;
					dir.y = 0;
					parentT.forward = Vector3.Lerp(parentT.forward, dir, rotationSpeed * Time.deltaTime);
					controller.SimpleMove(transform.forward * autonomousSpeed * Time.deltaTime);
				} else {
					animator.SetInteger("Speed", 0);
				}
			}
			break;
		case 4:

			bool attack = Input.GetMouseButtonDown(0);
			print ("FOOO, attack: " + attack);
			if(attack)
				animator.SetTrigger("Attack");
			break;
		}
	}

	public void setSpeed(float speed){
		autonomousSpeed = speed;
	}

	public void setControllable (bool control)
	{
		controllable = control;
	}
}

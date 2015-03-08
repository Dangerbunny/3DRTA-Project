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
			yield return new WaitForSeconds(5.9f);
			animator.SetTrigger("SitDown");
			break;
	   	case 3:
//			controller = GetComponentInParent<CharacterController>();
			animator.SetBool ("Grounded", true);
			break;
		}
	}
	
	void Update () {
		switch (sceneNumber) {
		case 1:
//			float distance = Vector3.Distance (focus.transform.position, transform.position);
//			if(distance > 32.5f && distance < 40f){
			if(focus != null){

				animator.SetTrigger("GetUp");
				Vector3 dir = (focus.transform.position - transform.position).normalized;
				dir.y = 0;
				transform.forward = Vector3.Lerp(transform.forward, dir, rotationSpeed * Time.deltaTime);
			}
			break;
		case 3:
			if(controllable){
				bool attack = Input.GetMouseButtonDown(0);
				if(attack)
					animator.SetTrigger("Attack");
			}
			break;
		}
	}

	public void setFocus(GameObject focus){
		this.focus = focus;
	}
	public void setSpeed(float speed){
		autonomousSpeed = speed;
	}

	public void setControllable (bool control)
	{
		controllable = control;
	}
	public bool isControllable ()
	{
		return controllable;
	}
}

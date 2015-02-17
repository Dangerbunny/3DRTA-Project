using UnityEngine;
using System.Collections;

public class LumberjackController : MonoBehaviour {

	public SceneManager sceneManager;

	public float rotationSpeed;

	 GameObject focus;
	 Animator animator;
	 int sceneNumber;
		
//	 Use this for initialization
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
			print ("Lumberjack: Searching for dog");
			break;
		}
	}
	
	// Update is called once per frame
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
//			print("Lumberjack: Kills wolf (after a truly epic battle) and is reunited with dog");
			break;
		}
	}
	
}

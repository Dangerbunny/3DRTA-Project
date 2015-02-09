using UnityEngine;
using System.Collections;

public class LumberjackController : MonoBehaviour {

	public SceneManager sceneManager;


	private GameObject focus;
	private Animator animator;
	private int sceneNumber;
	
//	 Use this for initialization
	void Start () {
		sceneNumber = sceneManager.getSceneNumber ();
		switch (sceneNumber) {
		case 1:
			focus = sceneManager.getActor(SceneManager.Actor.dog);
			print("Lumberjack: Chopping Wood");
			break;
	   	case 3:
			print ("Lumberjack: Searching for dog");
			animator = GetComponent<Animator>();
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch (sceneNumber) {
		case 1:
			float distance = Vector3.Distance (focus.transform.position, transform.position);
			if(distance > 30f && distance < 40f)
				print("Lumberjack: Calling out to dog to come back");
			else if(distance >= 40f)
				print("Lumberjack: Decides to let him go, look for him if not back by dark");
			break;
		case 3:
			float move = Input.GetAxisRaw("Vertical");
			if(move != 0)
				animator.SetBool("Moving", true);
			else
				animator.SetBool("Moving", false);

			bool attack = Input.GetMouseButtonDown(0);
			if(attack)
				animator.SetTrigger("Attack");
//			print("Lumberjack: Kills wolf (after a truly epic battle) and is reunited with dog");
			break;
		}
	}
	
}

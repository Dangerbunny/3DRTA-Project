using UnityEngine;
using System.Collections;

public class WolfController : MonoBehaviour {
	
	public int sceneNumber;

	public SceneManager sceneManager;
	private Animator animator;
	private GameObject dog, elder, boy, lumberjack, focus;
	private int hits = 3;
	private bool alive = true;
	
	NavMeshAgent agent;
	
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		switch (sceneNumber) {
		case 2:
			dog = sceneManager.getActor(SceneManager.Actor.dog);
			focus = dog;
			print("Wolf: Howling");
			break;
		case 3:
			agent = GetComponent<NavMeshAgent>();
			dog = sceneManager.getActor(SceneManager.Actor.dog);
			elder = sceneManager.getActor(SceneManager.Actor.elder);
			boy = sceneManager.getActor(SceneManager.Actor.boy);
			lumberjack = sceneManager.getActor(SceneManager.Actor.lumberjack);
			focus = elder;
			print ("Wolf: Talking with dog, waiting for lumberjack");
//			peopleEncountered = 0;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float distance;
		switch (sceneNumber) {
		case 2:
			distance = Vector3.Distance (focus.transform.position, transform.position);
			if(distance < 10f)
				print("Wolf: Talking to dog");
			break;
		case 3:
			if(alive){
				distance = Vector3.Distance (focus.transform.position, transform.position);
				
				if(distance < 30f){
					if(focus.name == "lumberjack"){
						print ("Dog: That's him!");
						print ("Wolf: Attacking Lumberjack");
						animator.SetBool("Moving", true);
						agent.SetDestination(focus.transform.position);
					}
				}
				else if(distance < 40f){
					if(focus.name != "lumberjack"){
						if(focus.name == "elder"){
							print("Dog: Tells wolf that's not him");
							sceneManager.enableActor(SceneManager.Actor.boy);
							sceneManager.nextCamera();
							focus = boy;
						}
						else if(focus.name == "boy"){
							print ("Dog: That's not him either");
							sceneManager.nextCamera();
							sceneManager.enableActor(SceneManager.Actor.lumberjack);
							focus = lumberjack;
						}
						
					} else{
						print ("Dog: Howls to get lumberjack's attention");
						print("Lumberjack: Goes to investigate");
					}
				}
			}
			break;
		}
	}

	public void takeDamage(){
		Debug.Log ("A HIT!");
		if (--hits == 0) {
			alive = false;
			animator.SetBool("Dead", true);
		}
	}
}

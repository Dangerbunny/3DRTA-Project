using UnityEngine;
using System.Collections;

public class WolfController : MonoBehaviour {

	/**
	 * WolfController and a number of the other character ish controllers need to be refactored,
	 * as they share a fair bit of code that could be cut down via inheritance
	 **/

	public SceneManager sceneManager;
	public float rotationSpeed;
	public float speed;
	public float impactForce;


	Animator animator;
	GameObject focus;
	bool alive = true;
	int sceneNumber;
	int hits = 4, anger = 0;
	Vector3 impact = Vector3.zero;
	float mass = 3.0F; // defines the character mass
	
	
//	NavMeshAgent agent;

	float immuneTimer = 0, immunePeriod = 1.2f;

	AudioSource audio;

	CharacterController controller;

	#region Start()
	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource> ();
		sceneNumber = sceneManager.getSceneNumber ();
		animator = GetComponent<Animator> ();
		switch (sceneNumber) {
		case 2:
			focus = sceneManager.getActor(SceneManager.Actor.dog);
			print("Wolf: Howling");
			break;
		case 3:
			controller = GetComponent<CharacterController>();
//			agent = GetComponent<NavMeshAgent>();

			focus = sceneManager.getActor(SceneManager.Actor.elder);
			print ("Wolf: Talking with dog, waiting for lumberjack");
			break;
		}
	}
	#endregion

	#region Update()
	void Update () {
		float distance;
		switch (sceneNumber) {
		case 2:
			distance = Vector3.Distance (focus.transform.position, transform.position);
			if(distance < 10f){
				print("Wolf: Talking to dog");
				Vector3 dir = (focus.transform.position - transform.position).normalized;
				dir.y = 0;
				transform.forward = Vector3.Lerp(transform.forward, dir, rotationSpeed * Time.deltaTime);
			}
			break;
		case 3:
			if(Input.GetKeyDown(KeyCode.KeypadEnter)){
				AddImpact(-transform.forward + new Vector3(0,20f,0), impactForce);
			}
			if(alive){
				if (impact.magnitude > 0.2F) controller.Move(impact * Time.deltaTime);
				impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);

				immuneTimer += Time.deltaTime;
				distance = Vector3.Distance (focus.transform.position, transform.position);

				if(distance < 30f){
//					bool attacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking");
//					if(focus.name == "lumberjack" && !attacking){
//						animator.SetBool("Moving", true);
////						agent.SetDestination(focus.transform.position);
//						Vector3 dir = (focus.transform.position - transform.position).normalized;
//						dir.y = 0;
//						transform.forward = Vector3.Lerp(transform.forward, dir, rotationSpeed * Time.deltaTime);
//						controller.SimpleMove(transform.forward * speed * Time.deltaTime);
//						print ("Movement: " + (transform.forward * speed * Time.deltaTime));
//						if(distance < 5f){
//							animator.SetTrigger("Attack");
//						}
//					}
				}
				else if(distance < 40f){
					if(focus.name != "lumberjack"){
						if(focus.name == "elder"){
							print("Dog: Tells wolf that's not him");
							sceneManager.enableActor(SceneManager.Actor.boy);
							sceneManager.nextCamera();
							focus = sceneManager.getActor(SceneManager.Actor.boy);
						}
						else if(focus.name == "boy"){
							print ("Dog: That's not him either");
//							sceneManager.nextCamera();
//							sceneManager.enableActor(SceneManager.Actor.lumberjack);
//							focus = sceneManager.getActor(SceneManager.Actor.lumberjack);
							//---------------------------------------------------------------------------------
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
	#endregion

	#region Damaging mechanics

	public void AddImpact(Vector3 dir, float force){
		dir.Normalize();
		if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
		impact += dir.normalized * force / mass;
	}

	IEnumerator enrage(Vector3 endScale){
		const int scaleFactor = 5;
		while (endScale.magnitude - transform.localScale.magnitude > 0.0001f) {
			transform.localScale = Vector3.Lerp (transform.localScale, endScale, Time.deltaTime*scaleFactor);
			yield return new WaitForSeconds(0);
		}
	}

	public void enableNavAgent(){
//		agent.enabled = true;
	}

	public bool isAlive(){
		return alive;
	}

	public void setFocus(GameObject f){
		focus = f;
	}

	public void takeDamage(){
		if (immuneTimer >= immunePeriod && focus.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Attacking")) {
			anger++;
			if(anger <= 3){
				Vector3 endScale = transform.localScale * 1.2f;
				StartCoroutine(enrage(endScale));
				audio.Play();
//				agent.enabled = false;
//				print ("RIGIDBODY: " + GetComponent<Rigidbody>() + " and adding force: " + (-transform.forward + new Vector3(0,2f,0)));
//				GetComponent<Rigidbody>().AddForce(125*(-transform.forward + new Vector3(0,2f,0)));
				AddImpact(-transform.forward, impactForce);
			}
			if (--hits == 0) {
					alive = false;
					animator.SetBool ("Dead", true);
			}
			immuneTimer = 0;
		}
	}
	#endregion
}

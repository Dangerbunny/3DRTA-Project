using UnityEngine;
using System.Collections;

public class WolfController : MonoBehaviour {
	
	public int sceneNumber;
	public GameObject[] other;
	public float speed, rotationSpeed;

	public SceneManager sceneManager;

	int peopleEncountered;
	
	enum Person{
		OLD_MAN = 0,
		BOY = 1,
		LUMBERJACK = 2,
		DOG = 3
	}
	
	Person person;
	
	NavMeshAgent agent;
	
	// Use this for initialization
	void Start () {
		switch (sceneNumber) {
		case 2:
			person = Person.DOG;
			print("Wolf: Howling");
			break;
		case 3:
			agent = GetComponent<NavMeshAgent>();
			print ("Wolf: Talking with dog, waiting for lumberjack");
			peopleEncountered = 0;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float distance;
		switch (sceneNumber) {
		case 2:
			person = Person.DOG-3;
			distance = Vector3.Distance (other[(int)person].transform.position, transform.position);
			if(distance < 10f)
				print("Wolf: Talking to dog");
			break;
		case 3:
			distance = Vector3.Distance (other[(int)person].transform.position, transform.position);
			
			if(distance < 30f){
				if(person == Person.LUMBERJACK){
					print ("Dog: That's him!");
					print ("Wolf: Attacking Lumberjack");
					agent.SetDestination(other[(int)person].transform.position);
				}
			}
			else 
			if(distance < 40f){
				if(person != Person.LUMBERJACK){
					if(peopleEncountered == 0){
						print("Dog: Tells wolf that's not him");
						peopleEncountered++;
					}
					else {//Otherwise people encountered = 1
						print ("Dog: That's not him either");
						sceneManager.nextCamera();
					}
					person++;
					other[(int)person].SetActive(true);
				} else{
					print ("Dog: Howls to get lumberjack's attention");
					print("Lumberjack: Goes to investigate");
				}
			} 
			break;
		}
	}
}

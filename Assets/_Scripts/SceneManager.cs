using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {

	public int sceneNumber;

	public Dictionary<Actor, GameObject> actors;
	
	CameraManager cameraManager;

	public enum Actor{
		elder,
		boy,
		lumberjack,
		dog,
		wolf
	}
	
	// Use this for initialization
	void Awake () {
		cameraManager = GetComponentInChildren<CameraManager> ();
		actors = new Dictionary<Actor,GameObject> ();
		switch (sceneNumber){
		case 1:
			actors.Add(Actor.dog, GameObject.Find("dog"));
			actors.Add(Actor.lumberjack, GameObject.Find("lumberjack"));
			break;
		case 2:
			actors.Add(Actor.dog, GameObject.Find("dog"));
			actors.Add(Actor.wolf, GameObject.Find("wolf"));
			break;
		case 3:
			actors.Add(Actor.elder, GameObject.Find("elder"));
			actors.Add(Actor.boy, GameObject.Find("boy"));
			actors.Add(Actor.lumberjack, GameObject.Find("lumberjack"));
			actors.Add(Actor.dog, GameObject.Find("dog"));
			actors.Add(Actor.wolf, GameObject.Find("wolf"));
			
			disableActor(Actor.elder);
			disableActor(Actor.boy);
			disableActor(Actor.lumberjack);
			break;
		}
	}
	

	public GameObject getActor(Actor a){
		return actors [a];
	}

	public int getSceneNumber ()
	{
		return sceneNumber;
	}

	public void playMusic(string mName){

	}
	public void playSFX(string eName){
		GetComponent<AudioSource>().Play();
	}

	public void nextCamera(){
		cameraManager.nextCamera ();
	}

	public void enableActor(Actor a){
//		Debug.Log ("Enabling: " + a);
		actors [a].SetActive (true);
	}
	public void disableActor(Actor a){
//		Debug.Log ("Disabling: " + a);
		actors [a].SetActive (false);
	}
}

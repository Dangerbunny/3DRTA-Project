using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	
	public GameObject[] cameras;
//	 GameObject currentCamera;

	 int camIndex = 0;

	void Start () {
//		currentCamera = cameras [0];

		for (int i = 1; i < cameras.Length; i ++)
			if(cameras[i] != cameras[0])
				cameras [i].SetActive (false);
	}

	/**
	 * Advance to the next camera
	 * */
	public void nextCamera(){
		cameras[camIndex].SetActive(false);
		camIndex++;
		print ("Advancing camera to: " + camIndex);
		cameras[camIndex].SetActive(true);
	}

	/** 
	 * Advance to the next camera, but retain animation control from this camera.
	 * This is used so that this camera's animation can manage events for the next few shots.
	 * Particularly usefull if the next shots are static and thus cannot fire their own animation
	 * events.
	 * */
	public void nextCameraAndRetain(){
		cameras[camIndex].GetComponent<Camera>().enabled = false;
		cameras[camIndex].GetComponent<AudioListener>().enabled = false;
		camIndex++;
		print ("Advancing camera to: " + camIndex);
		cameras[camIndex].SetActive(true);
	}
}

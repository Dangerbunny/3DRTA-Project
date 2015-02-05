using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	
	public GameObject[] cameras;
	//private Camera currentCamera;

	private int camIndex = 0;

	void Start () {
		//currentCamera = cameras [0];
	}

	public void nextCamera(){
		cameras[camIndex].SetActive(false);
		cameras [++camIndex].SetActive (true);
	}
}

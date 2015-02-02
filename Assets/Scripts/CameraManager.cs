using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	
	public Camera[] cameras;
	private Camera currentCamera;

	private int camIndex = 0;

	void Start () {
		currentCamera = cameras [0];
	}

	public void nextCamera(){
		currentCamera.enabled = false;
		currentCamera = cameras [++camIndex];
		currentCamera.enabled = true;
	}
}

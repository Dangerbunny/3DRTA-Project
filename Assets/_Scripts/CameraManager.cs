using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	
	public GameObject[] cameras;
	private GameObject currentCamera;

	private int camIndex = 0;

	void Start () {
		currentCamera = cameras [0];
		for (int i = 1; i < cameras.Length; i ++)
			if(cameras[i] != currentCamera)
				cameras [i].SetActive (false);
	}

	public void nextCamera(){
		currentCamera.SetActive(false);
		currentCamera = cameras [++camIndex];
		currentCamera.SetActive (true);
	}
}

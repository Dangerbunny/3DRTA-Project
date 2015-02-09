using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
	
	public GameObject[] cameras;
//	private GameObject currentCamera;

	private int camIndex = 0;

	void Start () {
//		currentCamera = cameras [0];

		for (int i = 1; i < cameras.Length; i ++)
			if(cameras[i] != cameras[0])
				cameras [i].SetActive (false);
	}

	public void nextCamera(){
		cameras[camIndex].SetActive(false);
		camIndex++;
		cameras[camIndex].SetActive(true);
	}
}

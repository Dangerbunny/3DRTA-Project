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

	public void nextCamera(){
		cameras[camIndex].SetActive(false);
		camIndex++;
		print ("Advancing camera to: " + camIndex);
		cameras[camIndex].SetActive(true);
	}
}

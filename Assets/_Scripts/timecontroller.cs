using UnityEngine;
using System.Collections;

public class timecontroller : MonoBehaviour
{
	//Written by Evan Krause, modified by Ray Cothern

	// use Up arrow to speed up
	// use Down Arrow to slow down
	// use shift to douple effect
	// press space to pause and unpause

	public float speedUp = 5.0f;
	public float slowDown = 5.0f;

	bool pause = false, fast = false , slow = false;

	void Update ()
	{
		if (speedUp > 99.0f)
			speedUp = 99.0f;
		if (speedUp <= 1.0f)
			speedUp = 1.01f;
		if (slowDown > 99.0f)
			slowDown = 99.0f;
		if (slowDown <= 1.0f)
			slowDown = 1.01f;

		if (Input.GetKeyDown (KeyCode.Space) || pause) {
			Time.timeScale = (Time.timeScale > 0) ? 0.0f : 1.0f;
		} else {
			// comment out this area to disable fast-forward/slow-mo
			if (Time.timeScale != 0) {
				float newTime = 1.0f;
				if (Input.GetKey (KeyCode.UpArrow) || fast) {
					if (Input.GetKey (KeyCode.LeftShift)) {
						newTime = speedUp * 2.0f;
					} else {
						newTime = speedUp;
					}
				} else if (Input.GetKey (KeyCode.DownArrow) || slow) {
					if (Input.GetKey (KeyCode.LeftShift)) {
						newTime = 1.0f / (slowDown * 2.0f);
					} else {
						newTime = 1.0f / (slowDown);
					}
				} else {
					newTime = 1.0f;
				}
				Time.timeScale = newTime;
			}
		}
	}

	public void Pause(){
		pause = true;
	}
	public void Fast(){
		fast = true;
	}
	public void Slow(){
		slow = true;
	}
	public void Normal(){
		pause = fast = slow = false;
	}
}

using UnityEngine;
using System.Collections;

public class HideMouse : MonoBehaviour {

	bool Paused = false;
	
	public void Update(){
		
		Screen.showCursor = false;
		
		if(Input.GetKeyDown("p") && Paused == false){ Screen.showCursor = true; Paused = true; }
		
		if(Input.GetKeyDown("p") && Paused == true){ Screen.showCursor = false; Paused = false;}
	}
}

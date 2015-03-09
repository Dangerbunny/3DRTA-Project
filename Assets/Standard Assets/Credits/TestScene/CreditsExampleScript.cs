using UnityEngine;
using System.Collections;

public class CreditsExampleScript : MonoBehaviour
{
	void Start()
	{
//		 Starting manually the credit roll if "Play On Awake" is false:
		// or:
		//Camera.main.SendMessage("beginCredits");

		// Callback

	}

	public static void playCredits(){
		Camera.main.GetComponent<Credits>().beginCredits();
		Camera.main.GetComponent<Credits>().endListeners += new Credits.CreditsEndListener(creditsEnded); // creditsEnded is the name of the function
	}

	static void creditsEnded(Credits c)
	{
		Debug.Log ("Quitting app");
		Application.Quit ();
	}
}

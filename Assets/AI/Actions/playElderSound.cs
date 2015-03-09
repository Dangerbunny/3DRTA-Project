using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class playElderSound : RAINAction
{
    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		Debug.Log ("PLAYING ELDER SOUND");
		MasterAudio.FireCustomEvent ("ElderD", Vector3.zero);
		GameObject.Find ("Ultimate Overlord").GetComponent<SceneManager> ().nextCamera ();
        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}
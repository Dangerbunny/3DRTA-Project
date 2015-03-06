using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class setControllable : RAINAction
{
    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		LumberjackController ljc = ai.Body.GetComponent<LumberjackController> ();
		ljc.setControllable (true);
		ai.Body.GetComponent<Animator>().SetInteger("Speed1", 0);
		SceneManager manager = GameObject.Find ("Ultimate Overlord").GetComponent<SceneManager>();
		manager.nextCamera ();
        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}
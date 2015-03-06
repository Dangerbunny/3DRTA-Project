using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINDecision]
public class isDead : RAINDecision
{
    private int _lastRunning = 0;

    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);

        _lastRunning = 0;
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        ActionResult tResult = ActionResult.FAILURE;
		WolfController wc = ai.Body.GetComponent<WolfController> ();
		tResult = (wc.isAlive () == false) ? ActionResult.SUCCESS : ActionResult.FAILURE;
		Debug.Log("Alive: " + wc.isAlive());
		
		if(!wc.isAlive()){
	        for (; _lastRunning < _children.Count; _lastRunning++)
	        {
	            tResult = _children[_lastRunning].Run(ai);
	            if (tResult != ActionResult.SUCCESS)
	                break;
	        }
		} else{
			Debug.Log("Health: " + wc.getHealth());
			ai.WorkingMemory.SetItem("health", wc.getHealth());
		}
        return tResult;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}
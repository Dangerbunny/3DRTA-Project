using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINDecision]
public class isControllable : RAINDecision
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

		LumberjackController ljc = ai.Body.GetComponent<LumberjackController> ();

		bool res = ljc.isControllable ();

		tResult = (res == false) ? ActionResult.SUCCESS : ActionResult.FAILURE;
		
		if(!res){
			for (; _lastRunning < _children.Count; _lastRunning++)
			{
			    tResult = _children[_lastRunning].Run(ai);
			    if (tResult != ActionResult.SUCCESS)
			        break;
			}
		}
        return tResult;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}
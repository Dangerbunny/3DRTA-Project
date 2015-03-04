using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Motion;

[RAINDecision("CheckDistance")]
public class CheckDistance : RAINDecision
{
	private int _lastRunning = 0;
	
	public Expression Target = new Expression();
	public Expression GreaterThan = new Expression();
	public Expression LessThan = new Expression();
	
	public override void Start(RAIN.Core.AI ai)
	{
		base.Start(ai);
		
		_lastRunning = 0;
	}
	
	public override ActionResult Execute(RAIN.Core.AI ai)
	{
		ActionResult tResult = ActionResult.RUNNING;
		
		if (Target.IsValid && Target.IsVariable)
		{
			MoveLookTarget tTarget = MoveLookTarget.GetTargetFromVariable(ai.WorkingMemory, Target.VariableName);
			float tDistance = (tTarget.Position - ai.Kinematic.Position).magnitude;
			
			if (GreaterThan.IsValid && (tDistance <= GreaterThan.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory)))
				return ActionResult.FAILURE;
			
			if (LessThan.IsValid && (tDistance >= LessThan.Evaluate<float>(ai.DeltaTime, ai.WorkingMemory)))
				return ActionResult.FAILURE;
		}
		
		
		for (; _lastRunning < _children.Count; _lastRunning++)
		{
			tResult = _children[_lastRunning].Run(ai);
			if (tResult != ActionResult.SUCCESS)
				break;
		}
		
		return tResult;
	}
}
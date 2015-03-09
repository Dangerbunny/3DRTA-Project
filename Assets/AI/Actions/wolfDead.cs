using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINDecision]
public class wolfDead : RAINDecision
{
    private int _lastRunning = 0;
	bool hasFired = false;

    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);

        _lastRunning = 0;
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        ActionResult tResult = ActionResult.FAILURE;

		if(!ai.Body.GetComponent<DogController>().getFocus().GetComponent<WolfController>().isAlive()){
			if(!hasFired){
				MasterAudio.FireCustomEvent("LJD", Vector3.zero);
				MasterAudio.FadePlaylistToVolume("Playlist1", 0, 2.5f);
				MasterAudio.StartPlaylist("Playlist2", "sc3-ending");
				SceneManager manager = GameObject.Find("Ultimate Overlord").GetComponent<SceneManager>();
				manager.getActor(SceneManager.Actor.lumberjack).GetComponent<LumberjackController>().setControllable(false);
				manager.getActor(SceneManager.Actor.lumberjack).GetComponent<PlayerControl>().enabled = false;
				manager.nextCamera();
				CreditsExampleScript.playCredits();

				hasFired = true;
			}
			tResult = ActionResult.SUCCESS;
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
using UnityEngine;
using System.Collections;

public class WatchDancePerception : AbstractPerception {

	public WatchDancePerception(GameObject source) : base(source){
	}

	public override Action GetAction(GameObject actor){
		ActionRunner actionRunner = (ActionRunner)actor.GetComponent("ActionRunner");
		if (actionRunner.CanStartDialogue(true)){
			return new WatchDanceAction(actor);
		} else {
			return null;
		}
	}
}

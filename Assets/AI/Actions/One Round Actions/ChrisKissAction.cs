using UnityEngine;
using System.Collections;

public class ChrisKissAction : AbstractOneRoundAction {

	public ChrisKissAction(GameObject actor) : base(actor){
	}

	protected override void UpdateOnlyRound(){
		CutScenePlayer.Play("Kiss Chris");
	}
}

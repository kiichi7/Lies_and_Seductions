using UnityEngine;
using System.Collections;

public class PCForcedSleepAction : AbstractOneRoundAction /*, SelectableAction***/ {

	public PCForcedSleepAction(GameObject actor) : base(actor){
		InitInteractionInfo(false, false, false);		
	}
	
	protected override void UpdateOnlyRound(){
		//Waypoint cabinWaypoint = (Waypoint)GameObject.Find("Waypoint: Cabin: Abby").GetComponent("Waypoint");
		//mover.JumpTo(cabinWaypoint.transform.position, cabinWaypoint.transform.rotation, false);
		
		Debug.Log("PCForcedSleepAction.UpdateOnlyRound()");
		
		state.SetTask(CharacterState.Task.NONE, null);
		TaskHelp.RemoveHelp();
		
		GameTime.SkipTimeInMinutes(GameTime.sleepDuration);
		CharacterManager.FullReset();
		CutScenePlayer.Play("forced night");
		Waypoint doorWaypoint = GameObject.Find("Waypoint: Lobby door to Cabin").GetComponent(typeof(TransitWaypoint)) as Waypoint;
		mover.JumpTo(doorWaypoint.transform.position, doorWaypoint.transform.rotation, false);
		
		// Lets save the game here...
		state.NightSlept();
		SaveLoad.Save();
	}
	
	//public MenuIconSet GetIconSet(PopupMenuGUI popupMenuGUI){
	//	return popupMenuGUI.sleepIcon;
	//}
}

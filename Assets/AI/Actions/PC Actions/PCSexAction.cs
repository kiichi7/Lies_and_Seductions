/**********************************************************************
 *
 * CLASS CharacterAnimator
 *
 * Copyright 2008 Tommi Horttana, Petri Lankoski, Jari Suominen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License 
 * at http://www.apache.org/licenses/LICENSE-2.0 Unless required 
 * by applicable law or agreed to in writing, software distributed 
 * under the License is distributed on an "AS IS" BASIS, WITHOUT 
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 * See the License for the specific language governing permissions and 
 * limitations under the License.
 *
 ***********************************************************************/
using UnityEngine;
using System.Collections;

public class PCSexAction : AbstractOneRoundAction, SelectableAction {

	private GameObject partner;

	public PCSexAction(GameObject actor, GameObject partner) : base(actor){
		InitInteractionInfo(false, false, false);
		
		this.partner = partner;
		
	}
	
	protected override void UpdateOnlyRound(){
		//Waypoint cabinWaypoint = (Waypoint)GameObject.Find("Waypoint: Cabin: Abby").GetComponent("Waypoint");
		//mover.JumpTo(cabinWaypoint.transform.position, cabinWaypoint.transform.rotation, false);
		
			// If time is skipped here, we need to check if we need to
		Debug.Log("PCSexAction.UpdateOnlyRound(): sex with " + partner.name);
		if(state.currentTask.task == CharacterState.Task.SEX) {
			state.TaskCompleted(); 
		}
		else {
			state.SetTask(CharacterState.Task.NONE, actor);
		}
		TaskHelp.RemoveHelp();
		
		CharacterManager.FullReset(partner, new StartDialogueAction(partner, actor, partner.name, "after_sex"));
		
		CutScenePlayer.Play("Sex with " + partner.name);
		
		if(GameTime.IsSleepOpen()) {
			state.NightSlept();
			GameTime.SkipTimeInMinutes(GameTime.sexDuration + GameTime.sleepDuration);
		}
		else { 
			GameTime.SkipTimeInMinutes(GameTime.sexDuration);
		}

		Waypoint doorWaypoint = (Waypoint)GameObject.Find("Waypoint: Lobby door to Cabin").GetComponent(typeof(TransitWaypoint));
		mover.JumpTo(doorWaypoint.transform.position, doorWaypoint.transform.rotation, false);

		CharacterState partnerState = partner.GetComponent(typeof(CharacterState)) as CharacterState;
		
		// SetJustHadSex() calls are for the save. Those are needed to be able to continue after load???
		partnerState.SetJustHadSex(true);
		SaveLoad.Save();
		partnerState.SetJustHadSex(false);
	}
	
	public MenuIconSet GetIconSet(PopupMenuGUI popupMenuGUI){
		return popupMenuGUI.sexIcon;
	}
}

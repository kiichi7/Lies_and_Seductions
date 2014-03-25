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

public class PCPopupMenuAction : AbstractActionWithEndCondition {

	private HotSpot target;
	private bool finished;
	
	public PCPopupMenuAction(GameObject actor, HotSpot target) : base(actor){
		InitInteractionInfo(false, false, false);

		this.target = target;
		finished = false;
	}
	
	protected override void UpdateFirstRound(){
		//Debug.Log("!!!!! -------  PCPopupMenuAction.UpdateFirstRound() -------- !!!!");
		GameObject character = target.GetMajorNPC();
		if(character!=null) {
			//Debug.Log("PCPopupMenuAction.UpdateFirstRound(): target: " + character.name);
			ImpressionMemory memory = character.GetComponent(typeof(ImpressionMemory)) as ImpressionMemory;
			memory.SendDisplayAttitude();
		}
		string objName="";
		GameObject npc=target.GetMajorNPC();
		if(npc) {
			objName=npc.name;
		}
		PopupMenuGUI.OpenMenu(target.GetAvailableActions(), target.GetHelpText(), objName);
	}
	
	protected override void UpdateAnyRound(){
		if (PopupMenuGUI.IsFinished()){
			finished = true;
		}
	}
	
	protected override bool IsCompleted(){
		return finished;
	}
	
	protected override void UpdateLastRound(bool interrupted){
		Action selectedAction = PopupMenuGUI.GetSelectedAction();
		actionRunner.ResetRoutine(selectedAction, false);
	}
	
	public override void OnActionGUI(){
		PopupMenuGUI.DrawGUI();
	}
	
	public override bool ShouldNPCStop(GameObject npc){
		return (target.GetMajorNPC() == npc);
	}
}

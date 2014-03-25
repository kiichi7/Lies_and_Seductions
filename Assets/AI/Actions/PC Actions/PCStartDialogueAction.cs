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

public class PCStartDialogueAction : AbstractOneRoundAction, SelectableAction {

	private GameObject target;
	private string conversationName;

	public PCStartDialogueAction(GameObject actor, GameObject target, string conversationName) : base(actor){
		InitInteractionInfo(false, false, false);
		
		this.target = target;
		this.conversationName = conversationName;
		
	}

	public MenuIconSet GetIconSet(PopupMenuGUI popupMenuGUI){
		return popupMenuGUI.talkIcon;
	}
	
	protected override void UpdateOnlyRound(){
		if(state.currentTask.task != CharacterState.Task.NONE && state.currentTask.npc == target) {
			if(state.currentTask.task != CharacterState.Task.DRINK) {
				state.SetTask(CharacterState.Task.NONE, null);
				TaskHelp.RemoveHelp();
			}
		}
		CharacterState targetState = (CharacterState)target.GetComponent("CharacterState");
		DialogueAgent.RequestDialogue(conversationName, actor, target, targetState.GetSituation());
	}
	
}

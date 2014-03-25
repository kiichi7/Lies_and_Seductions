/**********************************************************************
 *
 * CLASS 
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

public class StartDialogueAction : AbstractActionWithEndCondition {

	private GameObject target;
	private string conversationName;
	private string situation;

	public StartDialogueAction(GameObject actor, GameObject target, string conversationName, string situation) : base(actor){
		InitInteractionInfo(true, false, true);
		InitMovementInfo(target, DistanceConstants.TALKING_MAX_DISTANCE, false, true, false);
		
		this.target = target;
		this.conversationName = conversationName;
		this.situation = situation;
	}

	protected override void UpdateAnyRound(){
		//Debug.Log("StartDialogueAction.UpdateAnyRound()" + actor.name + " requesting dialogue  with " + target.name);
		DialogueAgent.RequestDialogue(conversationName, actor, target, situation);
	}
	
	protected override bool IsCompleted(){
		return false; //Will be interrupted when the dialogue request is accepted
	}
}
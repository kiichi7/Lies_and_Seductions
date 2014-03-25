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

public class OneLinerAction : AbstractNestingAction {
	
	private GameObject oneLinerTarget;

	public OneLinerAction(GameObject actor, string oneLinerType, GameObject oneLinerTarget) : base (actor){
		InitInteractionInfo(true, false);
		
		this.oneLinerTarget = oneLinerTarget;
		state.SetOneLinerTarget(oneLinerTarget);
		string conversationName = actor.name + ": " + oneLinerType;
		Conversation conversation = AILoader.GetConversation(conversationName);
		if (conversation == null){
			Debug.LogError("OneLinerAction.OneLinerAction(): " + conversationName + "not found!");
			return;
		}
		RootLine rootLine = conversation.GetRootLine();
		ArrayList responses = rootLine.GetResponses();
		ArrayList availableResponses = new ArrayList();
		foreach (Line response in responses){
			if (response.IsPrerequisiteMet()){
				availableResponses.Add(new VariantLine(response));
			}
		}
		if (availableResponses.Count == 0){
			Debug.LogError("neLinerAction.OneLinerAction(): " + conversation.GetName());
		} else {
			int random = (int)Random.Range(0, availableResponses.Count);
			Line oneLiner = (Line)availableResponses[random];
			QueueAction(new SayLineAction(actor, oneLiner.GetText(), true));
		}
	}
	
	protected override void UpdateFirstRound(){
		animator.TurnHead(oneLinerTarget.transform.position);
	}
	
	protected override void UpdateLastRound(bool interrupted){
		state.SetOneLinerTarget(null);
		animator.ResetHeadDirection();
	}
}

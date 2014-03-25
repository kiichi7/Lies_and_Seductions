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

public class GiveMoneyAction : AbstractSingleLoopAction {

	private GameObject recipient;
	private int amount;
	
	public GiveMoneyAction(GameObject actor, string targetName, int amount) : base(actor){
		InitAnimationInfo("give_beer", WrapMode.Once, Emotion.BodyParts.FACE);
		InitInteractionInfo(true, true, false);
		InitMovementInfo(GameObject.Find(targetName), DistanceConstants.ARMS_REACH, false, true, false);
		
		recipient = CharacterManager.GetCharacter(targetName);
		this.amount = amount;
	}
	
	/*protected override void UpdateFirstRound(){
		Item item = actorState.GetItem();
		if (item != null){
			string itemName = item.name;
			StartAnimation("give", WrapMode.Once, Emotion.BodyParts.FACE);
			GetItemAction getItemAction = new GetItemAction(recipient, itemName, actor.name);
			ActionRunner recipientActionRunner = (ActionRunner)recipient.GetComponent("ActionRunner");
			recipientActionRunner.FireInteraction(getItemAction, true);
		}
	}*/
	
	protected override void UpdateLastRound(bool interrupted){
		state.RemoveMoney(amount);
		CharacterState recipientState = (CharacterState)recipient.GetComponent("CharacterState");
		recipientState.AddMoney(amount);
	}
}

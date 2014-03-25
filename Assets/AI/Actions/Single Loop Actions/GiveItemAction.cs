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

public class GiveItemAction : AbstractItemHandlingAction, SelectableAction {

	private GameObject recipient;
	private Item item;
	private bool keepCopy;
	private bool errorOccured;

	public GiveItemAction(GameObject actor, string targetName, bool keepCopy) : base(actor){
		Debug.Log("GiveItemAction(" + actor.name + ", " + targetName + ")");
		errorOccured = false;
		item = state.GetItem();
		if(!item) {
			// Now we need to ensure that error in Dialogue definition (no item in hand when issuing a give command) 
			// does not cause null reference exception.  
			errorOccured = true;
			Debug.LogError("GiveItemAction.GiveItemAction() actor " + actor.name + " has no item to give");	
		}
		//Debug.Log("-- giving item " + item.name);
		InitAnimationInfo("give_" + item.name, WrapMode.Once, Emotion.BodyParts.FACE);
		InitInteractionInfo(true, true, false);
		InitMovementInfo(GameObject.Find(targetName), DistanceConstants.ARMS_REACH, false, true, false);
		
		recipient = CharacterManager.GetCharacter(targetName);
		
		this.keepCopy = keepCopy;
	}
	
	public GiveItemAction(GameObject actor, string targetName) : this(actor, targetName, false){
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
	
	protected override void UpdateFirstRound(){
		if(errorOccured) {
			return;
		}
		animator.StopAnimation("hold_" + state.GetItem().name);
		
		if(state.currentTask.task == CharacterState.Task.DRINK) {
			if(	state.currentTask.npc != recipient) {
				state.SetTask(CharacterState.Task.NONE, null);
			}
			else if(item.name.Equals("drink") || item.name.Equals("beer") || item.name.Equals("whiskey")) {
				state.TaskCompleted();	
			}
			TaskHelp.RemoveHelp();
		}
	}
	
	protected override void UpdateLastRound(bool interrupted){
		if(errorOccured) {
			return;
		}
		Item item = state.GetItem();
		if (!keepCopy){
			DropItem();
		}
		CharacterState recipientState = (CharacterState)recipient.GetComponent("CharacterState");
		recipientState.SetItem((Item)ItemFactory.CreateItem(item.name).GetComponent("Item"));
		CharacterAnimator recipientAnimator = (CharacterAnimator)recipient.GetComponent("CharacterAnimator");
		recipientAnimator.StartAnimation("hold_" + item.name, WrapMode.Loop, CharacterAnimator.HOLD_LAYER, new string[]{CharacterAnimator.FindShoulder(recipient.transform, item.name).name});
	}
	
	public MenuIconSet GetIconSet(PopupMenuGUI popupMenuGUI){
		return popupMenuGUI.giveIcon;
	}
}

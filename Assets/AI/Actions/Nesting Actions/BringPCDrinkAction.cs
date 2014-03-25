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

public class BringPCDrinkAction : AbstractNestingAction {

	private string drinkName;

	public BringPCDrinkAction(GameObject actor, string drinkName) : base(actor){
		InitInteractionInfo(true, false);
		this.drinkName = drinkName;
	}
	
	protected override void UpdateFirstRound(){
		CharacterState actorState = (CharacterState)actor.GetComponent(typeof(CharacterState));
		actorState.SetItem(null);
		GameObject barkeeper = actorState.GetCurrentArea().GetBarkeep();
		if(barkeeper==null) {
			Debug.LogError("BringPCDrinkAction.UpdateFirstRound(): Action iniated in the area without barkeep");
			return;	
		}
		string sellerName = "Waypoint: " + barkeeper.name; //actorState.GetCurrentArea().GetBarkeep().name;
		QueueAction(new GetItemAction(actor, drinkName, sellerName));
		QueueAction(new StartDialogueAction(actor, CharacterManager.GetPC(), actor.name, "bringing_drinks"));
	}
}

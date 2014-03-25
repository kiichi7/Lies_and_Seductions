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

public class GetItemAction : AbstractItemHandlingAction, SelectableAction {

	private string itemName;

	public GetItemAction(GameObject actor, string itemName, string targetName) : base(actor){
		InitAnimationInfo("get_" + itemName, WrapMode.Once, Emotion.BodyParts.FACE);
		InitInteractionInfo(true, true, false);
		InitMovementInfo(GameObject.Find(targetName), DistanceConstants.WAYPOINT_RADIUS, false, false, true);
		
		this.itemName = itemName;
	}
	
	protected override void UpdateFirstRound(){
		TakeItem(ItemFactory.CreateItem(itemName));
	}
	
	
	protected override void UpdateLastRound(bool interrupted){
		animator.StartAnimation("hold_" + itemName, WrapMode.Loop, CharacterAnimator.HOLD_LAYER, new string[]{CharacterAnimator.FindShoulder(actor.transform, itemName).name});
	}
	
	public MenuIconSet GetIconSet(PopupMenuGUI popupMenuGUI){
		if (itemName.Equals("beer")){
			return popupMenuGUI.buyBeerIcon;
		} else if (itemName.Equals("whiskey")){
			return popupMenuGUI.buyWhiskeyIcon;
		} else if (itemName.Equals("drink")){
			return popupMenuGUI.buyDrinkIcon;
		} else if (itemName.Equals("rose")){
			return popupMenuGUI.buyFlowersIcon;
		} else if (itemName.Equals("chocolate")){
			return popupMenuGUI.buyChocolateIcon;
		} else {
			Debug.LogError("Can't get this: " + itemName);
			return null;
		}
	}	
}

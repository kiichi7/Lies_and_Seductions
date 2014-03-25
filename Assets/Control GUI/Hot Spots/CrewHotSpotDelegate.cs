/**********************************************************************
 *
 * CLASS CrewHotSpotDelegate
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

public class CrewHotSpotDelegate : HotSpotDelegate {
	
	private const string helpText="After buying an item you can drop it by clickin the item and selecting drop. To give item to somebody, click that character and select give. Drink action is available for a beer, whiskey, and drink in the item's popup menu.";
	
	public bool sellDrinks;
	public bool sellFlowersAndChocolate;
	
	public void Awake() {
		enabled=false;	
	}
	
	public override ArrayList GetAvailableActions () {
		GameObject pc = CharacterManager.GetPC();
		ArrayList availableActions = new ArrayList();
		if (sellDrinks){
			availableActions.Add(new GetItemAction(pc, "beer", null));
			availableActions.Add(new GetItemAction(pc, "whiskey", null));
			availableActions.Add(new GetItemAction(pc, "drink", null));
		}
		if (sellFlowersAndChocolate){
			availableActions.Add(new GetItemAction(pc, "rose", null));
			availableActions.Add(new GetItemAction(pc, "chocolate", null));
		}
		return availableActions;
	}
	
	public override string GetHelpText() {
		if(IngameHelpMemory.ShouldShow(IngameHelpMemory.ITEM_HELP)) {
			return "";	
		}
		else {
			IngameHelpMemory.MarkAsUsed(IngameHelpMemory.ITEM_HELP);
			return helpText;
		}	
	}
	
	public override GameObject GetMajorNPC() {
		return null;
	}

}

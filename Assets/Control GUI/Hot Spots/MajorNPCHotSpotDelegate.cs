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

public class MajorNPCHotSpotDelegate : HotSpotDelegate {
	
	public void Awake() {
		enabled=false;	
	}
	
	public override ArrayList GetAvailableActions () {
		ArrayList availableActions = new ArrayList();
		GameObject pc = CharacterManager.GetPC();
		availableActions.Add(new PCStartDialogueAction(pc, gameObject, name));
		if (((CharacterState)pc.GetComponent("CharacterState")).GetItem() != null){
			//availableActions.Add(new GiveItemAction(pc, gameObject.name));
		}
		return availableActions;
	}
	
	public override string GetHelpText() {
		if(IngameHelpMemory.ShouldShow(IngameHelpMemory.MAJOR_NPC_HELP)) {
			return "";	
		}
		else {
			IngameHelpMemory.MarkAsUsed(IngameHelpMemory.MAJOR_NPC_HELP);
			return IngameHelpTexts.majorNPChelpText;
		}	
	}
	
	public override GameObject GetMajorNPC() {
		return gameObject;
	}
} 

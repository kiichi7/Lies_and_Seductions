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

public class DanceFloorHotSpotDelegate : HotSpotDelegate {

	public void Awake() {
		enabled=false;	
	}

	public override ArrayList GetAvailableActions () {
		ArrayList availableActions = new ArrayList();
		GameObject pc = CharacterManager.GetPC();
		availableActions.Add(new PCDanceAction(pc, "Waypoint: Dance Floor"));
		//PARTNER???
		/*CharacterState state = (CharacterState)pc.GetComponent("CharacterState");
		if (state.HasPokerPartner()){
			availableActions.Add(new PCPokerAction(pc, state.GetFollower(), "Poker Chair: Abby", 30));
		}*/
		return availableActions;
	}
	
	public override string GetHelpText() {
		return "";	
	}
	
	public override GameObject GetMajorNPC() {
		return null;
	}

}

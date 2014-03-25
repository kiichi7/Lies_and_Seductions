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

public class DropItemAction : AbstractOneRoundAction, SelectableAction {

	public DropItemAction(GameObject actor, string targetName) : base(actor){
		if (targetName != null){
			InitMovementInfo(GameObject.Find(targetName), DistanceConstants.WAYPOINT_RADIUS, false, false, false);
		}
	}
	
	protected override void UpdateOnlyRound(){
		//Debug.Log("================= DropItemAction.UpdateOnlyRound ======================");
		Item item = state.GetItem();
		animator.StopAnimation("hold_" + item.name);
		state.SetItem(null);
	}
	
	public MenuIconSet GetIconSet(PopupMenuGUI popupMenuGUI){
		return popupMenuGUI.dropIcon;
	}
}

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

public class EatChocolateAction : AbstractItemHandlingAction {

	public EatChocolateAction(GameObject actor, string targetName, bool sitDown) : base(actor){
		InitAnimationInfo("eat_chocolate", WrapMode.Once, Emotion.BodyParts.EYES);
		InitInteractionInfo(true, true, false);
		InitMovementInfo(GameObject.Find(targetName), DistanceConstants.SIT_DOWN_DISTANCE, sitDown, false, false);
	}
	
	protected override void UpdateFirstRound(){
		if (actor.name.Equals("Abby")) {
			FModManager.StartEventAtCamera(FModLies.EVENTID_LIES_ACTIONS_EATING);		
		} else {
			FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_EATING, actor.transform.position);	
		}
	}
	
	protected override void UpdateLastRound(bool interrupted){
		DropItem();
	}
}

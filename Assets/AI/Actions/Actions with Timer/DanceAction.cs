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

public class DanceAction : AbstractActionWithTimer {

	public DanceAction(GameObject actor, float durationInSeconds, string danceFloorName, bool withPC) : base(actor){
		InitAnimationInfo("dance_1", WrapMode.Loop, Emotion.BodyParts.FACE);
		Item item = state.GetItem();
		if(item) {
			animator.StopAnimation("hold_" + item.name);
			state.SetItem(null);
		}
		InitTimerInfo(durationInSeconds);
		InitInteractionInfo(!withPC, false, true);
		InitMovementInfo(GameObject.Find(danceFloorName), DistanceConstants.UNDEFINED, false, false, true);
	}
	
	protected override void UpdateFirstRound(){
		//FModSync.instance().AddSyncListener(this, 30, true);
	}
	
	protected override void UpdateLastRound(bool interrupted){
		//FModSync.instance().RemoveSyncListener(this);
	}
	
	public void TrigPerformed(){
		animator.RestartAnimation("dance_1");
	}
}

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

public class StandUpAction : AbstractSingleLoopAction {

	private Seat seat;
	private string seatType;
	private Transform seatHook;

	public StandUpAction(GameObject actor, Seat seat) : base (actor){
		this.seat = seat;
		this.seatType = seat.GetType();
		InitAnimationInfo("stand_up_" + seatType, WrapMode.Once, Emotion.BodyParts.FACE);
		InitInteractionInfo(true, true, false);
		
		seatHook = CharacterAnimator.FindChild(actor, seatType + "_hook");
		if (actor.name.Equals("Abby")) {
			FModManager.StartEventAtCamera(FModLies.EVENTID_LIES_ACTIONS_CHAIR);
		} else {
			FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_CHAIR, actor.transform.position);	
		}
	}
	
	protected override void UpdateFirstRound(){
		//Debug.Log("Sitting: StandUp.First: AttachSeatToHook: " + actor.name);
		AttachSeatToHook();
		//Debug.Log("Sitting: StandUp.First: StopAnimation: " + actor.name);
		animator.StopAnimation("sit_" + seatType);
		//state.SetCurrentSeat(null);
	}
	
	protected override void UpdateLastRound(bool interrupted){
		//Debug.Log("-------------------------- " + actor.name + ": StandUpAction ENDS -------------------------------");
		//Debug.Log("Sitting: StandUp.Last: DetatchSeatFromHook: " + actor.name);
		DetatchSeatFromHook();
		//Debug.Log("Sitting: StandUp.Last
		state.SetCurrentSeat(null);
		mover.RestoreSafeSpot();
	}
	
	private void AttachSeatToHook(){
		seat.Attach(seatHook);
	}
	
	private void DetatchSeatFromHook(){
		seat.Detach();
		seat.ResetPosition();
	}
}

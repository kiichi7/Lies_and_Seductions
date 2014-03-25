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

public class SitDownAction : AbstractSingleLoopAction {

	private Seat seat;
	private string seatType;
	private Transform seatHook;
	private bool seatWasTaken;
		
	public SitDownAction(GameObject actor, Seat seat) : base (actor){
		this.seat = seat;
		this.seatType = seat.GetType();
		InitAnimationInfo("sit_down_" + seatType, WrapMode.Once, Emotion.BodyParts.FACE);
		InitInteractionInfo(true, true, false);
		
		seatHook = CharacterAnimator.FindChild(actor, seatType + "_hook");
		seatWasTaken = false;
		
		if (actor.name.Equals("Abby")) {
			FModManager.StartEventAtCamera(FModLies.EVENTID_LIES_ACTIONS_CHAIR);
		} else {
			FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_CHAIR, actor.transform.position);	
		}
	}
	
	protected override void UpdateFirstRound(){
		if (seat.IsTaken()){
			seatWasTaken = true;
			actionRunner.ResetRoutine(true);
		} else {
			mover.SaveSafeSpot();
			//mover.JumpTo(seat.transform.position, seat.transform.rotation, true);
			mover.MatchHookWithGameObject(seatType + "_hook", seat.gameObject);
			AttachSeatToHook();
			state.SetCurrentSeat(seat);
		}
	}
	
	private void AttachSeatToHook(){
		seat.Attach(seatHook);
	}
	
	private void DetatchSeatFromHook(){
		seat.Detach();
	}
	
	protected override void UpdateLastRound(bool interrupted){
		string rootSpineName;
		if (actor == CharacterManager.GetMajorNPC("Emma")){
			rootSpineName = "root_spine_lumbar";
		} else {
			rootSpineName = "root_spine";
		}
		if (!seatWasTaken){
			if (actor.name.Equals("Abby")){
				animator.StartAnimation("sit_" + seatType, WrapMode.Loop, CharacterAnimator.SIT_LAYER, new string[]{"root_hip", ":" + seatType + "_hook"}, new string[]{rootSpineName, "null_waist"});
			} else {
				animator.StartAnimation("sit_" + seatType, WrapMode.Loop, CharacterAnimator.SIT_LAYER, new string[]{"root_pelvis", ":" + seatType + "_hook"}, new string[]{rootSpineName});
			}
			DetatchSeatFromHook();
		}
	}
}

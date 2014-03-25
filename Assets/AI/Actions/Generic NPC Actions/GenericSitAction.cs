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

public class GenericSitAction : AbstractGenericNPCAction {

	private Seat[] potentialSeats;
	private string seatType;

	public GenericSitAction(GameObject actor, string seatType, string animationName, GameObject seatParent) : base(actor){
		InitAnimationInfo(animationName, WrapMode.Loop, Emotion.BodyParts.NONE);
		InitInteractionInfo(false, false, true);
		
		Component[] seatComponents = seatParent.GetComponentsInChildren(typeof(Seat));
		potentialSeats = new Seat[seatComponents.Length];
		for (int i = 0; i < seatComponents.Length; i++){
			potentialSeats[i] = (Seat)seatComponents[i];
		}
		this.seatType = seatType;
		
	}
	
	protected override void UpdateFirstRound(){
		int tries = 0;
		while (state.GetCurrentSeat() == null || tries >= 30){
			Seat seat = potentialSeats[Random.Range(0, potentialSeats.Length)];
			if (!seat.IsTaken()){
				mover.MatchHookWithGameObject(seatType + "_hook", seat.gameObject);
				state.SetCurrentSeat(seat);
			} else {
				tries++;
			}
		}
	}
	
	protected override void UpdateLastRound(bool interrupted){
		state.SetCurrentSeat(null);
	}
}

/**********************************************************************
 *
 * CLASS TransitWaypoint
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

public class TransitWaypoint : Waypoint {
	
	public Area area;

	/*public override bool IsTransit(){
		return true;
	}*/
	
	public Area GetArea(){
		return area;
	}
		
	public void TransitCharacterTo(GameObject character){
		CharacterMover mover = (CharacterMover)character.GetComponent(typeof(CharacterMover)) as CharacterMover;
		CharacterState state = character.GetComponent(typeof(CharacterState)) as CharacterState;
		if(state.IsSeated()) {
			Debug.LogError("TransitWaypoint.TransitCharacterTo(): " + character.name + " is seated. Fixing...");
			Seat seat = state.GetCurrentSeat();
			seat.Detach();
			seat.ResetPosition();
			state.SetCurrentSeat(null);	
		}
		mover.JumpTo(transform.position, transform.rotation, false);
		area.Entered(character);
	}

	public override void OnDrawGizmos(){
		Gizmos.DrawIcon(transform.position, "Transit Waypoint.bmp");
	}
}

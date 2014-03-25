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

public class MoveToAction : AbstractNestingAction {

	private GameObject target;
	private float maximumDistance;
	private float minimumDistance;
	private bool sitDown;
	private bool turnToFaceTarget;
	private bool turnToMatchWaypoint;
	private bool follow;
	
	private StandUpAction standUpAction;
	private WalkToAction walkToAction;

	public MoveToAction(GameObject actor, GameObject target, float maximumDistance, float minimumDistance, bool sitDown, bool turnToFaceTarget, bool turnToMatchWaypoint, bool follow) : base(actor){
		this.target = target;
		if (target == null){
			Debug.LogError("MoveToAction.MoveToAction(): " + actor.name + " Can't move to a null target!");
		}
		Debug.Log("MoveToAction.MoveToAction():" + actor.name + " target: " + target.name + ", " + target.transform.position);
		this.maximumDistance = maximumDistance;
		this.minimumDistance = minimumDistance;
		this.sitDown = sitDown;
		this.turnToFaceTarget = turnToFaceTarget;
		this.turnToMatchWaypoint = turnToMatchWaypoint;
		this.follow = follow;
		
		standUpAction = null;
		walkToAction = null;
	}
	
	public void FastForward() {
		UpdateFirstRound();
		if (standUpAction != null && !standUpAction.IsFinished()){
			standUpAction.EndASAP(true);
		}
		if (walkToAction != null && !walkToAction.IsFinished()){
			walkToAction.FastForward();
		}
	}
	
	protected override void UpdateFirstRound(){
		bool needToStandUp;
		bool needToWalk;
		bool needToTurn;
		bool needToSitDown;
		
		GameObject walkTarget = target;
		
		Seat currentSeat = state.GetCurrentSeat();
		Seat targetSeat = null;
		if (sitDown){
			targetSeat = (Seat)target.GetComponent("Seat");
			if (currentSeat == targetSeat){
				needToStandUp = false;
				needToWalk = false;
				needToTurn = false;
				needToSitDown = false;
			} else {
				needToStandUp = currentSeat != null;
				needToWalk = true;
				needToTurn = true;
				needToSitDown = true;
				walkTarget = targetSeat.GetWaypoint().gameObject;
			}
		} else {
			needToStandUp = currentSeat != null;
			needToWalk = true;
			needToTurn = turnToFaceTarget || turnToMatchWaypoint;
			needToSitDown = false;
		}
		
		if (needToStandUp){
			QueueAction(standUpAction = new StandUpAction(actor, currentSeat));
		}
		if (needToWalk){
			if(CharacterManager.IsCharacter(target)) {
				QueueAction(walkToAction = new WalkToAction(actor, target, maximumDistance, minimumDistance, follow));
			}
			else {
				QueueAction(walkToAction = new WalkToAction(actor, walkTarget, maximumDistance, minimumDistance, follow));
			}
		}
		if (needToTurn){
			if (turnToFaceTarget){
				QueueAction(new TurnToFaceAction(actor, target));
			} else if (turnToMatchWaypoint){
				QueueAction(new TurnToMatchAction(actor, target));
			}
		}
		if (needToSitDown){
			QueueAction(new SitDownAction(actor, targetSeat));
		}
	}
	
	protected override void UpdateLastRound(bool interrupted){
	}
}
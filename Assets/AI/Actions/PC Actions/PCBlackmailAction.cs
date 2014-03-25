/**********************************************************************
 *
 * CLASS CharacterAnimator
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
using System;

public class PCBlackmailAction : AbstractActionWithEndCondition {


	private GameObject victim;
	
	public PCBlackmailAction(GameObject actor, GameObject victim) : base(actor){
		this.victim = victim;
	}
	
	protected override void UpdateFirstRound(){
		Debug.Log("PCBlackmailAction.UpdateFirstRound(): blackmailing " + victim.name);
		CutScenePlayer.Play(victim.name + " Blackmailed");
		//TaskHelp.RemoveGoal(TaskHelp.GOALS.ED);
		//TaskHelp.RemoveGoal(TaskHelp.GOALS.EMMA);
		//TaskHelp.ShowGoal(TaskHelp.GOALS.CHRIS);
		GameObject startingWaypoint = GameObject.Find("Waypoint: Lobby door to Cabin");
		mover.JumpTo(startingWaypoint.transform.position, startingWaypoint.transform.rotation, false);
		
		CharacterState chrisState = (CharacterState)CharacterManager.GetMajorNPC("Chris").GetComponent("CharacterState");
		chrisState.StartBlindDate(new TimeSpan(2, 30, 0));
		
		CharacterManager.FullReset();
		
		
		
	}
	
	protected override bool IsCompleted(){
		return !CutScenePlayer.IsPlaying();
	}
	
	protected override void UpdateLastRound(bool interrupted){
		TaskHelp.RemoveGoal(TaskHelp.GOALS.ED);
		TaskHelp.RemoveGoal(TaskHelp.GOALS.EMMA);
		TaskHelp.ShowGoal(TaskHelp.GOALS.CHRIS);
		// Lets save the game here...
		SaveLoad.Save();
	}
}

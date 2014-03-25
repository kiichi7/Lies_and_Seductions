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

public class FollowAction : AbstractNestingAction {

	public enum Reason {SEX, DANCE, POKER_WITHOUT_MONEY, POKER_WITH_MONEY, NPC, WAIT, DRINK}

	private GameObject targetCharacter;
	private CharacterState targetCharacterState;
	private float maximumDistance;
	private float minimumDistance;
	private Reason reason;
	private bool ending;
	
	public FollowAction(GameObject actor, GameObject targetCharacter, float maximumDistance, float minimumDistance, Reason reason, bool canStartDialogueWithAgents) : base(actor){
		InitInteractionInfo(true, canStartDialogueWithAgents);
		
		this.targetCharacter = targetCharacter;
		this.targetCharacterState = (CharacterState)targetCharacter.GetComponent("CharacterState");
		this.maximumDistance = maximumDistance;
		this.minimumDistance = minimumDistance;
		this.reason = reason;
		switch(reason) {
			case Reason.SEX:
				targetCharacterState.SetTask(CharacterState.Task.SEX, actor);
				TaskHelp.ShowHelp(TaskHelp.SEX, actor);
				break;
			case Reason.DANCE:
				targetCharacterState.SetTask(CharacterState.Task.DANCE, actor);
				TaskHelp.ShowHelp(TaskHelp.DANCE, null);
				break;
			case Reason.POKER_WITHOUT_MONEY:
				targetCharacterState.SetTask(CharacterState.Task.POKER, actor);
				TaskHelp.ShowHelp(TaskHelp.POKER, null);
				break;
			case Reason.POKER_WITH_MONEY:
				targetCharacterState.SetTask(CharacterState.Task.POKER, actor);
				TaskHelp.ShowHelp(TaskHelp.POKER, null);
				break;
			case Reason.DRINK:
				targetCharacterState.SetTask(CharacterState.Task.DRINK, actor);
				TaskHelp.ShowHelp(TaskHelp.DRINK, actor);
				break;
		}
		ending = false;
	}
	
	public FollowAction(GameObject actor, GameObject targetCharacter, Reason reason, bool canStartDialogueWithAgents) : this(actor, targetCharacter, DistanceConstants.ARMS_REACH, 0.0f, reason, canStartDialogueWithAgents){
	}
	
	public bool WillHaveSex(){
		return reason == Reason.SEX;
	}
	
	public bool WillDance(){
		return reason == Reason.DANCE;
	}
	
	public bool WillPlayPokerWithoutMoney(){
		return reason == Reason.POKER_WITHOUT_MONEY;
	}
	
	public bool WillPlayPokerWithMoney(){
		return reason == Reason.POKER_WITH_MONEY;
	}
	
	public void Replaced(){
		if (!ending){
			actionRunner.ResetRoutine();
		}
	}
	
	protected override void UpdateFirstRound(){
		if (reason != Reason.NPC){
			targetCharacterState.SetFollowerAction(this);
		}
	}
	
	protected override Action CreateDefaultAction(){
		return new MoveToAction(actor, targetCharacter, maximumDistance, minimumDistance, false, true, false, true);
	}
	
	protected override void UpdateLastRound(bool interrupted){
		ending = true;
		targetCharacterState.SetFollowerAction(null);
	}
	
}

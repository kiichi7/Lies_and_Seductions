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

public abstract class AbstractNestingAction : AbstractAction {

	protected Action currentAction;
	protected ArrayList nextActions;
	protected bool endedManually;
	protected bool completed;
	
	public AbstractNestingAction(GameObject actor) : base(actor){
		InitInteractionInfo(true, true);
		currentAction = null;
		nextActions = new ArrayList();
		endedManually = false;
		completed = false;
	}
	
	protected void InitInteractionInfo(bool canStartDialogueWithPC, bool canStartDialogueWithAgents){
		InitInteractionInfo(canStartDialogueWithPC, canStartDialogueWithAgents, false);
	}
	
	protected void QueueAction(Action action, bool clearPrevious){
		//if (actor.name.Equals("Chris")){
		//	Debug.Log(this + " ==== Queueing " + action);
		//}
		if (action == null){
			Debug.LogError("AbstractNestingAction.QueueAction(null, " + clearPrevious + "): Error: Called with null Action");
			return;
		}
		if (clearPrevious){
			if (currentAction != null){
				currentAction.EndASAP(true);
			}
			nextActions.Clear();
		}
		nextActions.Add(action);
		if (currentAction == null){
			AdvanceToNextAction();
		}
	}
	
	protected void QueueAction(Action action){
		QueueAction(action, false);
	}
	
	protected void End(){
		if (currentAction != null){
			currentAction.EndASAP(false);
		}
		nextActions.Clear();
		endedManually = true;
	}
	
	protected void AdvanceToNextAction(){
		//Debug.Log(this + " AdvancingToNextAction.");
		Action nextAction;
		if (nextActions.Count > 0){
			nextAction = (Action)nextActions[0];
			nextActions.RemoveAt(0);
		} else if (!endedASAP && !endedManually){
			nextAction = CreateDefaultAction();
			//Debug.Log("AbstractNestingAction.AdvanceToNextAction() " + this + " ==== Default " + nextAction);
		} else {
			nextAction = null;
		}
		currentAction = nextAction;
	}
	
	protected override void UpdateAnyRound(){
		//Debug.Log("AbstractNestingAction.UpdateAnyRound");
		if (currentAction != null){
			currentAction.UpdateAction();
		}
		if (currentAction == null || currentAction.IsFinished()){
			AdvanceToNextAction();
		}
		if (currentAction == null){
			//Debug.Log("currentAction == null");
			completed = true;
		}
	}
	
	public override void EndASAP(bool immediately){
		base.EndASAP(immediately);
		if (currentAction != null){
			currentAction.EndASAP(immediately);
		}
		nextActions.Clear();
	}
	
	protected override bool IsCompleted(){
		//Debug.Log("AbstractNestingAction.IsCompleted? " + completed);
		return completed;
	}
	
	protected virtual Action CreateDefaultAction(){
		return null;
	}
	
	public override void OnActionGUI(){
		if (currentAction != null){
			currentAction.OnActionGUI();
		}
	}
	
	public override void FastForwardMovement(){
		UpdateAction();
		base.FastForwardMovement();
		if(currentAction!=null) {
			currentAction.FastForwardMovement();
		}
		else {
			Debug.LogError("AbstractNestingAction.FastForwardMovement(): currentAction is null");	
		}
	}
}

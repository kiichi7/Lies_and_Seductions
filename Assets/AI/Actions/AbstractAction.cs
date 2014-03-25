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

public abstract class AbstractAction : Action {

	protected GameObject actor;
	protected ActionRunner actionRunner;
	
	private Action quickReaction;
	private MoveToAction moveToAction;
	
	protected CharacterState state;
	protected CharacterAnimator animator;
	protected CharacterMover mover;
	
	protected string animationName;
	protected WrapMode wrapMode;
	protected Emotion.BodyParts emotionBodyParts;
	
	protected bool canStartDialogueWithPC;
	protected bool canStartDialogueWithAgents;
	private bool canEndAnyTime;
	//private bool canCancelDuringMovement;
	
	protected bool started;
	protected bool endedASAP;
	protected bool endNextRound;
	
	private bool finished;

	public AbstractAction(GameObject actor) {
		this.actor = actor;

		state = actor.GetComponent(typeof(CharacterState)) as CharacterState;
		animator = actor.GetComponent(typeof(CharacterAnimator)) as CharacterAnimator;
		mover = actor.GetComponent(typeof(CharacterMover)) as CharacterMover;
		actionRunner = actor.GetComponent(typeof(ActionRunner)) as ActionRunner;

		animationName = null;
		wrapMode = WrapMode.Once;
		emotionBodyParts = Emotion.BodyParts.NONE;

		canStartDialogueWithPC = true;
		canStartDialogueWithAgents = true;
		canEndAnyTime = true;
		//canCancelDuringMovement = true;

		quickReaction = null;
		moveToAction = null;
		
		started = false;
		endedASAP = false;
		endNextRound = false;
		
		finished = false;
	}
	
	protected void InitAnimationInfo(string animationName, WrapMode wrapMode, Emotion.BodyParts emotionBodyParts){
		if (animationName == null || animationName.Equals("")){
			Debug.LogError("AbstractAction.InitAnimationInfo(): " + actor.name + ". Provide a name of the animation!");
		}
		this.animationName = animationName;
		this.wrapMode = wrapMode;
		this.emotionBodyParts = emotionBodyParts;
	}
	
	protected void InitInteractionInfo(bool canStartDialogueWithPC, bool canStartDialogueWithAgents, bool canEndAnyTime){
		this.canStartDialogueWithPC = canStartDialogueWithPC;
		this.canStartDialogueWithAgents = canStartDialogueWithAgents;
		this.canEndAnyTime = canEndAnyTime;
	}
	
	protected void InitMovementInfo(GameObject target, float maximumDistanceFromTarget, float minimumDistanceFromTarget, bool sitDown, bool turnToFaceTarget, bool turnToMatchWaypoint){
		if (target == null){
			Debug.LogError("AbstractAction.InitMovementInfoYou(): " + actor.name + ". No target provided!");
			return;
		}
		moveToAction = new MoveToAction(actor, target, maximumDistanceFromTarget, minimumDistanceFromTarget, sitDown, turnToFaceTarget, turnToMatchWaypoint, false);
		//this.canCancelDuringMovement = canCancelDuringMovement;		
	}
	
	protected void InitMovementInfo(GameObject target, float maximumDistanceFromTarget, bool sitDown, bool turnToFaceTarget, bool turnToMatchWaypoint){
		InitMovementInfo(target, maximumDistanceFromTarget, 0.0f, sitDown, turnToFaceTarget, turnToMatchWaypoint);
	}
		
	public GameObject GetActor(){
		return actor;
	}

	protected void StartAnimation(string animationName, WrapMode wrapMode, Emotion.BodyParts emotionBodyParts){
		this.animationName = animationName;
		this.wrapMode = wrapMode;
		this.emotionBodyParts = emotionBodyParts;
		animator.StartAnimation(animationName, wrapMode, emotionBodyParts);
	}
	
	public virtual void UpdateAction(){
		//Debug.Log("UpdateAction: " + this + ": " + actor.name);
		if (quickReaction != null){
			//Debug.Log("Updating quickReaction");
			quickReaction.UpdateAction();
			if (quickReaction.IsFinished()){
				quickReaction = null;
			}
		} else if (moveToAction != null){
			//Debug.Log("Updating moveToAction");
			moveToAction.UpdateAction();
			if (moveToAction.IsFinished()){
				//Debug.Log("moveToAction is finished.");
				moveToAction = null;
			}
		} else {
			//Debug.Log("Regular update.");
			if (!started){
				//Debug.Log("started == false");
				if (endNextRound){
					//Debug.Log("Ending before starting.");
					finished = true;
					EndedBeforeStarting();
					return;
				}
				if (animationName != null){
					if (!IsCompleted()){
						StartAnimation(animationName, wrapMode, emotionBodyParts);
					}
				}
				ActionDebug.LogActionStarted(actor, this);
				UpdateFirstRound();
				started = true;
			}
			//Debug.Log("Any Round: " + this + ": " + actor.name);
			UpdateAnyRound();
			if (IsLastRound()){
				//Debug.Log("Last Round: " + this + ": " + actor.name);
				ActionDebug.LogActionEnded(actor, this);
				UpdateLastRound(endedASAP);
				finished = true;
			}
		}
	}
	
	protected virtual void EndedBeforeStarting(){	
	}
	
	protected virtual void UpdateFirstRound(){
	}
	
	protected virtual void UpdateAnyRound(){
	}
	
	protected virtual void UpdateLastRound(bool interrupted){
	}

	public virtual void OnActionGUI(){	
	}
	
	public bool IsFinished(){
		return finished;
	}
	
	public bool IsLastRound(){
		//Debug.Log("IsLastRound?");
		if (endNextRound){
			//Debug.Log("Yes, because endNextRound == true");
			return true;
		} else if (!started){
			//Debug.Log("No, because not started");
			return false;
		} else {
			//Debug.Log("Depends on IsCompleted");
			return IsCompleted();
		}
	}
	
	protected abstract bool IsCompleted();
	
	public bool CanStartDialogue(bool withPC){
		//if(CharacterManager.IsMajorNPC(actor))
		//	Debug.Log("AbstractAction.CanStartDialogue(): " + actor.name + " at " + state.GetCurrentArea().name + " at " + GameTime.GetDateAndTime().Hour + " !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
		if(withPC) {
			//Debug.Log("-- with PC: OK !!!!!!!!!!!!!!!!!!!!!!!!!");
			return canStartDialogueWithPC;
		} else if (actor == CharacterManager.GetPC()) {
			Debug.Log("-- Actor is PC:" + canStartDialogueWithAgents);
			return canStartDialogueWithAgents;
		} else if(state.GetCurrentArea().name.Equals("Restaurant")) {
			if(GameTime.GetDateAndTime().Hour >= 21 || GameTime.GetDateAndTime().Hour < 7) {
				//Debug.Log("-- denied: requested at the restaurant after 21 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
				return false;	
			}
			else {
				//Debug.Log("-- location restaurant at the daytime: " + canStartDialogueWithAgents);
				return canStartDialogueWithAgents;	
			}
		} else if (state.GetCurrentArea().name.Equals("Cabin")) {
			//Debug.Log("-- denied: CABIN");
			return false;	
		} else {
			//Debug.Log("-- ELSE: " + canStartDialogueWithAgents);
			return canStartDialogueWithAgents;
		}
		
		/*
		if(CharacterManager.IsMajorNPC(actor))
			Debug.Log("AbstractAction.CanStartDialogue(): " + actor.name + " at " + state.GetCurrentArea().name + " at " + GameTime.GetDateAndTime().Hour + " !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
		if (withPC){
			Debug.Log("AbstractAction.CanStartDialogue(): with PC is true: granted");
			return canStartDialogueWithPC;
		} else if (actor != CharacterManager.GetPC() && state.GetCurrentArea().name.Equals("restaurant") && (GameTime.GetDateAndTime().Hour >= 21 || GameTime.GetDateAndTime().Hour < 7)){
			// PL: 7.4.2009: changed [...].Hour>=23 to 21 as I like to get character out from the 
			// Wrote abobve code before noticed that the bug here probable is "restaurant" (should be "Restaurant"
			// restaurant and if the characters can start dialogue in restaurant until it closes 
			// they stay there until midnight... 
			Debug.Log("AbstractAction.CanStartDialogue(): denied: requested in the restaurant after 21 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			return false;
		} else {
			Debug.Log("AbstractAction.CanStartDialogue():  ")
			return canStartDialogueWithAgents && !state.GetCurrentArea().name.Equals("Cabin");
		}*/
	}

	public virtual void EndASAP(bool immediately){
		//Debug.Log("Ending ASAP: " + this + ": " + actor.name);
		//Debug.Log("immediately == " + immediately + ", canEndAnyTime == " + canEndAnyTime + ", canCancelDuringMovement == " + canCancelDuringMovement + ", started == " + started);
		endedASAP = true;
		if (quickReaction != null){
			quickReaction.EndASAP(immediately);
		}
		if (moveToAction != null){
			moveToAction.EndASAP(immediately);
		}
		if (immediately || canEndAnyTime || !started){
			//Debug.Log("endNextRound = true");
			endNextRound = true;
		}
		//EndSubactionsASAP(immediately);
	}
	
	/*protected virtual void EndSubactionsASAP(bool immediately){
	}*/

	public void StartQuickReaction(Action quickReaction){
		this.quickReaction = quickReaction;
	}
	
	public virtual bool ShouldNPCStop(GameObject npc){
		//return following && Vector3.Distance(actor.transform.position, npc.transform.position) < DistanceConstants.STOP_WHEN_PC_APPROACHES_DISTANCE;
		return false;
	}
	
	public virtual void FastForwardMovement(){
		Debug.Log("AbstractAction.FastForwardMovement(): " + actor.name);
		if (moveToAction != null){
			moveToAction.FastForward();
		}
	}
	
	public virtual void Cancelled(){
	}
	
}

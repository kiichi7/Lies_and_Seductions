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

public class DialogueAction : AbstractNestingAction {

	private const float PATIENCE_IN_SECONDS = 5.0f;

	private GameObject target;
	private bool isPC;
	
	private SayLineAction sayLineAction;
	private Action fillerAction;
	private Action afterDialogueConsequenceAction;
	private Line nextLine;
	
	private bool busy;
	private bool controllerFinished;
	private bool hasBeenInterrupted;
	
	public DialogueAction(GameObject actor, string targetName, bool isPC) : base (actor){
		InitInteractionInfo(true, false);
		target = CharacterManager.GetCharacter(targetName);
		InitMovementInfo(target, DistanceConstants.TALKING_MAX_DISTANCE, DistanceConstants.TALKING_MIN_DISTANCE, false, true, false);
		
		this.isPC = isPC;
		this.sayLineAction = null;
		this.fillerAction = null;
		afterDialogueConsequenceAction = null;
		busy = true;
		controllerFinished = false;
		hasBeenInterrupted = false;
	}
	
	/*public void LookAt(Vector3 lookingPosition){
		((CharacterMover)actor.GetComponent("CharacterMover")).LookAt(lookingPosition);
	}*/
	
	public bool IsName(string name){
		return actor.name.Equals(name);
	}
	
	public bool HasBeenInterrupted(){
		return hasBeenInterrupted;
	}
	
	public void FixFacing(){
		QueueAction(new TurnToFaceAction(actor, target), !busy);
		busy = true;
	}
	
	public void PrepareLine(Line nextLine){
		this.nextLine = nextLine;
		animator.SetEmotion(nextLine.GetEmotion());
	}
	
	private Action CreateFillerAction(){
		Item item = state.GetItem();
		if (item != null && (item.name.Equals("beer") || item.name.Equals("whiskey") || item.name.Equals("drink")) && actor != CharacterManager.GetPC()){
			return new DrinkAction(actor, item.name);
		} else {
			return new IdleAction(actor, 1.0f, null, 0.0f, false);
		}
	}
	
	public void SayLine(bool npcNpcDialogue){
		sayLineAction = new SayLineAction(actor, nextLine.GetText(), npcNpcDialogue);
		QueueAction(sayLineAction, !busy);
		busy = true;
	}
	
	public void SetConsequenceAction(Action consequenceAction, bool afterDialogue){
		if (!afterDialogue){
			QueueAction(consequenceAction);
			busy = true;
		} else {
			afterDialogueConsequenceAction = consequenceAction;
		}
	}
	
	protected override Action CreateDefaultAction(){
		busy = false;
		if (!controllerFinished){
			return fillerAction = CreateFillerAction();
		} else {
			return null;
		}
	}
	
	protected override void EndedBeforeStarting(){
		UpdateLastRound(true);
	}
	
	protected override void UpdateFirstRound(){
		busy = false;
	}
	
	protected override void UpdateLastRound(bool interrupted){
		if (interrupted){
			hasBeenInterrupted = true;
		} else {
			actionRunner.ResetRoutine(afterDialogueConsequenceAction, false);
		}
	}
	
	public bool IsReady(){
		return !busy;
	}
	
	public void InterruptLine(){
		if (sayLineAction != null){
			sayLineAction.Interrupt();
		}
	}
	
	public void EndDialogue(bool wasWithPC){
		controllerFinished = true;
		if (fillerAction != null){
			fillerAction.EndASAP(false);
		}
		if (wasWithPC){
			state.PCDialogueEnded();
		}
	}
	
	/*public override void StartQuickReaction(Action quickReaction){
		if (currentSayLineAction != null){
			currentSayLineAction.StartQuickReaction(quickReaction);
		} else {
			fillerAction.StartQuickReaction(quickReaction);
		}	
	}*/
	
	public override void Cancelled(){
		hasBeenInterrupted = true;
	}
}

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

public class GenericDialogueAction : AbstractActionWithEndCondition {

	private SpeechBalloonWithSymbols speechBalloon;
	private bool ready;
	private bool completed;

	public GenericDialogueAction(GameObject actor, GameObject otherSpeakerGO) : base(actor){
		InitAnimationInfo("idle_pose_1", WrapMode.Loop, Emotion.BodyParts.NONE);
		InitMovementInfo(otherSpeakerGO, DistanceConstants.ARMS_REACH, false, true, false);
		InitInteractionInfo(false, false, true);
		speechBalloon = (SpeechBalloonWithSymbols)actor.GetComponent("SpeechBalloonWithSymbols");
		ready = false;
		completed = false;
	}
	
	protected override void UpdateFirstRound(){
		ready = true;
	}
	
	public bool IsReady(){
		return ready;
	}
	
	public void SayLine(){
		//Debug.Log("GenericDialogueAction.SayLine(): " + actor.name);
		speechBalloon.SayRandomLine();
	}
	
	public void EndLine(){
		//Debug.Log("GenericDialogueAction.EndLine()" + actor.name);
		speechBalloon.RemoveBalloon();
	}
	
	public void EndDialogue(){
		//Debug.Log("GenericDialogueAction.EndDialogue()" + actor.name);
		completed = true;
	}
	
	protected override bool IsCompleted(){
		return completed;
	}

}

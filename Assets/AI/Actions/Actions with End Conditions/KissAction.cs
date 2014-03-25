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

public class KissAction : AbstractActionWithEndCondition {

	private GameObject partner;
	private bool kissingStarted;
	private RespondToKissAction respondToKissAction;

	public KissAction(GameObject actor, GameObject partner, RespondToKissAction respondToKissAction) : base(actor){
		InitInteractionInfo(false, false, false);
		
		this.partner = partner;
		kissingStarted = false;
		this.respondToKissAction = respondToKissAction;
	}
	
	protected override void UpdateAnyRound(){
		if (!kissingStarted){
			if (respondToKissAction.IsReady()){
				StartAnimation("kiss", WrapMode.Once, Emotion.BodyParts.NONE);
				respondToKissAction.StartAnimation();
				kissingStarted = true;
			}
		}
	}
	
	protected override bool IsCompleted(){
		return kissingStarted && !animator.IsPlaying("kiss");
	}
}

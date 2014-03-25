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

public class NoBetAction : AbstractActionWithTimer {

	private string text;
	private const float DURATION = 2.0f;
	
	public NoBetAction(GameObject actor, string text) : base(actor){
		InitTimerInfo(DURATION);
		InitInteractionInfo(false, false, false);
		this.text = text;
	}
	
	protected override void UpdateFirstRound(){
		animator.StartAnimation("talk", WrapMode.Loop, CharacterAnimator.GENERIC_LAYER, new string[]{"bone_neck"});
	}

	protected override void UpdateLastRound(bool interrupted){
		animator.StopAnimation("talk");
	}

	public override void OnActionGUI(){
		PokerGUI.DrawBalloon(text);
	}
}

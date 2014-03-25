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

public class IdleAction : AbstractActionWithTimer {

	private const float MIN_TIME_BETWEEN_ANIMATION_CHANGE = 2.0f;
	private const float MAX_TIME_BETWEEN_ANIMATION_CHANGE = 10.0f;

	private float nextAnimationChangeTime;
	private bool playingSingleLoopAnimation;

	public IdleAction(GameObject actor, float durationInSeconds, GameObject target, float maximumDistanceFromTarget, bool turnToMatchWaypoint) : base (actor){
		//Debug.Log("Idle action! (" + actor.name + ")");
		InitTimerInfo(durationInSeconds);
		if (target != null){
			InitMovementInfo(target, maximumDistanceFromTarget, false, false, turnToMatchWaypoint);
		}
		
		nextAnimationChangeTime = GameTime.GetRealTimeSecondsPassed();
	}
	
	protected override void UpdateAnyRound(){
		if (!animator.IsPlaying("idle_pose_1") && !animator.IsPlaying("idle_pose_2") && (playingSingleLoopAnimation || GameTime.GetRealTimeSecondsPassed() > nextAnimationChangeTime)){
			int animationIndex = (int)Random.Range(0, 4);
			string animationName;
			WrapMode wrapMode;
			switch (animationIndex){
			case 0:
				animationName = "idle_pose_1";
				wrapMode = WrapMode.Loop;
				playingSingleLoopAnimation = false;
				break;
			case 1:
				animationName = "idle_pose_2";
				wrapMode = WrapMode.Loop;
				playingSingleLoopAnimation = false;
				break;
			case 2:
				animationName = "idle_action_1";
				wrapMode = WrapMode.Once;
				playingSingleLoopAnimation = true;
				break;
			case 3:
				animationName = "idle_action_2";
				wrapMode = WrapMode.Once;
				playingSingleLoopAnimation = true;
				break;
			default:
				animationName = null;
				wrapMode = WrapMode.Once;
				break;
			}
			//Debug.Log("IdleAction.UpdateAnyRound(): " +  actor.name + ": playing idle animation " + animationName);
			Emotion.BodyParts bodyParts;
			if (actor == CharacterManager.GetPC()){
				bodyParts = Emotion.BodyParts.FACE;
			} else {
				bodyParts = Emotion.BodyParts.ALL;
			}
			StartAnimation(animationName, wrapMode, bodyParts);
			nextAnimationChangeTime = Random.Range(MIN_TIME_BETWEEN_ANIMATION_CHANGE, MAX_TIME_BETWEEN_ANIMATION_CHANGE);
		}
	}
}

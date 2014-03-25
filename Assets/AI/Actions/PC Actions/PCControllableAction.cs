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

public class PCControllableAction : AbstractActionWithEndCondition {

	private PCMover pcMover;
	private bool sitting;
	private string seatType;
	private HotSpot clickedHotSpot;

	public PCControllableAction(GameObject actor, bool sitting, string seatType) : base(actor){
		//if (!sitting){
			InitAnimationInfo("idle_pose_1", WrapMode.Loop, Emotion.BodyParts.ALL);
		//}
		pcMover = (PCMover)actor.GetComponent("PCMover");
		this.sitting = sitting;
		this.seatType = seatType;
		clickedHotSpot = null;
	}
	
	protected override void UpdateAnyRound(){
		if (sitting){
			if (!animator.IsPlaying("sit_" + seatType)){
				StartAnimation("sit_" + seatType, WrapMode.Loop, Emotion.BodyParts.FACE);
			}
		} else if (state.GetSleepDept() > 0){
			//actionRunner.ResetRoutine(new PCSleepAction(actor), true);
			actionRunner.ResetRoutine(new PCForcedSleepAction(actor), true);
		} else {
			pcMover.MovePC();
		}
		FreeWalkGUI.UpdateCursor();
		GameObject clickedGameObject = FreeWalkGUI.GetClickedHotSpot();
		if (clickedGameObject != null){
			//Debug.Log("HotSpot clicked");
			clickedHotSpot = (HotSpot)clickedGameObject.GetComponent("HotSpot");
		}
	}
	
	protected override bool IsCompleted(){
		return clickedHotSpot != null;
	}
	
	public HotSpot GetClickedHotSpot(){
		return clickedHotSpot;
	}
	
	public override void OnActionGUI(){
		FreeWalkGUI.DrawGUI();
	}
	
	public override bool ShouldNPCStop(GameObject npc){
		return Vector3.Distance(npc.transform.position, actor.transform.position) < DistanceConstants.STOP_WHEN_PC_APPROACHES_DISTANCE;
	}
}

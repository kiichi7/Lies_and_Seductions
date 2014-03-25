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

public class PCDanceAction : AbstractActionWithEndCondition, SelectableAction {

	private const float ROTATION_SPEED = 40.0f;
	
	private GameObject partner;
	private bool rotating;
	
	private bool playingIdle;
	
	public PCDanceAction(GameObject actor, string targetName) : base(actor){
		InitAnimationInfo("idle_pose_2", WrapMode.Loop, Emotion.BodyParts.ALL);
		InitInteractionInfo(false, false, true);
		InitMovementInfo(GameObject.Find(targetName), DistanceConstants.WAYPOINT_RADIUS, false, false, true);
		partner = null;
		rotating = false;
		playingIdle = false;
	}
	
	protected override void UpdateFirstRound(){
		Item item = state.GetItem();
		if(item) {
			animator.StopAnimation("hold_" + item.name);
			state.SetItem(null);
		}
		if(state.currentTask.task == CharacterState.Task.DANCE) {
			state.TaskCompleted(); 
		}
		else {
			state.SetTask(CharacterState.Task.NONE, null);
		}
		TaskHelp.RemoveHelp();
		if (state.HasDancePartner()){
			partner = state.GetFollower();
			ActionRunner partnerActionRunner = (ActionRunner)partner.GetComponent("ActionRunner");
			partnerActionRunner.ResetRoutine(new DanceAction(partner, 1000.0f, "Waypoint: Dance Floor", true), true);
		}
		DanceModeController.StartDanceMode(this);		
	}
	
	protected override void UpdateAnyRound(){
		DanceModeController.UpdateDanceModeController();
		if (rotating){
			mover.Rotate(new Vector3(0.0f, ROTATION_SPEED * Time.deltaTime, 0.0f));
		}
	}
	
	protected override void UpdateLastRound(bool interrupted){
		if (partner != null){
			ActionRunner partnerActionRunner = (ActionRunner)partner.GetComponent("ActionRunner");
			partnerActionRunner.ResetRoutine(new StartDialogueAction(partner, actor, partner.name, "after_dance"), true);
			actionRunner.ResetRoutine(new FollowAction(actor, partner, Mathf.Infinity, 0.0f, FollowAction.Reason.NPC, true), false);
		}
	}
	
	protected override bool IsCompleted(){
		return DanceModeController.IsFinished();
	}
	
	public override void OnActionGUI(){
		if (started){
			DanceModeController.DrawGUI();
		}
	}
	
	public void DirectionsPressed(ArrayList directionsPressed){
		rotating = true;
		playingIdle = false;
		switch ((Direction)directionsPressed[0]){
		case Direction.LEFT:
			StartDanceMove("dance_feet_1", "dance_torso_1");
			break;
		case Direction.DOWN:
			StartDanceMove("dance_feet_2", "dance_torso_1");
			break;
		case Direction.RIGHT:
			StartDanceMove("dance_feet_1", "dance_torso_2");
			break;
		case Direction.UP:
			StartDanceMove("dance_feet_2", "dance_torso_2");
			break;
		}
	}
	
	public void StartIdle() {
		if(playingIdle == true) {
			return;
		}
		rotating = false;
		playingIdle = true;
		// "dance_idle", "idle_pose1"
		animator.StartAnimation("dance_idle", WrapMode.Loop, Emotion.BodyParts.NONE);
			
	}
	
	private void StartDanceMove(string feetAnimationName, string torsoAnimationName){
		animator.StartAnimation(feetAnimationName, WrapMode.Once, Emotion.BodyParts.NONE);
		animator.StartAnimation(torsoAnimationName, WrapMode.Once, CharacterAnimator.GENERIC_LAYER, new string[]{"bone_spine_lumbar"});
	}
	
	public MenuIconSet GetIconSet(PopupMenuGUI popupMenuGUI){
		return popupMenuGUI.danceIcon;
	}
	
	public void StopDancing(){
		animator.StartAnimation("idle_pose_1", WrapMode.Loop, Emotion.BodyParts.ALL);
		rotating = false;
	}
}

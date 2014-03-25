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
using System;

public class WalkToAction : AbstractActionWithEndCondition {

	private const float GHOST_MODE_DURATION = 3.0f;
	private const int MAXIMUM_NUMBER_OF_TROUBLESHOOTS = 100;
	private const int MAXIMUM_NUMBER_OF_ROUNDS = 10000;
	
	private const int MAXIMUM_NUMBER_FAILED_GOALS = 3;
	
	private const float HISTORY_LENGTH_IN_SECONDS = 0.5f;
	private const float EXPECTED_MINIMUM_DISTANCE_DURING_HISTORY = 0.05f;
	
	private class HistoryNode {
		private float time;
		private Vector3 position;
		
		public HistoryNode(float time, Vector3 position){
			this.time = time;
			this.position = position;
		}
		
		public float GetTime(){
			return time;
		}
		
		public Vector3 GetPosition(){
			return position;
		}	
	}
	
	private int failedGoals;
	
	private bool stoppingForPC;
	private bool stoppingForTarget;

	private GameObject target;
	private GameObject destination;
	private float maximumDistance;
	private float minimumDistance;
	private bool follow;
	private int numberOfTroubleshoots;
	private int numberOfRounds;
	private bool noSpaceToBackAway;
	
	private PathPlanner pathPlanner;
	//private PathFinder.PathFindingState pathFindingState;
	
	private ArrayList history;
	
	private ImpressionMemory memory;
	
	//private ArrayList path;

	public WalkToAction(GameObject actor, GameObject target, float maximumDistance, float minimumDistance, bool follow) : base(actor){
		InitAnimationInfo("walk", WrapMode.Loop, Emotion.BodyParts.FACE);
		stoppingForPC = false;
		this.target = target;
		destination = target;
		
		if (target == null){
			Debug.LogError("WalkToAction.WalkToAction(): " + actor.name + " Can't walk to a null target!");
		}
		
		this.maximumDistance = maximumDistance;
		this.minimumDistance = minimumDistance;
		this.follow = follow;
		numberOfTroubleshoots = 0;
		numberOfRounds = 0;
		failedGoals = 0;
		pathPlanner = actor.GetComponent(typeof(PathPlanner)) as PathPlanner;
		history = new ArrayList();
		memory = actor.GetComponent(typeof(ImpressionMemory)) as ImpressionMemory;
		//path = null;
	}
	
	/*private void StartPathFinding(){
		pathFindingState = PathFinder.StartPathFindWithPosition(actor, target);
		if (!pathFindingState.IsDone()){
			ContinuePathFinding();
		} else {
			path = pathFindingState.GetPath();
		}
	}
	
	private void ContinuePathFinding(){
		//Debug.Log(actor.name + " pathfinding");
		pathFindingState = PathFinder.ContinuePathFinding(pathFindingState);
		if (pathFindingState.IsDone()){
			path = pathFindingState.GetPath();
		}
	}*/
	
	private void ResetPath(){
		if (target == null){
			Debug.LogError("WalkToAction.ResetPath(): " + actor.name + " Can't walk to a null target!");
			return;
		}
		if (CharacterManager.IsCharacter(target)){
			CharacterState targetState = target.GetComponent(typeof(CharacterState)) as CharacterState;
			Seat targetSeat = targetState.GetCurrentSeat();
			if (targetSeat != null){
				destination = targetSeat.GetWaypoint().gameObject;
			} else {
				destination = target;
			}
		} else {
			destination = target;
		}
		pathPlanner.SetGoal(destination.transform.position, destination.name);
		
	}
	
	public void FastForward(){
		if (target == null){
			Debug.LogError("WalkToAction.FastForward(): " + actor.name + ", ERROR target is null");	
		}
		else{
			Debug.Log("WalkToAction.FastForward(): " + actor.name + ", target is :" + target.name + ", " + target.transform.position);
		}
		//Debug.Log("WalkToAction.FastForward(): NavigatonMesh.FindEnclosingPolygon: " + target.transform.position);
		try {
			Area targetArea = NavigationMesh.FindEnclosingPolygon(target.transform.position).GetArea();
			//Debug.Log("WalkToAction.FastForward(): targetArea.JumpInto");
			targetArea.JumpInto(actor);
			//Debug.Log("WalkToAction.FastForward(): ResetPath");
			ResetPath();
			//Debug.Log("WalkToAction.FastForward(): Destination position is: " + destination.transform.position);
			//Debug.Log("WalkToAction.FastForward(): SubGoal is: " + pathPlanner.GetSubGoal(true));
			//Debug.Log("WalkToAction.FastForward(): EnterGhostMode");
			mover.EnterGhostMode();
			//Debug.Log("WalkToAction.FastForward(): The loop");
			while (!IsCompleted() && !follow){
				//Debug.Log("WalkToAction.FastForward(): PerformWalk");
				PerformWalk(true);
			}
			//Debug.Log("WalkToAction.FastForward(): mover.ExitGhosMode");
			mover.ExitGhostMode();
			//Debug.Log("WalkToAction.FastForward(): WalkAction.FastForward done");
			Debug.Log("WalkToAction.FastForward(): numberOfRounds: " + numberOfRounds + "numberOfTroubleshoots:" + numberOfTroubleshoots);
			//Debug.Log("WalkToAction.FastForward(): numberOfTroubleshoots: " + numberOfTroubleshoots);
		}
		catch(Exception e) {
			Debug.LogError("WalkToAction.FastForward(): " + actor.name + ", ERROR: cannot fast forward to " + target.name + ": " + e);
		}
	}

	protected override void UpdateFirstRound(){
		//StartPathFinding();
		ResetPath();
	}
	
	private void PerformWalk(bool isFastForwarding){
		numberOfRounds++;
		
		float planarDistance = pathPlanner.GetPlanarDistance(actor.transform.position, destination.transform.position);
		
		if (planarDistance <= minimumDistance){
			try {
				noSpaceToBackAway = !mover.BackAwayFrom(destination.transform.position);
			}
			catch(Exception e){
				Debug.LogError("WalkToAction.PerformWalk(): " + actor.name + ", ERROR in CharacterMover.BackAwayFrom(): " + e);
				return;
			}
		} else {
			Vector3 subgoal = pathPlanner.GetSubGoal(isFastForwarding);
			if(pathPlanner.IsGoalDefined == false) {
				failedGoals++;
				Debug.LogError("WalkToAction.PerformWalk(): " + actor.name + ", planning route failed! Resetting path; TRY=" + failedGoals);
				ResetPath();
			}
			bool wasAbleToMove = mover.MoveTowards(subgoal);
			if (!wasAbleToMove){
				Troubleshoot();
			}
			//planarDistance = pathPlanner.GetPlanarDistance(actor.transform.position, destination.transform.position);
			if (/*follow*/(CharacterManager.IsCharacter(target))/* && planarDistance > maximumDistance*/){
				ResetPath();
			}
			/*
			history.Add(new HistoryNode(Time.time, actor.transform.position));
			bool longEnoughHistory = false;
			while (Time.time - ((HistoryNode)history[0]).GetTime() > HISTORY_LENGTH_IN_SECONDS){
				history.RemoveAt(0);
				longEnoughHistory = true;
			}
			if (longEnoughHistory && Vector3.Distance(actor.transform.position, ((HistoryNode)history[history.Count - 1]).GetPosition()) < EXPECTED_MINIMUM_DISTANCE_DURING_HISTORY){
				TroubleshootLastResort();
			}*/
		}	
	}
	
	private void Troubleshoot(){
		numberOfTroubleshoots++;
		Debug.Log("WalkToAction.Troubleshoot(): " + actor.name + ": impossible to move " + destination.name + "(" + destination.transform.position + ") Troubleshooting!");
		//Debug.Log("Current position: " + actor.transform.position);
		if (destination != target || pathPlanner.GetPlanarDistance(destination.transform.position, actor.transform.position) > maximumDistance + DistanceConstants.COLLISION_LEEWAY_DISTANCE){
			//bool wasAbleToMove = mover.MoveToTheRight();
			//if (!wasAbleToMove){
				mover.EnterGhostMode();
				mover.InvokeExitGhostMode(GHOST_MODE_DURATION);
			//}
		} else {
			maximumDistance += DistanceConstants.COLLISION_LEEWAY_DISTANCE;
		}
	}
	
	private void TroubleshootLastResort(){
		/*Debug.LogError("Moving " + actor.name + " to " + destination.name + "(" + destination.transform.position + ") causing major problem! Using last resort troubleshoot!");
		Debug.Log("Current position: " + actor.transform.position);
		Debug.Log("Target: " + target.name + " (" + target.transform.position + ")");
		Debug.Log("Destination: " + destination.name + " (" + destination.transform.position + ")");
		Debug.Log("SubGoal position: " + pathPlanner.GetSubGoal());
		Debug.Log("Goal position: " + pathPlanner.Goal);
		//???*/
	}
	
	/*private void DrawPath(){
		if (path.Count == 0){
			Debug.DrawLine(actor.transform.position, target.transform.position, Color.cyan);
		} else {
			Debug.DrawLine(actor.transform.position, ((Waypoint)path[0]).transform.position, Color.cyan);
			for (int i = 0; i < path.Count - 1; i++){
				Debug.DrawLine(((Waypoint)path[i]).transform.position, ((Waypoint)path[i + 1]).transform.position, Color.cyan);
			}
			Debug.DrawLine(((Waypoint)path[path.Count - 1]).transform.position, target.transform.position, Color.cyan);
		}
	}*/
	
	private bool ShouldStopForPC(){
		if (state.IsIgnoringPC()){
			return false;
		} else {
			GameObject pc = CharacterManager.GetPC();
			if (actor == pc){
				return false;
			} else if (!CharacterManager.IsMajorNPC(actor)){
				return false;
			} else {
				ActionRunner pcActionRunner = pc.GetComponent(typeof(ActionRunner)) as ActionRunner;
				if (pcActionRunner.ShouldNPCStop(actor)){
					return true;
				} else {
					return false;
				}
			}
		}
	}
	
	private bool ShouldStopForTarget(){
		return follow && pathPlanner.GetPlanarDistance(actor.transform.position, target.transform.position) < maximumDistance;
	}
	
	protected override void UpdateAnyRound(){
		if (CharacterManager.IsPC(actor) && !follow){
			if (Mathf.Abs(Input.GetAxis("PC Forward Backward")) > 0.1 || Mathf.Abs(Input.GetAxis("PC Left Right")) > 0.1){
				actionRunner.ResetRoutine();
			}
		}
		//JCsProfilerMethod pm = JCsProfiler.Instance.StartCallStopWatch("WalkToAction", actor.name, "UpdateAnyRound");
		/*if (!pathFindingState.IsDone()){
			//Debug.Log(actor.name + ": ContinuePathFinding.");
			//ContinuePathFinding();
		} else {*/
			if (!stoppingForPC && ShouldStopForPC()){
				stoppingForPC = true;
				Emotion emotion;
				int attitudeTotal = memory.GetAttitudeTotal();
				if (attitudeTotal < -10){
					emotion = Emotion.DISLIKE;
				} else if (attitudeTotal < 10){
					emotion = Emotion.NEUTRAL;
				} else if (attitudeTotal < 30){
					emotion = Emotion.LIKE;
				} else {
					emotion = Emotion.LOVE;
				}
				animator.SetEmotion(emotion);
				StartAnimation("idle_pose_1", WrapMode.Loop, Emotion.BodyParts.ALL);
			}
			if (!stoppingForTarget && ShouldStopForTarget()){
				stoppingForTarget = true;
				StartAnimation("idle_pose_1", WrapMode.Loop, Emotion.BodyParts.ALL);
			}
			if (stoppingForPC){
				animator.TurnHead(CharacterManager.GetPC().transform.position);
				if (!ShouldStopForPC()){
					animator.ResetHeadDirection();
					stoppingForPC = false;
					animator.SetEmotion(Emotion.NEUTRAL);
					if (!stoppingForTarget){
						StartAnimation("walk", WrapMode.Loop, Emotion.BodyParts.FACE);
					}
				}
			} else if (stoppingForTarget){
				if (!ShouldStopForTarget()){
					stoppingForTarget = false;
					StartAnimation("walk", WrapMode.Loop, Emotion.BodyParts.FACE);
				}
			} else if (!IsCompleted()){
				PerformWalk(false);
			}
		//}
		//pm.CallIsFinished();
	}
	
	protected override void UpdateLastRound(bool interrupted){
		if (stoppingForPC){
			animator.ResetHeadDirection();
		}
		pathPlanner.CancelGoal();
		// Hopefully this will make sure that character facing is correct...
		mover.RotateToMatch(target.transform.rotation);
	}
	
	protected override bool IsCompleted(){
		if (follow){
			CharacterState state = (CharacterState)target.GetComponent(typeof(CharacterState));
			if (state.GetCurrentArea().name.Equals("Cabin")){
				return true;
			}
		}
		if (target == null){
			Debug.LogError("WalkToAction.IsCompleted(): " + actor.name + " ERROR: Can't reach null target. WalkTo is completed.");
			return true;
		}
		if(failedGoals > MAXIMUM_NUMBER_FAILED_GOALS) {
			Debug.LogError("WalkToAction.IsCompleted(): " + actor.name + " ERROR: Failed to plan route to the goal. WalkTo is completed.");
			return true;	
		}
		float planarDistance = pathPlanner.GetPlanarDistance(destination.transform.position, actor.transform.position);
		return numberOfTroubleshoots > MAXIMUM_NUMBER_OF_TROUBLESHOOTS || numberOfRounds > MAXIMUM_NUMBER_OF_ROUNDS || (!follow && planarDistance < maximumDistance && (planarDistance > minimumDistance || noSpaceToBackAway));
	}
}
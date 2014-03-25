/***********************************************************************
 *
 * Copyright 2008 Pedro Teixeira 
 *
 * Licensed under the Apache License, Version   2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License at 
 * http://www.apache.org/licenses/LICENSE-2.0 Unless required by 
 * applicable law or agreed to in writing, software distributed under 
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES 
 * OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and 
 * limitations under the License. 
 *
 * Source: http://code.google.com/p/ninjaxnagame/
 *********************************************************************
 * 
 * Adapted for "Lies and Seductions" by
 * Tommi Horttana (2008) 
 *
 *********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PathPlanner : MonoBehaviour {
	
	public Color pathDebugColor;
	
	/// <summary>
	/// Algorithm for pathfinding.
	/// </summary>
	private AStar pathFinder;

	private List<Transit> transits;
	
	/// <summary>
	/// Holds current planned macro path.
	/// </summary>
	private List<NavmeshPolygon> plannedPath;
	
	//private Random random = new Random();

	/// <summary>
	/// Returns true if a goal has been defined.
	/// </summary>
	public bool IsGoalDefined;

	/// <summary>
	/// Position to achieve.
	/// </summary>
	public Vector3 Goal;
	
	/// <summary>
	/// A subgoal is a target that is the same convex polygon of the entity, and hence best path is just a straight line.
	/// </summary>
	public Vector3 SubGoal;
	
	private float timeOfLastSubGoalLookAhead;
	
	private const float SUB_GOAL_LOOK_AHEAD_INTERVAL = 1.0f;
	
	private string goalName;

	public void Awake() {
		//instanciates A* implemenation with defined heuristics
		//IPathHeuristic heuristic = new EuclideanPathHeuristic(new ObstaclesDensityPathHeuristic()/*, (a, b) => a*b/2*/);
		pathFinder = new AStar(/*heuristic*/);
		IsGoalDefined = false;
		timeOfLastSubGoalLookAhead = Time.time;
	}
	
	/// <summary>
	/// Determine target position.
	/// </summary>
	/// <param name="goal"></param>
	public void SetGoal(Vector3 goal, string goalName) {
		Goal = goal;
		IsGoalDefined = true;
		this.goalName = goalName;

		//try {
		PlanTransitPath();
		PlanPathToNextTransitOrGoal();
		//}
		//catch(Exception e) {
		//	Debug.LogError("PathPlanner.SetGoal(" + goalName + " " + goal + ") for " + name + " failed with ERROR " + e);	
		//}
		//Debug.Log("Path length: " + plannedPath.Count);
            
	}

	/// <summary>
	/// Cancels previous set target position.
	/// </summary>
	public void CancelGoal() {
		IsGoalDefined = false;
		Goal = Vector3.zero;
		goalName="";
	}

	
	private void PlanTransitPath(){
		try {
			NavmeshPolygon goalPolygon = NavigationMesh.FindEnclosingPolygon(Goal);
			NavmeshPolygon currentPolygon = NavigationMesh.FindEnclosingPolygon(transform.position);
		
			Area goalArea = goalPolygon.GetArea();
			Area currentArea = currentPolygon.GetArea();
		
			if (goalArea != currentArea){
				transits = currentArea.GetTransitListTo(goalArea, true);
			} else {
				transits = new List<Transit>();
			}
		}
		catch(Exception e) {
			Debug.LogError("PathPlanner.PlanTransitPath(): Unable to plan a route for " + name + ", to " + goalName + " " + Goal + " with ERROR " + e);
			CancelGoal();
		}	
	}
	
	private void PlanPathToNextTransitOrGoal(){
		Vector3 destination;

		if (transits != null && transits.Count > 0){
			destination = transits[0].transform.position;
		} else {
			destination = Goal;
		}

		try {
			NavmeshPolygon destinationPolygon = NavigationMesh.FindEnclosingPolygon(destination);
			NavmeshPolygon currentPolygon = NavigationMesh.FindEnclosingPolygon(transform.position);

			if (destinationPolygon.Equals(currentPolygon)){
				//reseting path, because we can just use straight line
				plannedPath = null;
				SubGoal = destination;
			} else {
				plannedPath = pathFinder.FindPath(transform.position, destination, NavigationMesh.FindEnclosingPolygon(transform.position), NavigationMesh.FindEnclosingPolygon(destination));
				if(plannedPath == null) {
					CancelGoal(); //TODO: add timer or tabu list to keep preventing from trying
				}			
			}
		}
		catch(Exception e) {
			Debug.LogError("PathPlanner.PlanPathToNextTransitOrGoal(): Unable to plan  a route for " + name + ", to " + goalName + " " +  Goal + " with ERROR " + e);
			CancelGoal();
		}
	}	

	public bool IsNextTransitPlanned(Transit transit){
		return transits != null && transits.Count > 0 && transits[0] == transit;
	}

	public void TransitEntered(Transit transit){
		if (IsGoalDefined){
			transits.Remove(transit);
			PlanPathToNextTransitOrGoal();
		}
	}

	private bool HasClearStraightPathTo(Vector3 targetPosition){
		Vector3 raycastDirection = targetPosition - transform.position;
		float raycastDistance = raycastDirection.magnitude;
		int raycastLayerMask = 1 << NavmeshEdge.NAVMESH_LAYER_NUMBER;
		CharacterController controller = (CharacterController)GetComponent("CharacterController");
		Vector3 leftwardVector = transform.TransformDirection(controller.radius * (- Vector3.right));
		Vector3 rightwardVector = transform.TransformDirection(controller.radius * Vector3.right);
		bool resultLeft = !Physics.Raycast(transform.position + leftwardVector, raycastDirection, raycastDistance, raycastLayerMask);
		bool resultRight = !Physics.Raycast(transform.position + rightwardVector, raycastDirection, raycastDistance, raycastLayerMask);
		return resultLeft && resultRight;
	}

	/// <summary>
	/// Asks path planner wich direction to go to according to current set goal.
	/// </summary>
	/// <returns></returns>
	public Vector3 GetSubGoal(bool isFastForwarding) {
		if (GetPlanarDistance(transform.position, SubGoal) < DistanceConstants.WAYPOINT_RADIUS || Time.time > timeOfLastSubGoalLookAhead + SUB_GOAL_LOOK_AHEAD_INTERVAL || isFastForwarding){
			//if there is no planned path
			if (plannedPath == null || plannedPath.Count == 0) {
				return SubGoal;
			}  
	
			//check if goal is visible
			if (HasClearStraightPathTo(Goal)){
				SubGoal = Goal;
				//plannedPath = null;
				return Goal;
			}
	
			//check where we are in the planned path
			try {
				NavmeshPolygon currentPolygon = NavigationMesh.FindEnclosingPolygon(transform.position);
			

				int indexOfPolygonInPath = plannedPath.FindIndex(m => m.Equals(currentPolygon));
				//something went terribly wrong and we're currently in unplanned mesh: rebuild path and try again
				if (indexOfPolygonInPath == -1) {
					PlanPathToNextTransitOrGoal();
					return SubGoal;
				}

				//we're at the last mesh node: plan was completed
				if(indexOfPolygonInPath == plannedPath.Count -1){
					if (transits != null && transits.Count > 0){
						SubGoal = transits[0].transform.position;
					} else {
						SubGoal = Goal;
					}
					plannedPath = null;
					return SubGoal;
				}
			
				NavmeshEdge targetPortal = null;
		
				for(int i = indexOfPolygonInPath; i < plannedPath.Count - 1; i++){
					NavmeshPolygon nearPolygon = plannedPath[i];
					NavmeshPolygon farPolygon = plannedPath[i + 1];
					NavmeshEdge portal = nearPolygon.GetPortal(farPolygon);
					if (HasClearStraightPathTo(portal.transform.position) || i == indexOfPolygonInPath){
						targetPortal = portal;
					} else {
						break;
					}
				}
		
				if (targetPortal == null){
					Debug.LogError("PathPlanner.GetSubGoal(): No portal found!");
				}

			
				SubGoal = targetPortal.transform.position;
				timeOfLastSubGoalLookAhead = Time.time;
			}
			catch(Exception e) {
				Debug.LogError(e);
				Debug.LogError("PathPlanner.GetSubGoal(): Unable to plan  a route for " + name + ", to " + goalName);
				CancelGoal();
			}
		}

		return SubGoal;
    }
    
   	public float GetPlanarDistance(Vector3 vector1, Vector3 vector2){
		return Vector3.Distance(vector1, new Vector3(vector2.x, vector1.y, vector2.z));
	}
    
    public void Update(){
    	if (IsGoalDefined && plannedPath != null){
    		Color pathColor = new Color(pathDebugColor.r, pathDebugColor.g, pathDebugColor.b);
	    	//Debug.DrawLine(transform.position, plannedPath[0].transform.position, Color.cyan);
    		for (int i = 0; i < plannedPath.Count - 1; i++){
    			Debug.DrawLine(plannedPath[i].transform.position, plannedPath[i + 1].transform.position, pathColor);
	    	}
    		if (transits != null && transits.Count > 0){
	    		Debug.DrawLine(plannedPath[plannedPath.Count - 1].transform.position, transits[0].transform.position, pathDebugColor);
	    	} else {
		    	Debug.DrawLine(plannedPath[plannedPath.Count - 1].transform.position, Goal, pathDebugColor);
    		}
    	}
    }
}

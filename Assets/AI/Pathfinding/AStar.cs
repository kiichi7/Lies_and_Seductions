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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode : IComparable {
	/// <summary>
	/// The parent of the node.
	/// </summary>
	private AStar controller;

	/// <summary>
	/// The mesh node associated with this node in the pathfinding algorithm
	/// </summary>
	public NavmeshPolygon NavmeshPolygon;

	/// <summary>
	/// The parent of the node.
	/// </summary>
	public AStarNode Parent;
	
	/// <summary>
	/// The accumulative cost of the path until now.
	/// </summary>
	public float Cost;

	/// <summary>
	/// The estimated cost to the goal from here. It tries to estimate taking into consideration obstacles
	/// on the way.
	/// </summary>
	public float GoalEstimate {
		get {
			if (controller.GoalNode.Equals(this))
				return 0;

			Vector3 from = NavmeshPolygon.transform.position;
			return Vector3.Distance(from, controller.GoalPosition);
			//return controller.PathHeuristic.EstimatePathCost(from, controller.GoalPosition);
		}
	}

	/// <summary>
	/// The cost plus the estimated cost to the goal from here.
	/// </summary>
	public float TotalCost {
		get {
			return Cost + GoalEstimate;
		}
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="controller">Allow refernce to finder so nodes can access information about goal position and other properties.</param>
	/// <param name="parent">Store node's parent in order to rebuild path.</param>
	/// <param name="meshNode">Underlying mesh node with map information</param>
	/// <param name="cost">The accumulative cost until now</param>
	public AStarNode(AStar controller, AStarNode parent, NavmeshPolygon navmeshPolygon, float cost) {
		Parent = parent;
		NavmeshPolygon = navmeshPolygon;
		Cost = cost;
		this.controller = controller;
	}

	/// <summary>
	/// Gets all "feasible" successors nodes from the current node and adds them to the successor list.
	/// This method might present diferent neighboors according to environment. 
	/// </summary>
	/// <remarks>TODO: Currently, dynamic entities are not taken into account.</remarks>
	/// <param name="successors">List in which the successors will be added</param>
	public void GetSuccessors(List<AStarNode> successors) {
		for (int i = 0; i < NavmeshPolygon.GetPortals().Count; i++) {
			NavmeshPolygon neighbour = ((NavmeshEdge)NavmeshPolygon.GetPortals()[i]).GetOtherPolygon(NavmeshPolygon);
			if (Parent != null && neighbour.Equals(Parent.NavmeshPolygon))
				continue;

			//use distance from cetroid of current mesh to midpoint in the boundary, except if at current where
			//we use the actual original position of entity
			Vector3 fromPosition = NavmeshPolygon.Equals(controller.StartNode) ? controller.StartPosition : NavmeshPolygon.transform.position;

			//we can use midpoint or centroid Boundaries[i] MidPoint
			float currentCost = Vector3.Distance(fromPosition, ((NavmeshEdge)NavmeshPolygon.GetPortals()[i]).transform.position);

			AStarNode newNode = new AStarNode(controller, this, neighbour, Cost + currentCost);

			successors.Add(newNode);
		}
	}

	public override bool Equals(object obj) {
		if(obj is AStarNode) {
			//Debug.Log("Heap.Contains: AStarNode.Equals: " + NavmeshPolygon.name + " and " + ((AStarNode)obj).NavmeshPolygon.name + " = " + (NavmeshPolygon == ((AStarNode)obj).NavmeshPolygon));
			return NavmeshPolygon == ((AStarNode)obj).NavmeshPolygon;
		}
		//Debug.Log("AStarNode.Equals: not an AStarNode");
		return false;
	}

	public override int GetHashCode() {
		return base.GetHashCode();
	}

	public int CompareTo(object obj) {
		/*if (Equals(obj)){
			return 0;
		} else {*/
			int result = -TotalCost.CompareTo(((AStarNode)obj).TotalCost);
			//Debug.Log("Heap.Contains: Comparing " + NavmeshPolygon.name + " to " + ((AStarNode)obj).NavmeshPolygon.name + ": " + result);
			//Debug.Log("Heap.Contains: The costs were: " + TotalCost + " and " + ((AStarNode)obj).TotalCost);
			return result;
		//}
	}
}



/// <summary>
/// Class for performing A* pathfinding
/// </summary>
public sealed class AStar {
	public Vector3 StartPosition;
	
	public Vector3 GoalPosition;
	
	public AStarNode GoalNode;
	
	public AStarNode StartNode;

	//public IPathHeuristic PathHeuristic;
 
	/// <summary>
	/// Instantiate A* algorithm with given heuristic to estimate cost between two points.
	/// </summary>
	/// <param name="pathHeuristic"></param>
	public AStar(/*IPathHeuristic pathHeuristic*/) {
		//if (pathHeuristic == null) throw new ArgumentNullException("pathHeuristics");
		//PathHeuristic = pathHeuristic;
	}

	/// <summary>
	/// Finds the shortest path from the start node to the goal node
	/// </summary>
	/// <param name="startPosition">Start position</param>
	/// <param name="goalPosition">Goal position</param>
	/// <param name="startMeshNode">Start mesh node</param>
	/// <param name="goalMeshNode">Goal mesh node</param>
	public List<NavmeshPolygon> FindPath(Vector3 startPosition, Vector3 goalPosition, NavmeshPolygon startPolygon, NavmeshPolygon goalPolygon) {
		//Debug.Log("AStar.FindPath");
		//Debug.Log("startPosition == " + startPosition);
		//Debug.Log("goalPosition == " + goalPosition);
		StartPosition = startPosition;
		GoalPosition = goalPosition;

		GoalNode = new AStarNode(this, null, goalPolygon, 0);
		StartNode = new AStarNode(this, null, startPolygon, 0);

		Heap openList = new Heap();
		Heap closedList = new Heap();
		List<AStarNode> solution = new List<AStarNode>();
		List<AStarNode> successors = new List<AStarNode>();

		int printed = 0;
		openList.Add(StartNode);
		while (openList.Count > 0) {
			//Debug.Log("AStar:main loop");
			// Get the node with the lowest TotalSum
			AStarNode nodeCurrent = (AStarNode)openList.Pop();
			if (printed < 1000){
				//Debug.Log("Current polygon: " + nodeCurrent.NavmeshPolygon.name + " - " + nodeCurrent.TotalCost);
				printed++;
			}

			// If the node is the goal copy the path to the solution array
			if (GoalNode.Equals(nodeCurrent)) {
				//Debug.Log("AStar:finish loop");
				while (nodeCurrent != null) {
					solution.Insert(0, nodeCurrent);
					nodeCurrent = nodeCurrent.Parent;
				}

				//convert solution 
				//Debug.Log("Path found");
				return solution.ConvertAll(an => an.NavmeshPolygon);
			}

			// Get successors to the current node
			successors.Clear();
			nodeCurrent.GetSuccessors(successors);
			foreach (AStarNode nodeSuccessor in successors) {
				//Debug.Log("AStar:successor loop");
				// Test if the currect successor node is on the open list, if it is and
				// the TotalSum is higher, we will throw away the current successor.
				//Debug.Log("AStarNode nodeOpen");
				AStarNode nodeOpen;
				if (openList.Contains(nodeSuccessor)) {
					//Debug.Log("openList check: nodeSuccessor (" + nodeSuccessor.NavmeshPolygon.name + ") - " + nodeSuccessor.TotalCost);
					//Debug.Log("openList contains nodeSuccessor");
					//Debug.Log("nodeOpen = (AStarNode)openList[openList.IndexOf(nodeSuccessor)];");
					nodeOpen = (AStarNode)openList.GetEqual(nodeSuccessor);
					//Debug.Log("openList check: nodeOpen (" + nodeOpen.NavmeshPolygon.name + ") - " + nodeOpen.TotalCost);
					//Debug.Log("if ((nodeOpen != null) && (nodeSuccessor.TotalCost > nodeOpen.TotalCost))");
					if ((nodeOpen != null) && (nodeSuccessor.TotalCost > nodeOpen.TotalCost)){
						//Debug.Log("continue;");
						//Debug.Log("openList check: continued");
						continue;
					} else {
						//Debug.Log("openList check: not continued");
					}
				
				}
					
				// Test if the currect successor node is on the closed list, if it is and
				// the TotalSum is higher, we will throw away the current successor.
				//Debug.Log("AStarNode nodeClosed;");
				AStarNode nodeClosed;
				//Debug.Log("if (closedList.Contains(nodeSuccessor)) {");
				if (closedList.Contains(nodeSuccessor)) {
					//Debug.Log("closedList check: nodeSuccessor (" + nodeSuccessor.NavmeshPolygon.name + ") - " + nodeSuccessor.TotalCost);
					//Debug.Log("closedList contains nodeSuccessor");
					//Debug.Log("nodeClosed = (AStarNode)closedList[closedList.IndexOf(nodeSuccessor)];");
					nodeClosed = (AStarNode)closedList.GetEqual(nodeSuccessor);
					//Debug.Log("closedList check: nodeClosed (" + nodeClosed.NavmeshPolygon.name + ") - " + nodeClosed.TotalCost);
					//Debug.Log("if ((nodeClosed != null) && (nodeSuccessor.TotalCost > nodeClosed.TotalCost))");
					if ((nodeClosed != null) && (nodeSuccessor.TotalCost > nodeClosed.TotalCost)){
						//Debug.Log("continue;");
						continue;
					}
				}
				
				// Remove the old successor from the open list
				//Debug.Log("openList.Remove(nodeSuccessor);");
				openList.Remove(nodeSuccessor);

				// Remove the old successor from the closed list
				//Debug.Log("closedList.Remove(nodeSuccessor);");
				closedList.Remove(nodeSuccessor);

				// Add the current successor to the open list
				//Debug.Log("penList.Push(nodeSuccessor);");
				if (printed < 1000){
					//Debug.Log("Adding to openList: " + nodeSuccessor.NavmeshPolygon.name + " - " + nodeSuccessor.TotalCost);
				}
				openList.Push(nodeSuccessor);
				//Debug.Log("AStar:successor loop finished");
			}
			// Add the current node to the closed list
			//Debug.Log("closedList.Add(nodeCurrent);");
			closedList.Add(nodeCurrent);
			//Debug.Log("AStar:main loop finished");
		}
		Debug.Log("AStart.FindPath() Path not found.");
		return null;
	}
}
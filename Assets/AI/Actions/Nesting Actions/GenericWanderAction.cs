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

public class GenericWanderAction : AbstractNestingAction {

	private const float MINIMUM_IDLE_DURATION = 3.0f;
	private const float MAXIMUM_IDLE_DURATION = 10.0f;
	private const float MAXIMUM_DISTANCE_FROM_WAYPOINT = 0.3f;

	private Waypoint[] waypoints;

	public GenericWanderAction(GameObject actor, GameObject wanderWaypointParent) : base(actor){
		InitInteractionInfo(false, true);
		
		Component[] components = wanderWaypointParent.GetComponentsInChildren(typeof(Waypoint));
		waypoints = new Waypoint[components.Length];
		for (int i = 0; i < components.Length; i++){
			waypoints[i] = (Waypoint)components[i];
		}
	}
	
	protected override Action CreateDefaultAction(){
		Waypoint waypoint = waypoints[Random.Range(0, waypoints.Length)];
		return new IdleAction(actor, Random.Range(MINIMUM_IDLE_DURATION, MAXIMUM_IDLE_DURATION), waypoint.gameObject, MAXIMUM_DISTANCE_FROM_WAYPOINT, false);
	}
	
	protected override void UpdateFirstRound(){
		Waypoint waypoint = null;
		while (waypoint == null || waypoint.IsBlocked(false)){
			waypoint = waypoints[Random.Range(0, waypoints.Length)];
		}
		
		mover.JumpTo(waypoint.transform.position, waypoint.transform.rotation, false);
		QueueAction(new IdleAction(actor, Random.Range(MINIMUM_IDLE_DURATION, MAXIMUM_IDLE_DURATION), waypoint.gameObject, MAXIMUM_DISTANCE_FROM_WAYPOINT, false));
	}
	
}

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

public class StandAroundAction : AbstractNestingAction {
	
	private Waypoint[] possibleWaypoints;
	private float durationInSeconds;
	
	public StandAroundAction(GameObject actor, float durationInSeconds, string waypointGroupName) : base(actor){
		InitInteractionInfo(true, true);
		
		GameObject waypointGroup = GameObject.Find(waypointGroupName);
		Component[] components = waypointGroup.GetComponentsInChildren(typeof(Waypoint));
		possibleWaypoints = new Waypoint[components.Length];
		for (int i = 0; i < components.Length; i++){
			possibleWaypoints[i] = (Waypoint)components[i];
		}
		this.durationInSeconds = durationInSeconds;
	}
	
	protected override void UpdateFirstRound(){
		Waypoint targetWaypoint = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)];
		QueueAction(new IdleAction(actor, durationInSeconds, targetWaypoint.gameObject, DistanceConstants.WAYPOINT_RADIUS, true));
	}
}

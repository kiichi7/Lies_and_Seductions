/**********************************************************************
 *
 * CLASS DistantConstants
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

public class DistanceConstants : MonoBehaviour {
	
	//How far a character can reach by their arm
	public float armsReach = 1.0f;
	
	//The maximum distance from which people can talk to each other
	public float talkingMaxDistance = 1.5f;
	
	//The minimum distance from which people can talk to each other
	public float talkingMinDistance = 1.0f;
	
	//The distance from which a character can sit on a chair
	public float sitDownDistance = 1.0f;
	
	//The distance at which an NPC stops to see if the PC has something to say
	public float stopWhenPCApproachesDistance = 2.1f;
	
	//The distance at which a character is considered to have reached a waypoint
	public float waypointRadius = 0.3f;
	
	public float collisionLeewayDistance = 1.0f;
	
	public float hotSpotRange = 2.0f;
	
	//danceDistance???
	
	
	//The multiplier applied due to irragular scaling of the scene
	public float scalingMultiplier = 1.0f;
	
	private static DistanceConstants instance;
	
	public void Awake(){
		instance = this;
		enabled=false;
	}
	
	public static float ARMS_REACH{
		get {
			return instance.armsReach * instance.scalingMultiplier;
		}
	}
	
	public static float TALKING_MAX_DISTANCE{
		get {
			return instance.talkingMaxDistance * instance.scalingMultiplier;
		}
	}
	
	public static float TALKING_MIN_DISTANCE{
		get {
			return instance.talkingMinDistance * instance.scalingMultiplier;
		}
	}

	public static float SIT_DOWN_DISTANCE{
		get {
			return instance.sitDownDistance * instance.scalingMultiplier;
		}
	}
	
	public static float STOP_WHEN_PC_APPROACHES_DISTANCE {
		get {			
			return instance.stopWhenPCApproachesDistance * instance.scalingMultiplier;
		}
	}
	
	public static float WAYPOINT_RADIUS {
		get {
			return instance.waypointRadius * instance.scalingMultiplier;
		}
			
	}
	
	public static float COLLISION_LEEWAY_DISTANCE {
		get {
			return instance.collisionLeewayDistance * instance.scalingMultiplier;
		}
	}
	
	public static float HOT_SPOT_RANGE {
		get {
			return instance.hotSpotRange * instance.scalingMultiplier;
		}
	}
	
	public static float UNDEFINED {
		get {
			return 1.0f;
		}
	} 
		

}

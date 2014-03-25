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

public class Transit : MonoBehaviour {

	private const float DELAY_BEFORE_FORCE = 1.0f;

	public TransitWaypoint destinationWaypoint;
	public bool isElevatorDoor;
	
	private Queue transitQueue = new Queue();
	private float waitingStarted = 0.0f;

	private void AddToQueue(GameObject character){
		transitQueue.Enqueue(character);
		TryTransit();
	}

	private void TryTransit(){
		if (transitQueue.Count > 0){
			float timeWaited = Time.time - waitingStarted;
			Debug.Log("Transit.TryTransit(): Time waited=" + timeWaited + " since " + waitingStarted);
			if (!destinationWaypoint.IsBlocked(true)){
				if (!destinationWaypoint.IsBlocked(false) || timeWaited > DELAY_BEFORE_FORCE){
					GameObject character = (GameObject)transitQueue.Dequeue();
					FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_DOOR,character.transform.position);
					destinationWaypoint.TransitCharacterTo(character);
					PathPlanner pathPlanner = (PathPlanner)character.GetComponent("PathPlanner");
					pathPlanner.TransitEntered(this);
					if(character != CharacterManager.GetPC()) { // NPC
						if (isElevatorDoor){
							FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_ELEVATOR,character.transform.position);
						} else {
							FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_DOOR,character.transform.position);
						}
					}
					waitingStarted = Time.time;
				}
			} 
		}
	}

	public void OnTriggerEnter (Collider collider) {
		//Debug.Log("Transit.OnTriggerEnter");
		GameObject character = collider.gameObject;
		if(character == CharacterManager.GetPC()) {
			//FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_DOOR);
			if(destinationWaypoint.area.name.Equals("Disco") && GameTime.IsDiscoOpen() == false ) {
				FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_DOOR_LOCKED);
				//Debug.Log("Transit.OnTriggerEnter(): Disco is closed");
				return;
			} else if(destinationWaypoint.area.name.Equals("Restaurant") && GameTime.IsRestaurantOpen() == false) {
				FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_DOOR_LOCKED);
				//Debug.Log("Transit.OnTriggerEnter(): Restaurant is closed");
				return;	
			} else if (destinationWaypoint.area.name.Equals("Cabin")){
				return;
			} 
		}
		PathPlanner pathPlanner = (PathPlanner)character.GetComponent("PathPlanner");
		if (character == CharacterManager.GetPC() || pathPlanner.IsNextTransitPlanned(this)){
			waitingStarted = Time.time;
			AddToQueue(character);
		}
		//Debug.Log("TransitWaypoint.OnTriggerEnter done.");
	}
	
	public void Update(){
		TryTransit();
	}
	
	public Area GetDestinationArea(){
		return destinationWaypoint.GetArea();
	}
	

}

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
using System.Collections.Generic;

public class Area : MonoBehaviour {

	public new Light light;
	public float smallSpeechTextDistance = 4.0f;
	public float noSpeechTextDistance = 8.0f;
	public GameObject barkeep;
	public Transit[] transits = new Transit[5];
	public TransitWaypoint mainTransitWaypoint;
	public GameObject chairParent;
	public GameObject loungerParent;
	public GameObject deckChairParent;
	public GameObject wanderWaypointParent;
	public int maximumNumberOfGenericNPCs = 5;
	public Material genericNPCMaterial;
	public int farClipPlain = 100;
	//public bool enableWaterSkyCamera = false;
	
	GameObject[] allLights;
	
	public void Awake() {
		allLights = GameObject.FindGameObjectsWithTag("Light");
		enabled = false;
	}		
	
	private void SwitchLighting(){
		//Debug.Log("Area.SwitchLighting()");
		//GameObject[] allLights = GameObject.FindGameObjectsWithTag("Light");
		foreach (GameObject lightGO in allLights){
			Light light = lightGO.light;
			if (light == this.light){
				light.enabled = true;
			} else {
				light.enabled = false;
			}
		}
	}
	
	public float GetSmallSpeechTextDistance(){
		return smallSpeechTextDistance;
	}
	
	public float GetNoSpeechTextDistance(){
		return noSpeechTextDistance;
	}
	
	public float GetPerceptionDistance(){
		return GetNoSpeechTextDistance();
	}
	
	public float GetEasyPerceptionDistance(){
		return GetSmallSpeechTextDistance();
	}
	
	public GameObject GetBarkeep(){
		return barkeep;
	}
	
	public List<Transit> GetTransitListTo(Area targetArea, bool tryRecursive){
		foreach (Transit transit in transits){
			if (transit != null && transit.GetDestinationArea() == targetArea){
				List<Transit> transitList = new List<Transit>();
				transitList.Add(transit);
				return transitList;
			}
		}
		if (tryRecursive){
			foreach (Transit transit in transits) {
				if (transit != null){
					Area nextArea = transit.GetDestinationArea();
					List<Transit> recursiveList = nextArea.GetTransitListTo(targetArea, false);
					if (recursiveList != null){
						recursiveList.Insert(0, transit);
						return recursiveList;
					}
				}
			}
		}
		//Debug.Log("List of transits not found!");
		return null;
	}
	
	public void SwitchSounds(){
		//Debug.Log("SwitchSounds");
		//Debug.Log("SWITCHSOUNDS AREA: " + name);
		//FModManager.StopAll();
		if (name.Equals("Disco")){
			//Debug.Log("Area.SwitchSounds(): Disco sounds");
			FModManager.Cue(FModLies.MUSICCUE_LIES_NIGHTCLUB_MUZAK);
			RestaurantDoorSounds.LeftLobby();
			NightclubDoorSounds.LeftLobby();
			DeckDoorSounds.LeftLobby();
		} else if (name.Equals("Lobby")) {
			//Debug.Log("Area.SwitchSounds(): Lobby sounds");
			FModManager.Cue(FModLies.MUSICCUE_LIES_GRAMDE_SILENZIO_FAST);
			FModManager.StopEvent(FModLies.EVENTID_LIES_DECK_SEAWIND);
			FModManager.StopEvent(FModLies.EVENTID_LIES_RESTAURANT_CLATTER_OF_CUTLERY);
			RestaurantDoorSounds.EnteredLobby();
			NightclubDoorSounds.EnteredLobby();
			DeckDoorSounds.EnteredLobby();
		} else if (name.Equals("Deck")) {
			//Debug.Log("Area.SwitchSounds(): Deck sounds");
			FModManager.Cue(FModLies.MUSICCUE_LIES_GRAMDE_SILENZIO_FAST);
			RestaurantDoorSounds.LeftLobby();
			NightclubDoorSounds.LeftLobby();
			DeckDoorSounds.LeftLobby();
			DeckBackdoorSounds.LeftRestaurant();
			RestaurantBackdoorSounds.EnteredDeck();
			FModManager.StartEvent(FModLies.EVENTID_LIES_DECK_SEAWIND);
			FModManager.StopEvent(FModLies.EVENTID_LIES_RESTAURANT_CLATTER_OF_CUTLERY);
		} else if (name.Equals("Restaurant")) {
			//Debug.Log("Area.SwitchSounds(): Restaurant sounds");
			//Debug.Log("RestaurantDoorSounds.LeftLobby");
			RestaurantDoorSounds.LeftLobby();
			//Debug.Log("NightcludDoorSounds.LeftLobby");
			NightclubDoorSounds.LeftLobby();
			//Debug.Log("DeckDoorSounds.LeftLobby");
			DeckDoorSounds.LeftLobby();
			//Debug.Log("RestaurantBackdoorSounds.LeftDeck");
			RestaurantBackdoorSounds.LeftDeck();
			//Debug.Log("DeckBackdoorSounds.EnteredRestaurant");
			DeckBackdoorSounds.EnteredRestaurant();
			//Debug.Log("FModManager.StopEvent(FModLies.EVENTID_LIES_DECK_SEAWIND)");
			FModManager.StopEvent(FModLies.EVENTID_LIES_DECK_SEAWIND);
			//Debug.Log("FModManager.StartEvent(FModLies.EVENTID_LIES_RESTAURANT_CLATTER_OF_CUTLERY)");
			FModManager.StartEvent(FModLies.EVENTID_LIES_RESTAURANT_CLATTER_OF_CUTLERY);
			//Debug.Log("FModManager.Cue(FModLies.MUSICCUE_LIES_RESTAURANT_MUZAK)");
			FModManager.Cue(FModLies.MUSICCUE_LIES_RESTAURANT_MUZAK);
			//Debug.Log("Restaurant sounds done.");
		}
	}
	
	public void Entered(GameObject enterer){
		Debug.Log("Area.Entered(" + enterer.name + ")");
		if (enterer == CharacterManager.GetPC()){
			AnimateSky.AreaChanged();
			SwitchLighting();
			SwitchSounds();
			CharacterManager.AreaChanged(this);
			CameraController.SetArea(this);
		}
		else {
			CharacterState state = enterer.GetComponent(typeof(CharacterState)) as CharacterState;
			state.AreaChanged();
		}
	}
	
	public void JumpInto(GameObject character){
		mainTransitWaypoint.TransitCharacterTo(character);
	}
}

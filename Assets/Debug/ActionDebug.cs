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

public class ActionDebug : MonoBehaviour {

	private static Hashtable openActions;
	
	public void Start(){
		openActions = new Hashtable();
		foreach (GameObject npc in CharacterManager.GetMajorNPCs()){
			ArrayList npcOpenActions = new ArrayList();
			openActions.Add(npc, npcOpenActions);
		}
		ArrayList pcOpenActions = new ArrayList();
		openActions.Add(CharacterManager.GetPC(), pcOpenActions);
	}
	
	public static void LogActionStarted(GameObject actor, Action action){
		if (openActions != null){
			ArrayList actorOpenActions = (ArrayList)openActions[actor];
			if (actorOpenActions != null){
				actorOpenActions.Add(action);
			}
		}
	}
	
	public static void LogActionEnded(GameObject actor, Action action){
		ArrayList actorOpenActions = (ArrayList)openActions[actor];
		if (actorOpenActions != null){
			actorOpenActions.Remove(action);
		}
	}
	
	private static void PrintActorReport(GameObject actor){
		Debug.Log("-------- " + actor.name + " --------");
		foreach (Action action in (ArrayList)openActions[actor]){
			Debug.Log(action);
		}
	}
	
	private static void PrintReport(){
		Debug.Log("-------- Action Debug Report: Currently Open Actions --------");
		PrintActorReport(CharacterManager.GetPC());
		foreach (GameObject npc in CharacterManager.GetMajorNPCs()){
			PrintActorReport(npc);
		}
		Debug.Log("-------- Action Debug Report Ended --------");
	}
	
	public void Update(){
		if (Input.GetKeyDown("r")){
			PrintReport();
		}
	}
}

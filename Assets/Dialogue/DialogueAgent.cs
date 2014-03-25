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

public class DialogueAgent : MonoBehaviour {

	private static float MAXIMUM_TALKING_DISTANCE = 6.0f;
	private static float MIN_CYCLE_LENGTH_IN_SECONDS = 1.0f;
	private static float MAX_CYCLE_LENGTH_IN_SECONDS = 30.0f;
	//private static float MAX_CYCLE_LENGTH_IN_SECONDS = 6.0f;
	
	private float nextRoundTimeInSeconds = 0.0f;
	private ArrayList dialogueControllers;
	
	private static DialogueAgent instance;

	private class Group {
		
		private Vector3 position;
		private ArrayList npcs;
		
		public Group(GameObject centralNPC){
			npcs = new ArrayList();
			npcs.Add(centralNPC);
			position = centralNPC.transform.position;
		}
		
		public ArrayList GetNPCs(){
			return npcs;
		}
		
		public bool CanBeAdded(GameObject npc){
			return Vector3.Distance(position, npc.transform.position) < MAXIMUM_TALKING_DISTANCE;
		}
		
		public void AddNPC(GameObject npc){
			npcs.Add(npc);
		}
	}
	
	public void Awake(){
		instance = this;
		dialogueControllers = new ArrayList();
	}

	private static ArrayList GetAvailableNPCs(ArrayList majorNPCs){
		ArrayList availableNPCs = new ArrayList();
		foreach (GameObject majorNPC in majorNPCs){
			ActionRunner actionRunner = majorNPC.GetComponent(typeof(ActionRunner)) as ActionRunner;
			if (actionRunner.CanStartDialogue(false)){
				availableNPCs.Add(majorNPC);
			}
		}
		return availableNPCs;
	}
		

	private static Group CreateGroup(ArrayList majorNPCs){
		GameObject centralNPC = (GameObject)majorNPCs[0];
		Group group = new Group(centralNPC);
		foreach (GameObject majorNPC in majorNPCs){
			if (group.CanBeAdded(majorNPC)){
				group.AddNPC(majorNPC);
			}
		}
		foreach (GameObject npcInGroup in group.GetNPCs()){
			majorNPCs.Remove(npcInGroup);
		}
		return group;
	}
	
	private static ArrayList SplitIntoGroups(GameObject[] majorNPCArray){
		ArrayList majorNPCs = new ArrayList(majorNPCArray);
		ArrayList availableNPCs = GetAvailableNPCs(majorNPCs);	
		ArrayList groups = new ArrayList();
		while (availableNPCs.Count > 0){
			groups.Add(CreateGroup(availableNPCs));
		}
		return groups;
	}

	private static ArrayList FindPossibleConversations(ArrayList groups){
		ArrayList conversations = AILoader.GetConversations();
		ArrayList possibleConversations = new ArrayList();
		foreach (Conversation conversation in conversations){		
			foreach (Group group in groups){
				if (conversation.CanBeHeldBy(group.GetNPCs())){
					possibleConversations.Add(conversation);
				}
			}
		}
		return possibleConversations;
	}
	
	private static void BeginConversation(Conversation conversation, string situation){
		Debug.Log("DialogueAgent.BeginningConversation(): " + conversation.GetName() + ", situation : " + situation);
		instance.dialogueControllers.Add(conversation.CreateDialogueController(situation));
	}
	
	public static void RequestDialogue(string dialogueName, GameObject requester, GameObject target, string situation){
		Debug.Log("DialogueAgent.RequestDialogue(): " + requester.name + " requests dialogue with " + target.name + " in situation " + situation);
		if (((ActionRunner)target.GetComponent(typeof(ActionRunner))).CanStartDialogue(requester == CharacterManager.GetPC())){
			Debug.Log("DialogueAgent.RequestDialogue(): Accepted");
			BeginConversation(AILoader.GetConversation(dialogueName), situation);
		}
	}
		
	public void Update(){
		ArrayList copyList = new ArrayList(dialogueControllers);
		foreach (DialogueController dialogueController in copyList){
			bool finished = dialogueController.UpdateDialogueController();
			if (finished){
				dialogueControllers.Remove(dialogueController);
			}
		}
		if (GameTime.GetRealTimeSecondsPassed() > nextRoundTimeInSeconds){
			GameObject[] majorNPCs = CharacterManager.GetMajorNPCs();
			ArrayList groups = SplitIntoGroups(majorNPCs);
			ArrayList possibleConversations = FindPossibleConversations(groups);
			if (possibleConversations.Count > 0){
				Conversation conversation = (Conversation)possibleConversations[Random.Range(0, possibleConversations.Count -1)];
				BeginConversation(conversation, "normal");
			}
			nextRoundTimeInSeconds += Random.Range(0.0f, MAX_CYCLE_LENGTH_IN_SECONDS);
		}	
	}
}

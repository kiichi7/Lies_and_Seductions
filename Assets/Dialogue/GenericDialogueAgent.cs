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

public class GenericDialogueAgent : MonoBehaviour {

	private const int NUMBER_OF_SPEAKERS = 2;
	private const float MAX_CYCLE_LENGTH_IN_SECONDS = 10.0f;

	private float nextRoundTimeInSeconds = 0.0f;
	private ArrayList dialogueControllers = new ArrayList();

	static private GenericDialogueAgent instance;

	static public void ResetDialogues() {
		ArrayList copyList = new ArrayList(instance.dialogueControllers);
		foreach (GenericDialogueController dialogueController in copyList){
			instance.dialogueControllers.Remove(dialogueController);
		}
	}

	public void Awake() {
		instance = this;
	}

	public void Update(){
		if (!Pause.IsPaused()){
			
			ArrayList copyList = new ArrayList(dialogueControllers);
			foreach (GenericDialogueController dialogueController in copyList){
				//Debug.Log("GenericDialogueAgent.Update: iterating");
				bool finished = dialogueController.UpdateGenericDialogueController();
				if (finished){
					//Debug.Log("--- removing dialogue controller");
					dialogueControllers.Remove(dialogueController);
				}
				
			}			
			
			if (GameTime.GetRealTimeSecondsPassed() > nextRoundTimeInSeconds){
				GameObject[] genericNPCs = CharacterManager.GetGenericNPCs();
				ArrayList potentialSpeakers = new ArrayList();
				foreach (GameObject genericNPC in genericNPCs){
					ActionRunner routineCreator = (ActionRunner)genericNPC.GetComponent("ActionRunner");
					if (routineCreator.CanStartDialogue(false)){
						potentialSpeakers.Add(genericNPC);
					}
				}
				if (potentialSpeakers.Count >= NUMBER_OF_SPEAKERS){
					GameObject[] speakers = new GameObject[NUMBER_OF_SPEAKERS];
					for (int i = 0; i < NUMBER_OF_SPEAKERS; i++){
						GameObject speaker = (GameObject)potentialSpeakers[Random.Range(0, potentialSpeakers.Count)];
						speakers[i] = speaker;
						potentialSpeakers.Remove(speaker);
					}
					dialogueControllers.Add(new GenericDialogueController(speakers));
					nextRoundTimeInSeconds += Random.Range(0.0f, MAX_CYCLE_LENGTH_IN_SECONDS);
				}
			}
			

		}
	}
}

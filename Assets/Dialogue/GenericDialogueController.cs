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

public class GenericDialogueController {

	private const float LINE_DURATION = 3.0f;
	private const int MINIMUM_NUMBER_OF_LINES = 3;
	private const int MAXIMUM_NUMBER_OF_LINES = 15;
	
	private GenericDialogueAction[] speakers;
	private int currentSpeakerIndex;
	private int numberOfLines;
	private int linesSpoken;
	private float lastLineTime;
	
	public GenericDialogueController(GameObject[] speakerGOs){
		speakers = new GenericDialogueAction[speakerGOs.Length];
		for (int i = 0; i < speakerGOs.Length; i++){
			GameObject speakerGO = speakerGOs[i];
			GameObject otherSpeakerGO = speakerGOs[0];
			if (otherSpeakerGO == speakerGO){
				otherSpeakerGO = speakerGOs[1];
			}
			GenericDialogueAction speaker = new GenericDialogueAction(speakerGO, otherSpeakerGO);
			speakers[i] = speaker;
			ActionRunner actionRunner = (ActionRunner)speakerGO.GetComponent("ActionRunner");
			actionRunner.ResetRoutine(speaker, false);
			numberOfLines = Random.Range(MINIMUM_NUMBER_OF_LINES, MAXIMUM_NUMBER_OF_LINES);
		}
		currentSpeakerIndex = 0;
		lastLineTime = -10.0f;
	}
	
	public bool UpdateGenericDialogueController(){
		if (ReadyToSayNextLine()){
			if (linesSpoken < numberOfLines){
				SayNextLine();
				linesSpoken++;
				return false;
			} else {
				EndDialogue();
				return true;
			}
		} else {
			return false;
		}
	}
	
	private bool ReadyToSayNextLine(){
		foreach (GenericDialogueAction speaker in speakers){
			if (!speaker.IsReady()){
				return false;
			}
		}
		return Time.time > lastLineTime + LINE_DURATION;
	}
	
	private void SayNextLine(){
		speakers[currentSpeakerIndex].EndLine();
		currentSpeakerIndex = (currentSpeakerIndex + 1) % speakers.Length;
		speakers[currentSpeakerIndex].SayLine();
		lastLineTime = Time.time;
	}
	
	private void EndDialogue(){
		//Debug.Log("GenericDialogueController.EndDialogue()");
		speakers[currentSpeakerIndex].EndLine();
		foreach (GenericDialogueAction speaker in speakers){
			speaker.EndDialogue();
		}
	}
}

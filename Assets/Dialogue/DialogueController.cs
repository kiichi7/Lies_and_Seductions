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

public class DialogueController {

	private DialogueAction[] speakers;
	private DialogueAction pc;
	private string situation;
	private ArrayList speakerMemories;

	private Line currentLine;
	private Line nextLine;
	private Line goodbyeLine;
	private DialogueAction currentSpeaker;
	private DialogueAction nextSpeaker;
	
	private bool started;
	private bool firstLineSaid;
	
	
	public DialogueController(RootLine rootLine, GameObject[] speakerGOs, GameObject pcGO, string situation){
		currentLine = rootLine;
		speakers = new DialogueAction[speakerGOs.Length];
		speakerMemories = new ArrayList();
		for (int i = 0; i < speakerGOs.Length; i++){
			GameObject speakerGO = speakerGOs[i]; 
			bool isPC = speakerGO == pcGO;
			if(!isPC) {
				speakerMemories.Add(speakerGOs[i].GetComponent("ImpressionMemory"));
			}			
			GameObject otherSpeakerGO = speakerGOs[0];
			if (otherSpeakerGO == speakerGO){
				otherSpeakerGO = speakerGOs[1];
			}
			DialogueAction speaker = new DialogueAction(speakerGO, otherSpeakerGO.name, isPC);
			speakers[i] = speaker;
			((ActionRunner)speakerGO.GetComponent("ActionRunner")).ResetRoutine(speaker, false);
			if (isPC){
				pc = speaker;
				goodbyeLine = GetGoodbyeLine(otherSpeakerGO);
			}
		}
		
		if(pc != null) {
			SendActionStarted();
		}
		
		this.situation = situation;
		//InitLookingDirections();
		currentSpeaker = null;
		started = false;
		firstLineSaid = false;
	}
	
	private Line GetGoodbyeLine(GameObject otherSpeakerGO){
		Conversation conversation = AILoader.GetConversation(otherSpeakerGO.name + ": exit");
		Line rootLine = conversation.GetRootLine();
		return (Line)rootLine.GetResponses()[0];
	}
	
	public void SendActionStarted() {
		Debug.Log("DialogueController.SendActionStarted()");
		foreach(ImpressionMemory memory in speakerMemories) {
			memory.SendActionStarted();	
		}
	}
	
	public void SendDisplayAttitudeChange() {
		Debug.Log("DialogueController.SendDisplayAttitudeChange()");
		foreach(ImpressionMemory memory in speakerMemories) {
			memory.SendDisplayAttitudeChange();	
		}
	}
	
	public string GetSituation(){
		return situation;
	}

	/*
	/ Return true if the dialogue has ended, and this method doesn't need to be called again.
	*/
	public bool UpdateDialogueController(){
		if (!started){
			started = true;
			return AdvanceToNextLine();
		}
		if (HasBeenInterrupted()){
			return true;
		} else {
			InterruptLineIfNeeded();
			MirrorIfNeeded();
			if (ReadyToSayNextLine()){
				FixFacing();
				bool noLineLeft = SayNextLine();
				if (noLineLeft){
					return true;
				}
				MirrorIfNeeded();
				return AdvanceToNextLine();
			} else {
				return false;
			}
		}
	}
	
	private void ComplainAboutInterruption(DialogueAction interruptedSpeaker){
		ArrayList otherSpeakers = new ArrayList(speakers);
		otherSpeakers.Remove(interruptedSpeaker);
		DialogueAction complainer = (DialogueAction)otherSpeakers[(int)Random.Range(0, otherSpeakers.Count)];
		complainer.StartQuickReaction(new OneLinerAction(complainer.GetActor(), "interrupted", CharacterManager.GetPC()));
	}
	
	private bool HasBeenInterrupted(){
		foreach (DialogueAction speaker in speakers){
			if (speaker.HasBeenInterrupted()){
				if (firstLineSaid){
					ComplainAboutInterruption(speaker);
				}
				EndDialogue();
				return true;
			}
		}
		return false;
	}
	
	private void InterruptLineIfNeeded(){
		if (pc != null && Input.GetKeyDown("space")){
			currentSpeaker.InterruptLine();
		}
	}
	
	private void MirrorIfNeeded(){
		if (pc != null){
			float leftRight = Input.GetAxis("PC Left Right");
			if (leftRight < -0.1f){
				CameraController.SetMirror(true);
			} else if (leftRight > 0.1f){
				CameraController.SetMirror(false);
			}
		}
	}
	
	private bool ReadyToSayNextLine(){
		if (pc != null && nextSpeaker == pc && nextLine == null){
			PollGUI();
			return nextLine != null;
		} else {
			bool currentSpeakerReady;
			if (currentSpeaker == null){
				currentSpeakerReady = true;
			} else {
				currentSpeakerReady = currentSpeaker.IsReady();
			}
			bool nextSpeakerReady;
			if (nextSpeaker == null){
				nextSpeakerReady = true;
			} else {
				nextSpeakerReady = nextSpeaker.IsReady();
			}
			return currentSpeakerReady && nextSpeakerReady;
		}
	}
	
	private void FixFacing(){
		foreach (DialogueAction speaker in speakers){
			speaker.FixFacing();
		}
	}
	
	private void PollGUI(){
		nextLine = DialogueGUI.GetSelectedPCLine();
		if (nextLine != null){
			nextSpeaker.PrepareLine(nextLine);
		}
	}
	
	private bool SayNextLine(){
		if (nextLine == null){
		Debug.Log("============================= NPC Dialogue is over =========================================");
EndDialogue();
			return true;
		}
		if (currentSpeaker != null){
			currentSpeaker.InterruptLine();
		}
		currentLine = nextLine;
		currentSpeaker = nextSpeaker;
		nextLine = null;
		nextSpeaker = null;
		//Debug.Log(currentSpeaker.GetActor().name + " says: " + currentLine.GetText().Replace("\n", "\\n"));
		bool npcNpcDialogue=false;
		if (pc == null) {
			npcNpcDialogue=true;
		}
		currentSpeaker.SayLine(npcNpcDialogue);
		firstLineSaid = true;
		currentLine.PerformConsequence(this, currentSpeaker.GetActor());
		if (pc != null){
			MoveCamera();
		}
		return false;
	}
	
	private void MoveCamera(){
		DialogueAction npc;
		if (currentSpeaker != pc){
			npc = currentSpeaker;
		} else if (speakers[0] == pc){
			npc = speakers[1];
		} else {
			npc = speakers[0];
		}
		CameraController.SetCameraAngle(currentLine.GetCameraAngle(), pc.GetActor(), npc.GetActor(), false);
	}
	
	private bool AdvanceToNextLine(){
		SpeakerType responseSpeakerType = currentLine.GetResponseSpeakerType();
		switch (responseSpeakerType){
		case SpeakerType.PC:
			return AdvanceToNextLinePC();
		case SpeakerType.NPC:
			return AdvanceToNextLineNPC();
		default:
			return false;
		}
	}
	
	private static ArrayList GetAvailableResponses(Line currentLine){
		ArrayList responses = currentLine.GetResponses();
		ArrayList availableResponses = new ArrayList();
		foreach (Line response in responses){
			if (response.IsPrerequisiteMet()){
				availableResponses.Add(new VariantLine(response));
			}
		}
		return availableResponses;
	}
	
	private bool AdvanceToNextLinePC(){
		ArrayList availableResponses = GetAvailableResponses(currentLine);
		if (availableResponses.Count == 0){
			EndDialogue();
			return true;
		} else if (availableResponses.Count == 1){
			nextLine = (Line)availableResponses[0];
			nextSpeaker = pc;
			nextSpeaker.PrepareLine(nextLine);
			return false;
		} else {
			availableResponses.Add(new VariantLine(goodbyeLine));
			DialogueGUI.AskNextPCLine(availableResponses);
			nextSpeaker = pc;
			return false;
		}
	}
	
	private bool AdvanceToNextLineNPC(){
		ArrayList availableResponses = GetAvailableResponses(currentLine);
		if (availableResponses.Count == 0){
			//Debug.Log("============================= NPC Dialogue should be over =========================================");
			return false;
		} else {
			nextLine = (Line)availableResponses[Random.Range(0, availableResponses.Count)];
			foreach (DialogueAction speaker in speakers){
				if (speaker.IsName(nextLine.GetSpeakerName())){
					nextSpeaker = speaker;
				}
			}
			nextSpeaker.PrepareLine(nextLine);
			return false;
		}
	}
			
		
	private Vector3 GetCenterVector(Vector3[] vectors){
		float maxX = -1000000f;
		float minX = 1000000f;
		float maxY = -1000000f;
		float minY = 1000000f;
		float maxZ = -1000000f;
		float minZ = 1000000f;
		foreach (Vector3 vector in vectors){
			if (vector.x > maxX){
				maxX = vector.x;
			}
			if (vector.x < minX){
				minX = vector.x;
			}
			if (vector.y > maxY){
				maxY = vector.y;
			}
			if (vector.y < minY){
				minY = vector.y;
			}
			if (vector.z > maxZ){
				maxZ = vector.z;
			}
			if (vector.z < minZ){
				minZ = vector.z;
			}
		}
		return new Vector3((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2);
	}
	
	/*private void InitLookingDirections(){
		Vector3[] positions = new Vector3[speakers.Length];
		for (int i = 0; i < speakers.Length; i++){
			positions[i] = speakers[i].GetActor().transform.position;
		}
		Vector3 averagePosition = GetCenterVector(positions);
		foreach (DialogueAction speaker in speakers){
			speaker.LookAt(averagePosition);
		}
	}*/
	
	private void EndDialogue(){
		foreach (DialogueAction speaker in speakers){
			speaker.EndDialogue(pc != null);
		}
		if (pc != null){
			CameraController.SetDefaultCameraAngle();
			SendDisplayAttitudeChange();
			DialogueGUI.Dismiss();	
		}
	}
	
	public void SetConsequenceAction(Action consequenceAction, bool afterDialogue){
		GameObject consequenceActor = consequenceAction.GetActor();
		foreach (DialogueAction dialogueAction in speakers){
			GameObject dialogueActor = dialogueAction.GetActor();
			if (dialogueActor == consequenceActor){
				dialogueAction.SetConsequenceAction(consequenceAction, afterDialogue);
			}
		}
	}
}

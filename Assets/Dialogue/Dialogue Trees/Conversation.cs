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
using System.Xml;

public class Conversation {

	//private string name;
	private ArrayList participants;
	private RootLine rootLine;
	private DialogueController currentController;
	private PersistentConversation persistentConversation;

	public Conversation(XmlElement element, LineCatalogue catalogue){
		persistentConversation = new PersistentConversation();
		persistentConversation.name = element.GetAttribute("name");
		
		if(SaveLoad.SaveExits()) {
			PersistentConversation tmp = (PersistentConversation)SaveLoad.ReadSaveFile(PersistentConversation.GetSaveFileName(persistentConversation.name));
			if(tmp != null) {
				persistentConversation = tmp;	
			}
			else {
				SaveLoad.ResetSave();	
			}
			
		}
		
		//Debug.Log("Conversation.Conversation() name=" + persistentConversation.name );
		participants = new ArrayList();
		XmlNodeList participantNodeList = element.GetElementsByTagName("participant");
		foreach (XmlElement participantElement in participantNodeList){
			string participantName = participantElement.GetAttribute("name");
			participants.Add(CharacterManager.GetCharacter(participantName));
		}
		
		XmlElement rootLineElement = (XmlElement)element.GetElementsByTagName("root_line").Item(0);
		rootLine = new RootLine(rootLineElement, catalogue, this);
		rootLine.ConnectLinks(catalogue);
		currentController = null;
	}
	
	public string GetName(){
		return persistentConversation.name;
	}
	
	public RootLine GetRootLine(){
		return rootLine;
	}
	
	public string GetSituation(){
		//if (currentController == null){
		//	Debug.Log(name + ": currentController == null");
		//}
		return currentController.GetSituation();
	}
	
	public VariableSet GetVariableSet(){
		return persistentConversation.variableSet;
	}
	
	public bool CanBeHeldBy(ArrayList npcs){
		if (participants.Count == 1){
			return false;
		} else {
			foreach (GameObject participant in participants){
				if (!npcs.Contains(participant)){
					return false;
				}
			}
			return true;
		}
	}
	
	public DialogueController CreateDialogueController(string situation){
		GameObject pc;
		if (participants.Contains(CharacterManager.GetPC())){
			pc = CharacterManager.GetPC();
		} else {
			pc = null;
		}
		//Debug.Log("Creating DialogueController for: " + persistentConversation.name);
		currentController = new DialogueController(rootLine, (GameObject[])participants.ToArray(typeof(GameObject)), pc, situation);
		return currentController;
	}
	
	public void Save() {
		persistentConversation.Save();	
	}
}

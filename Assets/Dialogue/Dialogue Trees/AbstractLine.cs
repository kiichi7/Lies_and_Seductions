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
using System;

public abstract class AbstractLine : Line {

	protected SpeakerType responseSpeakerType;
	protected ArrayList responses;
	private bool haveConnectedLinks;
	
	public AbstractLine(XmlElement element, LineCatalogue catalogue, Conversation conversation){
		responses = new ArrayList();
		int lineId = Int32.Parse(element.GetAttribute("id"));
		catalogue.AddLine(lineId, this);
		string responseSpeakerTypeString = element.GetAttribute("response_speaker_type");
		if (responseSpeakerTypeString.Equals("PC")){
			responseSpeakerType = SpeakerType.PC;
		} else if (responseSpeakerTypeString.Equals("NPC")){
			responseSpeakerType = SpeakerType.NPC;
		}
		XmlNodeList nodeList = element.ChildNodes;
		for (int i = 0; i < nodeList.Count; i++){
			XmlElement responseElement = (XmlElement)nodeList.Item(i);
			//Debug.Log("AbstractLine.AbstractLine(): responceElement=" + responseElement.ToString());
			if (responseElement.Name.Equals("regular_line")){
				responses.Add(new RegularLine(responseElement, catalogue, conversation));
			} else {
				responses.Add(new Link(responseElement, catalogue));
			}
		}
		haveConnectedLinks = false;
	}
	
	public SpeakerType GetResponseSpeakerType(){
		return responseSpeakerType;
	}
	
	public ArrayList GetResponses(){
		return responses;
	}
	
	public ArrayList ConnectLinks(LineCatalogue catalogue){
		if (!haveConnectedLinks){
			ArrayList originalResponses = new ArrayList(responses);
			for (int i = 0; i < originalResponses.Count; i++){
				//Debug.Log("ConnectLinks: " + GetText() + ": Iterating: " + i);
				LineInLoading response = (LineInLoading)originalResponses[i];
				//Debug.Log("ConnectLinks: " + GetText() + ": response = " + response);
				ArrayList replacingList = response.ConnectLinks(catalogue);
				//Debug.Log("ConnectLinks: " + GetText() + ": replacingList size: " + replacingList.Count);
				int index = responses.IndexOf(response);
				//Debug.Log("ConnectLinks: " + GetText() + ": index to remove at = " + index);
				responses.RemoveAt(index);
				responses.InsertRange(index, replacingList);
			}
			haveConnectedLinks = true;
		}
		ArrayList ownList = new ArrayList();
		ownList.Add(this);
		return ownList;
	}
	
	public bool IsLink(){
		return false;
	}
	
	public abstract string GetText();
	
	public abstract string GetSpeakerName();
	
	public abstract bool IsPrerequisiteMet();
	
	public abstract void PerformConsequence(DialogueController dialogueController, GameObject actor);
	
	public abstract Emotion GetEmotion();
	
	public abstract CameraAngle GetCameraAngle();
	
	public abstract string GetId();
}

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

public class RegularLine : AbstractLine {

	private string text;
	private string speakerName;
	private Prerequisite prerequisite;
	private Consequence consequence;
	private CameraAngle cameraAngle;
	private Emotion emotion;
	private string id;
	
	public RegularLine(XmlElement element, LineCatalogue catalogue, Conversation conversation) : base (element, catalogue, conversation){
		text = element.GetAttribute("text");
		speakerName = element.GetAttribute("speaker");
		if (speakerName.Equals("")){
			speakerName = "Abby";
		}
		prerequisite = new Prerequisite(element.GetAttribute("prerequisite"), speakerName, conversation);
		consequence = new Consequence(element.GetAttribute("consequence"), speakerName, conversation);
		cameraAngle = CameraAngle.GetCameraAngle(element.GetAttribute("camera_angle"));
		emotion = Emotion.GetEmotion(element.GetAttribute("expression"));
		id = element.GetAttribute("id");
	}
	
	public override string GetText(){
		return text;
	}
	
	public override string GetSpeakerName(){
		return speakerName;
	}
	
	public override bool IsPrerequisiteMet(){
		return prerequisite.GetValue();
	}
	
	public override void PerformConsequence(DialogueController dialogueController, GameObject actor){
		//Debug.Log("Performing consequence of: \"" + text + "\"");
		consequence.Perform(dialogueController, actor);
	}
	
	public override CameraAngle GetCameraAngle(){
		return cameraAngle;
	}
	
	public override Emotion GetEmotion(){
		return emotion;
	}
	
	public override string GetId() {
		return id;	
	}
}

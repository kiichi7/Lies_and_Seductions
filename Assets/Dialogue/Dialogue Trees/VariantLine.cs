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

public class VariantLine : Line {

	private Line originalLine;
	private string selectedText;
	private string id;

	public VariantLine(Line originalLine){
		this.originalLine = originalLine;
		string originalText = originalLine.GetText();
		ArrayList alternativeTexts = new ArrayList();
		while (originalText.Contains("|")){
			int index = originalText.IndexOf("|");
			alternativeTexts.Add(originalText.Substring(0, index));
			originalText = originalText.Substring(index + 1, originalText.Length - index - 1);
		}
		alternativeTexts.Add(originalText);
		int selectionIndex = Random.Range(0, alternativeTexts.Count);
		id = originalLine.GetId() + "_" + selectionIndex.ToString();
		selectedText = (string)alternativeTexts[selectionIndex];
	}
	
	public string GetText(){
		return selectedText;
	}
	
	public string GetSpeakerName(){
		return originalLine.GetSpeakerName();
	}	
	
	public SpeakerType GetResponseSpeakerType(){
		return originalLine.GetResponseSpeakerType();
	}
	
	public ArrayList GetResponses(){
		return originalLine.GetResponses();
	}
	
	public bool IsPrerequisiteMet(){
		return originalLine.IsPrerequisiteMet();
	}
	
	public void PerformConsequence(DialogueController dialogueController, GameObject actor){
		originalLine.PerformConsequence(dialogueController, actor);
	}
	
	public Emotion GetEmotion(){
		return originalLine.GetEmotion();
	}
	
	public CameraAngle GetCameraAngle(){
		return originalLine.GetCameraAngle();
	}
	
	public ArrayList ConnectLinks(LineCatalogue catalogue){
		Debug.LogError("VariantLine.ConnectLinks called!");
		return null;
	}
	
	public bool IsLink(){
		return false;
	}
	
	public string GetId() {
		return id;	
	}
}

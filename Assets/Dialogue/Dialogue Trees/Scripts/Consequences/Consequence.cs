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

public class Consequence {
	
	private ArrayList consequenceNodes;
	
	public Consequence(string code, string defaultNPCName, Conversation conversation){
		code = CodeInterpreter.RemoveComments(code);
		if (code.Equals("")){
			consequenceNodes = null;
		} else {
			try {
				consequenceNodes = ConsequenceInterpreter.Interpret(code, defaultNPCName, conversation);
			} catch (InvalidScriptException e) {
				e.PrintMessage();
				consequenceNodes = null;
			}
		}
	}
	
	public void Perform(DialogueController dialogueController, GameObject actor){
		if (consequenceNodes != null){
			foreach (ConsequenceNode consequenceNode in consequenceNodes){
				//Debug.Log("Performing consequence node.");
				consequenceNode.Perform(dialogueController, actor);
			}
		}
	}
}


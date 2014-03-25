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

public class SubtractFromInteger : ConsequenceNode {

	private string variableName;
	private VariableSet variableSet;
	private NumericOperand operand;

	public SubtractFromInteger(string variableName, string npcName, NumericOperand operand, Conversation conversation){
		this.variableName = variableName;
		if (npcName != null){
			GameObject npc = CharacterManager.GetCharacter(npcName);
			variableSet = ((CharacterState)npc.GetComponent("CharacterState")).GetVariableSet();
		} else {
			variableSet = conversation.GetVariableSet();
		}
		this.operand = operand;
	}	
	
	public void Perform(DialogueController dialogueController, GameObject actor){
		variableSet.SetIntegerVariableValue(variableName, variableSet.GetIntegerVariableValue(variableName) - (int)operand.GetValue());
	}
}

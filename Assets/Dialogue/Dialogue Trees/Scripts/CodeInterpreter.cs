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
using System;
using System.Text.RegularExpressions;

public class CodeInterpreter {

	public static String RemoveComments(string code){
		while (code.IndexOf("/*") != -1 && code.IndexOf("*/") != -1 && code.IndexOf("/*") < code.IndexOf("*/")){
			string comment = code.Substring(code.IndexOf("/*"), code.IndexOf("*/") + 2 - code.IndexOf("/*"));
			code = code.Replace(comment, "");
		}
		return code;
	}
    
	public static string RemoveSpaces(string code){
		while (code.StartsWith(" ")){
			code = code.Substring(1);
		}
		while (code.EndsWith(" ")){
			code = code.Substring(0, code.Length - 1);
		}
		return code;
	}
	
	public static string GetNPCName(string code, string defaultNPCName){
		string npcName = GetNPCName(code);
		if (npcName == null){
			npcName = defaultNPCName;
		}
		if (CharacterManager.GetCharacter(npcName) == null){
			throw new InvalidScriptException("No NPC found with the name: " + npcName);
		} else {
			return npcName;
		}
	}
	
	public static string GetNPCName(string code){
		int colonIndex = code.IndexOf(":");
		if (colonIndex == -1){
			return null;
		} else {
			return RemoveSpaces(code.Substring(0, colonIndex));
		}		
	}
	
	public static string GetVariableName(string code){
		int colonIndex = code.IndexOf(":");
		if (colonIndex == -1){
			return RemoveSpaces(code);
		} else {
			return RemoveSpaces(code.Substring(colonIndex + 1));
		}
	}
	
	public static StringOperand InterpretString(string code, string defaultNPCName, Conversation conversation){
		if (code.Length > 1 && code.StartsWith("\"") && code.EndsWith("\"")){
			return new StringConstant(code.Substring(1, code.Length - 2));
		} else {
			string npcNameNoDefault = GetNPCName(code);
			string npcNameWithDefault = GetNPCName(code, defaultNPCName);
			string variableName = GetVariableName(code);
			if (variableName.EndsWith("$")){
				if (variableName.StartsWith("_")){
					return InterpretExternalString(variableName, npcNameWithDefault, conversation);
				} else {
					return new GetString(variableName, npcNameNoDefault, conversation);
				}
			} else {
				throw new InvalidScriptException("Invalid string operand: " + code);
			}
		}
	}
		
	public static NumericOperand InterpretNumeric(string code, string defaultNPCName, Conversation conversation){
		try {
			return new NumericConstant(Int32.Parse(CodeInterpreter.RemoveSpaces(code)));
		} catch (FormatException exception){
			string npcNameNoDefault = GetNPCName(code);
			string npcNameWithDefault = GetNPCName(code, defaultNPCName);
			string variableName = GetVariableName(code);
			if (variableName.ToLower().Equals("total") || variableName.ToLower().Equals("attitude")){
				return new GetAttitudeTotal(npcNameWithDefault);
			} else if (variableName.EndsWith("#")){
				if (variableName.StartsWith("_")){
					return InterpretExternalNumeric(variableName, npcNameWithDefault);
				} else {
					return new GetInteger(variableName, npcNameNoDefault, conversation);
				}
			} else {
				return new GetImpression(variableName, npcNameWithDefault);
			}
		}
	}
	
	private static StringOperand InterpretExternalString(string variableName, string npcName, Conversation conversation){
		if (variableName.Equals("_item$")){
			return new GetItem(npcName);
		} else if (variableName.Equals("_location$")){
			return new GetLocation(npcName);
		} else if (variableName.Equals("_situation$")){
			return new GetSituation(conversation);
		} else {
			throw new InvalidScriptException("Invalid external string variable: " + variableName);
		}
	}
	
	private static NumericOperand InterpretExternalNumeric(string variableName, string npcName){
		if (variableName.Equals("_money#")){
			return new GetMoney(npcName);
		} else if (variableName.Equals("_drunkness#")){
			return new GetDrunkness(npcName);
		} else if (variableName.Equals("_hour#")){
			return new GetHour();
		} else if (variableName.Equals("_minute#")){
			return new GetMinute();
		} else  if (variableName.Equals("_days_left#")){
			return new GetDaysLeft();
		} else {
			throw new InvalidScriptException("Invalid external numeric variable: " + variableName);
		}
	}

}


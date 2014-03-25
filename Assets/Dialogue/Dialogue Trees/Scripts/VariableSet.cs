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

[Serializable()]
public class VariableSet {

	// PL, 22.10.2008 These needs to be public so that we can save VariableSet
	public Hashtable stringVariables;
	public Hashtable integerVariables;
	
	public VariableSet(){
		stringVariables = new Hashtable();
		integerVariables = new Hashtable();
	}

	public string GetStringVariableValue(string variableName){
		string value = (string)stringVariables[variableName];
		if (value == null){
			return "";
		} else {
			return value;
		}	
	}
	
	public int GetIntegerVariableValue(string variableName){
		object value = integerVariables[variableName];
		if (value == null){
			return 0;
		} else {
			return (int)value;
		}
	}
	
	public void SetStringVariableValue(string variableName, string value){
		if (integerVariables.ContainsKey(variableName)){
			Debug.LogError("Variable " + variableName + " is being used as both string and integer variable!");
		} else {
			stringVariables[variableName] = value;
		}
	}
	
	public void SetIntegerVariableValue(string variableName, int value){
		if (stringVariables.ContainsKey(variableName)){
			Debug.LogError("Variable " + variableName + " is being used as both string and integer variable!");
		} else {
			integerVariables[variableName] = value;
		}
	}
}

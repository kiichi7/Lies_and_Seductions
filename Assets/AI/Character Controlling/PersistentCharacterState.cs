/**********************************************************************
 *
 * CLASS AiLoader
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
using System;
using System.Collections;

[Serializable()]
public class PersistentCharacterState {

	public string name;
	public float lastSoberingMinute;
	public int nightsSlept;
	public int money;
	public float drunkness;
	
	public bool justHadSex;
	public bool startedBlindDate;

	public VariableSet variableSet;

	//public FollowAction followerAction;
	//public Seat currentSeat;


	// Serializer needs this
	public PersistentCharacterState() {	}
	
	public static string GetSaveFileName(string name) {
		return "save_state_" + name + ".bin";
	}
	
	public void Save() {
		//Debug.Log("PersistentCharacterState.Save() " + name);
		//Debug.Log("justHadSex = " + justHadSex);
		SaveLoad.WriteSaveFile(GetSaveFileName(name), this);
	}
		
}

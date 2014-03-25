/**********************************************************************
 *
 * CLASS SaveLoad
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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoad {

	// Currenly dispabled, because standalone player crashes when loading save
	// while loading saves seems to work in editor
	public static bool saveEnabled=false;
	
	// Saves the game
	public static void Save() {
		if(saveEnabled) {
			Debug.Log("SaveLoad.Save()");
			GameTime.Save();
			CharacterManager.Save();
			AILoader.Save();
		
			if(PlayerPrefs.GetInt("save") == 0) {
				PlayerPrefs.SetInt("save", 1);
			}
			//Debug.Log("-- PlayerPrefs.GetInt(save)==" + PlayerPrefs.GetInt("save"));
		}
	}
	
	// Removes the game save
	public static void ResetSave() {
		Debug.Log("SaveLoad.ResetSave()");
		PlayerPrefs.SetInt("save", 0);
	}
	
	// Checks if there is a valid game save
	public static bool SaveExits() {
		if(saveEnabled==false) {
			return false;	
		}
		//Debug.Log("SaveLoad.SaveExits()");
		if(PlayerPrefs.GetInt("save") == 0) {
			return false;		
		}
		else {
			return true;
			
		}
	}
	

	public static void WriteSaveFile(string filename, object obj) {
		string fullPath = GetFullPath(filename);
		try {
			Stream fileStream = File.Open(fullPath, FileMode.Create);			BinaryFormatter formatter = new BinaryFormatter();			formatter.Serialize(fileStream, obj);			fileStream.Close();
		}
		catch(Exception e) {
			Debug.LogError("SaveLoad.WriteSaveFile(): Failed to serialize object to a file " + GetFullPath(filename) + " (Reason: " + e.ToString() + ")");
		} 
	}
	
	public static object ReadSaveFile(string filename) {
		string fullPath = GetFullPath(filename);
		Debug.Log("SaveLoad() reading save file " + fullPath);
		try {
			Stream fileStream = File.Open(fullPath, FileMode.Open, FileAccess.Read);
			BinaryFormatter formatter = new BinaryFormatter();
			object obj= formatter.Deserialize(fileStream);
			fileStream.Close();
			return obj;
		}
		catch(Exception e) {
			Debug.LogError("SaveLoad.ReadSaveFile(): Failed to deserialize a file " + GetFullPath(filename) + " (Reason: " + e.ToString() + ")");
			return null;
		} 	
	}

	private static string GetFullPath(string filename) {
		if(Application.platform == RuntimePlatform.OSXEditor) { 
			return filename;
		}
		else {
			return Application.dataPath + "/" + filename;
		}
	}

}

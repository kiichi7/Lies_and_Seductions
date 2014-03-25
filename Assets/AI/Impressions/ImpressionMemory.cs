/**********************************************************************
 *
 * CLASS ImpressionMemory
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

public class ImpressionMemory : MonoBehaviour {
	
	private const float ALCOHOL_EFFECT_MULTIPLIER = 0.2f;
	
	private Hashtable opinions;
	PersistentImpressionMemory persistentImpressions;

	private ArrayList statusListeners = null;

	public void Awake() {
		statusListeners = new ArrayList();	
	}

	public void Start(){
		//Debug.Log("ImpressionMemory.Start() for " + gameObject.name);
		
		opinions = AILoader.GetOpinionsOfNPC(gameObject.name);
		
		//currentImpressions = new Hashtable();
		
		if(SaveLoad.SaveExits()) {
			//Debug.Log("ImpressionMemory.Start(): Using save file.");
			PersistentImpressionMemory tmp = (PersistentImpressionMemory)SaveLoad.ReadSaveFile(PersistentImpressionMemory.GetSaveFileName(name));
			if (tmp != null) {
				persistentImpressions = tmp;
			}
			else {
				Debug.LogError("ImpressionMemory.Start(): ERROR in loading save file. Creating new ImpressionMemory.");
				SaveLoad.ResetSave();
				InitMemory();	
			}		
		}
		else {
			InitMemory();
		}
		enabled = false;
	}
	
	private void InitMemory() {
		//Debug.Log("ImpressionMemory.Start(): Initializing Impressions from the scratch");
			persistentImpressions = new PersistentImpressionMemory();
			persistentImpressions.name = name;
			ArrayList impressions = AILoader.GetImpressions();
			foreach (string impression in impressions){
				persistentImpressions.currentImpressions.Add(impression, new CurrentImpression());
			}
	}
	
	public int GetCurrentImpressionStrength(string impression){
		
		return ((CurrentImpression)persistentImpressions.currentImpressions[impression]).GetStrength();
	}
		
	public int GetAttitudeTotal(){
		int attitudeTotal = 0;
		foreach (string impression in opinions.Keys){
			attitudeTotal += ((Opinion)opinions[impression]).GetMultiplier() 
					* ((CurrentImpression)persistentImpressions.currentImpressions[impression]).GetStrength();
		}
		CharacterState state = GetComponent(typeof(CharacterState)) as CharacterState;
		CharacterState pcState = CharacterManager.GetPC().GetComponent(typeof(CharacterState)) as CharacterState;
		attitudeTotal = attitudeTotal + (int)(state.GetDrunkness() * ALCOHOL_EFFECT_MULTIPLIER) - (int)(pcState.GetDrunkness() * ALCOHOL_EFFECT_MULTIPLIER);
		return attitudeTotal;
	}
	
	public void RegisterStatusListener(StatusListener listener) {
		statusListeners.Add(listener);
		//Debug.Log("ImpressionMemory.RegisterStatusListener()");
	}
	
	public void SendActionStarted() {
		//Debug.Log("ImpressionMemory.SendActionStarted() --------------------------------------");
		foreach(StatusListener listener in statusListeners) {
			listener.ActionStarted();
			//Debug.Log("---- broadcasting");
		}
	}
	
	public void SendDisplayAttitudeChange() {
		//Debug.Log("ImpressionMemory.SendDisplayAttitudeChange() -----------------------------");
		foreach(StatusListener listener in statusListeners) {
			listener.DisplayAttitudeChange();
			//Debug.Log("---- broadcasting");
		}
	}
	
	public void SendDisplayAttitude() {
		//Debug.Log("ImpressionMemory.SentDisplayAttitude() ----------------------------------");
		foreach(StatusListener listener in statusListeners) {
			listener.DisplayAttitude();
		}
	}
	
	/*public void BroadcastAttitudeChanged() {
		foreach(StatusListener listener in statusListeners) {
			listener.AttitudeChanged(GetAttitudeTotal());
		}
	}*/
	
	public void ImpressionAdjusted(GameObject source, ImpressionAdjuster impressionAdjuster){
		//Debug.Log("ImpressionMemory:ImpressionAdjusted");
		if (impressionAdjuster.AppliesTo(gameObject, source)){
			impressionAdjuster.AdjustCurrentImpression((CurrentImpression)persistentImpressions.currentImpressions[impressionAdjuster.GetImpression()]);
			//BroadcastAttitudeChanged();
		}
	}
	
	public void Save() {
		persistentImpressions.Save();
	}
	
}

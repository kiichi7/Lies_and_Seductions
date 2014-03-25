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

public class DebugGUI : MonoBehaviour{

	public GUIStyle backgroundStyle;
	public GUIStyle textStyle;
	private bool debugImpressions;
	
	private Hashtable oldValues;
	private Hashtable newValues;
	
	private Hashtable oldAttitudeTotals;
	private Hashtable newAttitudeTotals;
	
	private bool controlPressed;
	
	private bool []showCharacters;
	
	private enum STATES {IMPRESSIONS, SHORT, LEAKS, DEFAULT};
	private STATES state;
	
	private int allCount;
	private int texturesCount;
	private int meshesCount;
	private int textMeshesCount;
	private int gameObjecsCount;
	private int componentsCount;
	private int animationsCount;
	private int animationClipCount;
		
	public void Start() {
		controlPressed = false;
		int len = CharacterManager.GetMajorNPCs().Length + 1; 
		showCharacters = new bool[len];
		for(int n=0; n<len;n++) {
			showCharacters[n] = true;	
		}
		state = STATES.DEFAULT;
		CalcUsage();
	}
	
	private static Hashtable BuildNewestValues(){
		GameObject[] npcs = CharacterManager.GetMajorNPCs();
		ArrayList impressions = AILoader.GetImpressions();
		Hashtable newestValues = new Hashtable();
		foreach (GameObject npc in npcs){
			Hashtable newestValuesOfNPC = new Hashtable();
			foreach (string impression in impressions){
				ImpressionMemory impressionMemory = (ImpressionMemory)npc.GetComponent("ImpressionMemory");
				newestValuesOfNPC[impression] = impressionMemory.GetCurrentImpressionStrength(impression);
			}
			newestValues[npc] = newestValuesOfNPC;
		}
		return newestValues;
	}
	
	private static Hashtable BuildNewestAttitudeTotals(){
		GameObject[] npcs = CharacterManager.GetMajorNPCs();
		Hashtable newestTotals = new Hashtable();
		foreach (GameObject npc in npcs){
			ImpressionMemory impressionMemory = (ImpressionMemory)npc.GetComponent("ImpressionMemory");
			newestTotals[npc] = impressionMemory.GetAttitudeTotal();
		}
		return newestTotals;
	}
	
	private static bool HaveChanged(Hashtable newValues, Hashtable newestValues){
		GameObject[] npcs = CharacterManager.GetMajorNPCs();
		ArrayList impressions = AILoader.GetImpressions();
		foreach (GameObject npc in npcs){
			Hashtable newValuesOfNPC = (Hashtable)newValues[npc];
			Hashtable newestValuesOfNPC = (Hashtable)newestValues[npc];
			foreach (string impression in impressions){
				if ((int)newValuesOfNPC[impression] != (int)newestValuesOfNPC[impression]){
					return true;
				}
			}
		}
		return false;
	}
	
	private string GetValueRepresentation(GameObject npc, string impression){
		Hashtable oldValuesOfNPC = (Hashtable)oldValues[npc];
		Hashtable newValuesOfNPC = (Hashtable)newValues[npc];
		int oldValue = (int)oldValuesOfNPC[impression];
		int newValue = (int)newValuesOfNPC[impression];
		if (oldValue == newValue){
			return newValue.ToString();
		} else {
			return oldValue.ToString() + " -> " + newValue.ToString();
		}
	}
	
	private string GetAttitudeTotalRepresentation(GameObject npc){
		int oldTotal = (int)oldAttitudeTotals[npc];
		int newTotal = (int)newAttitudeTotals[npc];
		if (oldTotal == newTotal){
			return newTotal.ToString();
		} else {
			return oldTotal.ToString() + " -> " + newTotal.ToString();
		}
	}
	

	
	public void Update(){
		if (Input.GetKeyDown("left ctrl")) {
			controlPressed = true;
			return;
		}
		if(controlPressed || debugImpressions) {
			if (Input.GetKeyDown("z") ){
				switch(state) {
					case STATES.DEFAULT:
						state = STATES.IMPRESSIONS;
						debugImpressions = true;
						break;
					case STATES.IMPRESSIONS:
						state = STATES.SHORT;
						debugImpressions = true;
						break;
					case STATES.SHORT:
						state = STATES.LEAKS;
						debugImpressions = true;
						InvokeRepeating("CalcUsage", 2.0f, 2.0f);
						break;	
					case STATES.LEAKS:
						debugImpressions = false;
						state = STATES.DEFAULT;
						CancelInvoke();
						break;	
				}
				
				controlPressed = false;
			}
			else if(Input.anyKeyDown) {
				controlPressed = false;
				return;
			}
		}
		
		Hashtable newestValues = BuildNewestValues();
		Hashtable newestAttitudeTotals = BuildNewestAttitudeTotals();
		if (newValues == null || HaveChanged(newValues, newestValues)){
			oldValues = newValues;
			oldAttitudeTotals = newAttitudeTotals;
			newValues = newestValues;
			newAttitudeTotals = newestAttitudeTotals;
		}
		if (oldValues == null){
			oldValues = newValues;
			oldAttitudeTotals = newAttitudeTotals;
		}	
	}

	public void OnGUI(){
		switch(state) {
			case STATES.IMPRESSIONS:
				DrawImpressionDebugInfo();
				break;
			case STATES.SHORT:
				DrawShortInfo();
				break;
			case STATES.LEAKS:
				DrawLeaksInfo();
				break;
			default:
				break;
		}
	}
	
	private void CalcUsage() {
		allCount = FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length;
		texturesCount = FindObjectsOfTypeAll(typeof(Texture)).Length;
		meshesCount = FindObjectsOfTypeAll(typeof(Mesh)).Length;
		textMeshesCount = FindObjectsOfTypeAll(typeof(TextMesh)).Length;
		gameObjecsCount = FindObjectsOfTypeAll(typeof(GameObject)).Length;
		componentsCount = FindObjectsOfTypeAll(typeof(Component)).Length;
		animationsCount = FindObjectsOfTypeAll(typeof(Animation)).Length;
		animationClipCount =  FindObjectsOfTypeAll(typeof(AnimationClip)).Length;
		
	}
	
	private void DrawLeaksInfo() {
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		
		
		GUILayout.BeginArea(new Rect(0, 0, 500, 500));
		
		GUILayout.BeginVertical();
		GUILayout.Label("Toggle with Z");
		GUILayout.BeginHorizontal();
		
		GUILayout.BeginVertical(backgroundStyle);
		GUILayout.Label("All");
		GUILayout.Label("Textures");
		GUILayout.Label("Meshes");
		GUILayout.Label("TextMeshes");
		GUILayout.Label("GameObjects");
		GUILayout.Label("Components");
		GUILayout.Label("Animations");
		GUILayout.Label("AnimationClips");
		GUILayout.EndVertical();	
		
		GUILayout.BeginVertical(backgroundStyle);
		GUILayout.Label(allCount.ToString());        GUILayout.Label(texturesCount.ToString());        GUILayout.Label(meshesCount.ToString());
        GUILayout.Label(textMeshesCount.ToString());        GUILayout.Label(gameObjecsCount.ToString());        GUILayout.Label(componentsCount.ToString());
        GUILayout.Label(animationsCount.ToString());
        GUILayout.Label(animationClipCount.ToString());
        GUILayout.EndVertical();
        
        GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();

        GUILayout.EndArea();
        
	}
	
	private void DrawImpressionDebugInfo() {
		if (oldValues != null && newValues != null) {
			GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
			ArrayList impressions = AILoader.GetImpressions();
			GUILayout.BeginArea(new Rect(0, 0, GUIGlobals.screenWidth, GUIGlobals.screenHeight));
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			GUILayout.BeginVertical(backgroundStyle);
			GUILayout.Label("Toggle with Z");
			GUILayout.BeginHorizontal();
			int n = 0;
			foreach (GameObject npc in CharacterManager.GetMajorNPCs()){
				GUILayout.BeginVertical();
				GUILayout.Label(npc.name);
				if(showCharacters[n]) {
					foreach (string impression in impressions){
						//string impressionName = impression.GetName();
						//GUILayout.Label(impressionName);
						GUILayout.Label(impression);
					}
					GUILayout.Label("Attitude");
					GUILayout.Label("Money");
					GUILayout.Label("Drunkness");
					GUILayout.Label("Location:");
				}
				GUILayout.EndVertical();
				GUILayout.BeginVertical();
				showCharacters[n] = GUILayout.Toggle(showCharacters[n], "");
				if(showCharacters[n]) {
					foreach (string impression in impressions){
						string impressionRepresentation = GetValueRepresentation(npc, impression);
						GUILayout.Label(impressionRepresentation);
					}
					string attitudeTotalRepresentation = GetAttitudeTotalRepresentation(npc);
					GUILayout.Label(attitudeTotalRepresentation);
					CharacterState state = npc.GetComponent(typeof(CharacterState)) as CharacterState;
			
					GUILayout.Label(state.GetMoney().ToString());
					GUILayout.Label(state.GetDrunkness().ToString());
					GUILayout.Label(state.GetCurrentArea().name );
				}
				GUILayout.EndVertical();
				n++;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
	
	private void DrawShortInfo() {
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		//ArrayList impressions = AILoader.GetImpressions();
		GUILayout.BeginArea(new Rect(0, 0, GUIGlobals.screenWidth, GUIGlobals.screenHeight));
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.BeginVertical(backgroundStyle);
		GUILayout.Label("Toggle with Z");
		GUILayout.BeginHorizontal();
		int n = 0;
		foreach (GameObject npc in CharacterManager.GetMajorNPCs()){
			GUILayout.BeginVertical();
			GUILayout.Label(npc.name);
			if(showCharacters[n]) {
				GUILayout.Label("Attitude");
				GUILayout.Label("Money");
				GUILayout.Label("Drunkness");
				GUILayout.Label("Location:");
			}
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
			showCharacters[n] = GUILayout.Toggle(showCharacters[n], "");
			if(showCharacters[n]) {
				string attitudeTotalRepresentation = GetAttitudeTotalRepresentation(npc);
				GUILayout.Label(attitudeTotalRepresentation);
				CharacterState state = npc.GetComponent(typeof(CharacterState)) as CharacterState;
			
				GUILayout.Label(state.GetMoney().ToString());
				GUILayout.Label(state.GetDrunkness().ToString());
				GUILayout.Label(state.GetCurrentArea().name );
			}
			GUILayout.EndVertical();
			n++;
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	
	}

}

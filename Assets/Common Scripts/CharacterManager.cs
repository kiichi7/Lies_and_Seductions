/**********************************************************************
 *
 * CLASS CharacterManager
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

public class CharacterManager : MonoBehaviour {

	public GameObject pc;
	public GameObject[] majorNPCs;
	public GameObject[] genericNPCPrefabs;
	public float genericNPCsCountResetTimeout = 60.0f;
	
	private GameObject[] genericNPCs;
	
	private static CharacterManager instance;
	
	private ArrayList genericNPCsInfo;
	
	public void Awake(){
		instance = this;
		genericNPCs = new GameObject[0];
	}
	
	public static GameObject GetPC(){
		return instance.pc;
	}
	
	public static bool IsPC(GameObject character){
		if(instance.pc == character) {
			return true;	
		}
		else {
			return false;
		}
	}
	
	public static GameObject[] GetMajorNPCs(){
		return instance.majorNPCs;
	}
	
	public static GameObject[] GetNPCs(){
		GameObject[] npcs = new GameObject[instance.majorNPCs.Length + instance.genericNPCs.Length];
		for (int i = 0; i < npcs.Length; i++){
			if (i < instance.majorNPCs.Length){
				npcs[i] = instance.majorNPCs[i];
			} else {
				npcs[i] = instance.genericNPCs[i - instance.majorNPCs.Length];
			}
		}
		return npcs;
	}
	
	public static GameObject GetMajorNPC(string name){
		foreach (GameObject majorNPC in instance.majorNPCs){
			if (name.Equals(majorNPC.name)){
				return majorNPC;
			}
		}
		Debug.LogError("CharacterManager.GetMajorNPC(" + name + "): No major NPC found");
		return null;
	}
	
	public static GameObject[] GetGenericNPCs(){
		return instance.genericNPCs;
	}
	
	public static void Save() {
		foreach(GameObject character in instance.majorNPCs) {
			SaveCharacter(character);	
		}
		SaveCharacter(instance.pc);
	}
	
	public static void SaveCharacter(GameObject character) {
		CharacterState state = character.GetComponent(typeof(CharacterState)) as CharacterState;
		state.Save();
		if(instance.pc != character) {
			ImpressionMemory memory = character.GetComponent(typeof(ImpressionMemory)) as ImpressionMemory;
			memory.Save();
		}
	}
	
	public static GameObject GetCharacter(string name){
		if (instance.pc.name == name){
			return instance.pc;
		} else {
			return GetMajorNPC(name);
		}
	}
	
	public static GameObject[] GetCharacters(){
		GameObject[] characters = new GameObject[1 + instance.majorNPCs.Length + instance.genericNPCs.Length];
		for (int i = 0; i < characters.Length; i++){
			if (i == 0){
				characters[i] = instance.pc;
			} else if (i < instance.majorNPCs.Length + 1){
				characters[i] = instance.majorNPCs[i - 1];
			} else {
				characters[i] = instance.genericNPCs[i - instance.majorNPCs.Length - 1];
			}
		}
		return characters;
	}
	
	public static bool IsMajorNPC(GameObject npc){
		foreach (GameObject majorNPC in instance.majorNPCs){
			if (majorNPC == npc){
				return true;
			}
		}
		return false;
	}
	
	public static bool IsGenericNPC(GameObject character){
		foreach (GameObject genericNPC in instance.genericNPCs){
			if (genericNPC == character){
				return true;
			}
		}
		return false;
	}
	
	public static bool IsPCOrMajorNPC(GameObject character){
		if(instance.pc == character) {
			return true;	
		}
		return IsMajorNPC(character);
	}
	
	public static bool IsCharacter(GameObject entity){
		return IsPCOrMajorNPC(entity) || IsGenericNPC(entity);
	}
	
	public static void FullReset(GameObject specificNPC, Action specificAction){
		ActionRunner pcActionRunner = (ActionRunner)GetPC().GetComponent("ActionRunner");
		pcActionRunner.FullReset();
		foreach (GameObject majorNPC in instance.majorNPCs){
			ActionRunner actionRunner = (ActionRunner)majorNPC.GetComponent("ActionRunner");
			if (majorNPC == specificNPC){
				actionRunner.FullReset(specificAction);
			} else {
				actionRunner.FullReset();
			}
		}
	}
	
	public static void FullReset(){
		FullReset(null, null);
	}
	
	private void DestroyGenericNPCs(){
		if (genericNPCs != null){
			foreach (GameObject genericNPC in genericNPCs){
				ActionRunner actionRunner = (ActionRunner)genericNPC.GetComponent("ActionRunner");
				actionRunner.ResetRoutine(new GenericSelfDestructAction(genericNPC), true);
			}
		}
	}
	
	private void CreateGenericNPCs(Area area){
		int numberOfNPCs = Random.Range(0, area.maximumNumberOfGenericNPCs);
		Debug.Log("CharacterManager.CreateGenericNPCs(): Number of NPCs: " + numberOfNPCs + " (area maximum: " + area.maximumNumberOfGenericNPCs + ")");
		genericNPCs = new GameObject[numberOfNPCs];
		int genericNPCsCreated = 0;
		while (genericNPCsCreated < numberOfNPCs){
			GameObject prefab = genericNPCPrefabs[Random.Range(0, genericNPCPrefabs.Length)];
			Debug.Log("CharacterManager.GreateGenericNPCs():  Prefab: " + prefab.name);
			GameObject genericNPC = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);
			genericNPC.name = prefab.name;
			RoutineCreatorGeneric routineCreator = (RoutineCreatorGeneric)genericNPC.GetComponent("RoutineCreator");
			if (routineCreator.InitRoutine(area)){
				//Debug.Log("Generic created successfully.");
				Renderer renderer = (Renderer)genericNPC.GetComponentInChildren(typeof(Renderer));
				renderer.material = area.genericNPCMaterial;
				GameObject model = genericNPC.transform.Find(genericNPC.name + " Model").gameObject;
				genericNPC.name = genericNPC.name + " " + genericNPCsCreated;
				model.name = genericNPC.name + " Model";	
				genericNPCs[genericNPCsCreated] = genericNPC;
				genericNPCsCreated++;
			} else {
				Debug.Log("CharacterManager.GreateGenericNPCs(): Generic unsuitable for circumstances.");
			}
		}
	}
		
	public static void AreaChanged(Area area){
		GenericDialogueAgent.ResetDialogues();
		instance.DestroyGenericNPCs();
		instance.CreateGenericNPCs(area);
	}

}

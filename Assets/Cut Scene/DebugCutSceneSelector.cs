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

public class DebugCutSceneSelector : MonoBehaviour {

	private bool showSelector;
	private string []cutScenes = null;

	// Use this for initialization
	void Start () {
		showSelector=false;
		cutScenes = CutScenePlayer.GetCutSceneNames();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("9")){
			showSelector = !showSelector;
			Debug.Log("DebugCutSceneSelector.Update() showSelector=" + showSelector);
		}
	}
	
	void OnGUI() {
		
		if(showSelector == false) { return; }
		
		if(CutScenePlayer.IsPlaying()) { return; }
		
		
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
				
		GUILayout.BeginArea (new Rect(GUIGlobals.GetCenterX()-100, 100, 200, 700));
		foreach(string n in cutScenes) {
			if(GUILayout.Button(n)) {
				CutScenePlayer.OnlyPlayThis(n);	
			}	
		}
		if(GUILayout.Button("Reset Archievements")) {
			Archievements.Reset();	
		}
		GUILayout.EndArea ();
		
	}
	
}

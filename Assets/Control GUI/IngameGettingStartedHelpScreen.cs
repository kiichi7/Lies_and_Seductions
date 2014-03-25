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

public class IngameGettingStartedHelpScreen : MonoBehaviour {

	public Texture2D helpTexture;
	public Texture2D helpTextureMouseMove;
	public Vector2 size;
	public GUISkin gSkin;
	private float enabledTime; 

	void Start () {
		
		if(!gSkin) {
			Debug.LogError("IngameGettingStartedHelpScreen: gSkin not set");	
		}
		
		if(IngameHelpMemory.ShouldShow(IngameHelpMemory.START_HELP)) {
			enabled = false;
		}
		else {
			Pause.SetPausedWithoutGUI(true);
			enabledTime = Time.time;
		}
		
		IngameHelpMemory.MarkAsUsed(IngameHelpMemory.START_HELP);
		
	}
	
	public void Enable() {
		enabled = true;	
		Pause.SetPausedWithoutGUI(true);
		enabledTime = Time.time;
	}
	
	void Update () {
		if(Time.time - enabledTime > 1.0f && Input.anyKey) {
			enabled = false;
			Pause.SetPausedWithoutGUI(false);
		}
	}
	
	void OnGUI() {
		GUI.skin=gSkin;
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		GUILayout.BeginArea (new Rect(GUIGlobals.GetCenterX()-size.x/2, GUIGlobals.GetCenterY()-size.y/2, size.x, size.y), gSkin.GetStyle("helpBG"));
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(PlayerPrefs.GetInt("mouseMove") == 1) {
			GUILayout.Label(helpTextureMouseMove);	
		} else {
			GUILayout.Label(helpTexture);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.Label(IngameHelpTexts.gettingStartedHelpText);
		
		GUILayout.EndVertical();
		GUILayout.EndArea(); 
	}
}

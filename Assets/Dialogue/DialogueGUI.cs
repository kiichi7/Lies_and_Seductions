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

public class DialogueGUI : MonoBehaviour {

	private static DialogueGUI instance;
	
	public GUISkin dialogueSkin;
	public GUIStyle backgroundStyle;
	public Texture2D separatorTexture;

	private ArrayList lines = new ArrayList();
	
	private Line selectedLine = null;
	
	public void Awake(){
		instance = this;
		lines = new ArrayList();
		backgroundStyle.border.left = 22;
		backgroundStyle.border.right = 22;
		backgroundStyle.border.top = 24;
		backgroundStyle.border.bottom = 24;
		backgroundStyle.padding.left = 15;
		backgroundStyle.padding.right = 15;
		backgroundStyle.padding.top = 16;
		backgroundStyle.padding.bottom = 16;
		if (!dialogueSkin){
			Debug.LogError("DialogueGUI.Awake(): dialogueSkin is not set.");
		}
		enabled = false;
	}
	
	public void OnGUI(){
		if (lines.Count > 0){
			GUI.skin = dialogueSkin;
			GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
			GUILayout.BeginArea(new Rect(GUIGlobals.GetCenterX()-250, GUIGlobals.screenHeight-510, 500, 500));
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical(backgroundStyle);
			foreach (Line line in lines){
				if (lines.IndexOf(line) > 0){
					GUILayout.Label("", GUILayout.Height(2.0f));
				}
				if (GUILayout.Button(line.GetText())){
					lines = new ArrayList();
					selectedLine = line;
				}
			}
			GUILayout.EndVertical();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
	
	public static void AskNextPCLine(ArrayList lines){
		MouseHandler.UseDefaultCursor();
		instance.lines = lines;
		instance.enabled = true;
		MouseHandler.ReleaseCursorXPosition(); 
	}
	
	public static void Dismiss() {
		instance.enabled = false;
		MouseHandler.LockCursorXPosition();
	}
	
	public static Line GetSelectedPCLine(){
		Line selectedLine = instance.selectedLine;
		instance.selectedLine = null;
		return selectedLine;
	}
}

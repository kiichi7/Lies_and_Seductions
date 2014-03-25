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
 ***********************************************************************
 *
 * USAGE:
 * - Attach to a GameObject
 * - public static void CutScenePlayer.Play(cut-scene name) starts a cut-scene
 * - public static bool CutScenePlayer.IsPlaying() returns whether a cut-scene is playing
 * - each cut-scene can be timed from music or using its own time settings (see classes
 *   CutScene and CutSceneScreeen)
 *
 ******************************************************************/
using UnityEngine;
using System.Collections;

public class CutScenePlayer : MonoBehaviour, SyncListener {

	/**************************************************************
	 *
	 * PUBLIC VARIABLES, THESE ARE SET IN THE UNITY3D INSPECTOR
	 *
	 **************************************************************/
	
	public CutScene []cutScenes;
	
	public Texture2D backgroundWide;
	public Texture2D background4_3;
	
	public GUISkin gSkin;
	
	public bool pauseGame;
	
	// AI Scripts are attached to this object, if this is not null 
	// we'll do some CharcaterRelates stuff we exiting from cut-scene
	public GameObject AICube; 

	/**************************************************************
	 *
	 * PRIVATE VARIABLES 
	 *
	 **************************************************************/

	// For keeping track what were are currently playing
	private CutScene currentCutScene = null;
	private CutSceneScreen currScreen = null;

	// This class is a singleton
	private static CutScenePlayer instance;

	private bool onlyPlayThis;

	/**************************************************************
	 *
	 * OVERLOADED METHODS
	 *
	 **************************************************************/

	public void Awake(){
		if(!gSkin) {
			Debug.LogError("CutScenePlayer.Awake(): gSkin is not set!");	
		}
		instance = this;
		enabled = false;
	}

	void OnGUI () {
		ShowScreen();
	}
	
	
	/**************************************************************
	 *
	 * PUBLIC STATIC METHODS 
	 *
	 **************************************************************/
	
	public static void Play(string cutSceneName) {
		Debug.Log("CutScenePlayer.Play(" + cutSceneName + ")");
		CutScenePlayer.instance.onlyPlayThis=false;
		CutScenePlayer.instance.StartPlay(cutSceneName);
	}
	
	// Plays a cut-scene without triggering archievements, game over, or possible next cut-scenes
	public static void OnlyPlayThis(string cutSceneName) {
		Debug.Log("CutScenePlayer.OnlyPlayThis(" + cutSceneName + ")");
		CutScenePlayer.instance.onlyPlayThis=true;
		CutScenePlayer.instance.StartPlay(cutSceneName);
	}
	
	public static bool IsPlaying() {
		if(CutScenePlayer.instance.currentCutScene == null) {
			return false;	
		}
		else {
			return true;	
		}
	}
	
	public static string []GetCutSceneNames() {
		string [] names = new string[instance.cutScenes.Length];
		int n=0;
		foreach (CutScene cs in instance.cutScenes) {
			names[n++] = cs.name;
		}
		return names;
	}
	
		
	/**************************************************************
	 *
	 * IMPLEMENTED INTREFACES 
	 *
	 **************************************************************/
	
	public void TrigPerformed() {
	 	NextScreen();
	 }

	
	/**************************************************************
	 *
	 * PRIVATE METHODS 
	 *
	 **************************************************************/

	
	private void StartPlay(string cutSceneName) {
		if(pauseGame) {
			//Debug.Log("CutScenePlayer.StartPlay() pausing game");
			Pause.SetPaused(true);
			Screen.showCursor = true;
		}
		currentCutScene = FindCutscene(cutSceneName);
		currScreen = currentCutScene.GetFirst();
		if(!onlyPlayThis && !currentCutScene.archievement.Equals("")) {
			Archievements.Add(currentCutScene.archievement);
		}
		enabled = true;
		if(currentCutScene.syncFromMusic) {
			Debug.LogError("CutScenePlayer.StartPlay(): SyncWithMusic NOT IMPLEMENTED YET");
			FModManager.StartCutScene(this,cutSceneName);
		}
		else {
			FModManager.StartCutScene(cutSceneName);
			Invoke("NextScreen", currScreen.showTime);
		}
	}
	
	private void ShowScreen() {
			
	
		GUI.skin=gSkin;	
		
		GUI.depth = 0;
		// First we draw background
		bool isWide=GUIGlobals.IsWideScreen();
		GUIStyle bgStyle = new GUIStyle();
		if (isWide) {
			bgStyle.normal.background=backgroundWide;
		}
		else {
			bgStyle.normal.background=background4_3;
		}
		
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		GUI.Label( new Rect(0,0,GUIGlobals.screenWidth, GUIGlobals.screenHeight), "", bgStyle);
	
		// and  then we select correct picture 
		Texture2D img = currScreen.picture;
		if(!img) {
			// No more screens to show. Lets do the things needed to end this one.
			HandleCutSceneOver(true);
			return;
		}
		
		GUIStyle cutStyle = new GUIStyle();
		cutStyle.normal.background=img;
			
		// Now we need to make sure that cut-scene image is not distorted by the scale
		GUI.matrix=Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
			
		// Currently the cutscene images must be 1024*613 pixels
		float scale = Screen.width / GUIGlobals.screenWidth;
		if(GUIGlobals.IsWideScreen()==false)
		{
			// Fixing scale for the screens with aspect ration of 4:3
			scale=scale*1280/1024;
		}
		float wi = 1024*scale;	
		float he = 613*scale;
		float xpos=(Screen.width-wi)/2;
		float ypos=(Screen.height-he)/2;

		// and last, we draw the cut scene picture
		GUI.Label(new Rect (xpos, ypos, wi,he), "", cutStyle);
		
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		// Button for skipping the cut-scene
					
		if(GUI.Button( new Rect(GUIGlobals.GetCenterX()-50, GUIGlobals.screenHeight-80, 100, 50), "Skip...")) {
			Debug.Log("CutScenePlayer.ShowScreen() Skip clicked...");
			HandleCutSceneOver(true);
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
		}
	} 
	
	private void HandleCutSceneOver(bool cancelInvoke) {
		//Debug.Log("CutScenePlayer.HandleCutSceneOver()");
		//if(cancelInvoke) {
		CancelInvoke("NextScreen");
		//}
		
		if(!onlyPlayThis && currentCutScene.gameOverAfterThis) {
			// GAME OVER. Lets wrap up and load start screen
			currentCutScene = null;
			Application.LoadLevel("StartScreen");	
		}
		else {
			if(!onlyPlayThis && !currentCutScene.nextCutScene.Equals("")) {
				//CancelInvoke("NextScreen");
				//Debug.Log("CutScenePlayer.HandleCutSceneOver():starting new cut-scene " + currentCutScene.nextCutScene);
				CutScenePlayer.Play(currentCutScene.nextCutScene);	
			}
			else {
				// if we not have AICube the cut-scene is started from the main menu and we do not have characters
				// ready 
				if(AICube) {
					CharacterState pcState = (CharacterState)CharacterManager.GetPC().GetComponent("CharacterState");
					pcState.GetCurrentArea().Entered(CharacterManager.GetPC());
				}
				
				currentCutScene = null;
			
				if(pauseGame) {
					Pause.SetPaused(false);	
				}
				enabled = false;
				
			}
		}
	}
	
	private CutScene FindCutscene(string name) {
	 	foreach (CutScene cs in cutScenes) {
	 		if(cs.name == name) {
	 			return cs;	
	 		}	
	 	}
	 	Debug.LogError("CutScenePlayer: cut-scene " + name + " Not found");
	 	return null;		
	 }
	 
	 private void NextScreen() {
	 	//Debug.Log("CutSCenePlayer.NextScreen()");
	 	currScreen = currentCutScene.GetNext();
		if(currScreen == null) { // Was this the last screen of this cut-scene
			HandleCutSceneOver(false);
			return;	
		}
		if(currentCutScene.syncFromMusic == false) {
			Invoke("NextScreen", currScreen.showTime);	
		}
		else {
	 		Debug.LogError("CutScenePlayer.NextScreen(): SyncWithMusic NOT IMPLEMENTED YET");
		}
	 }
	 
}

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

public class TaskHelp : MonoBehaviour {

	public const string SEX = "GOAL: Go to the elevator at the lobby to have sex with ";
	public const string POKER = "GOAL: Go to the poker table at the restautarant, MahiMahi";
	public const string DANCE = "GOAL: Go to the dance floor at the Night Club, Mars & Venus";
	public const string DRINK = "GOAL: Get a drink to ";

	public const string GOAL_ED = "GOAL: Persuade Ed to help";
	public const string GOAL_EMMA = "GOAL: Persuade Emma to help";
	public const string GOAL_CHRIS = "GOAL: Seduce Chris";

	public enum GOALS {ED, EMMA, CHRIS};

	public GUISkin gSkin;
	public Vector2 helpOffset;

	private static TaskHelp instance = null;
	private string helpText;

	private string shownHelpText;

	private bool goalEd;
	private bool goalEmma;
	private bool goalChris;
	private bool task;
	
	private bool hidden;
	private int notifyCount;
	private string currentStyle;
	private const string DEFAULT_STYLE="taskHelpBG";
	private const string BLINK_STYLE="taskHelpBlinkBG";
		
	// Use this for initialization
	void Awake () {
		instance = this;
		//enabled = false;
		helpText = "";
		shownHelpText = "";
		goalEd=true;
		goalEmma=true;
		goalChris=false;
		task=false;
		hidden = false;
		currentStyle = DEFAULT_STYLE;
		SetHelpText();
		if(!gSkin) Debug.LogError("TaskHelp.Awake(): gSkin not set.");
	}
	
	// Update is called once per frame
	void OnGUI () {
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		GUI.depth = 1;
		GUI.skin = gSkin;
		
		GUIStyle style = gSkin.GetStyle(currentStyle);
		Vector2 size = style.CalcSize(new GUIContent(shownHelpText));
		GUI.Label (new Rect(helpOffset.x, GUIGlobals.screenHeight-helpOffset.y-size.y, size.x, size.y), shownHelpText, style); 
	}
	
	public static void PreferenceChanged() {
		if(PlayerPrefs.GetInt("ToolTip") == 1) {
			instance.enabled = true;	
		}
		else {
			instance.enabled = false;	
		}
	}
	
	private void SetHelpText() {
		if(PlayerPrefs.GetInt("ToolTip") == 1 && hidden == false) {
			enabled = true;	
		}
		else {
			enabled = false;	
		}
		
		shownHelpText="";
		bool goalAdded=false;
		
		if(goalChris) { 
			shownHelpText += GOAL_CHRIS;
			goalAdded=true;
		}
		if(goalEd) {
			if(goalAdded) shownHelpText += "\n";
			shownHelpText += GOAL_ED;
			goalAdded=true;
		}
		if(goalEmma) {
			if(goalAdded) shownHelpText += "\n";
			shownHelpText += GOAL_EMMA;
			goalAdded=true;
		}
		if(task) {
			if(goalAdded) shownHelpText += "\n";
			shownHelpText += helpText;
		}
	}
	
	private void Notify() {
		notifyCount = 0;
		FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_NEW_GOAL);
		InvokeRepeating("Blink", 0.5f, 0.5f);
		
	}
	
	private void Blink() {
		if(notifyCount >= 10) {
			currentStyle = DEFAULT_STYLE;
			CancelInvoke();	
		}
		else {
			if(notifyCount%2==0) {
				currentStyle = DEFAULT_STYLE;	
			}
			else {
				currentStyle = BLINK_STYLE;
			}
			notifyCount++;
			
		}
	}
	
	public static void Hide() {
		instance.enabled = false;
		instance.hidden=true;
	}
	
	public static void Reveal() {
		if(PlayerPrefs.GetInt("ToolTip") == 1) {
			instance.enabled = true;
		}	
		instance.hidden=false;
	}
	
	public static void ShowGoal(GOALS goal) {
		switch(goal) {
			case GOALS.ED:
				instance.goalEd = true;
				instance.SetHelpText();
				break;
			case GOALS.EMMA:
				instance.goalEmma = true;
				instance.SetHelpText();
				break;
			case GOALS.CHRIS:
				instance.goalChris = true;
				instance.SetHelpText();
				break;
		}
		instance.Notify();
	}
	
	public static void RemoveGoal(GOALS goal) {
		switch(goal) {
			case GOALS.ED:
				instance.goalEd = false;
				instance.SetHelpText();
				break;
			case GOALS.EMMA:
				instance.goalEmma = false;
				instance.SetHelpText();
				break;
			case GOALS.CHRIS:
				instance.enabled = false;
				break;
		}
		
	}
	
	public static void ShowHelp(string help, GameObject target) {
		//Debug.Log("TaskHelp.ShowHelp(" + help + ")");
		instance.helpText = help;
		if(target) {
			instance.helpText += target.name;
		}
		instance.task = true;
		instance.SetHelpText();
		instance.Notify();
		//instance.enabled = true;
	}
	
	public static void RemoveHelp() {
		//Debug.Log("TaskHelp.RemoveHelp()");
		instance.helpText = "";
		instance.task=false;
		instance.SetHelpText();
		//instance.enabled = false;
	}
	
}

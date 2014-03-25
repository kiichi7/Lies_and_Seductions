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
using System;
using System.Collections;
using UnityEngine;

public class Archievements : MonoBehaviour {
       public const byte VICTORY=1;
       public const byte VICTORY_BUT_IN_LOVE=2;
       public const byte SEX_WITH_ED=4;
       public const byte SEX_WITH_EMMA=8;
       public const byte SEX_WITH_LORD_JAMES=16;
       public const byte LOST_BET=32;
       public const byte BLACKMAILING_ED=64;
       public const byte BLACKMAILING_EMMA=128;

	public float offset=50;
	
       public Texture2D victoryTexture;
       public Texture2D VictoryButInLoveTexture;
       public Texture2D sexWithEdTexture;
       public Texture2D sexWithEmmaTexture;
       public Texture2D sexWithLordJamesTexture;
       public Texture2D lostBetTexture;
       public Texture2D blackmailingEdTexture;
       public Texture2D blackmailingEmmaTexture;
       public Texture2D notReachedTexture;

       static Archievements instance = null;
       private Hashtable toByteHash;

    public void Awake() {	
	 	instance=this;
	 	toByteHash = new Hashtable();
	 	toByteHash.Add("VICTORY", VICTORY);
	 	toByteHash.Add("VICTORY_BUT_IN_LOVE", VICTORY_BUT_IN_LOVE);
	 	toByteHash.Add("SEX_WITH_ED", SEX_WITH_ED);
	 	toByteHash.Add("SEX_WITH_EMMA", SEX_WITH_EMMA);
	 	toByteHash.Add("SEX_WITH_LORD_JAMES", SEX_WITH_LORD_JAMES);
	 	toByteHash.Add("LOST_BET", LOST_BET);
	 	toByteHash.Add("BLACKMAILING_ED", BLACKMAILING_ED);
	 	toByteHash.Add("BLACKMAILING_EMMA",BLACKMAILING_EMMA);
    	enabled=false;

    }
	  
    public static void Add(byte a) {
	  	byte archievement = (byte)PlayerPrefs.GetInt("a1");
	  	int res  = a & archievement;
	  	if(res == 0) {
	    	archievement = (byte)((int)a+(int)archievement);
	    	PlayerPrefs.SetInt("a1", (byte)archievement);
	  	}
	} 
	
	public static void Reset() {
		PlayerPrefs.SetInt("a1", (byte)0);
	}

    public static void Add(string a) {
		 Add((byte)instance.toByteHash[a]);
    }

	public static bool Is(byte a) {
	  	byte archievement = (byte)PlayerPrefs.GetInt("a1");
	  	int res = a & archievement;
	  	if (res == 0) {
	    	return false;
	  	}
	  	else {
	    	return true;
	  	}
	}

	public static bool Draw() {
	  	return(instance.DrawGUI());
	}

	private bool DrawGUI() {
	
	  	GUILayout.BeginArea(new Rect (0,200,GUIGlobals.screenWidth,GUIGlobals.screenHeight-300));
	  	
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
	  	if(Is(VICTORY)) {
	    	GUILayout.Label(victoryTexture, "archievementsLabel");
	  	}
	  	else {
	    	GUILayout.Label(notReachedTexture, "archievementsLabel");
	  	}
		GUILayout.Space(offset);
	  	if(Is(VICTORY_BUT_IN_LOVE)) {
	    	GUILayout.Label(VictoryButInLoveTexture, "archievementsLabel");
	  	}
	  	else {
	    	GUILayout.Label(notReachedTexture, "archievementsLabel");
	  	}
		GUILayout.Space(offset);
	  	if(Is(SEX_WITH_ED)) {
	    	GUILayout.Label(sexWithEdTexture, "archievementsLabel");
	  	}
	  	else {
	    	GUILayout.Label(notReachedTexture, "archievementsLabel");
	  	}
		GUILayout.Space(offset);
	  	if(Is(SEX_WITH_EMMA)) {
	    	GUILayout.Label(sexWithEmmaTexture, "archievementsLabel");
	  	}
	  	else {
	    	GUILayout.Label(notReachedTexture, "archievementsLabel");
	  	}
		
	  	GUILayout.FlexibleSpace();
	  	GUILayout.EndHorizontal();
	  	GUILayout.Space(offset);
	  	GUILayout.BeginHorizontal();
	  	GUILayout.FlexibleSpace();

		
	 	if(Is(SEX_WITH_LORD_JAMES)) {
	    	GUILayout.Label(sexWithLordJamesTexture, "archievementsLabel");
	  	}
	  	else {
	    	GUILayout.Label(notReachedTexture, "archievementsLabel");
	  	}
	  	GUILayout.Space(offset);
	  	if(Is(BLACKMAILING_ED)) {
	    	GUILayout.Label(blackmailingEdTexture, "archievementsLabel");
	  	}
	  	else {
	    	GUILayout.Label(notReachedTexture, "archievementsLabel");
	  	}	
		GUILayout.Space(offset);
	  	if(Is(BLACKMAILING_EMMA)) {
	    	GUILayout.Label(blackmailingEmmaTexture, "archievementsLabel");
	  	}
	  	else {
	    	GUILayout.Label(notReachedTexture, "archievementsLabel");
	  	}
	  	GUILayout.Space(offset);
	  	if(Is(LOST_BET)) {
	    	GUILayout.Label(lostBetTexture, "archievementsLabel");
	  	}
	  	else {
	    	GUILayout.Label(notReachedTexture, "archievementsLabel");
	  	}
		GUILayout.FlexibleSpace();
	  	GUILayout.EndHorizontal();
	  	GUILayout.FlexibleSpace();
	  	GUILayout.EndVertical();
	  	GUILayout.EndArea();

		if (GUI.Button( new Rect(GUIGlobals.GetCenterX()-50, GUIGlobals.screenHeight-80, 100,50 ), "Back") ) {
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
			return true;	
		}

		return false;

	}
}

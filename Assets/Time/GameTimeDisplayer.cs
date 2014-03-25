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
using System;

public class GameTimeDisplayer {

	DateTime endingDateAndTime;
	GUIStyle normalBackground;
	GUIStyle needleStyle;
	GUIStyle sleepStyle;
	GUIStyle sleepForcedStyle;
	GUIStyle restaurantStyle;
	GUIStyle discoStyle;

	float minutesW;
	float minutesH;
	
	Texture2D minutesNeedle;
	Texture2D hourNeedle;
	
	TimeGUISettings guiSettings;
	
	public GameTimeDisplayer(DateTime ends, TimeGUISettings gs) {
		endingDateAndTime = ends;
		guiSettings=gs;
		
		normalBackground = new GUIStyle();
		if(guiSettings.useAnalogClock) {
			normalBackground.normal.background = gs.timerBackgroundTexture;
		}
		else {
			normalBackground.normal.background = gs.timerDigitalBackgroundTexture;
		}
		sleepStyle = new GUIStyle();
		sleepStyle.normal.background = gs.sleepOpenTexture;
		sleepForcedStyle = new GUIStyle();
		sleepForcedStyle.normal.background = gs.forcedSleepTexture;
		restaurantStyle = new GUIStyle();
		restaurantStyle.normal.background = gs.restaurantOpenTexture;
		discoStyle = new GUIStyle();
		discoStyle.normal.background = gs.discoOpenTexture;

		minutesNeedle = gs.minutesNeedle;
		hourNeedle = gs.hourNeedle;
				
	}
	


	public void Draw(DateTime currentDateAndTime, bool restaurantOpen, bool discoOpen, bool sleepOpen, bool focedSleepNear) {
		// We want to display how much there is time left
		int timeLeft = GameTime.GetDaysLeft();
		// fix this and use this below
		
		GUI.skin = guiSettings.gSkin;
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		
		float clockGUIstartX=GUIGlobals.screenWidth - guiSettings.timerGUIPosition.x;
		float clockGUIstartY=GUIGlobals.screenHeight - guiSettings.timerGUIPosition.y;
		
		float iconSetX=clockGUIstartX + guiSettings.timerGUISize.x  + guiSettings.infoIconOffset.x;
		float iconSetY=clockGUIstartY - guiSettings.infoIconOffset.y;	
		
		if(sleepOpen) {
			if(!focedSleepNear) {
				GUI.Label(new Rect(iconSetX, iconSetY,
								guiSettings.infoIconSize.x, guiSettings.infoIconSize.y), "", sleepStyle);
			}
			else {
				GUI.Label(new Rect(iconSetX, iconSetY,
								guiSettings.infoIconSize.x, guiSettings.infoIconSize.y),"", sleepForcedStyle);
			}
			
			iconSetY += guiSettings.infoIconSize.y + guiSettings.infoIconOffset2;
		}
		if(restaurantOpen) {
			GUI.Label(new Rect(iconSetX, iconSetY,
								guiSettings.infoIconSize.x, guiSettings.infoIconSize.y),"", restaurantStyle);
			iconSetY += guiSettings.infoIconSize.y + guiSettings.infoIconOffset2;
		}
		if(discoOpen) {
			GUI.Label(new Rect(iconSetX, iconSetY,
								guiSettings.infoIconSize.x, guiSettings.infoIconSize.y),"", discoStyle);
		}
		
								
		
		GUI.BeginGroup(new Rect(clockGUIstartX, 
								clockGUIstartY, 
								guiSettings.timerGUISize.x, guiSettings.timerGUISize.y));
								
		GUI.Box(new Rect(0, 0, guiSettings.timerGUISize.x, guiSettings.timerGUISize.y), "", normalBackground);
		
		if(guiSettings.useAnalogClock==false) {
			GUI.Label(new Rect(0, 0, guiSettings.timerGUISize.x, guiSettings.timerGUISize.x), currentDateAndTime.ToString("HH:mm"));
		}
		else {
			/*
			Vector2 pivot = new Vector2(clockGUIstartX+guiSettings.timerGUISize.x/2, clockGUIstartY+guiSettings.timerGUISize.x/2);
			float rot = 12f;
			GUIUtility.RotateAroundPivot(rot, pivot);
			GUI.Label(new Rect(0, 0, guiSettings.timerGUISize.x, guiSettings.timerGUISize.x), minutesNeedle);
			GUIUtility.RotateAroundPivot(-rot, pivot);
			*/
		}
		// We draw days left same way with digital and analog clock
		GUI.Label(new Rect(	0, // start position, x 	
							guiSettings.timerGUISize.y-guiSettings.daysFieldHeight, // xx
							guiSettings.timerGUISize.x, 
							guiSettings.daysFieldHeight), 
							timeLeft.ToString()
							);

		GUI.EndGroup();		
	}

}

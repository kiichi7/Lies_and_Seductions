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

[System.Serializable]
public class TimeGUISettings {

	// Offset from right bottom corner
	// 110, 150 might be close
	public Vector2 timerGUIPosition;
	
	// <95, 125>
	public Vector2 timerGUISize;

	public int daysFieldHeight; 

	public Vector2 infoIconSize;
	
	// Offset from clock gui
	public Vector2 infoIconOffset;
	
	// offset between info icons
	public int infoIconOffset2;
	
	public Texture2D timerBackgroundTexture;
	public Texture2D timerDigitalBackgroundTexture;
	public Texture2D restaurantOpenTexture;
	public Texture2D discoOpenTexture;
	public Texture2D sleepOpenTexture;
	public Texture2D forcedSleepTexture;
	
	public Texture2D minutesNeedle;
	public Texture2D hourNeedle;
	
	public bool useAnalogClock=false;
	public GUISkin gSkin;

}

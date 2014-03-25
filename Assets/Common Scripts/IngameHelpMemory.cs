/**********************************************************************
 *
 * CLASS IngameHelpMemory
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

public class IngameHelpMemory {

	private const string prefsName="helpShown";

	public const byte START_HELP=1;
    public const byte ITEM_HELP=2;
    public const byte POKER_HELP=4;
    public const byte DANCE_HELP=8;
    public const byte ELEVATOR_HELP=16;
    public const byte MAJOR_NPC_HELP=32;
    //public const byte FREE2_HELP=64;
    //public const byte FREE3_HELP=128;

	public static void MarkAsUsed(byte item) {
		byte archievement = (byte)PlayerPrefs.GetInt(prefsName);
	  	int res  = item & archievement;
	  	if(res == 0) {
	    	archievement = (byte)((int)item+(int)archievement);
	    	PlayerPrefs.SetInt(prefsName, (byte)archievement);
	  	}	
	}
	
	public static void Unmark(byte item) {
		byte archievement = (byte)PlayerPrefs.GetInt(prefsName);
	  	int res  = item & archievement;
	  	if(res == 1) {
	    	archievement = (byte)((int)item-(int)archievement);
	    	PlayerPrefs.SetInt(prefsName, (byte)archievement);
	  	}
	}
	
	public static bool ShouldShow(byte item) {
		
		if(PlayerPrefs.GetInt("ToolTip") == 0) {
			return false;	
		}
		
		byte archievement = (byte)PlayerPrefs.GetInt(prefsName);
	  	int res = item & archievement;
	  	if (res == 0) {
	    	return false;
	  	}
	  	else {
	    	return true;
	  	}	
	}
	
	public static void Reset() {
		PlayerPrefs.SetInt(prefsName, (byte)0);
	}

}

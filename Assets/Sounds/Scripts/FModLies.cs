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
public class FModLies {

/* Copy paste Lies.h here and replace all:
	public static int => public static int
	public static int => public static int
	
	Don't use EventGroup/Event-relative indices - EVER!
	Use Project-unique event ids - ALWAYS!
*/


/*
    Total number of events in the project
*/
public static int EVENTCOUNT_LIES = 27;

/*
    Project-unique event ids
*/
public static int EVENTID_LIES_CUT_SCENE_PLACE_HOLDER_PLACE_HOLDER = 0;
public static int EVENTID_LIES_DECK_SEAWIND = 1;
public static int EVENTID_LIES_DECK_POOL = 2;
public static int EVENTID_LIES_LOBBY_NIGHTCLUB_NOISE = 3;
public static int EVENTID_LIES_LOBBY_RESTAURANT_NOISE = 4;
public static int EVENTID_LIES_LOBBY_DECK_NOISE = 5;
public static int EVENTID_LIES_DANCE_DANCESTEP_FAIL = 6;
public static int EVENTID_LIES_DANCE_HURRAA = 7;
public static int EVENTID_LIES_STARTMENU_MOUSECLICKED = 8;
public static int EVENTID_LIES_POPUPMENU_MENUOPEN = 9;
public static int EVENTID_LIES_POPUPMENU_ITEMCLICKED = 10;
public static int EVENTID_LIES_POPUPMENU_CANCEL = 11;
public static int EVENTID_LIES_RESTAURANT_CLATTER_OF_CUTLERY = 12;
public static int EVENTID_LIES_RESTAURANT_DINING = 13;
public static int EVENTID_LIES_ACTIONS_CHAIR = 14;
public static int EVENTID_LIES_ACTIONS_EATING = 15;
public static int EVENTID_LIES_ACTIONS_DRINKING = 16;
public static int EVENTID_LIES_ACTIONS_POKER_DEALING = 17;
public static int EVENTID_LIES_ACTIONS_POKER_CHIPS = 18;
public static int EVENTID_LIES_ACTIONS_ELEVATOR = 19;
public static int EVENTID_LIES_ACTIONS_DOOR_LOCKED = 20;
public static int EVENTID_LIES_ACTIONS_STEP = 21;
public static int EVENTID_LIES_ACTIONS_POKER_SHUFFLING = 22;
public static int EVENTID_LIES_ACTIONS_DOOR = 23;
public static int EVENTID_LIES_ACTIONS_ADDITUDE_UP = 24;
public static int EVENTID_LIES_ACTIONS_ADDITUDE_DOWN = 25;
public static int EVENTID_LIES_ACTIONS_NEW_GOAL = 26;

public static int EVENTCATEGORY_LIES_MASTER = 0;
public static int EVENTCATEGORYCOUNT_LIES_MASTER = 18;

public static int EVENTCATEGORY_LIES_MASTER_MUSIC = 0;
public static int EVENTCATEGORYCOUNT_LIES_MASTER_MUSIC = 0;
public static int EVENTCATEGORY_LIES_MASTER_AMBIENCE = 1;
public static int EVENTCATEGORYCOUNT_LIES_MASTER_AMBIENCE = 5;
public static int EVENTCATEGORY_LIES_MASTER_TEHOSTEET = 2;
public static int EVENTCATEGORYCOUNT_LIES_MASTER_TEHOSTEET = 0;
public static int EVENTCATEGORY_LIES_MASTER_IAMUSIC = 3;
public static int EVENTCATEGORYCOUNT_LIES_MASTER_IAMUSIC = 0;
public static int EVENTCATEGORY_LIES_MASTER_UI_FEEDBACK = 4;
public static int EVENTCATEGORYCOUNT_LIES_MASTER_UI_FEEDBACK = 4;

public static int EVENTREVERB_LIES_TEST = 0;

/*
    Music parameter ids
*/
public static int MUSICPARAM_LIES_CUTSCENESTARTED = 3;
public static int MUSICPARAM_LIES_DISCO_HIFIMODE = 4;
public static int MUSICPARAM_LIES_DANCE_POINTS = 5;
public static int MUSICPARAM_LIES_BAR_RANDOM = 6;

/*
    Music cue ids
*/
public static int MUSICCUE_LIES_DANCE_PORTAMENTO = 6;
public static int MUSICCUE_LIES_STARTSCREEN_MUZAK = 9;
public static int MUSICCUE_LIES_INTRO_CUTSCENE = 10;
public static int MUSICCUE_LIES_DANCE_VERSE = 14;
public static int MUSICCUE_LIES_DANCE_HOTMODE = 15;
public static int MUSICCUE_LIES_DANCE_BACK2VERSE = 16;
public static int MUSICCUE_LIES_NIGHTCLUB_MUZAK = 17;
public static int MUSICCUE_LIES_RESTAURANT_MUZAK = 18;
public static int MUSICCUE_LIES_GRANDE_SILENZIO_SLOW = 29;
public static int MUSICCUE_LIES_GRAMDE_SILENZIO_FAST = 30;
public static int MUSICCUE_LIES_SEX_THEME = 31;
public static int MUSICCUE_LIES_CS_THE_END = 32;
public static int MUSICCUE_LIES_CS_NARROW_VICTORY = 33;
public static int MUSICCUE_LIES_CS_VICTORY = 34;
public static int MUSICCUE_LIES_CS_INTRO = 35;
public static int MUSICCUE_LIES_CS_LOST_BET = 36;
public static int MUSICCUE_LIES_CS_BLACKMAILING = 37;
public static int MUSICCUE_LIES_CS_STARTMENU = 38;

}

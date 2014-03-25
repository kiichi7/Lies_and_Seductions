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
 ***********************************************************************/using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Runtime.InteropServices;


/***********************************************************************************************
 *
 * CLASS FModManager
 *
 ***********************************************************************************************/

public class FModManager : MonoBehaviour{

	private class SyncListenerNode {
		
		private SyncListener syncListener;
		private int everyNSyncPointUsed;
		
		public SyncListenerNode(SyncListener s, int e) {
			syncListener = s;
			everyNSyncPointUsed = e;
		}
		
		public SyncListener getSyncListener() {
			return syncListener;	
		}
		
		public int getEveryNSyncPointUsed() {
			return everyNSyncPointUsed;	
		}
		
		public void updateEveryNSyncPointUsed(int e) {
			everyNSyncPointUsed = e;	
		}
		
	}

	/***********************************************************************************************
 	 *
 	 * PRIVATE VARIABLES
	 *
	 ***********************************************************************************************/

   	private static bool initError = false;
   	private static bool ini=false;
   	private static string SoundPath="NOT SET";
	
	public enum FModStatus {
		OK,
		INITIALIZED,
		INIT_ERROR	
	};
	
	public struct VECTOR
    {
        public float x;        /* X co-ordinate in 3D space. */
        public float y;        /* Y co-ordinate in 3D space. */
        public float z;        /* Z co-ordinate in 3D space. */
    };
	
	public static VECTOR camPosition;
	public static VECTOR camUp;
	public static VECTOR camForward;
	
	// Cut-scene syncing
	public static SyncListener cutSceneListener = null;
	
	
	// Dance/NPC music syncing
	private static int beat = 1;
	//private bool started = false;
	private static ArrayList syncListenerNodes = new ArrayList();  
	
	/***********************************************************************************************
 	 *
 	 * PUBLIC METHODS
	 *
	 ***********************************************************************************************/
	
	void Awake() {
		FModStatus err = init();
		if(err == FModStatus.INIT_ERROR) {
			Debug.LogError("FModManager.Awake(): Failed to initialize FMOD!");
			enabled=false;	
		}
		else {
			Debug.Log("FModManager.Awake(): FMOD initalized");
		}
	}
	
   	private static FModStatus init() {
   		
   		if(initError) {
   			return FModStatus.INIT_ERROR;
   		}

   		if(ini) {
   			return FModStatus.OK;	
   		}
    	SoundPath = Application.dataPath + "/Sounds/";
   		//Debug.Log("FModMAnager sounds directory:" + SoundPath);
  		
  		
  		try {
			if(initEventSystem(SoundPath)!=0) 
			{  
				Debug.LogError("Failed to initialize FMOD, sounds path: " + SoundPath);
				ini=true;
				initError=true;
				return FModStatus.INIT_ERROR;	
			}
			startMusicSystem();
			//Debug.Log("FModManager.init(): musisystem started");
			setMusicCallback();
			//Debug.Log("FModManager.init(): musicallback started");
			ini=true;
			return FModStatus.INITIALIZED;
  		}
  		catch(Exception e) {
			Debug.LogError("FModManager.Awake(): Exception in initializing FMOD");
			ini=true;
			initError=true;
			return FModStatus.INIT_ERROR;
		}
   	}
	   
   	public static bool initOK() {
   		
   		//return false;
   		
   		if(ini==false) {
   			return false;	
   		}
   		if(initError==true) {
   			return false;	
   		}
   		return true;
   		
   		/*FModStatus fms = init();
		if (fms==FModStatus.INIT_ERROR) {
			FModManager fmm = FindObjectOfType(typeof (FModManager)) as FModManager;
			
			fmm.enabled = false;
//   			Debug.Log("ERROR: FModManager:::" + msg + ": " + fms.ToString());
   			return false;	
   		}
   		return true;*/
   	}
   
   	/******************************
   	 *
   	 * Sound control
   	 *
   	 ********************************/
         	
   	public static void UpdateCameraPosition(Vector3 position, Vector3 forward, Vector3 up) {
   		if (initOK() == false)
   			return;
   		
   		camPosition.x = position.x;
   		camPosition.y = position.y;
   		camPosition.z = position.z;
   		
   		camForward.x = forward.x;
   		camForward.y = forward.y;
   		camForward.z = forward.z;
   		
   		camUp.x = up.x;
   		camUp.y = up.y;
   		camUp.z = up.z;
   		
   		int err = setCameraPosition(ref camPosition, ref camForward, ref camUp);
   		
   		//Debug.Log("camera updated!!");
   		
   		if (err!=0) {
   			Debug.LogError("FModManager.UpdateCameraPosition(): ERROR: " + err.ToString());	
   		}
   	}
    
    void Update() {
   		int err = update();
   		if (err!=0) {
   	 		Debug.LogError("FModManager.Update(): ERROR: " + err.ToString());
   	 		return;
   	 	}
		if (syncListenerNodes.Count > 0 && musicBeatPassed()) {
			beat = beat % 32;
			beat++;
			foreach (SyncListenerNode s in syncListenerNodes) {
				if (s.getEveryNSyncPointUsed()==120 && beat%2==0 ||
					s.getEveryNSyncPointUsed()==240 ||
					s.getEveryNSyncPointUsed()==30 && beat%8==0) {
					s.getSyncListener().TrigPerformed();
					//Debug.Log("beat: " + beat + " Time: " + Time.time + "#" +syncListenerNodes.Count);
				}	
			}
		}
   		if (cutSceneListener!=null && syncPointDetected())
   			cutSceneListener.TrigPerformed();
   		
   	}

	public static void SetSoundVolume(float vol) {
		Debug.Log("FModManager.SetSoundVolume(): NOT IMPLEMENTED");
	}
	
	
	public static void SetMusicVolume(float vol) {
		Debug.Log("FModManager.SetMusicVolume(): NOT IMPLEMENTED");
	}

	public static void StartEvent(int systemid) {	

		//Debug.Log("FModManager.StartEvent(" + systemid + ") ini=" + ini + ", initError=" + initError );

		if (initOK() == false)
			return;	
		
				
		int err = startEvent(systemid);
   		//if (err!=0) {
   			//Debug.Log("ERROR: FModManager:::playEvent(): " + err.ToString());	
   		//}
	}
	
	public static void StartEvent(int systemid, Vector3 position) {
		//Debug.Log("StartEvent()" + systemid + " " + position);
		if (initOK() == false)
			return;
		int err = 666;
		VECTOR v;
		v.x = position.x;
		v.y = position.y;
		v.z = position.z;
		
		//Debug.Log("3d sound started: " + v.x + " " + v.y + " " + v.z);
		
		err = start3DEvent(systemid, ref v);
   		//if (err!=0) {
   			//Debug.Log("ERROR: FModManager:::playEvent(): " + err.ToString());	
   		//}
	}
   
   	public static void StartEventAtCamera(int systemid) {
		if (initOK() == false)
			return;
		int err = start3DEvent(systemid, ref camPosition);
   		//if (err!=0) {
   			//Debug.Log("ERROR: FModManager:::playEvent(): " + err.ToString());	
   		//}
	}

   
	public static void StartSyncSoundEvent(int systemid) {
		if (initOK() == false)
			return;
   		int err = startSyncEvent(systemid);
   		//if (err!=0) {
   		//	Debug.Log("FModManager.StartSyncSoundEvent(): ERROR in playEvent(): " + err.ToString());	
   		//}
	}
	   	
   	/*
	 * Add SyncListener you want to sync with music beats using this method.
	 * s : Listener
	 * syncPointAmount : number of syncpoints that are jumped over (1 gives all, 4 every 2 seconds etc...).
	 * firstOnBar : If the first trig needs to be on bar (on kick drum)
	 */
	public static void AddMusicSyncListener(SyncListener s, int bpm) {
		SyncListenerNode syncListenerNode = new SyncListenerNode(s,bpm);
		syncListenerNodes.Add(syncListenerNode);
		//Debug.Log("List size: " + syncListenerNodes.Count);
	}
	
	public static void RemoveMusicSyncListener(SyncListener sl) {
		//Debug.Log("ATTEMPTING TO REMOVE " + syncListenerNodes.Count);
		SyncListenerNode toBeRemoved = null;
		foreach (SyncListenerNode s in syncListenerNodes) {
			if (s.getSyncListener() == sl) {
				toBeRemoved = s;
			}	
		}
		if (toBeRemoved != null)
			syncListenerNodes.Remove(toBeRemoved);
		//Debug.Log("RESULT: " + syncListenerNodes.Count);
	}
	
	/*
	 * OBS: bpm e {120,240,30}
	 */
	public static void setMusicSyncBPM(SyncListener sl, int bpm) {
		//SyncListenerNode sln = null;
		//Debug.Log("FModSync.setSyncPointAmount(sl, " + bpm + ")");
		foreach (SyncListenerNode s in syncListenerNodes) {
			if (s.getSyncListener() == sl) {
				s.updateEveryNSyncPointUsed(bpm);
			}	
		}

	}
   	   
    public static void Cue(int id) {
   		if (initOK() == false)
   			return;
   		int err = promptCue(id);
   		if (err!=0) {
   			Debug.Log("ERROR: FModManager:::stopEvent(): " + err.ToString());	
   		}
   }
   
	public static void SetMusicParameterValue(int paramID, float param) {
		if (initOK() == false)
			return;
   		int err = setMusicParameter(paramID, param);
   	 	//if (err!=0) {
   	 		//Debug.Log("ERROR: FModManager:::setParam(): " + err.ToString());
   	 	//}
   	}


    public static void StopAll() {
    	if (initOK() == false)
			return;
   		int err = stopAllEvents("master");
   		//if (err!=0) {
   	 		//Debug.Log("ERROR: FModManager:::musicUpdate(): " + err.ToString());
   	 	//}
	}

	public static void StopEvent(int systemid) {
   		if (initOK() == false)
   			return;
   		int err = stopEvent(systemid);
   		//if (err!=0) {
   	 		//Debug.Log("ERROR: FModManager:::musicUpdate(): " + err.ToString());
   	 	//}
   	}
    
    public static void StartCutScene(SyncListener c, String cutScene) {
    	cutSceneListener = c;
    	StartCutScene(cutScene);
    }
    
    public static void StartCutScene(String cutScene) {
    	if (initOK() == false)
   			return;
    	StopAll();
       	if (cutScene.Equals("Victory but in Love")) {
    		Cue(FModLies.MUSICCUE_LIES_CS_NARROW_VICTORY);	
    	} else if (cutScene.Equals("Victory")) {
    		Cue(FModLies.MUSICCUE_LIES_CS_VICTORY);
    	} else if (cutScene.Equals("intro")) {
    		//Cue(FModLies.MUSICCUE_LIES_CS_INTRO);
    	} else if (cutScene.Equals("Emma Blackmailed") || cutScene.Equals("Ed Blackmailed")) {
    		Cue(FModLies.MUSICCUE_LIES_CS_BLACKMAILING);
    	} else if (cutScene.Equals("The End")) {
    		Cue(FModLies.MUSICCUE_LIES_CS_THE_END);
    	} else if (cutScene.Equals("Lost Bet")) {
    		Cue(FModLies.MUSICCUE_LIES_CS_LOST_BET);
    	} else if (cutScene.Equals("Sex with Lord James") || cutScene.Equals("Sex with Lord James") ||
    			cutScene.Equals("Sex with Ed") || cutScene.Equals("Sex with Emma")) {
    		Cue(FModLies.MUSICCUE_LIES_SEX_THEME);
    	} else if (cutScene.Equals("night") || cutScene.Equals("forced night")) {
			StartEvent(FModLies.EVENTID_LIES_CUT_SCENE_PLACE_HOLDER_PLACE_HOLDER);
			Cue(FModLies.MUSICCUE_LIES_GRANDE_SILENZIO_SLOW);
    		//Cue(FModLies.MUSICCUE_LIES_CS_INTRO); // temporarily
    	} 
    }

    
    public static void StopCutScene() {
    	if (initOK() == false)
   			return;
    	StopAll();
    	cutSceneListener = null;	
    }
        
  	/***********************************************************************************************
 	 *
 	 * PRIVATE METHODS
	 *
	 ***********************************************************************************************/
 

	[DllImport ("UnityFMod")]
   	private static extern int initEventSystem(string path);
   
   	[DllImport ("UnityFMod")]
   	private static extern int getGroup();
   	
 	[DllImport ("UnityFMod")]
   	public static extern int startEvent(int systemid);
   
   	[DllImport ("UnityFMod")]
   	private static extern int start3DEvent(int systemid, ref VECTOR position);
   
   	[DllImport ("UnityFMod")]
   	private static extern int stopEvent(int systemid);
   
   	[DllImport ("UnityFMod")]
   	private static extern int startMusicSystem();

   	[DllImport ("UnityFMod")]
   	private static extern int promptCue(int id);
   
   	[DllImport ("UnityFMod")]
   	private static extern int update();
   
   	[DllImport ("UnityFMod")]
   	private static extern int setMusicParameter(int paramid, float parameter);
   
   	[DllImport ("UnityFMod")]
   	private static extern int stopAllEvents(string path);
   	
	[DllImport ("UnityFMod")]
   	private static extern int startSyncEvent(int systemid);

	[DllImport ("UnityFMod")]
   	private static extern bool syncPointDetected();
   	
   	[DllImport ("UnityFMod")]
   	private static extern bool musicBeatPassed();
   	
   	[DllImport ("UnityFMod")]
   	private static extern int setMusicCallback();
   
   	[DllImport ("UnityFMod")]
   	private static extern int setCameraPosition(ref VECTOR position, ref VECTOR forward, ref VECTOR up);
   	
	// entry point exception...
	[DllImport ("UnityFMod")]
   	private static extern int debug(bool on);
   	
   	//[DllImport ("fmodex")]
   	//private static extern int FMOD_System_Create (ref IntPtr system);
   	
   	//[DllImport ("fmodevent")]
    //private static extern int FMOD_EventSystem_Create (ref IntPtr eventsystem);



}

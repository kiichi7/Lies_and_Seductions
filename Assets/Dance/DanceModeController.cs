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

public class DanceModeController : MonoBehaviour, SyncListener {

	// lenght of tick for updating 
	public float desiredSpeed=160;
	// correct hits needed for HOT MODE
	public int hotModeRequisite=20;

	// For calculating score
	public int hotModeScoreMultiplier=2;
	public int hotModeFailMultiplier=10;
	public int normalModeFailMultiplier=2;
	
	public const float QUICK_HELP_MIN_SHOW_TIME=1.0f; // sec
	public const float SCORE_MIN_SHOW_TIME=1.0f; // sec
	
	//Impression
	public float VISIBILITY_RANGE = 30.0f;
	
	public float baseScore=0.5f;
	
	public float doubleHitTimeWindow = 0.1f;
	
	// Go for idle animation if no dance actions (key press) within this window
	public float danceActionWindow = 0.6f;
	
	// list of BeatMarkers on screen
	private ArrayList beatMarkers;
	
	// Counter keeping track where in coregraphy we are currently
	private int currentBeat;
	private int beat;
	// for keeping track if we need NewBeatMarker
	// Done this way, because I had probles using ArrayList from Update() and OnGUI()
	private int lastBeat;

	private enum HitStatus {MISS, HIT, MISS_PRESS}
	
	// Score for the player
	private float score;
	private int successesInRow;
	// How many saw the dancing; got the impression push
	private ArrayList witnesses;
	private int impressionPushValue;
	
	private int correntHits;
	private int errors;

	// Are we in HOT MODE or in NORMAL MODE
	private bool hotMode;
	private int hotModeBeat;

	private bool coreographyEnded;
	private bool danceModeEnded;
	
	private float lastMissPressTime;
	private float lastPressTime;
	
	private GameObject pc;
	
	private PCDanceAction pcDanceAction;
	
	private static DanceModeController instance;
	
	private float debugDanceStartedTime;
	private float dialogEnabledTime;

	private enum STATES { SHOW_QUICK_HELP, MINIGAME, SHOW_SCORE };
	private STATES state;
	
	public void Awake(){
		instance = this;
		enabled=false;
	}	
	
	public static void StartDanceMode(PCDanceAction pcDanceAction){
		instance.InstanceStartDanceMode(pcDanceAction);
	}
	
	private void InstanceStartDanceMode(PCDanceAction pcDanceAction){
		// First we request garbage collection. This way hopefully, there will be 
		//  no carbage collection while minigame is on. 
		System.GC.Collect();

		this.pcDanceAction = pcDanceAction;
		beatMarkers = new ArrayList();
		//lastUpdate = GameTime.GetRealTimeSecondsPassed();
		currentBeat=0;
		hotMode=false;
		hotModeBeat=0;
		lastBeat=-1;
		beat=0;
		coreographyEnded = false;
		danceModeEnded = false;
		successesInRow=0;
		score=0;
		
		correntHits=0;
		errors=0;
		
		lastMissPressTime = Time.time;
		lastPressTime = Time.time;
		//myPaused=false;
		//sceneLightDefaultColor=sceneLight.color;
		//sceneLightDefaultIntensity=sceneLight.intensity;
		GameTime.SetGUIVisible(false);
		pc = CharacterManager.GetPC();
		if(PlayerPrefs.GetInt("ToolTip") == 1) {
			state = STATES.SHOW_QUICK_HELP;
			dialogEnabledTime = Time.time;
		}
		else {
			state = STATES.MINIGAME;
			StartTheBeat();
		}
		
		TaskHelp.Hide();
		
		CameraController.SetDanceCameraAngle();
	}
	
	
	private void StartTheBeat() {
		debugDanceStartedTime = Time.time;		
		// Start the music.
		//FModManager fmod = FModManager.instance();
		//fmod.playSyncEvent(FModLies.EVENT_LIES_TANSSI_SYNKKATESTI);
		FModManager.AddMusicSyncListener(this,120);
		FModManager.Cue(FModLies.MUSICCUE_LIES_DANCE_VERSE);

	}	
	
	public static bool IsFinished(){
		return instance.danceModeEnded;
	}
	
	public static void UpdateDanceModeController(){
		instance.InstanceUpdateDanceModeController();
	}
	
	public void InstanceUpdateDanceModeController () {
		switch(state) {
			case STATES.SHOW_QUICK_HELP:
				if(Time.time - dialogEnabledTime > QUICK_HELP_MIN_SHOW_TIME && Input.anyKey) {
					state = STATES.MINIGAME;
					StartTheBeat();
				}
				return;
			case STATES.SHOW_SCORE:
				if(Time.time - dialogEnabledTime > SCORE_MIN_SHOW_TIME && Input.anyKey) {
					danceModeEnded = true;	
				}
				return;
			case STATES.MINIGAME:
				break;		
			default:
				return;	
			
		}
		if(Input.GetKeyDown("space")) {
			if(hotMode) {
				TerminateHotMode(true);
			}
			coreographyEnded = true;
			TerminateDanceMode(true);
			return;	
		}
			
		ArrayList directionsPressed = new ArrayList();
		if(Input.GetKeyDown("a") || Input.GetKeyDown("left")) {
			directionsPressed.Add(Direction.LEFT);
		}
		if(Input.GetKeyDown("s") || Input.GetKeyDown("down")) {
			directionsPressed.Add(Direction.DOWN);
		}
		if(Input.GetKeyDown("d") || Input.GetKeyDown("right")) {
			directionsPressed.Add(Direction.RIGHT);
		}
		if(Input.GetKeyDown("w") || Input.GetKeyDown("up")) {
			directionsPressed.Add(Direction.UP);
		}
		
		
		// Let see if the key press is timed correctly and it is right key press
		if (directionsPressed.Count > 0){
			bool scored = false;
			lastPressTime = Time.time;
			pcDanceAction.DirectionsPressed(directionsPressed);
			foreach (BeatMarker marker in beatMarkers) {
				if (marker.Hit(directionsPressed) == true) {
					scored = true;
					UpdateScore(HitStatus.HIT);
					if (directionsPressed.Count == 0){
						break;
					}
				}			
			}
			if (scored == false && Time.time < lastMissPressTime + doubleHitTimeWindow) {
				UpdateScore(HitStatus.MISS_PRESS);
				FModManager.StartEvent(FModLies.EVENTID_LIES_DANCE_DANCESTEP_FAIL);
				lastMissPressTime = Time.time;
				if(hotMode) {
					pcDanceAction.StartIdle();
				}
			}
					
		}
		
		// If there has been no key press within certain limit we go for idle animation...
		if(Time.time > lastPressTime + danceActionWindow) {
			pcDanceAction.StartIdle();
		}
		
				
		// Add correct BeatMarker to list of active markers
		AddBeatMarker();
		
		ArrayList tmp = new ArrayList();
		foreach(BeatMarker marker in beatMarkers) {
			marker.UpdateBeatMarker(Time.deltaTime, desiredSpeed);
			// used and useless markes are put aside for removal
			if (marker.IsActive() == false) {
				tmp.Add(marker);
			}
		}

		// Removing markers
		bool thereHasBeenAMiss = false;
		foreach(BeatMarker marker in tmp) {
			if (marker.HasBeenHit() == false && !thereHasBeenAMiss) {
				UpdateScore(HitStatus.MISS);
				thereHasBeenAMiss = true;
				FModManager.StartEvent(FModLies.EVENTID_LIES_DANCE_DANCESTEP_FAIL);
			}
			beatMarkers.Remove(marker);
		}
		//Debug.Log("Ending update with " + beatMarkers.Count + " beat markers.");
		
		if (state == STATES.MINIGAME && coreographyEnded && beatMarkers.Count == 0){
			TerminateDanceMode(false);
		}
	}

	/*
	 * Adds beat correct beat markers for drawing
	 *
	 */
	private void AddBeatMarker() {
		if (lastBeat==beat) {
			// No time to trigger next beat yet. Returning
			return;	
		}
		//Debug.Log("Adding Beat Marker. Beat Number: " + beatNumber.ToString());		
		lastBeat=beat;
		
		// Check if we have still come coreography left. We just return if not.
		// We le the player play HOT MODE throught if it is on
		if(hotMode == false && (currentBeat*4+3 > DanceCoreography.NORMAL.Length)) {
			coreographyEnded = true;
			return;
		}
	
		//Add the new BeatMarkers
		int beatConverted;
		int[] coreography;
		if (hotMode){
			//If we've reached the end of the hot mode coreograpy, end hot mode
			if (hotModeBeat * 4 + 3 > DanceCoreography.HOT.Length) {
				TerminateHotMode(false);
				return;
			}
			beatConverted = hotModeBeat * 4;
			coreography = DanceCoreography.HOT;
		} 
		else {
			beatConverted = currentBeat * 4;
			coreography = DanceCoreography.NORMAL;
		}
		
		for (int i = 0; i < 4; i++){
			if (coreography[beatConverted + i] == 1){
				Direction direction;
				switch (i){
				case 0:
					direction = Direction.LEFT;
					break;
				case 1:
					direction = Direction.DOWN;
					break;
				case 2:
					direction = Direction.RIGHT;
					break;
				case 3:
					direction = Direction.UP;
					break;
				default:
					direction = Direction.NONE;
					Debug.LogError("DanceModeController.AddBeatMarker(): ERROR: Unrecognzed BeatMarker Direction");
					break;
				}
				BeatMarker newMarker = DanceGUI.CreateBeatMarker(direction);
				beatMarkers.Add(newMarker);
			}
		}
	}
			
	public void TrigPerformed() {
		/*if (myPaused==true) {
			return;	
		}*/
		
		beat++;
		if (hotMode==true) {
			hotModeBeat++;
		}
		else {
			currentBeat++;	
		}
		//Debug.Log("DanceModeController.TrigPerformed() " + Time.time);
		// debug code:
		//print(FModManager.instance().getTrig());
	}

	/*
	 * Caclulates score, and handles intialization of HOT MODE if prerequisite is met
	 *
	 * Params status (event occured)
	 * - MISS_PRESS		key pressed but incorrectly
	 * - HIT			correct key pressed, correct timing
	 * - MISS			BeatMarker goes out without hit
	 * 
	 * HIT is worth of a point (+1)
	 * MISS and MISS_PRESS gives negative points as defined in Unity (Normal Mode Fail Multiplier
	 * HOT MODE hit is worth poins as defined in Unity GUI (Hot Mode Scor Multiplier)
	 * HOT MODE fail gives minus points as defined in Unity GUI (Hot Mode Fail Multiplier)
	 */
	private void UpdateScore(HitStatus hitStatus) {
		int hitMultiplier;
		int failMultiplier;
		if (hotMode) {
			hitMultiplier = hotModeScoreMultiplier;
			failMultiplier = hotModeFailMultiplier;
		} else {
			hitMultiplier = 1;
			failMultiplier = normalModeFailMultiplier;
		}
		
		switch(hitStatus) {
			case HitStatus.HIT:
				//if (successesInRow < hotModeRequisite && hotMode==false) {
				successesInRow++;
				//}
				correntHits++;
				score += baseScore * hitMultiplier;
				if (successesInRow > 0 && successesInRow % 10 == 0){
					BroadcastWatchDancePerception();
				}
				break;
			case HitStatus.MISS:
				errors++;
				score -= baseScore * failMultiplier;
				successesInRow=0;
				if (hotMode==true) {
					TerminateHotMode(true);
				}
				break;
			case HitStatus.MISS_PRESS:
				errors++;
				score -= baseScore * failMultiplier;
				successesInRow=0;
				if (hotMode==true) {
					TerminateHotMode(true);
				}
				break;
			default:
				Debug.LogError("DanceModeController.UpdateScore() invalid status received");
				return;
		}
		if(successesInRow == hotModeRequisite && hotMode==false) {
			//Debug.Log("calcScore(): entering HOT MODE");
			StartHotMode();	
		}
		// Score cannot go below zero
		if(score<0) { score=0; }
		// Score cannot exceed 100
		if(score>100) { score=100;}
		FModManager.SetMusicParameterValue(FModLies.MUSICPARAM_LIES_DANCE_POINTS,score/10);
		//Debug.Log("DanceModeController.UpdateScore() Score: " + score + " SuccessesInRow: " + successesInRow + " HOT MODE: " + hotMode );
	}
	
	/*
	 * Initializes HOT MODE
	 */
	private void StartHotMode() {
		//Debug.Log("DanceModeController.StartHotMode()");
		FModManager.setMusicSyncBPM(this,240);
		FModManager.Cue(FModLies.MUSICCUE_LIES_DANCE_HOTMODE);
		hotMode=true;
		hotModeBeat=0;
		successesInRow=0;
		
		// No strobo if graphics quolity is set below good 
		/*QualityLevel level = QualitySettings.currentLevel;
		switch(level) {
			case QualityLevel.Simple:
			case QualityLevel.Fast:
			case QualityLevel.Fastest:
				return;	
		}
		InvokeRepeating("strobo", 0.1f, 0.1f);*/
		//strobo 
	}
	
	private void BroadcastWatchDancePerception(){
		Perception perception = new WatchDancePerception(pc);
		PerceptionManager.BroadcastPerception(perception);
	}
	
	/*
	 * Terminates hot mode,
	 * Params:
	 * - failed==true: mode terminated with failure
	 * - failed==false: player played coreography throught succesfully
	 */
	private void TerminateHotMode(bool failed) {
		//Debug.Log("DanceModeController.TerminateHotMode(" + failed + ")");
		hotMode=false;
		
		FModManager.setMusicSyncBPM(this,120);
		
		// CurrentBeat has been updated double speed, not we need to rewind a bit
		currentBeat += hotModeBeat/2;
		successesInRow=0;	
		hotModeBeat=0;
		if (failed) {
			//Debug.Log("Hot mode terminated due to failure.");
			FModManager.Cue(FModLies.MUSICCUE_LIES_DANCE_BACK2VERSE);
			beatMarkers.Clear();

		} else {
			FModManager.StartEvent(FModLies.EVENTID_LIES_DANCE_HURRAA);
		}
		
		pcDanceAction.StopDancing();
	}
	
	/*
	 * Does things needed for terminateing dance mode
	 *
	 */
	private void TerminateDanceMode(bool playerTerminated) {
		
		
		float corlen=Time.time - debugDanceStartedTime;
		//Debug.Log("-- Song lenght:" + corlen);
		//Debug.Log("-- Score: " + score);
		
		// score : 0 ... 100
		
		if(playerTerminated & score > 60) {
			score = score - 10;	
		}

		impressionPushValue = (int)score / 10 - 5;
		if(impressionPushValue<-1) {
				impressionPushValue = -1;
		}
		Debug.Log("DanceModeController.TerminateDanceMode(): impressionPushValue=" + impressionPushValue);
		
		ImpressionPush impressionPush = new ImpressionPush("Exquisite", new NumericConstant(impressionPushValue), false, null);
		Perception perception = new ImpressionPerception(pc, impressionPush);
		witnesses = PerceptionManager.BroadcastPerceptionAndReturnWhoSaw(perception);
		Debug.Log("DanceModeController.DrawGUI(): state==STATES.SHOW_SCORE, hits=" + instance.correntHits + ", error=" + instance.errors + ", witnesses=" + instance.witnesses.Count);
		FModManager.RemoveMusicSyncListener(this);
		FModManager.Cue(FModLies.MUSICCUE_LIES_NIGHTCLUB_MUZAK);
		GameTime.SetGUIVisible(true);
		TaskHelp.Reveal();
		state = STATES.SHOW_SCORE;
		dialogEnabledTime = Time.time;
		
		pcDanceAction.StopDancing();
		
		CameraController.SetDefaultCameraAngle();
	}
	
	public static void DrawGUI(){
		switch (instance.state) {
			case STATES.SHOW_QUICK_HELP:
				DanceGUI.DrawHelpGUI();
				break;
			case STATES.MINIGAME:
				DanceGUI.DrawGUI(instance.hotMode, instance.beatMarkers, (int)instance.score, instance.hotModeBeat, instance.successesInRow, instance.hotModeRequisite);
				break;
			case STATES.SHOW_SCORE:
				DanceGUI.DrawScoreGUI((int)instance.impressionPushValue, instance.correntHits, instance.errors, instance.witnesses);
				break;
		}
	}
}

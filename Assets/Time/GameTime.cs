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

public class GameTime : MonoBehaviour {
	
	// If it is day or night or sunset time. GameTime only uses DAY and NIGHT 
	// AnimateSky and AnimateWater needs uses SUNSET. Sunset is not handled here, as 
	// the lenght of sunset depends on sunset animation length 
	public enum TIME_OF_DAY {DAY,NIGHT, SUNSET}; 

	/***********************************************************
	 *
	 * PUBLIC MEMBERS, these are set in Inspector
	 *
	 ***********************************************************/
	
	public TimeDef betStartsTime;
	public TimeDef betEndsTime;
	public int discoOpensHour;
	public int discoClosesHour;
	public int restaurantOpensHour;
	public int restaurantClosesHour;
	public int latestSleepingTimeHour;
	public int earliestSleepingTimeHour;

	public float realMinutesInGameHour = 1.0f;

	public int warnAboutSleepMinutes = 60;

	public const int sleepDuration = 8*60;
	public const int sexDuration = 2*60;

	// Added by PL 1.9.2008
	public int sunsetHour = 21;
	public int sunriseHour = 6;

	// if SkipTime() skips sunset time, sunset is triggered if skip amount is less
	// than "maxSkipAndSunset"
	public int maxSkipAndSunset = 20;

	public float timeUpdateInterval=1.0f;
	
	public TimeGUISettings timeGUISettings;
	
	
	/***********************************************************
	 *
	 * PRIVATE MEMBERS
	 *
	 ***********************************************************/

	private DateTime startingDateAndTime;
	private DateTime endingDateAndTime;
	
	//
	private PersistentGameTime persistentTime;
	//private float realTimeSecondsPassed;
	
	//
	private DateTime currentDateAndTime;
	
	// 
	//private int minutesSkipped;
	
	private static GameTime instance;
	
	
	private TIME_OF_DAY timeOfDay;
	
	private GameTimeDisplayer gameTimeDisplayer;
	
	private bool discoOpen;
	private bool restaurantOpen;
	
	private bool GuiVisible;
	
	// Contains classes that are interested when DAY changes to NIGHT
	// and vise versa or when time has been skipped
	private ArrayList timeOfDayListeners = null;
	
	/***********************************************************
	 *
	 * PUBLIC STATIC METHODS
	 *
	 ***********************************************************/
	
	public static void Save() {
		instance.persistentTime.Save();	
	}

	public static DateTime GetDateAndTime(){
		return instance.currentDateAndTime;
	}
	
	public static void SetGUIVisible(bool v) {
		instance.GuiVisible = v;
	}
	
	public static void SkipTimeInMinutes(float minutes){
		//Debug.Log("GameTime.SkipTime(" + minutes + "), Time before: " + instance.currentDateAndTime);
		if (minutes < 0){
			Debug.LogError("Trying to skip time backwards!");
		} else {
			instance.persistentTime.minutesSkipped += minutes;
			instance.SendTimeSkipped();
			instance.RecalculateTime();
		}
		//Debug.Log("GameTime.SkipTime(" + minutes + "), Time after: " + instance.currentDateAndTime);

	}
	
	public static void SkipTimeInRealSeconds(float seconds){
		SkipTimeInMinutes(seconds / instance.realMinutesInGameHour);
	}

	public static void RegisterTimeOfDayListener(TimeOfDayListener listener) {
		instance.timeOfDayListeners.Add(listener);
	}
	
	public static TIME_OF_DAY GetTimeOfDay() {
		TIME_OF_DAY retval = TIME_OF_DAY.DAY;
		DateTime now = instance.currentDateAndTime;
		if(now.Hour >= instance.sunsetHour) {
			retval = TIME_OF_DAY.NIGHT;
		}
		else if(now.Hour <= instance.sunriseHour) {
			retval = TIME_OF_DAY.NIGHT;
		}
		return retval;
	}
	
	public static float GetRealTimeSecondsPassed(){
		return instance.persistentTime.realTimeSecondsPassed;
	}
	
	public static bool IsSleepOpen() {
		DateTime now = GetDateAndTime();
		DateTime open = GetTmpDate(instance.earliestSleepingTimeHour, 0, now);
		DateTime close = GetTmpDate(instance.latestSleepingTimeHour, 0, now);
		if(open.CompareTo(now) < 0 && close.CompareTo(now) > 0 ) {
			return true;
		}
		else {
			return false;	
		}

	}
	
	public static bool IsRestaurantOpen() {
		DateTime now = GetDateAndTime();
		DateTime open = GetTmpDate(instance.restaurantOpensHour, 0, now);
		DateTime close = GetTmpDate(instance.restaurantClosesHour, 0, now);
		if(open.CompareTo(now) < 0 && close.CompareTo(now) > 0 ) {
			return true;
		}
		else {
			return false;	
		}

	}
	
	public static bool WarnAboutSleepTime() {
		DateTime now = GetDateAndTime();
		DateTime close = GetTmpDate(instance.latestSleepingTimeHour, 0, now);
		
		DateTime sleepTimeWarnTime = now.AddMinutes(instance.warnAboutSleepMinutes);
		if(sleepTimeWarnTime.CompareTo(close) > 0) {
			return true;	
		}
		else {
			return false;	
		}
	}
	
	public static bool IsDiscoOpen() {
		DateTime now = GetDateAndTime();
		 
		DateTime open = GetTmpDate(instance.discoOpensHour, 0, now);
		DateTime close = GetTmpDate(instance.discoClosesHour, 0, now);
		if(open.CompareTo(now) < 0 && close.CompareTo(now) > 0 ) {
			return true;
		}
		else {
			return false;	
		}
	}
		
	/***********************************************************
	 *
	 * OVERLOADED METHODS, from MonoBehaviour
	 *
	 ***********************************************************/

	
	public void Awake(){
		//Debug.Log("GameTime.Awake()");
		instance = this;
		persistentTime = new PersistentGameTime();
		
		startingDateAndTime = new DateTime(betStartsTime.year, betStartsTime.month, betStartsTime.day, betStartsTime.hour, betStartsTime.minutes, 0);
		endingDateAndTime = new DateTime(betEndsTime.year, betEndsTime.month, betEndsTime.day, betEndsTime.hour, betEndsTime.minutes, 0);
		timeOfDay = GameTime.GetTimeOfDay();
		
		timeOfDayListeners = new ArrayList();
		gameTimeDisplayer = new GameTimeDisplayer(endingDateAndTime, timeGUISettings);
		UpdateTime();
		
		discoOpen = IsDiscoOpen();
		restaurantOpen = IsRestaurantOpen();
		GuiVisible=true;	
	}
	
	public void Start(){
		//Debug.Log("GameTime.Start()");
		if(SaveLoad.SaveExits()) {
			PersistentGameTime tmp = (PersistentGameTime)SaveLoad.ReadSaveFile(PersistentGameTime.GetSaveFileName());
			if(tmp != null) {
				persistentTime = tmp;
				RecalculateTime();
			}
			else {
				SaveLoad.ResetSave();	
			}
		}
		InvokeRepeating("UpdateTime", timeUpdateInterval, timeUpdateInterval);
	}
	
	public void OnGUI(){
		if (!Pause.IsGUIRemoved() && GuiVisible){
			gameTimeDisplayer.Draw(currentDateAndTime, restaurantOpen, discoOpen, IsSleepOpen(), WarnAboutSleepTime());
		}
		
	}

	public void Update(){
		if ((Input.GetKey("left shift") || Input.GetKey("right shift")) && Input.GetKeyDown("s")){
			if (Pause.IsPaused()) {
				Debug.Log("GameTime.Update(): Cannot fast forward time when game is paused");
				return;	
			}
			if(currentDateAndTime.Hour >= latestSleepingTimeHour && currentDateAndTime.Hour < 6) {
				Debug.LogError("GameTime.Update() ERROR: fastforward time request catched while the night cut-scene should be playing");
				return;	
			}
			SkipTimeInMinutes(60);
		}
		
	}
	
	/***********************************************************
	 *
	 * PRIVATE METHODS
	 *
	 ***********************************************************/	

	
	
	private void UpdateTime() {
		if (!Pause.IsPaused()){
			persistentTime.realTimeSecondsPassed += timeUpdateInterval;
			RecalculateTime();
			if (currentDateAndTime >= endingDateAndTime){
				// Time ran aout. Abby lost the bet.
				
				// Reset the save
				SaveLoad.ResetSave();
				
				// Play Game over cut-scene
				CutScenePlayer.Play("Lost Bet");
			}
		}
		
		bool dOpen = IsDiscoOpen();
		//Debug.Log("GameTime.UpdateTime(): disco open: " + dOpen);
		if(dOpen!=discoOpen) {
			//Debug.Log("-- Setting Disco Door Stuff");
			discoOpen = dOpen;
			NightclubDoorSounds.NightclubOpen(dOpen);
			ClosedSigns.SetDiscoOpen(dOpen);
		}
		bool rOpen = IsRestaurantOpen();
		//Debug.Log("GameTime.UpdateTime(): restaurant open: " + rOpen);
		if(rOpen != restaurantOpen) {
			//Debug.Log("-- Setting Restaurant Doors Stuff");
			restaurantOpen = rOpen;
			RestaurantDoorSounds.RestaurantOpen(rOpen);
			ClosedSigns.SetRestaurantOpen(rOpen);
		}
		
	}
	
	public static float GetMinutesPassed(){
		return instance.persistentTime.realTimeSecondsPassed / instance.realMinutesInGameHour + instance.persistentTime.minutesSkipped;
	}
	
	public static int GetNightsPassed(){
		if (instance.currentDateAndTime.Hour < instance.latestSleepingTimeHour){
			return instance.currentDateAndTime.Day - instance.startingDateAndTime.Day - 1;
		} else {
			return instance.currentDateAndTime.Day - instance.startingDateAndTime.Day;
		}
	}
	
	private static int DateDiff(DateTime d1, DateTime d2) {
		if(d1.Day>d2.Day) {
			Debug.LogError("GameTimeDisplayer ERROR, cannot calculate days left");
			return -1;	
		}
		return d2.Day - d1.Day;
	}
	
	public static int GetDaysLeft(){
		return DateDiff(instance.currentDateAndTime, instance.endingDateAndTime);
	}
	
	private void RecalculateTime(){
		float minutesPassed = GetMinutesPassed();
		currentDateAndTime = startingDateAndTime + new TimeSpan(0, (int)minutesPassed, 0);
		
		// Let see if it is sunrise or sunset
		TIME_OF_DAY currTimeOfDay = GameTime.GetTimeOfDay();
		if(timeOfDay != currTimeOfDay) {
			timeOfDay = currTimeOfDay;
			switch(timeOfDay) {
				case TIME_OF_DAY.DAY:
					SendSetDay();
					break;
				case TIME_OF_DAY.NIGHT:
					// We only trigger sunset if we haven't skipped over subset starting time too much
					if(sunsetHour == currentDateAndTime.Hour && currentDateAndTime.Minute < maxSkipAndSunset) {
						SendSetSunset();
					}
					else {
						SendSetNight();
					}					
					break;
				default:
					Debug.LogError("GameTime.UpdateTime(): Unreconized TIME_OF_DAY");
					break;
			}	
		}
	}
		
	private void SendSetDay() {
		Debug.Log("GameTime.SendSetDay() Days left: " + GetDaysLeft());
		foreach(TimeOfDayListener listener in timeOfDayListeners) {
			listener.SetDay();
		} 
	}
	
	private void SendSetNight() {
		//Debug.Log("GameTime.SendSetDay() " + currentDateAndTime.ToShortTimeString());
		foreach(TimeOfDayListener listener in timeOfDayListeners) {
			listener.SetNight();
		} 
	}
	
	private void SendSetSunset() {
		//Debug.Log("GameTime.SendSetSunset() " + currentDateAndTime.ToShortTimeString());
		foreach(TimeOfDayListener listener in timeOfDayListeners) {
			listener.SetSunset();
		} 
	}

	private void SendTimeSkipped() {
		//Debug.Log("GameTime.SendTimeSkipped() " + currentDateAndTime.ToShortTimeString());
		foreach(TimeOfDayListener listener in timeOfDayListeners) {
			listener.TimeSkipped();
		} 

	}

	// TODO: Simplified This...
	private static DateTime GetTmpDate(int  hour, int minutes, DateTime now) {
		// Return DateTime usable for checking if now is after or before given hour and minutes
		// Complexity comes, because we have disco that is open between, e.g., 20 to 04. 
		// We assume that open times are not passing 06 (e.g., there is no open times such as
		// 23 to 07). There is easier way for this, but my brain refuses to figure it out. 
		if(hour < 6) {
			// Closing time is after midnight...
			if(now.Hour < 6) {
				// and now is after midnight, no correction to Day
				return new DateTime(now.Year, now.Month, now.Day,hour,minutes, 0);
			}
			else {
				// we are still in previous day, need to add one to current day to get
				// comparison right
				return new DateTime(now.Year, now.Month, now.Day+1,hour,minutes, 0);
			}
		}
		else {
			// closing time is before midnight
			if(now.Hour < 6) {
				// and now is after mifnight, so we need to reduce one from the day
				return new DateTime(now.Year, now.Month, now.Day-1,hour,minutes, 0);
			}
			else {
				// no correction...
				return new DateTime(now.Year, now.Month, now.Day,hour,minutes, 0);
			}
		}
	} 

	
}

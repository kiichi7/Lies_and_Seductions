/**********************************************************************
 *
 * CLASS CharacterState
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

public class CharacterState : MonoBehaviour {

	private const float SOBERING_ONE_STEP_DURATION_IN_IN_GAME_MINUTES = 30.0f;
	private const float AFTER_PC_DIALOGUE_IGNORE_TIME_IN_SECONDS = 10.0f;
	private const float AFTER_NPC_AREA_CHANGE_IGNORE_TIME_IN_SECONDS = 2.0f;

	PersistentCharacterState state;

	private Item item;
	private FollowAction followerAction;
	private GameObject oneLinerTarget;
	private Seat currentSeat;
	private float lastPCDialogueEndedInSeconds;
	private float lastAreaChangeInSeconds;
	
	public int money = 200;
	public DateTime blindDateStartingTime;
	public TimeSpan blindDateLength;
	
	private ArrayList statusListeners;
	
	public enum Task { DRINK, SEX, POKER, DANCE, NONE };
	
	public class TaskInfo {
		public GameObject npc;
		public Task task;
		public TaskInfo(Task g, GameObject o) {
			npc = o;
			task = g;	
		}
	}
	
	public TaskInfo currentTask;
	
	public void Awake(){
		//Debug.Log("CharacterState.Awake()");
		//Debug.Log("-- " + gameObject.name);
		
		statusListeners = new ArrayList();
		state = new PersistentCharacterState();
		
		item = null;
		followerAction = null;
		oneLinerTarget = null;
		currentSeat = null;
		lastPCDialogueEndedInSeconds = - 100.0f;
		state.lastSoberingMinute = 0;
		state.nightsSlept = 0;
		state.name = name;
		state.money = money;
		state.drunkness = 0.0f;
		state.justHadSex = false;
		state.startedBlindDate = false;
		state.variableSet = null;
		blindDateStartingTime = new DateTime(1, 1, 1);
		blindDateLength = new TimeSpan(0, 0, 0);
		currentTask = new TaskInfo(Task.NONE, null);		
	}

	public void Start() {
		//Debug.Log("CharacterState.Start() for " + gameObject.name);
		if(SaveLoad.SaveExits() && CharacterManager.IsPCOrMajorNPC(gameObject)) {
			Load();
			BroadcastDrunknessChanged(state.drunkness);
		}
		// Sobering up is done in SoberUp() call, we do not need to do it in every frame (Update())
		InvokeRepeating("SoberUp", 10.0f, 10.0f);
		// No Update(), LateUpdate(), FixedUpdate(), and OnGUI() calls for this scripts.
		enabled=false;
	}

	public void RegisterStatusListener(StatusListener listener) {
		statusListeners.Add(listener);
	}

	public void BroadcastDrunknessChanged(float d) {
		foreach(StatusListener listener in statusListeners) {
			listener.DrunknessValueChanged(d);
		}
	}

	public void Load() {
		//Debug.Log("CharacterState.Load()");
		PersistentCharacterState tmp = (PersistentCharacterState)SaveLoad.ReadSaveFile(PersistentCharacterState.GetSaveFileName(state.name));
		if(tmp != null) {
			state = tmp;
		}
		else {
			Debug.LogError("CharacterState.Load(): Failed to load stated file for " + state.name);
			SaveLoad.ResetSave();	
		}
		
		// Now we need to do some fixing to make thigs OK...
		if(state.startedBlindDate) {
			//CharacterState chrisState = (CharacterState)CharacterManager.GetMajorNPC("Chris").GetComponent("CharacterState");
			//chrisState.StartBlindDate(new TimeSpan(2, 30, 0));
			StartBlindDate(new TimeSpan(2, 30, 0));
		}
		if (JustHadSex()){
			//Debug.Log("Just had sex!");
			ActionRunner actionRunner = GetComponent(typeof(ActionRunner)) as ActionRunner;
			actionRunner.FullReset(new StartDialogueAction(gameObject, CharacterManager.GetPC(), name, "after_sex"));
			SetJustHadSex(false);
		}
	}
	
	public void Save() {
		state.Save();
	}

	
	/*public void Start() {

	}*/
	
	
	public void AreaChanged() {
		lastAreaChangeInSeconds = Time.time;
	}
		
	public Area GetCurrentArea(){
		NavmeshPolygon currentPolygon;
		if (currentSeat == null){
			currentPolygon = NavigationMesh.FindEnclosingPolygon(transform.position);
		} else {
			currentPolygon = NavigationMesh.FindEnclosingPolygon(currentSeat.GetWaypoint().transform.position);
		}
		return currentPolygon.GetArea();
	}
	
	public Item GetItem(){
		return item;
	}
	
	public VariableSet GetVariableSet(){
		if (state.variableSet == null){
			state.variableSet = new VariableSet();
		}
		return state.variableSet;
	}
	
	public int GetMoney(){
		return state.money;
	}
	
	public float GetDrunkness(){
		return state.drunkness;
	}
	
	public GameObject GetFollower(){
		if (followerAction == null){
			Debug.LogError("CharacterState.GetFollower(): Error: No one's following the PC!");
		}
		return followerAction.GetActor();
	}
	
	public bool HasSexPartner(){
		return (followerAction != null && followerAction.WillHaveSex());
	}
	
	public bool HasDancePartner(){
		return (followerAction != null && followerAction.WillDance());
	}
	
	public bool HasPokerPartnerWithoutMoney(){
		return (followerAction != null && followerAction.WillPlayPokerWithoutMoney());
	}
	
	public bool HasPokerPartnerWithMoney(){
		return (followerAction != null && followerAction.WillPlayPokerWithMoney());
	}

	public GameObject GetOneLinerTarget(){
		return oneLinerTarget;
	}
	
	public Seat GetCurrentSeat(){
		return currentSeat;
	}
	
	public bool IsOnBlindDate(){
		return state.startedBlindDate && GameTime.GetDateAndTime() < blindDateStartingTime + blindDateLength;
	}
	
	public bool JustHadSex(){
		return state.justHadSex;
	}
	
	public string GetSituation(){
		if (IsOnBlindDate()){
			EndBlindDate();
			return "blind_date";
		} else {
			return "normal";
		}
	}
	
	public int GetSleepDept(){
		return Mathf.Max(0, GameTime.GetNightsPassed() - state.nightsSlept);
	}
	
	public bool IsIgnoringPC(){
		// We ignore PC after area change for a while to reduce change that NPC blocks the waypoint used to enter to the
		// area.
		bool ignoreAfterAreaChange = Time.time < lastAreaChangeInSeconds + AFTER_NPC_AREA_CHANGE_IGNORE_TIME_IN_SECONDS;
		// Just after dalogue has ended NPC should not stop next to PC so...
		bool ignoreAfterDialogueEnd = Time.time < lastPCDialogueEndedInSeconds + AFTER_PC_DIALOGUE_IGNORE_TIME_IN_SECONDS;
		
		return ignoreAfterAreaChange || ignoreAfterDialogueEnd;
	}
	
	/*public void SetCurrentArea(Area area){
		this.currentArea = area;
	}*/
	
	public void TaskCompleted() {
		Debug.Log("CharacterState.GoalReached()" + currentTask.task + " npc=" + currentTask.npc.name);
		currentTask.task = Task.NONE;
		currentTask.npc = null;
	}
	
	public void SetTask(Task g, GameObject target) {
		if(target) {	
			Debug.Log("CharacterState.SetTask(): setting goal, goal=" + g + " npc=" + target.name);
		} else {
			Debug.Log("CharacterState.SetTask(): setting goal, goal=" + g);	
		}
			
		if(currentTask.task != Task.NONE) {
			Debug.Log("CharacterState.TaskCompleted(): Player cancelled a task, Sending impression Unreliable to " + currentTask.npc.name);
			PerceptionManager.SendImpression(currentTask.npc, "Unreliable", 1);	
		}
		currentTask.task =  g;
		currentTask.npc = target;
	}
	
	public void SetItem(Item item){
		//if (item != null){
		//	Debug.Log(name + " setting item to " + item.name);
		//} else {
		//	Debug.Log(name + " setting item to null");
		//}
		//CharacterAnimator animator = (CharacterAnimator)GetComponent("CharacterAnimator");
		if (this.item != null){
			//animator.StopAnimation("hold_" + this.item.name);
			GameObject.Destroy(this.item.gameObject);
		}
			
		this.item = item;
		if (item != null){
			//animator.StartAnimation("hold_" + item.name, WrapMode.Loop, new string[]{CharacterAnimator.FindShoulder(transform, item.name).name});
			Transform hand = CharacterAnimator.FindChild(gameObject, item.name + "_hook");
			item.SetHand(hand, gameObject == CharacterManager.GetPC());
		}
	}
	
	public void SetMoney(int money){
		//Debug.Log("CharacterState.SetMoney(" + money + ") for " + name);
		state.money = money;
	}
	
	public void AddMoney(int amount){
		state.money += amount;
		//Debug.Log("CharacterState.AddMoney(" + amount + ") for " + name + " money=" + state.money);
	}
	
	public void RemoveMoney(int amount){
		state.money -= amount;
		//Debug.Log("CharacterState.RemoveMoney(" + amount + ") for " + name + " money=" + state.money);
	}
	
	public void Drink(int alcoholLevel){
		//Debug.Log("Drinking: " + alcoholLevel);
		state.drunkness += alcoholLevel;
		BroadcastDrunknessChanged(state.drunkness);
	}
	
	public void SetFollowerAction(FollowAction followerAction){
		if (this.followerAction != null){
			this.followerAction.Replaced();
		}
		this.followerAction = followerAction;
	}
	
	public void SetOneLinerTarget(GameObject oneLinerTarget){
		this.oneLinerTarget = oneLinerTarget;
		/*if(oneLinerTarget) {
			state.oneLinerTarget = oneLinerTarget.name;
		}
		else {
			state.oneLinerTarget = "";
		}*/
	}
	
	public bool IsSeated() {
		return this.currentSeat != null;
	}
	
	public void SetCurrentSeat(Seat currentSeat){
		//Debug.Log("CharacterState.SetCurrentSeat(): to " + currentSeat + " from " + this.currentSeat);
		CharacterAnimator animator = GetComponent(typeof(CharacterAnimator)) as CharacterAnimator;
		if (this.currentSeat != null){
			this.currentSeat.SetTaken(false);
			//animator.StopAnimation("sit_" + this.currentSeat.GetType());
			//Debug.Log("CharacterState.SetCurrentSeat()" + name + " Stopping Sitting Animation (sit_" + this.currentSeat.GetType() + ")");
		}
		this.currentSeat = currentSeat;
		if (currentSeat != null){
			currentSeat.SetTaken(true);
		}
	}
	
	public void NightSlept(){
		state.nightsSlept++;
	}
	
	private void SoberUp(){
		//Debug.Log("CharacterState.SoberUp()");
		float minutesPassedSinceLastSobering = GameTime.GetMinutesPassed() - state.lastSoberingMinute;
		if (minutesPassedSinceLastSobering > 0){
			state.drunkness = Math.Max(0.0f, state.drunkness - minutesPassedSinceLastSobering / SOBERING_ONE_STEP_DURATION_IN_IN_GAME_MINUTES);
			BroadcastDrunknessChanged(state.drunkness);
			state.lastSoberingMinute = GameTime.GetMinutesPassed();
		}
	}
		
	public void StartBlindDate(TimeSpan length){
		state.startedBlindDate = true;
		blindDateStartingTime = GameTime.GetDateAndTime();
		blindDateLength = length;
	}
	
	private void EndBlindDate(){
		state.startedBlindDate = false;
	}
	
	public void SetJustHadSex(bool justHadSex){
		state.justHadSex = justHadSex;
	}
	
	public void PCDialogueEnded(){
		lastPCDialogueEndedInSeconds = Time.time;
	}
	
	public void ResetState(){
		SetItem(null);
		SoberUp();
		BroadcastDrunknessChanged(state.drunkness);
		SetFollowerAction(null);
		SetOneLinerTarget(null);
	}
}

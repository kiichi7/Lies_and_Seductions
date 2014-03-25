/**********************************************************************
 *
 * CLASS CharacterAnimator
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

public class ActionRunner : MonoBehaviour {
	
	public RoutineCreator routineCreator;

	private Action currentRoutine;
	private Action nextRoutine;
	
	public void Start() {
		ResetRoutine();
	}
	
	public void ResetRoutine(Action nextRoutine, bool immediately){
		Debug.Log("ActionRunner.ResetRoutine(): Resetting " + name + "'s Routine to: " + nextRoutine);
		if (currentRoutine != null && !currentRoutine.IsFinished()){
			currentRoutine.EndASAP(immediately);
		}
		/*if (nextRoutine != null){
			this.nextRoutine = nextRoutine;
		} else {
			this.nextRoutine = routineCreator.CreateRoutine();
		}*/
		if (this.nextRoutine != null){
			this.nextRoutine.Cancelled();
		}
		this.nextRoutine = nextRoutine;
		if (currentRoutine == null){
			AdvanceToNextRoutine();
		}
		if (currentRoutine == null){
			Debug.Log("ActionRunner.ResetRoutine(): currentRoutine null after reset");
		}
	}
	
	private void AdvanceToNextRoutine(){
		if (nextRoutine == null){
			nextRoutine = routineCreator.CreateRoutine();
		}
		currentRoutine = nextRoutine;
		nextRoutine = null;
	}
	
	public void ResetRoutine(bool immediately){
		ResetRoutine(null, immediately);
	}
	
	public void ResetRoutine(){
		ResetRoutine(null, false);
	}
	
	public void FullReset(Action routine){
		Debug.Log("ActionRunner.FullReset(): " + name);
		if (gameObject != CharacterManager.GetPC()){
			ResetRoutine(routine, true);
			while (!currentRoutine.IsFinished()){
				//Debug.Log("ActionRunner.FullReset(): Looping currentRoutine: " + currentRoutine);
				currentRoutine.UpdateAction();
			}
		}
		CharacterState state = GetComponent("CharacterState") as CharacterState;
		state.ResetState();
		CharacterAnimator animator = GetComponent("CharacterAnimator") as CharacterAnimator;
		animator.ResetAnimations();
		if (gameObject != CharacterManager.GetPC()){
			//Debug.Log("ActionRunner.FullReset(): AdvanceToNextRoutine");
			AdvanceToNextRoutine();
			//Debug.Log("FF: ActionRunner.FullReset: FastForwardMovement: " + name);
			currentRoutine.FastForwardMovement();
			//Debug.Log("ActionRunner.FullReset(): FullReset done");
		}
	}
	
	public void FullReset(){
		FullReset(null);
	}
	

	public void Update () {
		//JCsProfilerMethod pm = JCsProfiler.Instance.StartCallStopWatch("ActionRunner", gameObject.name, "Update");
		if (!Pause.IsPaused()){
			//Debug.Log("ActionRunner.Update: " + name);
			currentRoutine.UpdateAction();
			if (currentRoutine.IsFinished()){
				AdvanceToNextRoutine();
			}
		}
       //pm.CallIsFinished();
	}
		
	public void OnGUI(){
		if (!Pause.IsGUIRemoved()){
			GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
			
			//!!! null reference exeptions from here!!!
			if(currentRoutine!=null) {
				currentRoutine.OnActionGUI();
			}
			else {
				Debug.LogError("ActionRunner.OnGUI(): currentRoutine is null");	
			}
		}
	}
		
	public bool CanStartDialogue(bool withPC){
		return currentRoutine.CanStartDialogue(withPC) && (nextRoutine == null || nextRoutine.CanStartDialogue(withPC));
	}
	
	public void StartQuickReaction(Action quickReaction){
		currentRoutine.StartQuickReaction(quickReaction);
	}
	
	public bool ShouldNPCStop(GameObject npc){
		return currentRoutine.ShouldNPCStop(npc);
	}
}

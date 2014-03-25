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

public class PCEndingAction : AbstractOneRoundAction {

	private Ending ending;
	
	public PCEndingAction(GameObject actor, Ending ending) : base(actor){
		this.ending = ending;
	}
	
	protected override void UpdateOnlyRound(){
		string cutSceneName = "";
		switch (ending){
		case Ending.VICTORY_LOVE:
			cutSceneName = "Victory but in Love";
			break;
		case Ending.VICTORY_NO_LOVE:
			cutSceneName = "Victory";
			break;
		case Ending.LOST_BET:
			cutSceneName = "Lost Bet";
			break;
		}
		Debug.Log("PCEndingActionUpdateOnlyRound(): ending: " + cutSceneName);
		CutScenePlayer.Play(cutSceneName);
		SaveLoad.ResetSave();
	}
}

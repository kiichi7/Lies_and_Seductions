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

public class WatchDanceAction : AbstractNestingAction {

	private GameObject pc;
	
	public WatchDanceAction(GameObject actor) : base(actor){
		InitInteractionInfo(false, false);
		
		pc = CharacterManager.GetPC();
	}
	
	protected override void UpdateFirstRound(){
		// NPCs repeat the line repeatly.
		// Also this action seems to cancel Emma's dance action, which is not cool 
		if (CharacterManager.IsMajorNPC(actor)){
			QueueAction(new OneLinerAction(actor, "good dancing", pc));
		}
		QueueAction(new TurnToFaceAction(actor, pc));
	}
	
	protected override Action CreateDefaultAction(){
		if (!DanceModeController.IsFinished()){
			return new IdleAction(actor, 2.0f, null, 0.0f, false);
		} else {
			return null;
		}
	}
	
	/*protected override bool IsCompleted(){
		return DanceModeController.IsFinished() && base.IsCompleted();	
	}*/
}

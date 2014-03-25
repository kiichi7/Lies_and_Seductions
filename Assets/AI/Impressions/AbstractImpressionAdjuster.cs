/**********************************************************************
 *
 * CLASS AbstractImpressionAdjuster
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

public abstract class AbstractImpressionAdjuster : ImpressionAdjuster {

	protected string impression;
	protected NumericOperand operand;
	protected bool excludeSpeaker;
	protected GameObject recepient;
	
	public AbstractImpressionAdjuster(string impressionName, NumericOperand operand, bool excludeSpeaker, string recepientName){		
		impression = AILoader.GetImpression(impressionName);
		if (impression == null){
			throw new InvalidScriptException("Invalid impression name: " + impressionName);
		}
		this.operand = operand;
		this.excludeSpeaker = excludeSpeaker;
		if (recepientName != null){
			recepient = CharacterManager.GetMajorNPC(recepientName);
			if (recepient == null){
				throw new InvalidScriptException("Invalid NPC name: " + recepientName);
			} 
		} else {
			recepient = null;
		}
	}
	
	public string GetImpression(){
		return impression;
	}

	public bool AppliesTo(GameObject npc, GameObject speaker){
		if (recepient != null && recepient == npc){
			return false;
		} else if (excludeSpeaker && npc == speaker){
			return false;
		} else {
			return true;
		}
	}
	
	public void Perform(DialogueController dialogueController, GameObject actor){
		//Debug.Log("AbstractImpressionAdjuster.Perform(): " + actor.name);
		PerceptionManager.BroadcastPerception(new ImpressionPerception(actor, this));
	}
	
	public abstract void AdjustCurrentImpression(CurrentImpression currentImpression);
}
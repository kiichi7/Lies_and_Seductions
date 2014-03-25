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

public class GetImpression : NumericOperand {
	
	//private Impression impression;
	string impression;
	private GameObject npc;
	
	public GetImpression(string impressionName, string npcName){
		impression = AILoader.GetImpression(impressionName);
		if (impression == null){
			throw new InvalidScriptException("Invalid impression name: " + impressionName);
		}
		npc = CharacterManager.GetMajorNPC(npcName);
		if (npc == null){
			throw new InvalidScriptException("Invalid NPC name:" + npcName);
		}
	}
	
	public object GetValue(){
		ImpressionMemory impressionMemory = (ImpressionMemory)npc.GetComponent("ImpressionMemory");
		return impressionMemory.GetCurrentImpressionStrength(impression);
	}
}


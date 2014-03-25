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

public class KissConsequenceNode : AbstractActionConsequenceNode {

	private GameObject partner;

	public KissConsequenceNode(GameObject actor, GameObject partner) : base(actor, false){
		this.partner = partner;
	}

	protected override Action[] CreateActions(){
		Action[] actions = new Action[2];
		RespondToKissAction respondToKissAction = new RespondToKissAction(CharacterManager.GetPC(), actor);
		actions[0] = respondToKissAction;
		actions[1] = new KissAction(actor, CharacterManager.GetPC(), respondToKissAction);
		return actions;
	}
}

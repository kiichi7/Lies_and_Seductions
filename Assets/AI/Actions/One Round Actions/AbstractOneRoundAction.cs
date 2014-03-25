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

public abstract class AbstractOneRoundAction : AbstractAction {

	private bool completed;

	public AbstractOneRoundAction(GameObject actor) : base(actor){
		completed = false;
	}
	
	protected override void UpdateAnyRound(){
		UpdateOnlyRound();
		completed = true;
	}
	
	protected sealed override void UpdateFirstRound(){
	}
	
	protected sealed override void UpdateLastRound(bool interrupted){
	}
	
	protected abstract void UpdateOnlyRound();
	
	protected override bool IsCompleted(){
		return completed;
	}
}

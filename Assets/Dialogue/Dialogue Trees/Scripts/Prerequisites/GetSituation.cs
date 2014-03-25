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

public class GetSituation : StringOperand {

	private Conversation conversation;

	public GetSituation(Conversation conversation){
		if (conversation == null){
			Debug.LogError("GetSituation.GetSituation(): NULL CONVERSATION!");
		}
		this.conversation = conversation;
	}
	
	public object GetValue(){
		if (conversation == null){
			Debug.LogError("GetSituation.GetValue(): NULL CONVERSATION!");
		}
		return conversation.GetSituation();
	}
}

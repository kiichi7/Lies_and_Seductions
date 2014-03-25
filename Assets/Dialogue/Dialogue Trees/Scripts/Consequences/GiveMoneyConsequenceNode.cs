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

public class GiveMoneyConsequenceNode : AbstractActionConsequenceNode {

	private string recipientName;
	private NumericOperand amount;

	public GiveMoneyConsequenceNode(GameObject actor, string recipientName, NumericOperand amount) : base(actor, false){
		this.recipientName = recipientName;
		this.amount = amount;
	}

	protected override Action[] CreateActions(){
		Action[] actions = new Action[1];
		actions[0] = new GiveMoneyAction(actor, recipientName, (int)amount.GetValue());
		return actions;
	}
}

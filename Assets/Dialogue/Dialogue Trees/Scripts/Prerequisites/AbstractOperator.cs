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

public abstract class AbstractOperator {
	
	protected Operand[] operands;
	
	public AbstractOperator(){
		operands = new Operand[0];
	}
	
	public AbstractOperator(Operand operand0){
		operands = new Operand[1];
		operands[0] = operand0;
	}
	
	public AbstractOperator(Operand operand0, Operand operand1){
		operands = new Operand[2];
		operands[0] = operand0;
		operands[1] = operand1;
	}
	
	public abstract object GetValue();
}

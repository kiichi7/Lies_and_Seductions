/**********************************************************************
 *
 * CLASS CurrentImpression
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

[Serializable()]
public class CurrentImpression {

	public int strength;
	
	public CurrentImpression(){
		strength = 0;
	}
	
	public int GetStrength(){
		return strength;
	}
	
	public void SetStrength(int strength){
		this.strength = strength;
	}
	
	public void AdjustStrength(int power){
		//Debug.Log("Adjusting impression.");
		int powerAbs = Math.Abs(power);
		int highValueAbs = powerAbs + 2;
		int middleValueAbs = powerAbs;
		int lowValueAbs = Math.Max(1, powerAbs - 2);
		int strengthComparable = strength * Math.Sign(power);
		if (strengthComparable >= highValueAbs){
			strengthComparable += 0;
		} else if (strengthComparable >= middleValueAbs){
			strengthComparable += 1;
		} else {
			strengthComparable += lowValueAbs;
			if (strengthComparable > middleValueAbs){
				strengthComparable = middleValueAbs;
			}
		}
		strength = strengthComparable * Math.Sign(power);
	}
}

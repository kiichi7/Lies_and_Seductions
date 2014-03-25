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

public class ItemCosts : MonoBehaviour {

	public int beerCost;
	public int whiskeyCost;
	public int drinkCost;
	public int chocolateCost;
	public int flowersCost;
	
	private static ItemCosts instance;
	
	public void Awake(){
		if (instance){
			Debug.LogError("Two ItemCosts instances!");
		}
		instance = this;
		enabled = false;
	}
	
	public static int GetCost(string itemName){
		if (itemName.Equals("beer")){
			return instance.beerCost;
		} else if (itemName.Equals("whiskey")){
			return instance.whiskeyCost;
		} else if (itemName.Equals("drink")){
			return instance.drinkCost;
		} else if (itemName.Equals("chocolate")){
			return instance.chocolateCost;
		} else if (itemName.Equals("flowers")){
			return instance.flowersCost;
		} else {
			Debug.LogError("There's no item called " + itemName);
			return 0;
		}
	}

}

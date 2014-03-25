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

public class ClosedSigns : MonoBehaviour {

	public GameObject restaurantMainDoorSign;
	public GameObject restaurantBackDoorSign;
	public GameObject discoDoorSign;
	public Material restaurantMainDoorSignMaterial;
	public Material restaurantBackDoorSignMaterial;
	public Material discoDoorSignMaterial;
	public Material noSignMaterial;

	private static ClosedSigns instance;
	
	public void Awake(){
		instance = this;
		//discoDoorSign.renderer.material = discoDoorSignMaterial;
		//restaurantMainDoorSign.renderer.material = restaurantMainDoorSignMaterial;
		//restaurantBackDoorSign.renderer.material = restaurantBackDoorSignMaterial;
	}
	
	public static void SetRestaurantOpen(bool open){
		Debug.Log("ClosedSigns.SetRestaurantOpen(" + open + ")");
		if (open){
			instance.restaurantMainDoorSign.renderer.material = instance.noSignMaterial;
			instance.restaurantBackDoorSign.renderer.material = instance.noSignMaterial;
			//instance.restaurantMainDoorSign.renderer.enabled = false;
			//instance.restaurantBackDoorSign.renderer.enabled = false;
		} else {
			instance.restaurantMainDoorSign.renderer.material = instance.restaurantMainDoorSignMaterial;
			instance.restaurantBackDoorSign.renderer.material = instance.restaurantBackDoorSignMaterial;
			//instance.restaurantMainDoorSign.renderer.enabled = true;
			//instance.restaurantBackDoorSign.renderer.enabled = true;
		}	
	}
	
	public static void SetDiscoOpen(bool open){
		Debug.Log("ClosedSigns.SetDiscoOpen(" + open + ")");
		if (open){
			instance.discoDoorSign.renderer.material = instance.noSignMaterial;
			//instance.discoDoorSign.renderer.enabled = false;
		} else {
			instance.discoDoorSign.renderer.material = instance.discoDoorSignMaterial;
			//instance.discoDoorSign.renderer.enabled = true;
		}
	}

}

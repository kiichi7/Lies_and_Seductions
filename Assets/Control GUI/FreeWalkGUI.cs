/**********************************************************************
 *
 * CLASS FreeWalkGUI
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

public class FreeWalkGUI : MonoBehaviour {

	public Texture2D hotSpotCursor;
	
	private static int HOT_SPOT_LAYER_NUMBER = 8;
	
	private static FreeWalkGUI instance;
	
	public void Awake(){
		instance = this;
		enabled=false;
	}
	
	public static void UpdateCursor(){
		if (GetHotSpotUnderCursor() != null){
			MouseHandler.UseCursor(instance.hotSpotCursor);
		} else {
			MouseHandler.UseDefaultCursor();
		}
	}
	
	private static GameObject GetHotSpotUnderCursor(){
		int hotSpotLayerMask = 1 << HOT_SPOT_LAYER_NUMBER;
		GameObject objectUnderCursor = MouseHandler.GetObjectUnderCursor(hotSpotLayerMask);
		if (objectUnderCursor != null && objectUnderCursor.layer == HOT_SPOT_LAYER_NUMBER && Vector3.Distance(objectUnderCursor.transform.position, CharacterManager.GetPC().transform.position) < DistanceConstants.HOT_SPOT_RANGE){
			return objectUnderCursor;
		} else {
			return null;
		}
	}
	
	public static GameObject GetClickedHotSpot(){
		if (MouseHandler.IsLeftClicked()){
			return GetHotSpotUnderCursor();
		} else {
			return null;
		}
	}

	public static void DrawGUI(){
	}
}

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

public class Pause : MonoBehaviour {

	private static bool isPaused;
	private static bool isGUIRemoved;
	
	public static bool IsPaused(){
		return isPaused;
	}
	
	public static bool IsGUIRemoved(){
		return isGUIRemoved;
	}
	
	public void Update(){
		if (Input.GetKeyDown("p")){
			isPaused = !isPaused;
			isGUIRemoved = false;
		}
	}

	public static void SetPausedWithoutGUI(bool isPaused) {
		Pause.isPaused = isPaused;
		Pause.isGUIRemoved = false;
	}

	public static void SetPaused(bool isPaused){
		Pause.isPaused = isPaused;
		Pause.isGUIRemoved = isPaused;
	}
}

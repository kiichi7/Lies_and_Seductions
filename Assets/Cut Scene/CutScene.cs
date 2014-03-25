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
 ***********************************************************************
 * Usage:
 * - public CutScene  cs;
 * 		CutSceneScreen s = cs.GetFirts();
 * 	 ...
 * 		cs.GetNext();
 * 		if(cs==null) { // cut-scene ended ...
 *
 * - set cut-scene details in inspectror
 *
 ******************************************************************/

using UnityEngine;
using System.Collections;

[System.Serializable]
public class CutScene {

	/**************************************************************
	 *
	 * PUBLIC VARIABLES, THESE ARE SET IN THE UNITY3D INSPECTOR
	 *
	 **************************************************************/


	// Name of cut-scene
	public string name;
	
	// Screens to show
	public CutSceneScreen []screens;
	
	public bool syncFromMusic=false;
	public bool gameOverAfterThis=false;
	public string nextCutScene="";
	public string archievement="";
	
	/**************************************************************
	 *
	 * PRIVATE VARIABLES 
	 *
	 **************************************************************/
	
	private int currScreen=0;
	
	/**************************************************************
	 *
	 * PUBLIC METHODS 
	 *
	 **************************************************************/
	
	public CutSceneScreen GetFirst() {
		//Debug.Log("CutScene.GetFirst()");
		currScreen=0;
		return 	screens[currScreen];
	}
	
	public CutSceneScreen GetNext() {
		currScreen++;
		//Debug.Log("CutScene.GetNext() screen number: " + currScreen);
		if(screens.Length <= currScreen) {
			return null;
		}
		else {
			return screens[currScreen];
		}
	}
}

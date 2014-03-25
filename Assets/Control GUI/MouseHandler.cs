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

public class MouseHandler : MonoBehaviour {

	public Texture2D defaultCursor;
	
	private Texture2D currentCursor;

	public static MouseHandler instance;

	private bool cursorStored;
	private Texture2D previousCursor;
	//private bool gamePaused;

	private bool mouseXLocked;
	private bool mouseXLockActive;

	// Use this for initialization
	public void Awake () {
		
		if (!Application.isEditor)  {	
			Screen.showCursor = false;
		}
		
		instance = this;
		cursorStored = false;
		UseDefaultCursor();
		LockCursorXPosition();
	}
	
	public static void LockCursorXPosition() {
		instance.mouseXLocked = true;
		instance.ActivateReleaseXLock();
	} 
	
	public static void ReleaseCursorXPosition() {
		instance.mouseXLocked = false;
		instance.ActivateReleaseXLock();
	}
	
	private void ActivateReleaseXLock() {
		if(PlayerPrefs.GetInt("mouseMove") == 1) {
			if(mouseXLocked) {
				mouseXLockActive = true;
				
			}
			else {
				mouseXLockActive = false;	
			}
		}
		else {
			mouseXLockActive = false;
		}
	}
	
	public static void UseDefaultCursor(){
		instance.currentCursor = instance.defaultCursor;
	}
	
	public static void UseCursor(Texture2D cursor){
		instance.currentCursor = cursor;
	}
	
	public void OnGUI(){
		if(Pause.IsPaused()) {
			previousCursor = currentCursor;
			UseDefaultCursor();
			cursorStored=true;
			return;	
		} 
		if(cursorStored) {
			cursorStored=false;
			currentCursor = previousCursor;
			if (!Application.isEditor)  {	
				Screen.showCursor = false;
			}
		}
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		GUI.depth = -1;
		Vector3 GUICoord = GUIGlobals.ScreenCoordToGUICoord(Input.mousePosition);
		float xCoord = GUICoord.x;
		if(mouseXLockActive) {
			xCoord = GUIGlobals.GetCenterX();
		}
		GUI.Label(new Rect(xCoord - currentCursor.width / 2, GUICoord.y - currentCursor.height / 2, currentCursor.width, currentCursor.height), currentCursor);
	}
	
	public static GameObject GetObjectUnderCursor(int layerMask){
		if(PopupMenuGUI.IsFinished()==false) {
			return null;	
		}
		Ray mouseRay;
		RaycastHit rayHit; 
		if(instance.mouseXLockActive) {
			mouseRay = Camera.main.ScreenPointToRay( new Vector3(Screen.width/2, Input.mousePosition.y,  Input.mousePosition.z));
		}
		else {
			mouseRay = Camera.main.ScreenPointToRay( Input.mousePosition );
		}
		if( Physics.Raycast( mouseRay.origin, mouseRay.direction, out rayHit, Mathf.Infinity, layerMask) ){
			return rayHit.transform.gameObject;
		} else {
			return null;
		}
	}
	
	public static bool IsLeftClicked(){
		return Input.GetMouseButtonDown(0);
	}
}

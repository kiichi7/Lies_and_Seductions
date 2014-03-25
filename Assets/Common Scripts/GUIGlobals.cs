/**********************************************************************
 *
 * CLASS GUIGlobals
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
 * 
 * Contains CLASS GUIGlobals
 *	- All varaibles and methods are static
 * 
 * Usage, e.g.,:
 * GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
 * - handles conversion to different screen sized
 * - place GUI elements as if the screen would be 1280x854
 *
 **********************************************************************/

using UnityEngine;
using System.Collections;

/************************************************************************************
 *
 * CLASS GUIGlobals
 *
 ************************************************************************************/
public class GUIGlobals {

	/************************************************************************************
	 *
	 * PUBLIC STATIC VARIABLES
	 *
	 ************************************************************************************/

	// GUI layout is designed for resolution of 1280x854, we need this information for scaling to
	// other screen sizes
	public static float screenWidth=1280.0f;
	public static float screenHeight=854.0f;
	
	/************************************************************************************
	 *
	 * PUBLIC STATIC METHODS
	 *
	 ************************************************************************************/

	// Returns Matrix for GUI.matrix for scaling GUI Elements
	static public Matrix4x4 GetGUIScaleMatrix()
	{
		float xScale = Screen.width / screenWidth;
		float yScale = Screen.height / screenHeight;
		return Matrix4x4.TRS(
			Vector3.zero,
			Quaternion.identity,
			new Vector3(xScale, yScale, 1)
		);
	}
	
	// Return x coordinate of the center of screen
	static public float GetCenterX()
	{
		return screenWidth / 2;
	}
	
	// Return y coordinate of the centet of screen
	static public float GetCenterY()
	{
		return 	screenHeight / 2;
	}
	
	// Return true if screen is 16:9 or 16:10
	static public bool IsWideScreen()
	{
		float screenRation = (float)Screen.width/(float)Screen.height;
		if (Mathf.Abs(screenRation-screen16_9) < 0.01)
		{
			return true;	
		}
		if (Mathf.Abs(screenRation-screen16_10) < 0.01)
		{
			return true;	
		}
		
		return false;	
	}
	
	// Converts ...
	static public Vector3 ScreenCoordToGUICoord(Vector3 screenCoord){
		return new Vector3(screenCoord.x / Screen.width * screenWidth, screenHeight - screenCoord.y / Screen.height * screenHeight, screenCoord.z);
	}
	
	// Converts ...
	static public Vector3 WorldCoordToGUICoord(Vector3 worldCoord){
		return ScreenCoordToGUICoord(Camera.main.WorldToScreenPoint(worldCoord));
	}

	/************************************************************************************
	 *
	 * PRIVATE VARIABLES
	 *
	 ************************************************************************************/

	private static float screen16_9=1.777778f;
	private static float screen16_10=1.6f;

}

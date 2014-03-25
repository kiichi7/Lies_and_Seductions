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

public class BeatMarker {
		
	//public int id = -1;
	private Vector2 position;
	public bool isActive;
	public Direction direction;
	private int currentHotModeFrame;
	
	private float creationTime;
	
	// BeatMarker matched to players key hit
	public bool hasBeenHit;
	
	private Texture2D normalIcon;
	private Texture2D hitIcon;
	private Texture2D[] hotModeIcons;
	
	public BeatMarker(Texture2D normalIcon, Texture2D hitIcon, Texture2D[] hotModeIcons, Direction direction){
		isActive = true;
		currentHotModeFrame = 0;
		creationTime = GameTime.GetRealTimeSecondsPassed();
		hasBeenHit = false;
		this.normalIcon = normalIcon;
		this.hitIcon = hitIcon;
		this.hotModeIcons = hotModeIcons;
		this.direction = direction;
		InitializePosition(direction);
	}
	
	private void InitializePosition(Direction direction){
		int column;
		switch (direction){
		case Direction.LEFT:
			column = 0;
			break;
		case Direction.DOWN:
			column = 1;
			break;
		case Direction.RIGHT:
			column = 2;
			break;
		case Direction.UP:
			column = 1;
			break;
		default:
			column = -1;
			break;
		}
		position = new Vector2(DanceGUI.beatMarkerLeftRowOffset + column * DanceGUI.beatMarkerXOffset, 0);
	}
	
	public bool IsActive(){
		return isActive;
	}
	
	public bool HasBeenHit(){
		return hasBeenHit;
	}
	
	public void DrawGUI(bool hotMode) {
		GUIStyle style = new GUIStyle();
		Texture2D currentIcon;
		if(hasBeenHit) {
			currentIcon = hitIcon;
		} else {
			if(hotMode) {
				currentIcon = hotModeIcons[currentHotModeFrame];
				currentHotModeFrame++;
				if(currentHotModeFrame >= hotModeIcons.Length) {
					currentHotModeFrame = 0; 
				}
			} else {
				currentIcon = normalIcon;
			}
		}
		style.normal.background = currentIcon;
		
		int drawWidth;
		if (direction == Direction.UP) {
			//We might need to use DanceMode.beatMarkerDrawWidth
			drawWidth = 2*DanceGUI.beatMarkerXOffset+DanceGUI.beatMarkerWidth;
		} else {
			drawWidth = DanceGUI.beatMarkerDrawWidth;	
		}
		
		// Now we need to calculate where to draw the
		
		//float drawY = position.y + DanceGUI.beatMarkerHeight - DanceGUI.beatMarkerDrawHeight;
		float drawY = position.y + DanceGUI.beatMarkerHeight / 2 - DanceGUI.beatMarkerDrawHeight;
		float drawX = position.x - drawWidth / 2;
		
		GUI.Label(new Rect(drawX, drawY, drawWidth, DanceGUI.beatMarkerDrawHeight), "", style);
		
		// BOX for DEBUGGING... The box and BeatMarker should match
		//GUI.Box(new Rect(pos.x,pos.y, DanceMode.beatMarkerWidth, DanceMode.beatMarkerHeight), "");
		}
	
	
	public void UpdateBeatMarker(float deltaTime, float desiredSpeed) {
		float sp = desiredSpeed * deltaTime;
		position.y += sp;
		if(position.y > DanceGUI.targetAreaEndY) {
			isActive=false;
			
			// DEBUG: to check is the BeatMarker speed has been correct
			//float dd = GameTime.GetRealTimeSecondsPassed() - creationTime;
			//if(Mathf.Abs(4.0f-dd) > 0.05f) {
			//	Debug.LogError("BeatMarkerUpdateBeatMarker(): ERROR: Beatmarker moved with wrong speed. Delta: " + (4.0f-dd).ToString());
			//}
		}	
	
	}
	
	
	public bool Hit(ArrayList directionsPressed){
		/*
		 * Returns true if correct key is pressed when the BeatMarker is in the target area and otherwise false
		 *
		 */
		if(hasBeenHit == true) {
			// already hit...
			return false;
		}
		if(isActive==false) {
			// already out...
			return false;	
		}
		if (directionsPressed.Contains(direction)) {
			// directionPressed matches to BeatMarker...	
			// now we need to see if timing is correct
			if (position.y >= DanceGUI.targetAreaStartY && position.y <= DanceGUI.targetAreaEndY){
				hasBeenHit = true;
				//Debug.Log("BeatMarker.Hit(): time=" + Time.time + " position=" + position.y);
				directionsPressed.Remove(direction);
				return true;
			}
			/*float dy = position.y+DanceGUI.beatMarkerHeight - DanceGUI.danceScreenHeight;
			if(dy > 0) {
				hasBeenHit = true;
				return true;
			}*/
				
		}
		return false;
	}	
}
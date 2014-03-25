/**********************************************************************
 *
 * CLASS CameraAngle
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

public class CameraAngle {
	public float angle;
	public float forwardOffset;
	public float height;
	public float tilt;
	public float distance;
	public float fieldOfView;
	public float spinningSpeed;
	public bool moveWithTarget;
	
	public CameraAngle(float angle, float forwardOffset, float height, float tilt, float distance, float fieldOfView, float spinningSpeed, bool moveWithTarget){
		this.angle = angle;
		this.forwardOffset = forwardOffset;
		this.height = height;
		this.tilt = tilt;
		this.distance = distance;
		this.fieldOfView = fieldOfView;
		this.spinningSpeed = spinningSpeed;
		this.moveWithTarget = moveWithTarget;
	}
	
	public static CameraAngle BEHIND = new CameraAngle(0.0f, 0.0f, 0.8f, 0.0f, 4.0f, 60.0f, 0.0f, true);
	public static CameraAngle SEMI_CLOSE_UP_PC = new CameraAngle(225.0f, 0.6f, 0.7f, 0.0f, 0.9f, 60.0f, 0.0f, false);
	public static CameraAngle SEMI_CLOSE_UP_NPC = new CameraAngle(135.0f, 0.6f, 0.7f, 0.0f, 0.9f, 60.0f, 0.0f, false);
	public static CameraAngle CLOSE_UP_PC = new CameraAngle(200.0f, 0.1f, 0.7f, 0.0f, 0.7f, 60.0f, 0.0f, false);
	public static CameraAngle CLOSE_UP_NPC = new CameraAngle(160.0f, 0.1f, 0.7f, 0.0f, 0.7f, 60.0f, 0.0f, false);
	public static CameraAngle POKER = new CameraAngle(-15.0f, 0.175f, 0.27f, 19.0f, 1.5f, 50.0f, 0.0f, true);
	public static CameraAngle DANCE = new CameraAngle(-60.0f, 1.45f, 0.18f, 6.0f, 6.0f, 60.0f, 0.0f, false);
		
	public static CameraAngle GetCameraAngle(string xmlName){
		if (xmlName.Equals("semi_close_up(abby)")){
			return SEMI_CLOSE_UP_PC;
		} else if (xmlName.Equals("semi_close_up(npc)")){
			return SEMI_CLOSE_UP_NPC;
		} else if (xmlName.Equals("close_up(abby)")){
			return CLOSE_UP_PC;
		} else if (xmlName.Equals("close_up(npc)")){
			return CLOSE_UP_NPC;
		} else {
			Debug.LogError("Invalid camera angle in conversation file!");
			return null;
		}	
	}
	
	public void MoveCloser(){
		distance-=0.2f;
	}
	
	public void MoveFarther(){
		distance+=0.2f;
	}
	
	public void TurnClockwise(){
		angle+=2.0f;
	}
	
	public void TurnCounterClockwise(){
		angle-=2.0f;
	}
	
	public void MoveUp(){
		height+=0.025f;
	}
	
	public void MoveDown(){
		height-=0.025f;
	}
	
	public void TiltUp(){
		tilt-=2.0f;
	}
	
	public void TiltDown(){
		tilt+=2.0f;
	}
	
	public void MoveForward(){
		forwardOffset+=0.025f;
	}
	
	public void MoveBackward(){
		forwardOffset-=0.05f;
	}
	
	public void IncreaseFieldOfView(){
		fieldOfView += 2.0f;
	}
	
	public void DecreaseFieldOfView(){
		fieldOfView -= 2.0f;
	}
	
	public void PrintValues(){
		Debug.Log("Angle: " + angle + ", forward offset " + forwardOffset + ", height " + height + ", tilt " + tilt + ", distance " + distance + ", field of view " + fieldOfView);
	}
}
	
	

/**********************************************************************
 *
 * CLASS StatusIndicatorBillboard
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
 * Billboard for displaying drunkness and attitude indicator animation on the top 
 * of a character's head.
 *
 * USAGE:
 * - Attach the script to a cube or plai n, scale the cube accordlinly.
 * - Attack the cube to a character
 * - Attach material of cube to the script (animateThis). Material should have
 *   FlatBlendFX shader attached to it.
 * - Attach cameta to the script (mainCamera)
 * - Attach charcater to the script (character)
 * 
 *
 ***********************************************************************/
using UnityEngine;
using System.Collections;

public class StatusIndicatorBillboard : MonoBehaviour, StatusListener {

	/**************************************************************
	 *
	 * CONST MEMBERS
	 *
	 **************************************************************/
	
	// These can be used to control drunkness indicator animation speed
	public const float MILDLY_DRUNK_FRAME_TIME=0.2f;
	public const float HEAVILY_DRUNK_FRAME_TIME=0.1f;
	
	// Attitude indicatior is faded from totaly transparent (0.0) to not transparent (1.0).
	// Steps (descreasing transparency by ATTITUDE_ANIMATION_FADE_STEP) are perforemend in 
	// every  ATTITUDE_ANIMATION_FRAME_TIME
	// ATTITUDE_ANIMATION_FRAME_TIME also controls all attitude animations speeds
	public const float ATTITUDE_ANIMATION_FRAME_TIME=0.1f;
	public const float ATTITUDE_ANIMATION_FADE_STEP = 0.1f;
	
	// Delay before we start showning attitude change after receiving show request.
	// If we show attitude indictor immediately, last speech ballon will interfere with the indicator
	public const float ATTITUDE_CHANGE_ANIMATION_START_DELAY=3.0f;

	// Attitude indicatior is shown this amount of seconds
	public const float ATTITUDE_INDICATOR_SHOW_TIME = 3.0f;
	
	// We are showing in which direction attitude changed by scaling indicatior up or down
	// Lerping/InverseLerping from current_scale to current_scale + this value 
	public const float ATTITUDE_DISPLAY_SCALE_CHANGE = 0.1f;

	public const int HIGH_ATTITUDE_LIMIT = 35;
	public const int LOW_ATTITUDE_LIMIT = -10;
	public const int ZERO_ATTITUDE_INDICATOR = 5;
	
	private Vector3 billboardDefSize = new Vector3(0.3f, 0.36f, 0.001f);

	/**************************************************************
	 *
	 * PUBLIC MEMBERS, set these in inspector
	 *
	 **************************************************************/

	public Camera mainCamera;
	
	// We are displaying this characters drunkness state
	public GameObject character;
	
	
	public Material animateThis;

	// Animation frames
	public Texture2D []drunknessAnimationTextures = new Texture2D[50];
	
	public Texture2D []attitudeIndicatorTextures = new Texture2D[20];
	
	// Below this we do not show billboard
	public float notDrunkLevel=2.0f;
	// If drunkness level is below this we show slow drunk animation 
	// and if it is over we show faster animation
	public float mildlyDrunkLevel=6.0f;

	/**************************************************************
	 *
	 * PRIVATE MEMBERS
	 *
	 **************************************************************/
	
	private float drunkness;
	private int attitude;
	private int oldAttitude;
	private int frame;
	private int counter;
	private ImpressionMemory memory;
	
	private enum SHOW_STATE { DEFAULT, WAITING_ACTION, SHOWING_ATTITUDE };
	private SHOW_STATE showState;
	
	private Vector3 defaultScale;
	
	private float attitudeAnimationStartTime;
	private enum ATTITUDE_ANIM_STATE { FADE_IN, VISIBLE, FADE_OUT, NONE, SHOW_INCREASE, SHOW_DECREASE };
	private ATTITUDE_ANIM_STATE attitudeAnimState;
	private float attitudeFade;

	/**************************************************************
	 *
	 * METHODS FROM MONOBEHAVIOUR
	 *
	 **************************************************************/

	public void Start() {
		CharacterState state = (CharacterState)character.GetComponent(typeof(CharacterState));
		drunkness = state.GetDrunkness();
		state.RegisterStatusListener(this);
		
		if(CharacterManager.IsPC(character)) {
			memory = null;
		} 
		else { 
			memory = character.GetComponent(typeof(ImpressionMemory)) as ImpressionMemory;
			memory.RegisterStatusListener(this);
		};
		InitDrunknessAnimation();
		showState = SHOW_STATE.DEFAULT;
		defaultScale = new Vector3(billboardDefSize.x, billboardDefSize.y, billboardDefSize.z);
		renderer.enabled = false;
		enabled = false;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		// Making sure that the billboard box is facing toward the camera
		transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.back, mainCamera.transform.rotation * Vector3.up);
	}
	
	/**************************************************************
	 *
	 * PUBLIC METHDS
	 *
	 **************************************************************/
	 
	 public void HideIfNeeded() {
	 	if(enabled == false) {
	 		renderer.enabled = false;	
	 	}	
	 }
	
	/**************************************************************
	 *
	 * IMPLEMENTED INTERFACE from StatusListener
	 *
	 **************************************************************/
	
	public void DrunknessValueChanged(float val) {
		drunkness = val;
		if(showState != SHOW_STATE.SHOWING_ATTITUDE) {
			InitDrunknessAnimation();
		}
	}
	
	public void ActionStarted() {
		if(memory == null) return;
		showState = SHOW_STATE.WAITING_ACTION;
		oldAttitude = memory.GetAttitudeTotal();
		attitude = oldAttitude;
	}
	
	public void DisplayAttitudeChange() {
		
		// Call this to display attitude change of a charcacter. You need to init action by calling
		// ActionStarted()
		
		if(memory == null) return; // Returning if the character does not have ImpressionMemory (the character is PC)
		
		if(showState != SHOW_STATE.WAITING_ACTION) {
			Debug.LogError("StatusIndicatorBillboard.DisplayAttitudeChange() called without calling first StatusIndicatorBillboard.ActionStarted()");
			return;
		}
		attitude = memory.GetAttitudeTotal();
		
		if (attitude > oldAttitude) {
		  FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_ADDITUDE_UP);	
		} else if (attitude < oldAttitude) {
		  FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_ADDITUDE_DOWN);	
		}
		
		InitAttitudeAnimations();
		InvokeRepeating("AnimateAttitudeChange", ATTITUDE_CHANGE_ANIMATION_START_DELAY, ATTITUDE_ANIMATION_FRAME_TIME);	
	}

	
	public void DisplayAttitude() {
		// Call this to display current attitude of a character
		//Debug.Log("StatusIndicatorBillboard.DisplayAttitude()" + character.name + ", ");
		
		if(memory == null) return;		
		attitude = memory.GetAttitudeTotal();
		
		InitAttitudeAnimations();
		InvokeRepeating("AnimateAttitude", ATTITUDE_ANIMATION_FRAME_TIME, ATTITUDE_ANIMATION_FRAME_TIME);
	}
		
	/**************************************************************
	 *
	 * PRIVATE METHODS
	 *
	 **************************************************************/
		
	private void AnimateAttitude() {
		switch(attitudeAnimState) {
			case ATTITUDE_ANIM_STATE.FADE_IN:
				attitudeFade += ATTITUDE_ANIMATION_FADE_STEP;
				if(attitudeFade > 1.0f) {
					attitudeAnimState = ATTITUDE_ANIM_STATE.VISIBLE;
					attitudeAnimationStartTime = Time.time;
					attitudeFade = 1.0f;
				}
				animateThis.SetFloat("_Blend", attitudeFade);
				break;
			case ATTITUDE_ANIM_STATE.VISIBLE:
				if(Time.time - attitudeAnimationStartTime > ATTITUDE_INDICATOR_SHOW_TIME) {
					attitudeAnimState = ATTITUDE_ANIM_STATE.FADE_OUT;
				}
				break;
			case ATTITUDE_ANIM_STATE.FADE_OUT:
				attitudeFade -= ATTITUDE_ANIMATION_FADE_STEP;
				if(attitudeFade < 0) {
					attitudeAnimState = ATTITUDE_ANIM_STATE.NONE;
					CancelInvoke();
					InitDrunknessAnimation();
					attitudeFade = 0.0f;
					//Debug.Log("StatusIndicatorBillboard.AnimateAttitude(): terminating animation of " + character.name);	
				}
				animateThis.SetFloat("_Blend", attitudeFade);
				break;	
			
		}
	}

	
	private void AnimateAttitudeChange() {
		float passedSecs = Time.time - attitudeAnimationStartTime;
		float animSpeed = passedSecs/ATTITUDE_INDICATOR_SHOW_TIME;
		switch(attitudeAnimState) {
				case ATTITUDE_ANIM_STATE.FADE_IN:
				attitudeFade += ATTITUDE_ANIMATION_FADE_STEP;
				if(attitudeFade > 1.0f) {
					attitudeAnimationStartTime = Time.time;
					attitudeFade = 1.0f;
					if(attitude == oldAttitude) {
						attitudeAnimState = ATTITUDE_ANIM_STATE.VISIBLE;
					}
					else if(attitude < oldAttitude) {
						attitudeAnimState = ATTITUDE_ANIM_STATE.SHOW_DECREASE;
					}
					else {
						attitudeAnimState = ATTITUDE_ANIM_STATE.SHOW_INCREASE;
					}
				}
				animateThis.SetFloat("_Blend", attitudeFade);
				break;
			case ATTITUDE_ANIM_STATE.SHOW_INCREASE:
				if(passedSecs > ATTITUDE_INDICATOR_SHOW_TIME) {
					attitudeAnimState = ATTITUDE_ANIM_STATE.FADE_OUT;
				}
				transform.localScale = new Vector3(Mathf.Lerp(defaultScale.x, defaultScale.x + ATTITUDE_DISPLAY_SCALE_CHANGE, animSpeed), Mathf.Lerp(defaultScale.y, defaultScale.y + ATTITUDE_DISPLAY_SCALE_CHANGE, animSpeed), defaultScale.z);
				break;
			case ATTITUDE_ANIM_STATE.SHOW_DECREASE:
				if(passedSecs > ATTITUDE_INDICATOR_SHOW_TIME) {
					attitudeAnimState = ATTITUDE_ANIM_STATE.FADE_OUT;
				}
				transform.localScale = new Vector3(Mathf.Lerp(defaultScale.x, defaultScale.x - ATTITUDE_DISPLAY_SCALE_CHANGE, animSpeed), Mathf.Lerp(defaultScale.y, defaultScale.y - ATTITUDE_DISPLAY_SCALE_CHANGE, animSpeed), defaultScale.z);
				break;
			case ATTITUDE_ANIM_STATE.VISIBLE:
				if(passedSecs > ATTITUDE_INDICATOR_SHOW_TIME) {
					attitudeAnimState = ATTITUDE_ANIM_STATE.FADE_OUT;
				}
				break;
			case ATTITUDE_ANIM_STATE.FADE_OUT:
				attitudeFade -= ATTITUDE_ANIMATION_FADE_STEP;
				if(attitudeFade < 0) {
					attitudeAnimState = ATTITUDE_ANIM_STATE.NONE;
					CancelInvoke();
					InitDrunknessAnimation();
					attitudeFade = 0.0f;	
				}
				animateThis.SetFloat("_Blend", attitudeFade);
				break;	
			
		}

	}
	
	private void InitAttitudeAnimations() {
		CancelInvoke(); // lets make sure that there are no other animations playing
		attitudeFade=0;
		enabled = true;
		renderer.enabled = true;
		attitudeAnimState = ATTITUDE_ANIM_STATE.FADE_IN;
		showState = SHOW_STATE.SHOWING_ATTITUDE;
		animateThis.SetTexture("_MainTex", GetAttitudeTexture());
		animateThis.SetFloat("_Blend", 0);	
		
		transform.localScale = new Vector3(billboardDefSize.x, billboardDefSize.y, billboardDefSize.z);
	}
	
	private Texture2D GetAttitudeTexture() {
		int index;
		if(attitude == 0) {
			index = ZERO_ATTITUDE_INDICATOR;
		}
		else if(attitude >= HIGH_ATTITUDE_LIMIT) {
			index = attitudeIndicatorTextures.Length-1;
		}
		else if(attitude <= LOW_ATTITUDE_LIMIT) {
			index = 0;	
		}
		else if(attitude > 0) {
			float changePercentage = (float)attitude/(float)HIGH_ATTITUDE_LIMIT;
			index = (int)(changePercentage*(float)(attitudeIndicatorTextures.Length-ZERO_ATTITUDE_INDICATOR));	
			index += ZERO_ATTITUDE_INDICATOR;
		}
		else {
			float changePercentage = 1-((float)attitude/(float)LOW_ATTITUDE_LIMIT);
			index = (int)(changePercentage*(float)ZERO_ATTITUDE_INDICATOR);
		}
		return attitudeIndicatorTextures[(int)index];
	}
	
	/**********************************************************************************************/
	 // Drunkness indications handling starts
	
	private void InitDrunknessAnimation() {
		//Debug.Log("StatusIndicatorBillboard.InitDrunknessAnimation() " + character.name + " drunkness: " + drunkness);
		
		transform.localScale =  new Vector3(billboardDefSize.x, billboardDefSize.y, billboardDefSize.z);
		
		if(showState != SHOW_STATE.WAITING_ACTION) {
			showState = SHOW_STATE.DEFAULT;
		}
		
		if(drunkness <= notDrunkLevel) {
			frame=0;
			animateThis.SetTexture("_MainTex", drunknessAnimationTextures[frame]);
			animateThis.SetFloat("_Blend", 0.0f);
			enabled=false;
			renderer.enabled = false;
			CancelInvoke();
		}
		else {
			if(!enabled) {
				enabled=true;
				renderer.enabled = true;
				counter = 0;
				frame = 0;
				animateThis.SetFloat("_Blend", 1.0f);
				animateThis.SetTexture("_MainTex", drunknessAnimationTextures[frame]);
			}
			if(drunkness <= mildlyDrunkLevel) {
				InvokeRepeating("AnimateDrunknessLevel", MILDLY_DRUNK_FRAME_TIME, MILDLY_DRUNK_FRAME_TIME);
			}
			else {
				InvokeRepeating("AnimateDrunknessLevel", HEAVILY_DRUNK_FRAME_TIME, HEAVILY_DRUNK_FRAME_TIME);
			}
		}
	}
	
	private void AnimateDrunknessLevel() {
        if(drunkness <= mildlyDrunkLevel) {
        		frame++;
        	counter++;
        }
        else {
        	frame += 2;	
        }	
        	
        if(frame >= drunknessAnimationTextures.Length) {
        	frame = 0;
        	counter = 0;	
       	}
       	animateThis.SetFloat("_Blend", 1.0f);
        animateThis.SetTexture("_MainTex", drunknessAnimationTextures[frame]);
	}
	
}

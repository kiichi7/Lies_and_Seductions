/**********************************************************************
 *
 * CLASS PCMover
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

public class PCMover : MonoBehaviour {
	
	public float speed = 1.0f;
	public int gravity = 20;
	public int rotationSpeed = 60;
	
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 rotationDirection = Vector3.zero;
	//private Vector3 neckRotationDirection = Vector3.zero;
	private bool grounded = false;
	private bool walkingForwards = false;
	private bool walkingBackwards = false;
	private bool turningWithoutMoving = false;
	private bool idle = false;
	private float changeIdleAnimTime;
	private bool idleActionPlayed;
	
	private CharacterAnimator animator;
	CharacterController controller;
	//public Transform neck;

	public void Start(){
		animator = GetComponent(typeof(CharacterAnimator)) as CharacterAnimator;
		controller = (CharacterController)gameObject.GetComponent(typeof(CharacterController));
		idleActionPlayed = false;
		enabled=false;
	}


	public void MovePC () {
		bool userMovedForwards = false;
		bool userMovedBackwards = false;
		bool userTurning = false;
		
		if (grounded) {
			float forwardBackward = 0.0f;
			float leftright =  0.0f;
			
			if(PlayerPrefs.GetInt("mouseMove")==0) {
				forwardBackward = Input.GetAxis("PC Forward Backward");
				leftright = Input.GetAxis("PC Left Right");
				if(Mathf.Abs(leftright) > 0.1f) {
					userTurning = true;	
				}
			}
			else {
				forwardBackward = Input.GetAxis("PC Forward Backward Mouse");
				
				/*int w = Screen.width/2;
				leftright = (float)(Input.mousePosition.x-w)/w;
				if(Mathf.Abs(leftright) > 0.1f) {
					userTurning = true;	
				}
				else {
					leftright=0.0f;
				}*/
				leftright = Input.GetAxis("Mouse Follow X");
			}
			
			if(forwardBackward > 0.1f) {
				userMovedForwards = true;	
			}
			else if(forwardBackward < -0.1f){
				userMovedBackwards = true;
			}
			
			moveDirection = new Vector3(0, 0, forwardBackward);
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
		
			rotationDirection = new Vector3(0, leftright, 0);
			
			transform.Rotate(rotationDirection * Time.deltaTime * rotationSpeed);
			
		}
		moveDirection.y -= gravity * Time.deltaTime;
		
		//CharacterController controller = (CharacterController)gameObject.GetComponent(typeof(CharacterController));
		//Debug.Log("moveDirection = " + moveDirection);
		CollisionFlags flags = controller.Move(moveDirection * Time.deltaTime);
		grounded = (flags & CollisionFlags.CollidedBelow) != 0;
					
		//Debug.Log("userMovedForwards=" + userMovedForwards + ", userMovedBackwards=" + userMovedBackwards + ", userTurning=" + userTurning + ", turningLeftOrRight=" + turningLeftOrRight);
					
		if (userMovedForwards){
			if (!walkingForwards){
				//Debug.Log("Started AnimationWalking Forwards");
				animator.StartAnimation("walk", WrapMode.Loop, Emotion.BodyParts.FACE);
			}
			walkingForwards = true;
		} else {
			walkingForwards = false;
		} 
		if (userMovedBackwards){
			if (!walkingBackwards){
				//Debug.Log("Started Animation Walking Backwards");
				animator.StartAnimation("walk", WrapMode.Loop, Emotion.BodyParts.FACE, -1.0f);
			}
			walkingBackwards = true;
		} else {
			walkingBackwards = false;
		}
		if (userTurning && !walkingForwards && !walkingBackwards) {
			if (!turningWithoutMoving){
				//Debug.Log("Started Animation Turning");
				animator.StartAnimation("turn", WrapMode.Loop, Emotion.BodyParts.FACE);
			}
			turningWithoutMoving = true;
		} else {
			turningWithoutMoving = false;
		}
		if (!walkingForwards && !walkingBackwards && !turningWithoutMoving){
			if (!idle){
				changeIdleAnimTime = Time.time;
			}
			idle = true;
		} else {
			idle = false;
		}
		if(idle) {
			HandleIdleAnimation();
		}
	}
	
	private void HandleIdleAnimation() {
		if(changeIdleAnimTime < Time.time) {
			//Debug.Log("PCMover.HandleIdleAnimation(). Selecting idle animation");
			int anim;
			if(idleActionPlayed) {
				anim = Random.Range(0,2);
				idleActionPlayed = false;
			}
			else {
				anim = Random.Range(0,6);
			}
			string animName = "idle_pose_1";
			WrapMode wrapMode = WrapMode.Loop;
			Emotion.BodyParts bodyParts = Emotion.BodyParts.FACE;
			switch(anim) {
				case 0:
					animName = "idle_pose_1";
					wrapMode = WrapMode.Loop;
					//bodyParts = Emotion.BodyParts.FACE;
					bodyParts = Emotion.BodyParts.NONE;
					changeIdleAnimTime = Time.time + 3.4f;
					break;
				case 1:
					animName = "idle_pose_2";
					wrapMode = WrapMode.Loop;
					//bodyParts = Emotion.BodyParts.FACE;
					bodyParts = Emotion.BodyParts.NONE;
					changeIdleAnimTime = Time.time + 3.2f;
					break;
				case 2:
					animName = "idle_pose_1";
					wrapMode = WrapMode.Loop;
					//bodyParts = Emotion.BodyParts.FACE;
					bodyParts = Emotion.BodyParts.NONE;
					changeIdleAnimTime = Time.time + 3.4f;
					break;
				case 3:
					animName = "idle_pose_2";
					wrapMode = WrapMode.Loop;
					//bodyParts = Emotion.BodyParts.FACE;
					bodyParts = Emotion.BodyParts.NONE;
					changeIdleAnimTime = Time.time + 3.2f;
					break;
				case 4:
					animName = "idle_action_1";
					wrapMode =  WrapMode.Once;
					//bodyParts =  Emotion.BodyParts.FACE;
					bodyParts = Emotion.BodyParts.NONE;
					changeIdleAnimTime = Time.time + 4.5f;
					idleActionPlayed = true;
					break;
				case 5:
					animName = "idle_action_2";
					wrapMode =  WrapMode.Once;
					//bodyParts =  Emotion.BodyParts.FACE;
					bodyParts = Emotion.BodyParts.NONE;
					changeIdleAnimTime = Time.time + 2.6f;
					idleActionPlayed = true;
					break;
			}
			//Debug.Log("PCMover.HandleIdleAnimation(): Playing " + animName);
			animator.StartAnimation(animName, wrapMode, bodyParts);
		}
	}
}

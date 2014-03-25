/**********************************************************************
 *
 * CLASS StairTest
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

public class StairTest : MonoBehaviour {
	
	public float speed = 1.0f;
	public int gravity = 20;
	public int rotationSpeed = 60;
	
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 rotationDirection = Vector3.zero;
	//private Vector3 neckRotationDirection = Vector3.zero;
	private bool grounded = false;
	private bool walkingForwards = false;
	private bool walkingBackwards = false;
	
	private CharacterAnimator animator;
	
	//public Transform neck;

	public void Start(){
		animator = (CharacterAnimator)GetComponent("CharacterAnimator");
	}

	public void Update(){
		MovePC();
	}

	public void MovePC () {
		bool userMovedForwards = false;
		bool userMovedBackwards = false;
		if (grounded) {
			float forwardBackward = Input.GetAxis("PC Forward Backward");
			moveDirection = new Vector3(0, 0, forwardBackward);
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			
			if (forwardBackward > 0.1){
				userMovedForwards = true;
			} else if (forwardBackward < -0.1){
				userMovedBackwards = true;
			}
			
			rotationDirection = new Vector3(0, Input.GetAxis("PC Left Right"), 0);
			//neckRotationDirection = new Vector3(0, Input.GetAxis("Head Left Right"), 0);
			
			transform.Rotate(rotationDirection * Time.deltaTime * rotationSpeed);
			//neck.Rotate(neckRotationDirection * Time.deltaTime * rotationSpeed);
			
		}
		moveDirection.y -= gravity * Time.deltaTime;
		
		CharacterController controller = (CharacterController)gameObject.GetComponent(typeof(CharacterController));
		CollisionFlags flags = controller.Move(moveDirection * Time.deltaTime);
		grounded = (flags & CollisionFlags.CollidedBelow) != 0;
					
		if (userMovedForwards && !walkingForwards){
			animator.StartAnimation("walk", WrapMode.Loop, Emotion.BodyParts.FACE);
			//animation.Blend("idle", 0.0F);
			walkingForwards = true;
		} else if (userMovedBackwards && !walkingBackwards){
			animator.StartAnimation("walk", WrapMode.Loop, Emotion.BodyParts.FACE, -1.0f);
			walkingBackwards = true;
		} else if (!userMovedForwards && !userMovedBackwards && (walkingForwards || walkingBackwards)) {
			animator.StartAnimation("idle_pose_1", WrapMode.Loop, Emotion.BodyParts.ALL);
			//animation.Blend("walk", 0.0F);
			walkingForwards = false;
			walkingBackwards = false;
		}	
	}
}

/**********************************************************************
 *
 * CLASS CharacterMover
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

public class CharacterMover : MonoBehaviour {
	
	private const float MINIMUM_WALK_SPEED = 0.1f;
	
	public float speed = 2.0f;
	public float gravity = 20.0f;
	public TransitWaypoint startingWaypoint;
	
	private bool grounded = false;
	private Vector3 safeSpot = Vector3.zero;
	private bool lookInLateUpdate = false;
	private Vector3 lookPosition = Vector3.zero;
	private bool rotateInLateUpdate = false;
	//private Vector3 rotationAngles = Vector3.zero;
	private Quaternion overrideRotation;
		
	private CharacterController controller;
	private CharacterState state;
	
	public void Start(){
		controller  = gameObject.GetComponent(typeof(CharacterController)) as CharacterController;
		/*if (gameObject != CharacterManager.GetPC()){
			DisableObstaclesForNPC();
		}*/
		if (startingWaypoint != null){
			startingWaypoint.TransitCharacterTo(gameObject);
		}
		else {
			if(CharacterManager.IsMajorNPC(gameObject) || CharacterManager.IsPC(gameObject)) {
				Debug.LogError("CharacterMover.Start() startingWaypoint is not set for " + name);
			}
		}
		state = GetComponent(typeof(CharacterState)) as CharacterState;
	}
	
	/*private void DisableObstaclesForNPC(){
		GameObject[] obstacles = (GameObject[])FindObjectsOfType(typeof(GameObject));
		foreach (GameObject obstacle in obstacles){
			if (obstacle.collider != null && obstacle != gameObject && obstacle.tag != "Character" && obstacle.tag != "Floor"){
				Physics.IgnoreCollision(controller, obstacle.collider);
			}
		}
	}*/
	
	private bool HasMoved(Vector3 oldPosition, Vector3 newPosition){
		return Vector3.Distance(oldPosition, newPosition) > MINIMUM_WALK_SPEED * Time.deltaTime;
	}
	
	private bool MoveInDirection(Vector3 moveDirection3D, Vector3 lookPosition){
		Vector3 moveDirection;
		if (grounded) {
			LookAt(lookPosition);
			
			moveDirection = (new Vector3(moveDirection3D.x, 0.0f, moveDirection3D.z)).normalized;
			moveDirection *= speed;
		} else {
			moveDirection = Vector3.zero;
		}
		moveDirection.y -= gravity;
		
		Vector3 oldPosition = transform.position;
		
		CollisionFlags flags = controller.Move(moveDirection * Time.deltaTime);
		grounded = (flags & CollisionFlags.CollidedBelow) != 0;
		if (HasMoved(oldPosition, transform.position)){
			return true;
		} else {
			return false;
		}
	}
	
	public bool MoveTowards(Vector3 targetPosition) {
		Debug.DrawLine(transform.position, targetPosition, Color.green);
		return MoveInDirection(targetPosition - transform.position, targetPosition);
	}
	
	public bool BackAwayFrom(Vector3 targetPosition){
		return MoveInDirection(- targetPosition + transform.position, targetPosition);
	}
	
	public bool MoveToTheRight(){
		Vector3 oldPosition = transform.position;
		controller.Move(transform.TransformDirection(Vector3.right) * Time.deltaTime);
		if (HasMoved(oldPosition, transform.position)){
			return true;
		} else {
			return false;
		}
	}
	
	public void Update(){
		
		if (!grounded && state.GetCurrentSeat() == null){
			controller.Move(new Vector3(0, -gravity, 0) * Time.deltaTime);
		}
	}
	
	private void SetGhostMode(bool onOff){
		foreach (GameObject character in CharacterManager.GetCharacters()){
			if (character != gameObject){
				Physics.IgnoreCollision(controller, character.collider, onOff);
			}
		}
	}
	
	public void EnterGhostMode(){
		SetGhostMode(true);
	}
	
	public void ExitGhostMode(){
		SetGhostMode(false);
	}
	
	public void InvokeExitGhostMode(float duration){
		Invoke("ExitGhostMode", duration);
	}

	public void JumpTo(Vector3 position, Quaternion rotation, bool preserveY){
		float y;
		if (preserveY){
			y = transform.position.y;
		} else {
			y = position.y;
		}
		transform.position = new Vector3(position.x, y, position.z);
		transform.rotation = rotation;
		grounded = false;
	}
	
	public void SaveSafeSpot(){
		safeSpot = transform.position;
	}
	
	public void RestoreSafeSpot(){
		transform.position = safeSpot;
	}
	
	public void LookAt(Vector3 targetPosition){
		transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
		//lookPosition = targetPosition;
		//lookInLateUpdate = true;
	}
	
	public void Rotate(Vector3 rotationAngles){
		transform.Rotate(rotationAngles);
		overrideRotation = transform.rotation;
		//this.rotationAngles = rotationAngles;
		rotateInLateUpdate = true;
	}
	
	public void RotateToMatch(Quaternion rotation){
		transform.rotation = rotation;
		overrideRotation = rotation;
		rotateInLateUpdate = true;
	}
	
	public void MatchHookWithGameObject(string hookName, GameObject other){
		Transform hook = CharacterAnimator.FindChild(transform, hookName);
		
		Vector3 hookRotationAngles = hook.rotation.eulerAngles;
		//Vector3 thisRotationAngles = transform.rotation.eulerAngles;
		Vector3 otherRotationAngles = other.transform.rotation.eulerAngles;
		
		Vector3 rotationAnglesDelta = otherRotationAngles - hookRotationAngles;
		Rotate(rotationAnglesDelta /*new Vector3(0.0f, 150.0f, 0.0f)*/);
		
		Vector3 hookPosition = hook.position;
		//Vector3 thisPosition = transform.position;
		Vector3 otherPosition = other.transform.position;
		
		Vector3 positionDelta = otherPosition - hookPosition;
		transform.Translate(positionDelta, Space.World);
	}

	
	public void LateUpdate(){
		/*if (rotateInLateUpdate){
			//transform.rotation = overrideRotation;
			//transform.Rotate(rotationAngles);
			//rotateInLateUpdate = false;
		}*/
		if (lookInLateUpdate){
			//Debug.Log("CharacterMover.LateUpdate(): look at");
			transform.LookAt(new Vector3(lookPosition.x, transform.position.y, lookPosition.z));
			lookInLateUpdate = false;
		}
	}
}

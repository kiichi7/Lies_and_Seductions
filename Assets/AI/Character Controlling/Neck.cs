/**********************************************************************
 *
 * CLASS Neck
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

public class Neck : MonoBehaviour {

	public Vector3 forwardVector = Vector3.forward;
	public Vector3 upVector = Vector3.up;
	public Vector3 rightVector = Vector3.right;
	public Vector3 inverseForwardVector = Vector3.forward;
	public Vector3 inverseUpVector = Vector3.up;
	public Vector3 inverseRightVector = Vector3.right;
	public float lookDamping = 0.5f;
	public float maxHeadTurningAngle = 90.0f;

	private Quaternion defaultRotation;
	private Quaternion overrideRotation;
	//The direction towards the look target in world coordinates
	private Vector3 lookTargetDirectionWorld;
	//The current facing direction in world coordinates
	private Vector3 currentLookDirectionWorld;
	private bool isLooking;
	private bool lookDirectionReady;
	
	public void Start(){
		//rightVector = Vector3.Cross(forwardVector, upVector);
		//inverseRightVector = Vector3.Cross(inverseForwardVector, inverseUpVector);
		defaultRotation = transform.localRotation;
		overrideRotation = Quaternion.identity;
		lookTargetDirectionWorld = Vector3.zero;
		currentLookDirectionWorld = Vector3.zero;
		isLooking = false;
		lookDirectionReady = false;
	}

	public void SetLookPosition(Vector3 lookPosition){
		Vector3 lookTargetDirection3DWorld = (lookPosition - transform.position).normalized;
		//Looking direction in world coordinates
		lookTargetDirectionWorld = new Vector3(lookTargetDirection3DWorld.x, 0, lookTargetDirection3DWorld.z);
		if (!isLooking) {
			isLooking = true;
			//Current facing in world coordinates
			//currentLookDirectionWorld = ;
			//overrideRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
		}
	}
	
	public void RemoveLookPosition(){
		isLooking = false;
		lookDirectionReady = false;
		transform.localRotation = defaultRotation;
	}
	
	private Vector3 CorrectlyAlignedToActualNeckCoords(Vector3 vector){
		return vector.x * inverseRightVector + vector.y * inverseUpVector + vector.z * inverseForwardVector;
		//return vector.x * rightVector + vector.y * upVector + vector.z * forwardVector;
	}
	
	private Vector3 ActualToCorrectlyAlignedNeckCoords(Vector3 vector){
		return vector.x * rightVector + vector.y * upVector + vector.z * forwardVector;
		//return vector.x * inverseRightVector + vector.y * inverseUpVector + vector.z * inverseForwardVector;
	}
	
	public Vector3 GetCurrentLookDirectionWorldCoords(){
		if (isLooking){
			return currentLookDirectionWorld;
		} else {
			return transform.TransformDirection(ActualToCorrectlyAlignedNeckCoords(Vector3.forward));
		}
	}
	
	private Transform GetRootTransform(){
		Transform currentTransform = transform;
		while (currentTransform.parent != null){
			currentTransform = currentTransform.parent;
		}
		return currentTransform;
	}
	
	public void Update(){
		if (isLooking){
			if (!lookDirectionReady){
				overrideRotation = transform.rotation;
			}
			Vector3 characterForwardDirectionWorld = GetRootTransform().forward;
			currentLookDirectionWorld = Vector3.RotateTowards(currentLookDirectionWorld, lookTargetDirectionWorld, lookDamping, Mathf.Infinity);
			while (Vector3.Angle(currentLookDirectionWorld, characterForwardDirectionWorld) > maxHeadTurningAngle){
				currentLookDirectionWorld = Vector3.RotateTowards(currentLookDirectionWorld, characterForwardDirectionWorld, lookDamping, Mathf.Infinity);
			}
			Vector3 currentLookDirectionActualNeck = transform.InverseTransformDirection(currentLookDirectionWorld);
			//Vector3 upDirectionActualNeck = transform.InverseTransformDirection(Vector3.up);
			overrideRotation = overrideRotation * Quaternion.FromToRotation(ActualToCorrectlyAlignedNeckCoords(Vector3.forward), currentLookDirectionActualNeck);
			transform.rotation = overrideRotation;
			Vector3 newUpDirectionActualNeck = transform.InverseTransformDirection(Vector3.up);
			overrideRotation = overrideRotation * Quaternion.FromToRotation(ActualToCorrectlyAlignedNeckCoords(Vector3.up), newUpDirectionActualNeck);
			lookDirectionReady = true;
		}
		DrawDebugLine();
	}
	
	private void DrawDebugLine(){
		Color color;
		if (isLooking){
			color = Color.magenta;
		} else {
			color = Color.blue;
		}
		Debug.DrawLine(transform.position, transform.position + GetCurrentLookDirectionWorldCoords() * 10, color);
	}
	
	public void LateUpdate(){
		if (lookDirectionReady){
			transform.rotation = overrideRotation;
		}
	}
}

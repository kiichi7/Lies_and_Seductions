/**********************************************************************
 *
 * CLASS CameraControllerOld
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

public class CameraControllerOld : MonoBehaviour {

	// The target we are following
	public Transform target;
	// The distance in the x-z plane to the target
	public float distance = 5.0f;
	// the height we want the camera to be above the target
	public float height = 3.0f;

	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;

	public void LateUpdate () {
		// Early out if we don't have a target
		if (!target)
			return;
	
		// Calculate the current rotation angles
		float wantedRotationAngle = target.eulerAngles.y;
		//Add 180 if the user wants to look at the target from the front.
		if (Input.GetKey("f")){
			wantedRotationAngle += 180f;
		}
	
		//First, make the camera look at the target at its level.
	
		float wantedHeight = target.position.y;
		
		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y - height;
	
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		// Convert the angle into a rotation
		// The quaternion interface uses radians not degrees so we need to convert from degrees to radians
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;

		// Set the height of the camera
		transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
	
		// Always look at the target
		transform.LookAt (target);
	
		//Elevate the camera by the height given.
		transform.position = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
	}
}
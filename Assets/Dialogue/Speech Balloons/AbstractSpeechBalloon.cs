/**********************************************************************
 *
 * CLASS AbstractSpeechBalloon
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

public abstract class AbstractSpeechBalloon : MonoBehaviour {

	/*********************************************************************************************
	 *
	 * PUBLIC MEMBERS, set these in INSPECTOR 
	 *
	 ********************************************************************************************/

	public GUISkin balloonSkin;
	public Material fontMaterial;
	
	// do not set for the PC. We do not use billboards when PC is having conversation witn NPC
	public GameObject speechBubbleBackgroundMaster;

	public float horizontalDistanceMultiplier = 1.0f;
	public int delay = 5;
	
	/*********************************************************************************************
	 *
	 * PROTECTED MEMBERS 
	 *
	 ********************************************************************************************/
	
	protected enum ArrowDirection {Left, Right};
		
	// We need camera for speech balloon billboards
	protected Camera mainCamera;
	
	protected GameObject textObject;
	protected TextMesh textMeshComponent;
	
	protected GameObject bubbleBackgroundObject;
	protected MeshRenderer bubbleBackgroundRenderer;
	
	private bool scaled;
	
	/*********************************************************************************************
	 *
	 * PUBLIC OVERLOADED METHODS from MONOBEHAVIOR 
	 *
	 ********************************************************************************************/
	
	public void Start() {
		//Debug.Log("AbstractSpeechBalloon.Start(): " + name);
		mainCamera=null;
		bubbleBackgroundObject=null;
		textObject = null;
		bubbleBackgroundRenderer=null;
		if(speechBubbleBackgroundMaster) {
			// Initializations for the billboard speech bubbles
			
			// In order to make the billboard to face toward camera, we need to find the main camera
			Object []cams = FindObjectsOfType (typeof(Camera));
			foreach(Camera c in cams) {
				if(c.CompareTag("MainCamera")) {
					mainCamera=c;	
				}	
			}
			if(mainCamera==null) {
				Debug.LogError("AbstractSpeechBalloon.Start(): Cannot Find Camera with Tag: MainCamera");	
			}
			
			// Speech bubble background
			bubbleBackgroundObject = (GameObject)Instantiate(speechBubbleBackgroundMaster, Vector3.zero, Quaternion.identity);
			bubbleBackgroundObject.transform.parent = transform;
			bubbleBackgroundObject.transform.position = GetBubblePosition();
			bubbleBackgroundObject.transform.localScale = GetBubbleScale();
			bubbleBackgroundRenderer = bubbleBackgroundObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
			bubbleBackgroundRenderer.enabled = false;
			
			// Text object for speech bubble
			textObject = new GameObject("myText " + name);
			textObject.AddComponent("TextMesh");
      		textObject.AddComponent("MeshRenderer"); 
      		textMeshComponent = textObject.GetComponent(typeof(TextMesh)) as TextMesh;
      		textMeshComponent.font = GetFont();
      		MeshRenderer meshRenderer = textObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
      		meshRenderer.materials = new Material[1];
      		meshRenderer.material = fontMaterial;
      		
      		// Now attach speech bubble text to speech bubble
      		textObject.transform.parent = bubbleBackgroundObject.transform;
      		textObject.transform.localPosition = new Vector3(0,0,0.01f);
      		textObject.transform.Rotate(Vector3.up * 180);
      		
      		// Scaling text
      		textObject.transform.localScale = GetFontScale();
      		scaled = false;
      		enabled = false;
		}
	}
	
	public void LateUpdate() {
		// If we have a billboard speech bubble, we need to make sure that billboard is facing toward the main camera
		if(bubbleBackgroundObject && mainCamera) {
			Vector3 diff = mainCamera.transform.position - bubbleBackgroundObject.transform.position;
			float sqrtDist = diff.sqrMagnitude;
			if(sqrtDist > 5) {
				if(scaled) {
					bubbleBackgroundObject.transform.localScale = GetBubbleScale();
					bubbleBackgroundObject.transform.position = GetBubblePosition();
					scaled = false;
				}
			}
			else {
				Vector3 defScale = GetBubbleScale();
				Vector3 defPos = GetBubblePosition();
				float yMove = 0.05f;
				float scaleFactor = 0.5f;
				if(sqrtDist > 4) {
					scaleFactor = 0.9f;
					yMove = 0.01f;
					
				}
				else if(sqrtDist>3) {
					scaleFactor = 0.8f;
					yMove = 0.02f;
				}
				else if(sqrtDist>2) {
					scaleFactor = 0.7f;
					yMove = 0.03f;
				}
				else if(sqrtDist>1) {
					scaleFactor = 0.6f;
					yMove = 0.04f;
				}
				bubbleBackgroundObject.transform.localScale = new Vector3(defScale.x*scaleFactor, defScale.y*scaleFactor, 1.0f);
				bubbleBackgroundObject.transform.position = new Vector3(defPos.x, defPos.y-yMove, defPos.z);
				scaled = true;
			}
			bubbleBackgroundObject.transform.LookAt(bubbleBackgroundObject.transform.position + mainCamera.transform.rotation * Vector3.back, mainCamera.transform.rotation * Vector3.up);
		}
	}
	
	
	/*********************************************************************************************
	 *
	 * PRIVATE METHODS 
	 *
	 ********************************************************************************************/

	
	private float GetAngleBetweenVectors(Vector2 vector1, Vector2 vector2){
		float angle = Mathf.Atan2(vector2.y, vector2.x) - Mathf.Atan2(vector1.y, vector1.x);
		while (angle > Mathf.PI){
			angle -= Mathf.PI * 2;
		}
		while (angle < - Mathf.PI){
			angle += Mathf.PI * 2;
		}
		return angle; 
	}
	
	protected bool PCCanSeeBalloon(Vector3 arrowPosition){
		GameObject pc = CharacterManager.GetPC();
		CharacterState pcState = (CharacterState)pc.GetComponent("CharacterState");
		CharacterState ownState = (CharacterState)GetComponent("CharacterState");
		if (pcState.GetCurrentArea() == ownState.GetCurrentArea() && arrowPosition.z > 0){
			return true;
		} else {
			return false;
		}
	}
	
	protected Vector3 GetBubblePosition(){
		CharacterController characterController = (CharacterController)collider;
		Vector3 controllerTopCenterWorld = transform.TransformPoint(new Vector3(characterController.center.x, characterController.center.y + characterController.height / 2 + 0.3f, characterController.center.z));
		
		return controllerTopCenterWorld;
	}

	protected Vector3 GetArrowPosition(ArrowDirection arrowDirection, int balloonHeight){
		CharacterController characterController = (CharacterController)collider;
		Vector3 controllerTopCenterWorld = transform.TransformPoint(new Vector3(characterController.center.x, characterController.center.y + characterController.height / 2, characterController.center.z));

		float horizontalDistanceWorld = collider.bounds.extents.x * horizontalDistanceMultiplier;
		Vector3 horizontalDistanceVectorWorld;
		if (arrowDirection == ArrowDirection.Left){
			horizontalDistanceVectorWorld = Camera.main.transform.right * horizontalDistanceWorld;
		} else {
			horizontalDistanceVectorWorld = - Camera.main.transform.right * horizontalDistanceWorld;
		}
		Vector3 arrowPositionWorld = controllerTopCenterWorld + horizontalDistanceVectorWorld;
		Vector3 arrowPositionScreen = GUIGlobals.WorldCoordToGUICoord(arrowPositionWorld);
		if (arrowPositionScreen.y < balloonHeight){
			arrowPositionScreen = new Vector3(arrowPositionScreen.x, balloonHeight, arrowPositionScreen.z);
		}
		return arrowPositionScreen;
	}
	
	protected ArrowDirection GetArrowDirection(){
		Vector2 talkerForward = new Vector2(transform.forward.x, transform.forward.z);
		Vector2 cameraForward = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z);
		float angleInRadians = GetAngleBetweenVectors(cameraForward, talkerForward);
		if (angleInRadians < 0.0f){
			return ArrowDirection.Left;
		} else {
			return ArrowDirection.Right;
		}
	}
	
	/*********************************************************************************************
	 *
	 * PUBLIC ABSTRACT METHODS, Implemented in derived classes 
	 *
	 ********************************************************************************************/
	
	public abstract Font GetFont();
	public abstract Vector3 GetFontScale();
	public abstract Vector3 GetBubbleScale();
	public abstract void RemoveBalloon();
	public abstract void RemoveIfNeeded();
}

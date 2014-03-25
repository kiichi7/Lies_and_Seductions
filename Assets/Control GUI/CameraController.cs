/**********************************************************************
 *
 * CLASS CameraController
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

public class CameraController : MonoBehaviour {
	
	class HideInfo {
		public GameObject obj;
		public Component[] renderers;
		public AbstractSpeechBalloon speechBalloon;
		public StatusIndicatorBillboard statusIndicator;
		
		public HideInfo(GameObject obj, Component []renderers, 
						AbstractSpeechBalloon speechBalloon, StatusIndicatorBillboard statusIndicator) {
			this.obj = obj;
			this.renderers = renderers;
			this.speechBalloon = speechBalloon;
			this.statusIndicator = statusIndicator;
		}
	}
	
	public float cameraSwitchDamping = 3.0f;
	public float rotationDamping = 2.0f;
	public float moveCloserStepSize = 0.1f;
	
	public bool enableDevCameraControls = false;
	
	// Tryed to render sky and water with second camera. Did not work out: color correction 
	// effect applied to the first camera barfs... 
	//public Camera skyWaterCamera;
	
	private static CameraController instance;
	
	private CameraAngle destinationAngle;
	private CameraAngle currentAngle;
	
	private Quaternion rotationAtAngleChange;
	private Vector3 positionAtAngleChange;
	private float currentRotationAngle;
	private float spinningAngle;
	
	private Transform target;
	private bool angleChanged;
	private bool mirror;
	
	private ArrayList hiddenCharacters;
	
	public void Awake(){
		instance = this;
	}
	
	public void Start(){
		SetDefaultCameraAngle();
		currentAngle = new CameraAngle(destinationAngle.angle, destinationAngle.forwardOffset, destinationAngle.height, destinationAngle.tilt, destinationAngle.distance, destinationAngle.fieldOfView, destinationAngle.spinningSpeed, true);
		currentRotationAngle = target.transform.eulerAngles.y;
		angleChanged = true;
		mirror = false;
		hiddenCharacters = new ArrayList();
	}
	
	public void Update(){
		if (Input.GetKey("1")){
			destinationAngle = CameraAngle.BEHIND;
		} else if (Input.GetKey("2")){
			destinationAngle = CameraAngle.SEMI_CLOSE_UP_PC;
		} else if (Input.GetKey("3")){
			destinationAngle = CameraAngle.CLOSE_UP_PC;
		} else if (Input.GetKey("x")) {
			string fileName="screenshot" + Time.time + ".png";
			Application.CaptureScreenshot(fileName);	
		} else if (enableDevCameraControls) {
			if (Input.GetKey("t")){
				destinationAngle.MoveCloser();
			} else if (Input.GetKey("g")){
				destinationAngle.MoveFarther();
			} else if (Input.GetKey("f")){
				destinationAngle.TurnClockwise();
			} else if (Input.GetKey("h")){
				destinationAngle.TurnCounterClockwise();
			} else if (Input.GetKey("u")){
				destinationAngle.MoveUp();
			} else if (Input.GetKey("j")){
				destinationAngle.MoveDown();
			} else if (Input.GetKey("i")){
				destinationAngle.TiltDown();
			} else if (Input.GetKey("k")){
				destinationAngle.TiltUp();
			} else if (Input.GetKey("o")){
				destinationAngle.MoveForward();
			} else if (Input.GetKey("l")){
				destinationAngle.MoveBackward();
			} else if (Input.GetKey("b")){
				destinationAngle.DecreaseFieldOfView();
			} else if (Input.GetKey("n")){
				destinationAngle.IncreaseFieldOfView();
			} else if (Input.GetKey("m")){
				destinationAngle.PrintValues();
			}
		}
	}
	
	private void UpdateCurrentAngle(){
		float destinationAngleAngleWithMirror;
		if (mirror){
			destinationAngleAngleWithMirror = -destinationAngle.angle;
		} else {
			destinationAngleAngleWithMirror = destinationAngle.angle;
		}
		currentAngle.forwardOffset = Mathf.Lerp(currentAngle.forwardOffset, destinationAngle.forwardOffset, cameraSwitchDamping * Time.deltaTime);
		currentAngle.angle = Mathf.LerpAngle(currentAngle.angle, destinationAngleAngleWithMirror, cameraSwitchDamping * Time.deltaTime);
		currentAngle.height = Mathf.Lerp(currentAngle.height, destinationAngle.height, cameraSwitchDamping * Time.deltaTime);			currentAngle.tilt = Mathf.Lerp(currentAngle.tilt, destinationAngle.tilt, cameraSwitchDamping * Time.deltaTime);
		currentAngle.distance = Mathf.Lerp(currentAngle.distance, destinationAngle.distance, cameraSwitchDamping * Time.deltaTime);
		currentAngle.fieldOfView = Mathf.Lerp(currentAngle.fieldOfView, destinationAngle.fieldOfView, cameraSwitchDamping * Time.deltaTime);
		currentAngle.moveWithTarget = destinationAngle.moveWithTarget;
	}
	
	private void UpdateCameraPosition(){
		if (instance.angleChanged || currentAngle.moveWithTarget){
			positionAtAngleChange = new Vector3(target.position.x, CharacterManager.GetPC().transform.position.y, target.position.z);
			rotationAtAngleChange = target.rotation;
			instance.angleChanged = false;
		}
		spinningAngle += destinationAngle.spinningSpeed * Time.deltaTime;
		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, rotationAtAngleChange.eulerAngles.y, rotationDamping * Time.deltaTime);
		transform.position = positionAtAngleChange;
		transform.rotation = Quaternion.Euler (0, currentRotationAngle + spinningAngle, 0);
		transform.Translate(Vector3.forward * currentAngle.forwardOffset);
		transform.RotateAround(transform.position, Vector3.up, currentAngle.angle);
		transform.RotateAround(transform.position, transform.right, currentAngle.tilt);
		transform.Translate((Vector3.zero - Vector3.forward) * currentAngle.distance);
		transform.Translate(Vector3.up * currentAngle.height);
		camera.fieldOfView = currentAngle.fieldOfView;
	}
	
	private bool IsObstructed(){
		Transform targetHead = CharacterAnimator.FindChild(target, "bone_head");
		RaycastHit[] hits = Physics.RaycastAll(targetHead.position, transform.position - targetHead.position, Vector3.Distance(targetHead.position, transform.position));
		Collider[] collidersOnCamera = Physics.OverlapSphere(transform.position, 0.01f);
		foreach (RaycastHit hit in hits){
			if (hit.collider.tag.Equals("Wall")){
				return true;
			}
		}
		foreach (Collider collider in collidersOnCamera){
			if (collider.tag.Equals("Wall")){
				return true;
			}
		}
		return false;
	}
	
	private void HandleObstruction(){
		//Debug.Log("HandleObstruction");
		//Debug.Log("Distance to head before: " + Vector3.Distance(transform.position, targetHead.position));
		bool obstructed = IsObstructed();
		do {
			//Debug.Log("Looping.");
			obstructed = IsObstructed();
			if (obstructed){	
				transform.Translate(Vector3.forward * moveCloserStepSize);
			}
		} while (obstructed == true);
		//Debug.Log("Distance to head after: " + Vector3.Distance(transform.position, targetHead.position));
		//transparentPC.UpdateAlpha();
	}
	
	private void UpdateAudio(){
		FModManager.UpdateCameraPosition(target.position, target.forward, target.up);
	}
	
	// LateUpdate is called once per frame
	public void LateUpdate () {
		UpdateCurrentAngle();
		UpdateCameraPosition();
		if (CharacterManager.IsPC(target.gameObject) || CharacterManager.IsMajorNPC(target.gameObject)){
			HandleObstruction();
		}
		HideCloseCharacters();
		UpdateAudio();
	}
	
	private void HideCloseCharacters() {
		// Is there better way to do this?
		GameObject []all = CharacterManager.GetCharacters();
		foreach(GameObject character in all) {
			float sqrtLen = (character.transform.position - transform.position).sqrMagnitude;
			// We hide all characters that are closer than close clipping plane to camera.
			if(sqrtLen<0.9f) {
				// First we see if the object is already hidden
				HideInfo hideInfo = GetHideInfo(character);
				if(hideInfo == null) {
					// Not hidden, we need to hide it by disabling renderers.
					Component [] renderers = character.GetComponentsInChildren(typeof(Renderer));
					foreach(Renderer renderer in renderers) {
						renderer.enabled = false;	
					}
					AbstractSpeechBalloon speechBalloonScript = character.GetComponentInChildren(typeof(AbstractSpeechBalloon)) as AbstractSpeechBalloon;
					StatusIndicatorBillboard statusIndicatorScript = character.GetComponentInChildren(typeof(StatusIndicatorBillboard)) as StatusIndicatorBillboard;
					hiddenCharacters.Add(new HideInfo(character, renderers, speechBalloonScript, statusIndicatorScript));
				}
			}
			else {
				HideInfo hideInfo = GetHideInfo(character);
				if(hideInfo != null) {
					hiddenCharacters.Remove(hideInfo);
					// Now we make the character visible again by enabling all its renderers
					foreach(Renderer renderer in hideInfo.renderers) {
						renderer.enabled = true;	
					}
					// After enabling renderers we need to disable billboar speech balloon
					// if the billboard is not displaying anything...
					if(hideInfo.speechBalloon) {
						hideInfo.speechBalloon.RemoveIfNeeded();
					}
					if(hideInfo.statusIndicator) {
						hideInfo.statusIndicator.HideIfNeeded();
					}
				}	
			}
		}
	}
	
	private HideInfo GetHideInfo(GameObject obj) {
		foreach(HideInfo hidden in hiddenCharacters) {
			if(hidden.obj == obj) {
				return hidden;	
			} 
		}
		return null;	
	}
	
	private void ClearMirror(){
		mirror = false;
	}
	
	public static void SetCameraAngle(CameraAngle destinationAngle, GameObject pc, GameObject npc, bool useDamping){
		if (destinationAngle == CameraAngle.SEMI_CLOSE_UP_NPC || destinationAngle == CameraAngle.CLOSE_UP_NPC){
			instance.target = npc.transform;
		} else {
			instance.target = pc.transform;
		}
		instance.destinationAngle = destinationAngle;
		if (!useDamping){
			instance.currentAngle.forwardOffset = destinationAngle.forwardOffset;
			if (instance.mirror){
				instance.currentAngle.angle = -destinationAngle.angle;
			} else {
				instance.currentAngle.angle = destinationAngle.angle;
			}
			instance.currentAngle.height = destinationAngle.height;
			instance.currentAngle.distance = destinationAngle.distance;			
		}
		instance.angleChanged = true;
		instance.spinningAngle = 0.0f;
	}
	
	public static void SetDefaultCameraAngle(){
		instance.destinationAngle = CameraAngle.BEHIND;
		instance.target = CharacterManager.GetPC().transform;
		instance.angleChanged = true;
		instance.spinningAngle = 0.0f;
		instance.ClearMirror();
	}
	
	public static void SetPokerCameraAngle(){
		instance.destinationAngle = CameraAngle.POKER;
		instance.target = PokerController.GetTable().transform;
		instance.angleChanged = true;
		instance.spinningAngle = 0.0f;
		instance.ClearMirror();
	}
	
	public static void SetDanceCameraAngle(){
		instance.destinationAngle = CameraAngle.DANCE;
		instance.target = CharacterManager.GetPC().transform;
		instance.angleChanged = true;
		instance.spinningAngle = 0.0f;
		instance.ClearMirror();
	}
	
	public static void SetMirror(bool mirror){
		instance.mirror = mirror;
	}
	
	public static void SetArea(Area area) {
		//Debug.Log("CameraController.SetArea(" + area.gameObject.name  +"): far clip plane=" + area.farClipPlain);
		instance.gameObject.camera.farClipPlane = area.farClipPlain;
	}
}

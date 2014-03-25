/**********************************************************************
 *
 * CLASS CharacterAnimator
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
using System;

public class CharacterAnimator : MonoBehaviour {

	private const string COPY_SUFFIX = "_copy";
	public const int BOTTOM_LAYER = 0;
	public const int EMOTION_LAYER = 1;
	public const int GENERIC_LAYER = 2;
	public const int SIT_LAYER = 3;
	public const int HOLD_LAYER = 4;
	
	private string emotionName = null;
	private Emotion.BodyParts emotionBodyParts;
	
	//public float neckTurningStep = 9.0f;
	//public float maxNeckTurningAngle = 90.0f;
	
	//public Vector3 forwardVectorOfNeck = Vector3.forward;
	//public Vector3 upVectorOfNeck = Vector3.up;

	//private bool turnNeck = false;
	//private Vector3 neckLookingPosition = Vector3.zero;
	//private float neckTurningAngle = 0.0f;
	
	private Animation modelAnimation;
	private Neck neck;
	
	private bool animationsPaused = false;
	
	private CharacterState state;
	
	public void Start(){
		modelAnimation = FindChild(transform, name + " Model").animation;
		neck = GetComponentInChildren(typeof(Neck)) as Neck;
		state = GetComponent(typeof(CharacterState)) as CharacterState;
	}

	public void StartAnimation(string animationName, WrapMode wrapMode, int layer, string[] recursiveBoneNames, string[] unrecursiveBoneNames, float speed, bool updateEmotionLayer){
		/*if (animationName.Equals("sit_chair")){
			try {
				if (modelAnimation[animationName] == null){
					Debug.LogError("CharacterAnimator.StartAnimation(): Animation " + animationName + " doesn't exist on " + name + "!");
					return;
				}
				string copyName = animationName + COPY_SUFFIX;
				if (modelAnimation[copyName] != null){
					//Debug.Log("CharacterAnimator.StartAnimation() " + name + " removing clip " + copyName);
					AnimationClip c = modelAnimation[copyName].clip;
					modelAnimation.RemoveClip(modelAnimation[copyName].clip);
					Destroy(c);
				}
				//Debug.Log("CharacterAnimator.StartAnimation() " + name + " adding clip " + copyName);
				modelAnimation.AddClip(modelAnimation[animationName].clip, copyName);
				modelAnimation[copyName].layer = 3;
				modelAnimation[copyName].wrapMode = wrapMode;
				modelAnimation[copyName].speed = speed;
			
				//Debug.Log("CharacterAnimator.StartAnimation() " + name + " clip count: " + modelAnimation.GetClipCount());
			
				if (gameObject == CharacterManager.GetPC() && animationName.Equals("walk")){
					//Debug.Log("CharacterAnimator.StartAnimation(): animationevent created !!!!!!!!!!!!!!!!" + Time.time);
					AnimationEvent footstep = new AnimationEvent();
					footstep.time = 0;
					footstep.functionName = "Step";
					modelAnimation[copyName].clip.AddEvent(footstep);
					footstep = new AnimationEvent();
					footstep.time = 0.7f;
					footstep.functionName = "Step";
					modelAnimation[copyName].clip.AddEvent(footstep);
				}
			
				foreach (string recursiveBoneName in recursiveBoneNames){
					modelAnimation[copyName].AddMixingTransform(FindChild(transform, recursiveBoneName));
				}
				foreach (string unrecursiveBoneName in unrecursiveBoneNames){
					modelAnimation[copyName].AddMixingTransform(FindChild(transform, unrecursiveBoneName), false);
				}
				modelAnimation.CrossFade(animationName);
				if (updateEmotionLayer){
					UpdateEmotionLayer(emotionBodyParts);
				}
			} catch (Exception exception){
				Debug.LogError("CharacterAnimator.StartAnimation(): " + name + ", Animation error caught: " + exception);
			}			
			return;	
		}*/
		
		
		//Debug.Log("CharacterAnimator.StartAnimation(): " + name + ", starting " + animationName);
		try {
			if (modelAnimation[animationName] == null){
				Debug.LogError("CharacterAnimator.StartAnimation(): Animation " + animationName + " doesn't exist on " + name + "!");
				return;
			}
			string copyName = animationName + COPY_SUFFIX;
			if (modelAnimation[copyName] != null){
				//Debug.Log("CharacterAnimator.StartAnimation() " + name + " removing clip " + copyName);
				AnimationClip c = modelAnimation[copyName].clip;
				modelAnimation.RemoveClip(modelAnimation[copyName].clip);
				Destroy(c);
			}
			//Debug.Log("CharacterAnimator.StartAnimation() " + name + " adding clip " + copyName);
			modelAnimation.AddClip(modelAnimation[animationName].clip, copyName);
			modelAnimation[copyName].layer = layer;
			modelAnimation[copyName].wrapMode = wrapMode;
			modelAnimation[copyName].speed = speed;
			
			//Debug.Log("CharacterAnimator.StartAnimation() " + name + " clip count: " + modelAnimation.GetClipCount());
			
			if (gameObject == CharacterManager.GetPC() && animationName.Equals("walk")){
				//Debug.Log("CharacterAnimator.StartAnimation(): animationevent created !!!!!!!!!!!!!!!!" + Time.time);
				AnimationEvent footstep = new AnimationEvent();
				footstep.time = 0;
				footstep.functionName = "Step";
				modelAnimation[copyName].clip.AddEvent(footstep);
				footstep = new AnimationEvent();
				footstep.time = 0.7f;
				footstep.functionName = "Step";
				modelAnimation[copyName].clip.AddEvent(footstep);
			}
			
			foreach (string recursiveBoneName in recursiveBoneNames){
				modelAnimation[copyName].AddMixingTransform(FindChild(transform, recursiveBoneName));
			}
			foreach (string unrecursiveBoneName in unrecursiveBoneNames){
				modelAnimation[copyName].AddMixingTransform(FindChild(transform, unrecursiveBoneName), false);
			}
			modelAnimation.CrossFade(copyName);
			if (updateEmotionLayer){
				UpdateEmotionLayer(emotionBodyParts);
			}
		} catch (Exception exception){
			Debug.LogError("CharacterAnimator.StartAnimation(): " + name + ", Animation error caught: " + exception);
		}
	}
	
	private void StartAnimation(string animationName, WrapMode wrapMode, int layer, string[] recursiveBoneNames, string[] unrecursiveBoneNames, float speed){
		StartAnimation(animationName, wrapMode, layer, recursiveBoneNames, unrecursiveBoneNames, speed, true);
	}
	
	public void StartAnimation(string animationName, WrapMode wrapMode, Emotion.BodyParts emotionBodyParts, float speed){
		StartAnimation(animationName, wrapMode, BOTTOM_LAYER, new string[]{}, new string[]{}, speed);
		this.emotionBodyParts = emotionBodyParts;
		UpdateEmotionLayer(emotionBodyParts);
	}
	
	public void StartAnimation(string animationName, WrapMode wrapMode, Emotion.BodyParts emotionBodyParts){
		StartAnimation(animationName, wrapMode, emotionBodyParts, 1.0f);
	}
	
	/*public void StartAnimation(string animationName, WrapMode wrapMode, int layer, string[] recursiveBoneNames, string[] unrecursiveBoneNames, float speed){
		StartAnimation(animationName, wrapMode, layer, recursiveBoneNames, unrecursiveBoneNames, speed);
	}*/
	
	public void StartAnimation(string animationName, WrapMode wrapMode, int layer, string[] recursiveBoneNames, string[] unrecursiveBoneNames){
		StartAnimation(animationName, wrapMode, layer, recursiveBoneNames, unrecursiveBoneNames, 1.0f);
	}
	
	public void StartAnimation(string animationName, WrapMode wrapMode, int layer, string[] recursiveBoneNames, float speed){
		StartAnimation(animationName, wrapMode, layer, recursiveBoneNames, new string[]{}, speed);
	}

	public void StartAnimation(string animationName, WrapMode wrapMode, int layer, string[] recursiveBoneNames){
		StartAnimation(animationName, wrapMode, layer, recursiveBoneNames, 1.0f);
	}

	public void SetEmotion(Emotion emotion){
		if (!CharacterManager.IsGenericNPC(gameObject)){
			this.emotionName = emotion.GetAnimationName();
			UpdateEmotionLayer(emotionBodyParts);
		}
	}
	
	private void UpdateEmotionLayer(Emotion.BodyParts emotionBodyParts){
		if (emotionName != null){
			string[] boneNames;
			switch (emotionBodyParts){
			case Emotion.BodyParts.NONE:
				boneNames = new string[0];
				break;
			case Emotion.BodyParts.EYES:
				if (name.Equals("Ed")){
					boneNames = new string[3];
					boneNames[0] = "bone_scarf_left";
					boneNames[1] = "bone_scarf_middle";
					boneNames[2] = "bone_scarf_right";
				} else {
					boneNames = new string[4];
					boneNames[0] = "bone_brow_1_left";
					boneNames[1] = "bone_brow_1_right";
					boneNames[2] = "bone_brow_2_left";
					boneNames[3] = "bone_brow_2_right";
				}
				break;
			case Emotion.BodyParts.FACE:
				boneNames = new string[1];
				boneNames[0] = "BONE_HEAD";
				break;
			case Emotion.BodyParts.ALL:
				if (state.GetItem() == null && state.GetCurrentSeat() == null){
					boneNames = new string[0];
				} else {
					boneNames = new string[1];
					boneNames[0] = "BONE_HEAD";
				}
				break;
			default:
				boneNames = null;
				break;
			}
			if (emotionBodyParts == Emotion.BodyParts.NONE){
				StopAnimation(emotionName);
			} else {
				StartAnimation(emotionName, WrapMode.Loop, EMOTION_LAYER, boneNames, new string[]{}, 1.0f, false);
			}
		}
	}

	public bool IsPlaying(string animationName){
		return modelAnimation.IsPlaying(animationName + COPY_SUFFIX);
	}
	
	public void StopAnimation(string animationName){
		//Debug.Log("CharacterAnimator.StopAnimation(): " + name + ", stopping " + animationName);
		string copyName = animationName + COPY_SUFFIX;
		modelAnimation.Stop(copyName);
	}
	
	public void RestartAnimation(string animationName){
		//Debug.Log("CharacterAnimator.RestartAnimation(): " + name + ", restarting " + animationName);
		string copyName = animationName + COPY_SUFFIX;
		modelAnimation.Rewind(copyName);
	}
	
	public void TurnHead(Vector3 targetPosition){
		neck.SetLookPosition(targetPosition);
	}
	
	public void ResetHeadDirection(){
		neck.RemoveLookPosition();
	}
	
	public static Transform FindChild(Transform parent, string endOfName){
		Component[] transforms = parent.GetComponentsInChildren(typeof(Transform));
		foreach (Transform transform in transforms){
			if (transform.name.ToLower().EndsWith(endOfName.ToLower())){
				return transform;
			}
		}
		Debug.LogError("CharacterAnimator.FindChild(): Transform with " + endOfName + " not found under " + parent.name);
		return null;
	}
	
	public static Transform FindShoulder(Transform parent, string itemName){
		Transform child = FindChild(parent, itemName + "_hook");
		do {
			child = child.parent;
		} while (!child.name.ToLower().Contains("bone_shoulder"));
		return child;
	}
	
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
	
	public static Transform FindChild(GameObject parent, string endOfName){
		return FindChild(parent.transform, endOfName);
	}
	
	private void PauseAllAnimations(){
		animationsPaused = true;
		foreach (AnimationState animationState in modelAnimation){
			animationState.speed = 0.0f;
		}
	}
	
	private void UnpauseAllAnimations(){
		animationsPaused = false;
		foreach (AnimationState animationState in modelAnimation){
			animationState.speed = 1.0f;
		}
	}
	
	public void Update(){
		if (gameObject == CharacterManager.GetPC()){
			string copyName = "hold_beer_copy";
			bool playing = modelAnimation.IsPlaying(copyName);
			//Debug.Log("Playing hold_beer? " + playing);
			//if (playing){
			//	Debug.Log("Layer: " + modelAnimation[copyName].layer);
			//	Debug.Log("WrapMode: " + modelAnimation[copyName].wrapMode);
			//}
			copyName = "idle_pose_1_copy";
			playing = modelAnimation.IsPlaying(copyName);
			//Debug.Log("Playing idle_pose_1? " + playing);
			//if (playing){
			//	Debug.Log("Layer: " + modelAnimation[copyName].layer);
			//	Debug.Log("WrapMode: " + modelAnimation[copyName].wrapMode);
			//}
		}
		if (Pause.IsPaused() && !animationsPaused){
			PauseAllAnimations();
		} else if (!Pause.IsPaused() && animationsPaused){
			UnpauseAllAnimations();
		}
	}
	
	public void ResetAnimations(){
		Debug.Log("CharacterAnimator.ResetAnimations(): " + name);
		modelAnimation.Stop();
	}
	
	/*public void LateUpdate(){
		if (!Pause.IsPaused()){
			Transform neckTransform = FindChild(transform, "bone_neck");
			if (turnNeck){
				neckTransform.Rotate(neckTurningAngle * Vector3.Dot(upVectorOfNeck, Vector3.right), neckTurningAngle * Vector3.Dot(upVectorOfNeck, Vector3.up), neckTurningAngle * Vector3.Dot(upVectorOfNeck, Vector3.forward));
				Vector3 forwardVectorOfNeckWorldCoord = neckTransform.right * forwardVectorOfNeck.x + neckTransform.up * forwardVectorOfNeck.y + neckTransform.forward * forwardVectorOfNeck.z;
				Vector2 lookDirection = new Vector2(forwardVectorOfNeckWorldCoord.x, forwardVectorOfNeckWorldCoord.z);;
				Vector2 neckPosition = new Vector2(neckTransform.position.x, neckTransform.position.z);
				Vector2 targetPosition = new Vector2(neckLookingPosition.x, neckLookingPosition.z);
				Vector2 targetDirection = targetPosition - neckPosition;
				float angleInRadians = GetAngleBetweenVectors(lookDirection, targetDirection);
				if (angleInRadians < -0.3f){
					neckTurningAngle += neckTurningStep;
				} else if (angleInRadians > 0.3f){
					neckTurningAngle -= neckTurningStep;
				}
				if (neckTurningAngle > maxNeckTurningAngle){
					neckTurningAngle = maxNeckTurningAngle;
				} else if (neckTurningAngle < -maxNeckTurningAngle){
					neckTurningAngle = -maxNeckTurningAngle;
				}
			}
		}
	}*/
}
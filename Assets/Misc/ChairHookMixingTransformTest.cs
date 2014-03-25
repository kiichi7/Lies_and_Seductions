using UnityEngine;
using System.Collections;

public class ChairHookMixingTransformTest : MonoBehaviour {

	private bool started = false;
	
	public Transform model;
	public Transform[] recursiveMixingTransforms;
	public Transform[] unrecursiveMixingTransforms;

	void Update() {
		if (!started){
			CharacterAnimator animator = (CharacterAnimator)GetComponent("CharacterAnimator");
			animator.StartAnimation("walk", WrapMode.Loop, Emotion.BodyParts.NONE);
			animator.StartAnimation("sit_chair", WrapMode.Loop, CharacterAnimator.SIT_LAYER, new string[]{"root_pelvis", ":chair_hook"}, new string[]{"root_spine"});
			started = true;
			
			/*model.animation.Play("walk");
			model.animation["sit_chair"].layer = 1;
			foreach (Transform mixingTransform in recursiveMixingTransforms){
				model.animation["sit_chair"].AddMixingTransform(mixingTransform);
			}
			foreach (Transform mixingTransform in unrecursiveMixingTransforms){
				model.animation["sit_chair"].AddMixingTransform(mixingTransform, false);
			}
			model.animation.Play("sit_chair");*/
		}
	}
}
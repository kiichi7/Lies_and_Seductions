using UnityEngine;
using System.Collections;

public class AddMixingTransformTest : MonoBehaviour {
	private bool legsWalking = false;
	private bool torsoWalking = false;

	// Use this for initialization
	void Start () {
		animation["emotion:happy"].AddMixingTransform(transform.Find("NULL_WAIST/ROOT_SPINE"));
		animation["emotion:angry"].AddMixingTransform(transform.Find("NULL_WAIST/ROOT_SPINE"));
		animation.wrapMode = WrapMode.Loop;
		animation["idle"].layer = -1;
		animation["walk"].layer = -1;
		animation.Stop();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Horizontal") > 0 && !legsWalking){
			animation.CrossFade("walk");
			legsWalking = true;
		} else if (Input.GetAxis("Horizontal") < 0 && legsWalking){
			animation.CrossFade("idle");
			legsWalking = false;
		}
		if (Input.GetAxis("Vertical") > 0 && torsoWalking){
			animation.CrossFade("emotion:happy");
			torsoWalking = false;
		} else if (Input.GetAxis("Vertical") < 0 && !torsoWalking){
			animation.CrossFade("emotion:angry");
			torsoWalking = true;
		}
	}
}

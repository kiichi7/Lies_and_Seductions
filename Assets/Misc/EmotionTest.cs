using UnityEngine;
using System.Collections;

public class EmotionTest : MonoBehaviour {

	public int speed = 1;
	public int gravity = 20;
	public int rotationSpeed = 60;
	
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 rotationDirection = Vector3.zero;
	private bool grounded = false;
	private bool walking = false;
	
	private AnimationState idle;
	private AnimationState walk;
	private AnimationState emotionHappy;
	private AnimationState emotionNeutral;
	private AnimationState emotionAngry;
	
	public Transform lowerBack;
	public Transform head;

	public void Start(){
		animation.wrapMode = WrapMode.Loop;
		
		idle = animation["idle"];
		walk = animation["walk"];
		emotionHappy = animation["emotion:happy"];
		emotionNeutral = animation["emotion:neutral"];
		emotionAngry = animation["emotion:angry"];
		
		idle.layer = -1;
		walk.layer = -1;
		
		animation.Play("idle");
	}

	public void FixedUpdate(){
		bool userMoved = false;
		if (grounded) {
			moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
			
			if (moveDirection.magnitude > 0.1){
				userMoved = true;
			}
			
			rotationDirection = new Vector3(0, Input.GetAxis("Horizontal"), 0);
			
			transform.Rotate(rotationDirection * Time.deltaTime * rotationSpeed);
			
		}
		moveDirection.y -= gravity * Time.deltaTime;

		CharacterController controller = (CharacterController)gameObject.GetComponent(typeof(CharacterController));
		CollisionFlags flags = controller.Move(moveDirection * Time.deltaTime);
		grounded = (flags & CollisionFlags.CollidedBelow) != 0;
		
		if (userMoved){
		}
					
		if (userMoved && !walking){
			animation.CrossFade("walk");
			//animation.Blend("idle", 0.0F);
			walking = true;
		} else if (!userMoved && walking) {
			animation.CrossFade("idle");
			//animation.Blend("walk", 0.0F);
			walking = false;
		}
		updateEmotion();
	}
	
	private void updateEmotion(){
		bool isHappy = Input.GetKeyDown("1");
		bool isNeutral = Input.GetKeyDown("2");
		bool isAngry = Input.GetKeyDown("3");
		
		if (walking){
			emotionHappy.AddMixingTransform(head);
			emotionNeutral.AddMixingTransform(head);
			emotionAngry.AddMixingTransform(head);			
		} else {
			emotionHappy.AddMixingTransform(lowerBack);
			emotionNeutral.AddMixingTransform(lowerBack);
			emotionAngry.AddMixingTransform(lowerBack);
		}
		if (isHappy){
			Debug.Log("Happy");
			animation.CrossFade("emotion:happy");
		} else if (isNeutral){
			Debug.Log("Neutral");
			animation.CrossFade("emotion:neutral");
		} else if (isAngry){
			Debug.Log("Angry");
			animation.CrossFade("emotion:angry");
		}
		
	}
}
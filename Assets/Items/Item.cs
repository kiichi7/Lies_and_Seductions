using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	private Transform hand;
	
	public void SetHand(Transform hand, bool isHotSpot){
		this.hand = hand;
		if (!isHotSpot){
			HotSpot hotSpot = (HotSpot)GetComponentInChildren(typeof(HotSpot));
			if (hotSpot != null){
				hotSpot.gameObject.layer = 0;
			}
		}
		//transform.parent = hand;
		//transform.localPosition = Vector3.zero;
		//transform.localRotation = Quaternion.identity;
		//transform.localScale = new Vector3(transform.localScale.x / transform.lossyScale.x, transform.localScale.y / transform.lossyScale.y, transform.localScale.z / transform.lossyScale.z);
	}	
	
	public void LateUpdate(){
		if (hand){
			transform.position = hand.position;
			transform.rotation = hand.rotation;
		}
	}

}

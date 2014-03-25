using UnityEngine;
using System.Collections;

public class Seat : MonoBehaviour {

	public string seatType = "chair";
	public Waypoint waypoint;
	
	private Transform hook = null;
	private Vector3 originalPosition;
	private Quaternion originalRotation;
	private bool taken;

	public Waypoint GetWaypoint(){
		return waypoint;
	}

	public void Awake(){
		originalPosition = transform.position;
		originalRotation = transform.rotation;
		taken = false;
	}

	public bool IsTaken(){
		return taken;
	}
	
	public void SetTaken(bool taken){
		this.taken = taken;
	}

	public void Attach(Transform hook){
		this.hook = hook;
	}
	
	public void Detach(){
		//Debug.Log("Seat.Detach");
		hook = null;
	}
	
	public void ResetPosition(){
		transform.position = originalPosition;
		transform.rotation = originalRotation;
	}
	
	public new string GetType(){
		return seatType;
	}

	public void Update () {
		if (hook != null){
			transform.position = hook.position;
			transform.rotation = hook.rotation;
		}
	}
}

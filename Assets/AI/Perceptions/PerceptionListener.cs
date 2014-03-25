using UnityEngine;
using System.Collections;

public class PerceptionListener : MonoBehaviour {

	public float maxVisionAngle = 90.0f;

	ImpressionMemory impressionMemory;
	ActionRunner actionRunner;
	Neck neck;

	public void Start(){
		impressionMemory = (ImpressionMemory)GetComponent("ImpressionMemory");
		actionRunner = (ActionRunner)GetComponent("ActionRunner");
		neck = (Neck)GetComponentInChildren(typeof(Neck));
		enabled=false;
	}
	
	public bool CanSee(Vector3 sourcePosition){
		Vector3 lookDirection = neck.GetCurrentLookDirectionWorldCoords();
		Vector3 ownPosition = transform.position;
		Vector3 sourceDirection = (sourcePosition - ownPosition).normalized;
		float angle = Vector3.Angle(lookDirection, sourceDirection);
		
		bool viewBlocked = Physics.Raycast(neck.transform.position, sourcePosition - neck.transform.position, Vector3.Distance(neck.transform.position, sourcePosition), NavmeshEdge.NAVMESH_LAYER_NUMBER);
		bool lookingAtPC = angle < maxVisionAngle;
		//Debug.Log("PerceptionListener.CanSee() viewBlocked=" + viewBlocked + ", lookingPC=" +  lookingAtPC);
		return lookingAtPC && !viewBlocked;
	}

	public void Perceive(Perception perception){
		//Debug.Log("PerceptionListener.Perceive()" + name + " is perceiving!");
		GameObject source = perception.GetSource();
		ImpressionAdjuster adjuster = perception.GetImpressionAdjuster();
		Action action = perception.GetAction(gameObject);
		
		if (adjuster != null){
			impressionMemory.ImpressionAdjusted(source, adjuster);
		}
		
		if (action != null){
			actionRunner.ResetRoutine(action, false);
		}
	}
}
using UnityEngine;
using System.Collections;

public class PerceptionManager /*: MonoBehaviour*/{

	//private PerceptionManager instance;
	
	/*public void Start(){
		//instance = this;
		enabled=false;
	}*/
	
	public static void BroadcastPerception(Perception perception){
		//Debug.Log("Broadcasting perception.");
		foreach (GameObject npc in CharacterManager.GetMajorNPCs()){
			//Debug.Log("Iterating broadcast: " + npc.name);
			CharacterState sourceState = perception.GetSource().GetComponent(typeof(CharacterState)) as CharacterState;
			Area sourceArea = sourceState.GetCurrentArea();
			PerceptionListener perceptionListener = npc.GetComponent(typeof(PerceptionListener)) as PerceptionListener;
			float distance = Vector3.Distance(perception.GetPosition(), npc.transform.position);
			//Debug.Log("Distance okay? " + (distance < sourceArea.GetPerceptionDistance()));
			//Debug.Log("Vision okay?" + (perceptionListener.CanSee(perception.GetPosition())));
			if (distance < sourceArea.GetEasyPerceptionDistance() || (distance < sourceArea.GetPerceptionDistance() && perceptionListener.CanSee(perception.GetPosition()))){
				//Debug.Log("PerceptionManager.BroadcastPerception(): " + npc.name);
				perceptionListener.Perceive(perception);
			}
		}
	}
	
	public static ArrayList BroadcastPerceptionAndReturnWhoSaw(Perception perception){
		//Debug.Log("Broadcasting perception.");
		ArrayList witnesses = new ArrayList();
		foreach (GameObject npc in CharacterManager.GetMajorNPCs()){
			//Debug.Log("Iterating broadcast: " + npc.name);
			CharacterState sourceState = perception.GetSource().GetComponent(typeof(CharacterState)) as CharacterState;
			Area sourceArea = sourceState.GetCurrentArea();
			PerceptionListener perceptionListener = npc.GetComponent(typeof(PerceptionListener)) as PerceptionListener;
			float distance = Vector3.Distance(perception.GetPosition(), npc.transform.position);
			//Debug.Log("Distance okay? " + (distance < sourceArea.GetPerceptionDistance()));
			//Debug.Log("Vision okay?" + (perceptionListener.CanSee(perception.GetPosition())));
			if (distance < sourceArea.GetEasyPerceptionDistance() || (distance < sourceArea.GetPerceptionDistance() && perceptionListener.CanSee(perception.GetPosition()))){
				perceptionListener.Perceive(perception);
				witnesses.Add(npc);
			}
		}
		return witnesses;
	}
	
	public static void SendImpression(GameObject target, string impression, int val) {
		//Debug.Log("FollowAction.SendImpression " + impression + " to " + targetCharacter.name);
		ImpressionPush impressionPush = new ImpressionPush(impression, new NumericConstant(val), false, null);
		Perception perception = new ImpressionPerception(CharacterManager.GetPC(), impressionPush);
		PerceptionListener perceptionListener = target.GetComponent(typeof(PerceptionListener)) as PerceptionListener;
		perceptionListener.Perceive(perception);
	}
}

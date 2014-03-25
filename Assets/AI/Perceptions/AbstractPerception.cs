using UnityEngine;
using System.Collections;

public abstract class AbstractPerception : Perception {

	protected GameObject source;
	
	public AbstractPerception(GameObject source){
		this.source = source;
	}

	public GameObject GetSource(){
		return source;
	}
	
	public virtual ImpressionAdjuster GetImpressionAdjuster(){
		return null;
	}
	
	public virtual Action GetAction(GameObject actor){
		return null;
	}
	
	public Vector3 GetPosition(){
		return source.transform.position;
	}

}

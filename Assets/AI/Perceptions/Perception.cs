using UnityEngine;
using System.Collections;

public interface Perception {

	GameObject GetSource();
	
	ImpressionAdjuster GetImpressionAdjuster();
	
	Action GetAction(GameObject actor);
	
	Vector3 GetPosition();
}

using UnityEngine;
using System.Collections;

public interface PokerStage {

	//Returns true if the stage is finished
	bool UpdateStage();
	
	void DrawGUI();

}

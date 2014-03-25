/**********************************************************************
 *
 * CLASS 
 *
 * Copyright 2008 Tommi Horttana, Petri Lankoski, Jari Suominen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License 
 * at http://www.apache.org/licenses/LICENSE-2.0 Unless required 
 * by applicable law or agreed to in writing, software distributed 
 * under the License is distributed on an "AS IS" BASIS, WITHOUT 
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 * See the License for the specific language governing permissions and 
 * limitations under the License.
 *
 ***********************************************************************/
using UnityEngine;
using System.Collections;

public class DanceGUI : MonoBehaviour {
	
	public GUISkin gSkin;
	public Texture2D helpPic;
	
	public BeatMarkerIconSet beatMarkers;
	public BeatMarkerIconSet beatMarkersHit;
	public BeatMarkerIconSetHotMode beatMarkersHotMode;
	
	// Effect Textures for a Beatmarker in HOT MODE
	//public Texture2D []hotModeBeatMarkerEffects;
	
	public DanceBackgroundTextureSet background;
	public DanceBackgroundTextureSet backgroundHotMode;
	
	// HOT MODE counter
	public Texture2D []hotModeScore = new Texture2D[11];
	public Texture2D []hotModeLeft = new Texture2D[11];
	
	// Score counter, NORMAL MODE, the code will expect that there is 26 items in array	
	public Texture2D []scoreCounter = new Texture2D[26];
	// Score counter, HOT MODE, the code will expect that there is 26 items in array
	public Texture2D []scoreCounterHotMode = new Texture2D[26];
	
	//public Texture2D []scoreCounterAfterGame = new Texture2D[10];
	
	// size of dance GUI
	public static int danceScreenHeight=640;
	public static int danceScreenWidth=250; 

	// distance between beat markers
	public static int beatMarkerXOffset=82;
	public static int beatMarkerLeftRowOffset=44;
	
	//beat marker size
	public static int beatMarkerWidth=32; 
	public static int beatMarkerHeight=32;
		
	public static int beatMarkerDrawWidth=48;
	public static int beatMarkerDrawHeight=70;
	
	//target area height, counted from the top of the drop area (the center of the beat marker must hit this area)
	public static int targetAreaStartY = 610;
	public static int targetAreaEndY = 690;
	
	private static DanceGUI instance;
	
	private const string helpText = "Match the dropping block with music. \n\nPress SPACE to give up. You reveive small penalty if you do this.\n\nWhat the others (who see you dancing) think about you may change according how well you dance.\n\nPress any key to start.";
	
	/******************************************************************************
	 *
	 * PUBLIC STATIC METHODS
	 *
	 *****************************************************************************/
	public static void DrawGUI(bool hotMode, ArrayList beatMarkers, int score, int hotModeBeat, int successesInRow, int hotModeRequisite){
		instance.InstanceDrawGUI(hotMode, beatMarkers, score, hotModeBeat, successesInRow, hotModeRequisite);
	}
	
	public static bool DrawScoreGUI(int score, int hits, int errors, ArrayList witnesses) {
		return instance.InstanceDrawScoreGUI(score, hits, errors, witnesses);
	}
	
	public static bool DrawHelpGUI() {
		return instance.InstanceDrawHelpGUI();
	}
	
	public static BeatMarker CreateBeatMarker(Direction direction){
		Texture2D normalIcon = null;
		Texture2D hitIcon = null;
		Texture2D[] hotModeIcons = null;
		switch (direction){
		case Direction.LEFT:
			normalIcon = instance.beatMarkers.left;
			hitIcon = instance.beatMarkersHit.left;
			hotModeIcons = instance.beatMarkersHotMode.left;
			break;
		case Direction.DOWN:
			normalIcon = instance.beatMarkers.down;
			hitIcon = instance.beatMarkersHit.down;
			hotModeIcons = instance.beatMarkersHotMode.down;
			break;
		case Direction.RIGHT:
			normalIcon = instance.beatMarkers.right;
			hitIcon = instance.beatMarkersHit.right;
			hotModeIcons = instance.beatMarkersHotMode.right;
			break;
		case Direction.UP:
			normalIcon = instance.beatMarkers.up;
			hitIcon = instance.beatMarkersHit.up;
			hotModeIcons = instance.beatMarkersHotMode.up;
			break;
		}
		return new BeatMarker(normalIcon, hitIcon, hotModeIcons, direction);
	}
		

	/******************************************************************************
	 *
	 * PRIVATE METHODS
	 *
	 *****************************************************************************/
	
	private string NPCListToString(ArrayList witnesses) {
		int n = 0;
		string feedbackText = "";
		foreach(GameObject npc in witnesses) {
				
			feedbackText += npc.name;
			if(witnesses.Count == 1) {
				feedbackText += " ";
			} else if(n >= witnesses.Count) {
				feedbackText += " and ";	
			}
			else {
				feedbackText += ", ";
			}
			n++;
		}
		return feedbackText;
	}
	
	private string GetWitnessesCommentWithPositiveImpressions(ArrayList witnesses) {
		if(witnesses.Count > 0) {
			return ", and " + NPCListToString(witnesses) + "saw you dancing!";
		}
		else {
			return ", but unfortunately no-one saw you dancing!";	
		}
	}
	
	private bool InstanceDrawScoreGUI(int score, int hits, int errors, ArrayList witnesses) {
		//Debug.Log("DanceGUI.InstanceDrawScoreGUI(" + score + ", " + hits + ", " + errors);
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		GUI.skin = gSkin;	
		GUILayout.BeginArea(new Rect(GUIGlobals.GetCenterX()-100, 200, 200, 300), "", "helpBG");
		GUILayout.BeginVertical();
		string feedbackText = "";
		switch(score) {
			case 5:
				feedbackText = "That was perfect" + GetWitnessesCommentWithPositiveImpressions(witnesses);
				break;
			case 4:
				feedbackText = "That was exquisite" + GetWitnessesCommentWithPositiveImpressions(witnesses);
				break;
			case 3:
				feedbackText = "That was cool" + GetWitnessesCommentWithPositiveImpressions(witnesses);
				break;
			case 2:
				feedbackText = "That was good" + GetWitnessesCommentWithPositiveImpressions(witnesses);
				break;
			case 1:
				feedbackText = "That was okey" + GetWitnessesCommentWithPositiveImpressions(witnesses);
				break;
			case 0:
				feedbackText = "That was decent dancing.";
				break;
			case -1:
				feedbackText = "You sucked, ";
				if(witnesses.Count > 0) {
					feedbackText += "and " + NPCListToString(witnesses);
					feedbackText += " saw it!";
				}
				else {
					feedbackText += "luckily no-one saw you!";	
				}
				break;
			default:
				feedbackText = "What happened?"; // Error, we should newer be here...
				break;	
		}
		GUILayout.Label(feedbackText);
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Succeeded Moves:");
		GUILayout.FlexibleSpace();
		GUILayout.Label(hits.ToString());
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Failed Moves: ");
		GUILayout.FlexibleSpace();
		GUILayout.Label(errors.ToString());
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Press any key to continue.");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
		
		return false;
	}
	
	/*private bool InstanceDrawScoreGUI(int score, int hits, int errors, ArrayList witnesses) {
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		GUI.skin = gSkin;	
		GUILayout.BeginArea(new Rect(GUIGlobals.GetCenterX()-100, 200, 200, 300), "", "helpBG");
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Succeeded Moves:");
		GUILayout.FlexibleSpace();
		GUILayout.Label(hits.ToString());
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Who saw:");
		GUILayout.FlexibleSpace();
		string saw = "";
		if(witnesses.Count > 0) {
			foreach(GameObject npc in witnesses) {
				saw += npc.name + " ";
			}
		}
		else {
			saw = "no-one";	
		}
		GUILayout.Label(saw.ToString());
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Failed Moves: ");
		GUILayout.FlexibleSpace();
		GUILayout.Label(errors.ToString());
		GUILayout.EndHorizontal();
		
		GUILayout.Space (40);
		int s = score/10;
		GUILayout.Label(scoreCounterAfterGame[Mathf.Min(s, 9)]);
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("HOTNESS", "Large Text");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Press any key to continue.");
		//if(GUILayout.Button("Continue")) {
		//	return true;	
		//}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
		return false;		
	}*/
	
	private bool InstanceDrawHelpGUI() {
		
		// Help is shown only once. If it already shown we just return here...
		if(IngameHelpMemory.ShouldShow(IngameHelpMemory.DANCE_HELP)) { return true; }
		
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		if(gSkin) {
			GUI.skin = gSkin;	
		}
		GUILayout.BeginArea(new Rect(GUIGlobals.GetCenterX()-160, 200, 320, 520), "", "helpBG");
		GUILayout.BeginVertical();
		
		
		GUILayout.Label(helpPic, "helpImage");
		
		
		GUILayout.Label(helpText);

		GUILayout.FlexibleSpace();		

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
		GUILayout.EndArea();
		
		return false;	
	}
	
	private void InstanceDrawGUI(bool hotMode, ArrayList beatMarkers, int score, int hotModeBeat, int successesInRow, int hotModeRequisite){
		// GUI and icons are designed for 4:3. We do not want stretching in wide screen displayes
		// so we make strange scale...
		float yScale = Screen.height / GUIGlobals.screenHeight;
		GUI.matrix = Matrix4x4.TRS(
			Vector3.zero,
			Quaternion.identity,
			new Vector3(yScale, yScale, 1)
		);
				
		// GUI placement, NEEDS A PROPER SOLUTION
		float miniGameGUIStartX;
		if(GUIGlobals.IsWideScreen()) {
			miniGameGUIStartX = GUIGlobals.screenWidth - danceScreenWidth - 50;
		}
		else {
			miniGameGUIStartX = 1024 - danceScreenWidth - 50;
		}
		
		float miniGameGUIStartY = 100;
		
		DanceBackgroundTextureSet currentBackground;
		
		if(hotMode == true) {
			currentBackground = backgroundHotMode;	
		} else {
			currentBackground = background;
		}
		
		//Draw the backgroud for the DANCE MINI game
		
		// Drawing the bavkground for the score area
		GUIStyle scoreAreaStyle = new GUIStyle();
		scoreAreaStyle.normal.background = currentBackground.scoreArea;
		GUI.Box(new Rect(miniGameGUIStartX,miniGameGUIStartY-50,
						 danceScreenWidth, 50),"", scoreAreaStyle);
		
		// Draw the drop area...
		GUIStyle dropAreaStyle = new GUIStyle();
		dropAreaStyle.normal.background = currentBackground.dropArea;
		GUI.Box(new Rect(miniGameGUIStartX, miniGameGUIStartY, 
						 danceScreenWidth, danceScreenHeight),"", dropAreaStyle);
		
		// Draw the target area...
		GUIStyle targetAreaStyle = new GUIStyle();
		targetAreaStyle.normal.background = currentBackground.targetArea;
		GUI.Box(new Rect(miniGameGUIStartX,miniGameGUIStartY+danceScreenHeight-2,
						 danceScreenWidth, 38),"", targetAreaStyle);
		
		
		// Now we create score counters...
		GUIStyle hotScoreStyle= new GUIStyle();
		GUIStyle scoreCounterStyle = new GUIStyle();
		int currScoreCounter = score / 4;
		if(hotMode == true) {
			int hot = 10 - (int)(((float)hotModeBeat/(float)DanceCoreography.HOT.Length)*10.0f);
			hotScoreStyle.normal.background=hotModeLeft[hot];
			scoreCounterStyle.normal.background=scoreCounter[currScoreCounter];
		} else {
			int hot = (int)(((float)successesInRow/(float)hotModeRequisite)*10.0f);
			hotScoreStyle.normal.background=hotModeScore[hot];
			scoreCounterStyle.normal.background=scoreCounterHotMode[currScoreCounter];
		}
		//Debug.Log("Success: " + successesInRow.ToString() + "/" + hotModeRequisite.ToString() + " hot: " + hot.ToString());
		GUI.BeginGroup(new Rect(miniGameGUIStartX,miniGameGUIStartY-50,danceScreenWidth, 50));
		
		GUI.Box(new Rect(10, 10, danceScreenWidth-20, 10), "", hotScoreStyle);
		GUI.Box(new Rect(8, 22, danceScreenWidth-20, 20), "", scoreCounterStyle);
		GUI.EndGroup();
		

		//  This is a group for drawing BeatMarkers
		GUI.BeginGroup(new Rect(miniGameGUIStartX, miniGameGUIStartY, 
								danceScreenWidth, danceScreenHeight+200));
		
		foreach(BeatMarker marker in beatMarkers) {
			marker.DrawGUI(hotMode);
		}
		GUI.EndGroup();
	}
	
	/******************************************************************************
	 *
	 * OVERLOADED METHODS, FROM MonoBehavior
	 *
	 *****************************************************************************/	
	
	public void Awake(){
		instance = this;
		if(!gSkin) {
			Debug.LogError("DanceGUI.Awake(): gSkin not set!");
		}
		enabled=false;	
	}
	

}

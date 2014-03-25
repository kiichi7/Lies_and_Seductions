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

public class PokerGUI : MonoBehaviour {

	public GUIStyle backgroundStyle;
	public GUIStyle separatorStyle;
	public GUIStyle stackBackgroundStyle;
	public GUIStyle balloonStyle;
	public Vector2 balloonTopLeftPosition;
	public Vector2 balloonSize;
	public GUISkin skin;
	
	public Vector2 bettingTopLeftCorner;
	public Vector2 showDownTopLeftCorner;
	public Vector2[] ownCardTopLeftCorners = new Vector2[2];
	public Vector2 ownCardSize;
	public Vector2 potPosition;
	public Vector2 potGUISize;
	public Vector2 ownStackPosition;
	public Vector2 ownStackGUISize;
	public Vector2 opponentStackPosition;
	
	private static PokerGUI instance;
	
	public void Awake(){
		instance = this;
		enabled=false;
	}
	
	public static void SetSkin(){
		GUI.skin = instance.skin;
	}
	
	public static void DrawBalloon(string text){
		//Debug.Log("PokerGUI:DrawBalloon - " + text);
		GUILayout.BeginArea (new Rect (instance.balloonTopLeftPosition.x, instance.balloonTopLeftPosition.y, instance.balloonSize.x, instance.balloonSize.y));
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.Box(text, instance.balloonStyle);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	public static void DrawBettingGUI(int potSize, int callCost, int raise, PCPokerAction action){
		instance.InstanceDrawBettingGUI(potSize, callCost, raise, action);
	}
	
	private void InstanceDrawBettingGUI(int potSize, int callCost, int raise, PCPokerAction action){
		GUILayout.BeginArea(new Rect(bettingTopLeftCorner.x, bettingTopLeftCorner.y, 1000, 1000));
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		
		GUILayout.BeginVertical(backgroundStyle);
		GUILayout.Label("THE POT: $" + potSize);
		GUILayout.Label("", separatorStyle, GUILayout.Height(2.0f));
		GUILayout.Label("Call: " + callCost);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Raise: $" + raise);
		GUILayout.BeginVertical();
		if (GUILayout.Button("", "arrow_up")){
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
			action.PlusPressed();
		}
		if (GUILayout.Button("", "arrow_down")){
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
			action.MinusPressed();
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Label("Total: $" + (callCost + raise));
		GUILayout.Label("", separatorStyle, GUILayout.Height(2.0f));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Fold")){
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
			action.FoldPressed();
		}
		if (GUILayout.Button("Bet")){
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
			action.BetPressed();
		}
		
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void DrawShowDownHand(string playerName, EvaluatedHand hand){
		if (hand == null){
			Debug.LogError("PokerGUI.DrawShowDownHand(" + playerName + "null): ERROR: hand cannot  be null" );
			return;
		}
		GUILayout.BeginVertical();
		GUILayout.Label(playerName + ": " + hand.GetName());
		GUILayout.BeginHorizontal();
		foreach (Card card in hand.GetCardsInOrder()){
			//GUIStyle cardStyle = new GUIStyle();
			GUILayout.Label(card.GetTexture(), "showdown_card");
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	public static void DrawShowDownGUI(int potSize, string winnerName, string loserName, EvaluatedHand winningHand, EvaluatedHand losingHand, bool isDraw, bool mustQuit, ShowDownStage stage){
		instance.InstanceDrawShowDownGUI(potSize, winnerName, loserName, winningHand, losingHand, isDraw, mustQuit, stage);
	}
	
	private void InstanceDrawShowDownGUI(int potSize, string winnerName, string loserName, EvaluatedHand winningHand, EvaluatedHand losingHand, bool isDraw, bool mustQuit, ShowDownStage stage){
		GUILayout.BeginArea(new Rect(showDownTopLeftCorner.x, showDownTopLeftCorner.y, 1000, 1000));
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		
		GUILayout.BeginVertical(backgroundStyle);
		if (!isDraw){
			GUILayout.Label(winnerName + " wins $" + potSize);
		} else {
			GUILayout.Label("It's a draw!");
		}
		DrawShowDownHand(winnerName, winningHand);
		DrawShowDownHand(loserName, losingHand);
		GUILayout.BeginHorizontal();
		if (!mustQuit){
			if (GUILayout.Button("Continue")){
				FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
				stage.ContinuePressed();
			}
		}
		if (GUILayout.Button("Quit")){
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
			stage.QuitPressed();
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	private void DrawStackSize(Vector2 position, Vector2 size, string text){
		GUILayout.BeginArea(new Rect(position.x, position.y, size.x, size.y));
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.BeginVertical(stackBackgroundStyle);
		GUILayout.Label(text);
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	public static void DrawStackSizes(int potSize, int ownStack, int opponentStack, bool withMoney){
		instance.DrawStackSize(instance.potPosition, instance.potGUISize, "$" + potSize);
		string ownStackText;
		if (withMoney){
			ownStackText = "Money available: $" + ownStack;
		} else {
			ownStackText = "Playing just for fun";
		}
		instance.DrawStackSize(instance.ownStackPosition, instance.ownStackGUISize, ownStackText);
		//instance.DrawStackSize(opponentStack, instance.opponentStackPosition);
	}
	
	private void DrawCard(int index, Card card){
		GUI.Label(new Rect(ownCardTopLeftCorners[index].x, ownCardTopLeftCorners[index].y, ownCardSize.x, ownCardSize.y), card.GetTexture());
	}
	
	public static void DrawOwnHand(ArrayList ownHand){
		for (int i = 0; i < ownHand.Count; i++){
			instance.DrawCard(i, (Card)ownHand[i]);
		}
	}
}

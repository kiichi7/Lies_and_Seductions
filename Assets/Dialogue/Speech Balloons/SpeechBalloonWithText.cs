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
using System;

public class SpeechBalloonWithText : AbstractSpeechBalloon {
	// Inherited from MonoBehaviour through AbstractSpeechBalloon

	public Texture2D arrowTexturePointingLeft;
	public Texture2D arrowTexturePointingRight;
	private const int lineThickness = 8;
	private const int balloonOverArrowTip = 16;
	//public int balloonWidth = 400;
	private const int textLineLengthGUI = 25;
	private const int textLineLengthBillboard = 25;
	private Vector2 bigCharacterSize = new Vector2(14, 19);
	private Vector2 smallCharacterSize = new Vector2(7, 10);

	private const int arrowWidth = 37;
	private const int arrowHeight = 20;

	private string text = "";
	private int lines=0;
	private int longestLine=0;
	
	private bool npcNpcDialogue=false;
	
	public void OnGUI(){
		// We draw PC-NPC dialogues speech balloons here.
		// NPC-NPC dialogue is handled using TextMeshes and 
		// Billboard Game Object
		// Say() and RemoveBalloon() methods are used to post and remove balloons
		// in all cases.
		if (!Pause.IsGUIRemoved()){
			if (!text.Equals("") && npcNpcDialogue==false){
				GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
				if (balloonSkin){
					GUI.skin = balloonSkin;
				}
				DrawBalloon(text);
			}
		}
	}
	
	private void WordWrap(string sourceText, int textLineLength){
		string wrappedText = "";
		bool done = false;
		lines = 1;
		longestLine=0;
		while(!done){
			string line;
			if (sourceText.Length <= textLineLength){
				line = sourceText;
				if(line.Length > longestLine) {
					longestLine = line.Length;
				}
				done = true;
			} else {
				int numberOfCharactersToSearch =  Math.Min(textLineLength, sourceText.Length);
				int lastIndexOfSpace = sourceText.LastIndexOf(" ", numberOfCharactersToSearch - 1, numberOfCharactersToSearch);
				int splitIndex;
				if (lastIndexOfSpace != -1){
					splitIndex = lastIndexOfSpace;
				} else {
					splitIndex = textLineLength;
				}
				line = sourceText.Substring(0, splitIndex) + "\n";
				if(line.Length > longestLine) {
					longestLine = line.Length;
				}
				lines++;
				sourceText = sourceText.Substring(splitIndex);
				if (sourceText.StartsWith(" ")){
					sourceText = sourceText.Substring(1);
				}
			}
			wrappedText = wrappedText + line;
		}
		text = wrappedText;
	}

	/*private float GetDistance(){
		return Vector3.Distance(transform.position, CharacterManager.GetPC().transform.position);
	}*/
	
	/*private string ModifyTextByDistance(string text, float distance, Area area){
		if (distance > area.GetNoSpeechTextDistance()){
			return "...";
		} else {
			return text;
		}
	}*/
	
	private string GetStyleName(float distance, Area area){
		if (distance < area.GetSmallSpeechTextDistance()){
			return "box";
		} else if (distance < area.GetNoSpeechTextDistance()){
			return "small_font";
		} else {
			return "three_dots";
		}		
	}

	private Vector2 GetTextPartSize(string wrappedText, string styleName){
		GUIStyle style = balloonSkin.GetStyle(styleName);
		RectOffset padding = style.padding;
		RectOffset border = style.border;
		int numberOfLines = 0;
		int longestLineLength = 0;
		int lineStartIndex = 0;
		int lineEndIndex = 0;
		bool done = false;
		while (!done) {
			lineStartIndex = lineEndIndex;
			lineEndIndex = wrappedText.IndexOf("\n", lineStartIndex + 1);
			if (lineEndIndex == -1){
				lineEndIndex = wrappedText.Length - 1;
				done = true;
			}
			numberOfLines++;
			int lineLength = lineEndIndex - lineStartIndex;
			if (lineLength > longestLineLength){
				longestLineLength = lineLength;
			}
		}
		Vector2 characterSize;
		if (styleName.Equals("small_font")){
			characterSize = smallCharacterSize;
		} else {
			characterSize = bigCharacterSize;
		}		
		int balloonWidth = padding.left /*+ border.left*/ + (int)((longestLineLength + 1) * characterSize.x) /*+ border.right*/ + padding.right;
		int balloonHeight = padding.top /*+ border.left*/ + (int)(numberOfLines * characterSize.y) /*+ border.right*/ + padding.bottom;
		
		return new Vector2(balloonWidth, balloonHeight);
	}
	
	private Vector2 GetTopLeftPosition(Vector3 arrowPosition, ArrowDirection arrowDirection, Vector2 textPartSize){
		//Calculate the position of the top left corner of the balloon in relation to the arrow position.
		float topLeftX;
		if (arrowDirection == ArrowDirection.Left) {
			topLeftX = - textPartSize.x + balloonOverArrowTip + arrowWidth;
		} else {
			topLeftX = - balloonOverArrowTip - arrowWidth;
		}
		float topLeftY = - textPartSize.y - arrowHeight + lineThickness;
		//Calculate the absolute position of the top left corner of the balloon.
		Vector3 topLeftPosition = arrowPosition + new Vector3(topLeftX, topLeftY, 0);
		if (topLeftPosition.y < 0){
			topLeftPosition = new Vector3(topLeftPosition.x, 0, 0);
		}
		return new Vector2(topLeftPosition.x, topLeftPosition.y);		
	}
	

	private void DrawBalloon(string text){
		//float distance = GetDistance();
		//CharacterState pcState = (CharacterState)CharacterManager.GetPC().GetComponent("CharacterState");
		//Area area = pcState.GetCurrentArea();
		//string textSeen = ModifyTextByDistance(text, distance, area);
		string styleName = "box"; //GetStyleName(distance, area);
		Vector2 textPartSize = GetTextPartSize(text, styleName);
		
		ArrowDirection arrowDirection = GetArrowDirection();
		Vector3 arrowPosition = GetArrowPosition(arrowDirection, (int)textPartSize.y + arrowHeight);
		Vector2 topLeftPosition = GetTopLeftPosition(arrowPosition, arrowDirection, textPartSize);
		
		if (PCCanSeeBalloon(arrowPosition)){
			GUI.depth = (int)arrowPosition.z;
			DrawTextPart(text, styleName, arrowDirection, topLeftPosition, textPartSize);
			DrawArrow(arrowPosition, arrowDirection);
		}
	}

	private void DrawTextPart(string textSeen, string styleName, ArrowDirection arrowDirection, Vector2 topLeftPosition, Vector2 textPartSize){
		//Draw the balloon.
		GUILayout.BeginArea (new Rect (topLeftPosition.x, topLeftPosition.y, textPartSize.x, textPartSize.y));
		GUILayout.BeginVertical();
		//GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		//if (arrowDirection == ArrowDirection.Right){
		//	GUILayout.FlexibleSpace();
		//}
		GUILayout.Box(textSeen, styleName);
		//if (arrowDirection == ArrowDirection.Left){
		//	GUILayout.FlexibleSpace();
		//}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
		
	
	private void DrawArrow(Vector3 arrowPosition, ArrowDirection arrowDirection){
		Texture2D arrowTexture;
		float topLeftX;
		if (arrowDirection == ArrowDirection.Left){
			arrowTexture = arrowTexturePointingLeft;
			topLeftX = arrowPosition.x;
		} else {
			arrowTexture = arrowTexturePointingRight;
			topLeftX = arrowPosition.x - arrowWidth;
		}
		
		float topLeftY = arrowPosition.y - arrowHeight;
		GUIStyle arrowStyle = new GUIStyle();
		arrowStyle.normal.background=arrowTexture;
		GUI.Label(new Rect(topLeftX, topLeftY, arrowWidth, arrowHeight), "", arrowStyle);
	}

	public void Say(string text, bool npcNpcDialogue){
		Debug.Log("SpeechBallonWithText.Say(" + text + "," + npcNpcDialogue + "), name=" + name );
		
		if(text.Equals("")) {
			Debug.LogError("SpeechBalloonWithText.Say() called with empty string");
			return; 
		}
		enabled = true;
		this.npcNpcDialogue = npcNpcDialogue;
		if(npcNpcDialogue) {
			
			// Wrapped word is in this.text after WordWrap() call
			// lines tells how many lines there in this.text
			// longestLine tells how many characters the longest line have
			WordWrap(text, textLineLengthBillboard);
			// Now we turn speech balloon visible
			bubbleBackgroundRenderer.enabled = true;
			// and add text to it
			textMeshComponent.text = this.text;
			// and place the text in correct position 
			//Debug.Log("-- longestLine: " + longestLine);
			//Debug.Log("-- lines: " + lines);
			float yOffset = 0.05f;
			if(lines>1) {
				yOffset = 0.025f*(float)lines;
			}
			textObject.transform.localPosition = new Vector3(0.008f*(float)longestLine, yOffset, 0.005f);
			
		} else {
			if(bubbleBackgroundRenderer) {
				// Making sure that there is no billboard balloon and GUI balloon displaying
				// at the same time...
				bubbleBackgroundRenderer.enabled = false;
				textMeshComponent.text = "";
			}
			// Wrapped word is in this.text after WordWrap() call
			WordWrap(text, textLineLengthGUI);
			// The balloon is drawn in OnGUI() call
		}

	}
	
	public override void RemoveBalloon(){
		enabled = false;
		text = "";
		if(bubbleBackgroundRenderer) {
			bubbleBackgroundRenderer.enabled = false;
			textMeshComponent.text = "";
		}
	}
	
	public override Font GetFont() {
		return balloonSkin.font;	
	}
	
	public override Vector3 GetFontScale() {
		// GetBubbleScale() and GetFontScale() have dependency, as bubble's transform is parent of text's transform.
		// So, if you change one, make sure to change another 
		return new Vector3(0.0125f, 0.025f, 1.0f);
	}
	
	public override Vector3 GetBubbleScale() {
		// GetBubbleScale() and GetFontScale() have dependency, as bubble's transform is parent of text's transform.
		// So, if you change one, make sure to change anothe
		return new Vector3(3.0f, 1.5f, 1.0f);	
	}
	
	public override void RemoveIfNeeded() {
		if(enabled == false) {
			RemoveBalloon();
		}
	}
}

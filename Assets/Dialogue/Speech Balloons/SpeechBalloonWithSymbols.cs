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

public class SpeechBalloonWithSymbols : AbstractSpeechBalloon {

	public Texture2D balloonTexturePointingLeft;
	public Texture2D balloonTexturePointingRight;
	public Texture2D[] symbolTextures;
	public int arrowOffsetFromCornerX;
	public int arrowOffsetFromCornerY;
	
	//private Texture2D[] symbolsShown;
	private string symbolsShown;

	public const int balloonWidth = 108;
	public const int balloonHeight = 96;
	
	public void SayRandomLine(){
		//Debug.Log("SpeechBallonWithSymbols.SayRandomLine(): " + name);
		
		enabled=true;
		bubbleBackgroundRenderer.enabled = true;
		int numberOfSymbols = Random.Range(1, 6);
		symbolsShown = "";
		int n=0;
		int lines = 1;
		for (int i = 0; i < numberOfSymbols; i++){
			if(n>2) {
				symbolsShown +=  "\n";
				n=0;
				lines++;
			}
			else {
				n++;
			}
			char ch = ((char) ( (short) 'a' + (short)Random.Range(0,26)));
			symbolsShown += ch;
		}
		textMeshComponent.text = symbolsShown;
		switch(lines) {
			case 1:
				textObject.transform.localPosition = new Vector3(0.06f*numberOfSymbols, 0.1f, 0.001f);
				break;
			case 2:
				textObject.transform.localPosition = new Vector3(0.18f, 0.15f, 0.001f);
				break;
			default:
				Debug.LogError("SpeechBalloonWithSymbols.SayRandomLine(): line count out of bounds: lines=" + lines);
				break;
		}		
		//Debug.Log("SayRandomLine(): line set to \"" + symbolsShown + "\"========================================================");
	}
	
	public override void RemoveBalloon(){
		//Debug.Log("SpeechBallonWithSymbols.RemoveBalloon()");
		enabled = false;
		symbolsShown = "";
		textMeshComponent.text = "";
		bubbleBackgroundRenderer.enabled = false;
	}
	
	public override Font GetFont() {
		return balloonSkin.GetStyle("symbols").font;	
	}
	
	public override Vector3 GetFontScale() {
		return new Vector3(0.15f, 0.15f, 1.0f);
	}
	
	public override Vector3 GetBubbleScale() {
		return new Vector3(1.4f, 1.4f, 1.0f);	
	}
	
	public override void RemoveIfNeeded() {
		if(enabled == false) {
			RemoveBalloon();
		}	
	}
}

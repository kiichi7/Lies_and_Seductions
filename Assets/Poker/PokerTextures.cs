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

public class PokerTextures : MonoBehaviour {

	public Texture2D[] hearts = new Texture2D[13];
	public Texture2D[] diamonds = new Texture2D[13];
	public Texture2D[] clubs = new Texture2D[13];
	public Texture2D[] spades = new Texture2D[13];
	
	public Texture2D cardBack;
	
	public Texture2D stack;
	
	private static PokerTextures instance;
	
	public void Awake(){
		instance = this;
		enabled = false;
	}	
	
	public static Texture2D GetTexture(Suit suit, int rank){
		switch (suit){
		case Suit.HEARTS:
			return instance.hearts[rank - 2];
		case Suit.DIAMONDS:
			return instance.diamonds[rank - 2];
		case Suit.CLUBS:
			return instance.clubs[rank - 2];
		case Suit.SPADES:
			return instance.spades[rank - 2];
		default:
			return null;
		}
	}
	
	public static Texture2D GetBackTexture(){
		return instance.cardBack;
	}
}

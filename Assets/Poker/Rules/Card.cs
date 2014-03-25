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

public class Card {
	
	private Suit suit;
	private int rank;
	private Texture2D texture;
	
	public Card(Suit suit, int rank){
		if (rank < 2 || rank > 14){
			Debug.LogError("Card rank must be between 2 and 14");
		}
		this.suit = suit;
		this.rank = rank;
		texture = PokerTextures.GetTexture(suit, rank);
	}
	
	public Suit GetSuit(){
		return suit;
	}
	
	public int GetRank(){
		return rank;
	}
	
	public Texture2D GetTexture(){
		return texture;
	}
	
	public int CompareTo(Card otherCard){
		if (rank < otherCard.GetRank()){
			return -1;
		} else if (rank == otherCard.GetRank()){
			return 0;
		} else {
			return 1;
		}
	}
}
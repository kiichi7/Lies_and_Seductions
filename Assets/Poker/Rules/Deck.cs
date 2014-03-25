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

public class Deck {

	protected ArrayList allCards;
	protected ArrayList cards;
	
	public Deck(){
	}
	
	public void Init(){
		allCards = new ArrayList();
		GenerateCards();
		cards = new ArrayList(allCards);
		Shuffle();
	}
	
	private void GenerateCards(){
		foreach (Suit suit in Enum.GetValues(typeof(Suit))){
			for (int rank = 2; rank <= 14; rank++){
				allCards.Add(new Card(suit, rank));
			}
		}
	}
	
	public void Shuffle(){
		ArrayList unshuffledCards = new ArrayList(cards);
		ArrayList shuffledCards = new ArrayList();
		while (unshuffledCards.Count > 0){
			Card card = (Card)unshuffledCards[(int)UnityEngine.Random.Range(0, unshuffledCards.Count)];
			unshuffledCards.Remove(card);
			shuffledCards.Add(card);
		}
		cards = shuffledCards;
	}
	
	public Card Draw(){
		if (cards.Count == 0){
			Debug.Log("The Deck is empty!");
		}
		Card card = (Card)cards[0];
		cards.Remove(card);
		return card;
	}
	
	public SimulatedDeck GetSimulatedDeck(){
		return new SimulatedDeck(allCards);
	}
}

using UnityEngine;
using System.Collections;

public class SimulatedDeck : Deck {

	public SimulatedDeck(ArrayList cards) {
		this.cards = new ArrayList(cards);
	}
	
	public void RemoveCard(Card card){
		cards.Remove(card);
	}
	
}
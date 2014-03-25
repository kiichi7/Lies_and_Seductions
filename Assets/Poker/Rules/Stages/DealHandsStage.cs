using UnityEngine;
using System.Collections;

public class DealHandsStage : PokerStage {
	
	private Deck deck;
	private ArrayList players;
	
	public DealHandsStage(Deck deck, ArrayList players){
		this.deck = deck;
		this.players = players;
	}

	public bool UpdateStage(){
		foreach (PokerPlayer player in players){
			if (player.IsBusy()){
				return false;
			}
		} 
		foreach (PokerPlayer player in players){
			player.Draw(deck.Draw(), deck.Draw());
		}
		return true;
	}
	
	public void DrawGUI(){
	}
}

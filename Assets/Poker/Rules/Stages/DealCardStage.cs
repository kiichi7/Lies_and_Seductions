using UnityEngine;
using System.Collections;

public class DealCardStage : PokerStage {

	private Deck deck;
	private Board board;
	
	public DealCardStage(Deck deck, Board board){
		this.deck = deck;
		this.board = board;
	}

	public bool UpdateStage(){
		FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_POKER_DEALING);
		board.Draw(deck.Draw());
		return true;
	}
	
	public void DrawGUI(){
	}
}

using UnityEngine;
using System.Collections;

public class SetupStage : PokerStage {

	private Deck deck;
	private Pot pot;
	private Board board;
	private ArrayList players;

	public SetupStage(Deck deck, Pot pot, Board board, ArrayList players){
		this.deck = deck;
		this.pot = pot;
		this.board = board;
		this.players = players;
	}

	public bool UpdateStage(){
		deck.Init();
		pot.Clear(players);
		board.Clear();
		foreach (PokerPlayer player in players){
			player.ClearHand();
		}
		return true;
	}
	
	public void DrawGUI(){
	}
}

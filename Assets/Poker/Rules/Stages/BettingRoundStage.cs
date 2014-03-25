using UnityEngine;
using System.Collections;

public class BettingRoundStage : PokerStage {

	private Deck deck;
	private Board board;
	private Pot pot;
	private ArrayList players;
	
	private PokerPlayer currentPlayer;
	
	public BettingRoundStage(Deck deck, Board board, Pot pot, ArrayList players, PokerPlayer dealer){
		this.deck = deck;
		this.board = board;
		this.pot = pot;
		this.players = players;
		currentPlayer = dealer;
	}

	public bool UpdateStage(){
		if (pot.HasOnlyOnePlayerIn()){
			return true;
		} else if (currentPlayer.IsBusy()){
			return false;
		} else {
			return NextTurn();
		}
	}
	
	//Returns true if the betting round is over.
	private bool NextTurn(){
		int index = players.IndexOf(currentPlayer);
		index++;
		if (index >= players.Count){
			index = 0;
		}
		currentPlayer = (PokerPlayer)players[index];
		if (pot.IsFinished(currentPlayer)){
			return true;
		}
		if (pot.IsIn(currentPlayer)){
			currentPlayer.Bet(deck, board, pot, players);
		}
		return false;
	}
	
	public void DrawGUI(){
	}
}

using UnityEngine;
using System.Collections;

public class MinimumBetsStage : PokerStage {

	private const int MINIMUM_BET = 100;
	
	private Pot pot;
	private ArrayList players;
	private PokerPlayer dealer;
	private PokerPlayer currentPlayer;
	private bool firstDone;

	public MinimumBetsStage(Pot pot, ArrayList players, PokerPlayer dealer){
		this.pot = pot;
		this.players = players;
		this.dealer = dealer;
		currentPlayer = dealer;
		firstDone = false;
	}
	
	public bool UpdateStage(){
		if (currentPlayer.IsBusy()){
			return false;
		} else {
			return NextTurn();
		}
	}
	
	private bool NextTurn(){
		int index = players.IndexOf(currentPlayer);
		index++;
		if (index >= players.Count){
			index = 0;
		}
		currentPlayer = (PokerPlayer)players[index];
		if (!firstDone){
			currentPlayer.RaiseMinimumBet(pot, MINIMUM_BET);
			firstDone = true;
		} else {
			currentPlayer.CallMinimumBet(pot);
		}
		if (currentPlayer == dealer){
			return true;
		} else {
			return false;
		}
	}
	
	public void DrawGUI(){
	}
}

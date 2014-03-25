using UnityEngine;
using System.Collections;

public class ShowDownStage : PokerStage {
	
	private Board board;
	private Pot pot;
	private ArrayList players;
	private PCPokerAction pcPlayer;
	private bool buttonPressed;
	
	private string winnerName;
	private string loserName;
	private EvaluatedHand winningHand;
	private EvaluatedHand losingHand;
	private bool isDraw;
	private bool showDownDone;
	
	public ShowDownStage(Board board, Pot pot, ArrayList players, PCPokerAction pcPlayer){
		this.pot = pot;
		this.board = board;
		this.players = players;
		this.pcPlayer = pcPlayer;
		buttonPressed = false;
		winnerName = null;
		loserName = null;
		winningHand = null;
		losingHand = null;
		isDraw = false;
		showDownDone = false;
	}

	public bool UpdateStage(){
		if (!showDownDone){
			//showdown-ääni
			showDownDone = true;
			ArrayList participants = new ArrayList();
			foreach (PokerPlayer player in players){
				if (pot.IsIn(player)){
					participants.Add(player);
				}
			}
			ArrayList winners = new ArrayList();
			foreach (PokerPlayer participant in participants){
				if (winners.Count == 0){
					winners.Add(participant);
				} else if (participant.GetEvaluatedHand(board.GetCards()).CompareTo(
						((PokerPlayer)winners[0]).GetEvaluatedHand(board.GetCards())) > 0){
					winners.Clear();
					winners.Add(participant);
				} else if (participant.GetEvaluatedHand(board.GetCards()).CompareTo(
						((PokerPlayer)winners[0]).GetEvaluatedHand(board.GetCards())) == 0){
					winners.Add(participant);
				}
			}
			
			PokerPlayer winner = null;
			PokerPlayer loser = null;
			if (winners.Count > 1){
				isDraw = true;
				winner = (PokerPlayer)winners[0];
				loser = (PokerPlayer)winners[1];
			} else {
				isDraw = false;
				winner = (PokerPlayer)winners[0];
				foreach (PokerPlayer player in players){
					if (player != winner){
						loser = player;
					}
				}
			}
			winnerName = winner.GetName();
			loserName = loser.GetName();
			winningHand = winner.GetEvaluatedHand(board.GetCards());
			losingHand = loser.GetEvaluatedHand(board.GetCards());
			
			foreach (PokerPlayer sharer in winners){
				int share = pot.GetPotSize() / winners.Count;
				sharer.Win(share);
			}
			
			if (!isDraw){
				pcPlayer.SendPerception(winners.Contains(pcPlayer), pot.GetPotSize() - pot.GetHighestBet());
			}
		}
		return buttonPressed;
	}	
	
	public void DrawGUI(){
		if (showDownDone){
			bool tooLateForNewGame = /*GameTime.GetDateAndTime().Hour > 4 (GameTime.restaurantClosesHour??) || */((CharacterState)pcPlayer.GetActor().GetComponent("CharacterState")).GetSleepDept() > 0;
			
			//Debug.Log("tooLateForNewGame = " + tooLateForNewGame);
			//Debug.Log("Abby broke?" + (pcPlayer.GetStack() == 0));
			
			PokerGUI.DrawShowDownGUI(pot.GetPotSize() - pot.GetHighestBet(), winnerName, loserName, winningHand, losingHand, isDraw, pcPlayer.GetStack() == 0 || tooLateForNewGame, this);
			if (tooLateForNewGame){
				PokerGUI.DrawBalloon("Seems they're closing.");
			} else if (pcPlayer.GetStack() == 0){
				PokerGUI.DrawBalloon("Seems you're broke!");
			}
		}
	}
	
	public void ContinuePressed(){
		buttonPressed = true;
		PokerController.SetPlayAgain(true);
	}
	
	public void QuitPressed(){
		buttonPressed = true;
		PokerController.SetPlayAgain(false);
	}
}

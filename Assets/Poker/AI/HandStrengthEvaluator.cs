using UnityEngine;
using System.Collections;

public class HandStrengthEvaluator {
	
	private const int SIMULATION_ROUNDS = 100;
	
	private static double Simulate(ArrayList ownHand, ArrayList board, 
			ArrayList opponentHands, Deck deck){
		SimulatedDeck simulatedDeck = deck.GetSimulatedDeck();
		simulatedDeck.Shuffle();
		foreach (Card card in ownHand){
			simulatedDeck.RemoveCard(card);
		}
		foreach (Card card in board){
			simulatedDeck.RemoveCard(card);
		}
		foreach (ArrayList opponentHand in opponentHands){
			foreach (Card card in opponentHand){
				simulatedDeck.RemoveCard(card);
			}
		}
		ArrayList simulatedBoard = new ArrayList(board);
		while (simulatedBoard.Count < 5){
			simulatedBoard.Add(simulatedDeck.Draw());
		}
		ArrayList simulatedOpponentHands = new ArrayList();
		foreach (ArrayList opponentHand in opponentHands){
			ArrayList simulatedOpponentHand = new ArrayList(opponentHand);
			simulatedOpponentHands.Add(simulatedOpponentHand);
			while (simulatedOpponentHand.Count < 2){
				simulatedOpponentHand.Add(simulatedDeck.Draw());
			}
		}
		
		EvaluatedHand ownEvaluatedHand = HandEvaluator.EvaluateHand(ownHand,
				simulatedBoard);
		ArrayList opponentEvaluatedHands = new ArrayList();
		foreach (ArrayList simulatedOpponentHand in simulatedOpponentHands){
			opponentEvaluatedHands.Add(HandEvaluator.EvaluateHand(simulatedOpponentHand, simulatedBoard));
		}
		EvaluatedHand bestOpponentEvaluatedHand = null;
		foreach (EvaluatedHand opponentEvaluatedHand in opponentEvaluatedHands){
			if (bestOpponentEvaluatedHand == null || 
					opponentEvaluatedHand.CompareTo(bestOpponentEvaluatedHand) > 0){
				bestOpponentEvaluatedHand = opponentEvaluatedHand;
			}
		}
		int comparison = ownEvaluatedHand.CompareTo(bestOpponentEvaluatedHand);
		if (comparison > 0){
			return 1.0;
		} else if (comparison == 0){
			return 0.5;
		} else {
			return 0.0;
		}
	}
	
	public static double EvaluateHandStrength(ArrayList ownHand, ArrayList board,
			ArrayList opponentHands, Deck deck){
		double score = 0.0;
		for (int i = 0; i < SIMULATION_ROUNDS; i++){
			score += Simulate(ownHand, board, opponentHands, deck);
		}
		return score / SIMULATION_ROUNDS;
	}
}
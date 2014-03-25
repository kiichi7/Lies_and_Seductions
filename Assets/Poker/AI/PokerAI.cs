using UnityEngine;
using System.Collections;

public class PokerAI {
	
	public const double RAISE_STEP = 100.0f;

	private PokerPlayer player;
	private int peekingSkill;
	private int playingSkill;
	
	public PokerAI(PokerPlayer player, int peekingSkill, int playingSkill) {
		Debug.Log("PokerAI.PokerAI(peekingSkill = " + peekingSkill + ", playingSkill=" + playingSkill + ")");
		this.player = player;
		this.peekingSkill = peekingSkill;
		this.playingSkill = playingSkill;
	}
	
	private int D100(){
		return (int)Random.Range(1, 101);
	}
	
	private double CalculatePotOdds(Pot pot, double raise){
		double cost = pot.GetCallCost(player) + raise;
		double potSize = pot.GetPotSize();
		double potOdds = cost / (cost + potSize);
		return potOdds;
	}
	
	public ArrayList PeekHands(ArrayList players){
		ArrayList peekedHands = new ArrayList();
		foreach (PokerPlayer player in players){
			if (player != this.player){
				ArrayList peekedHand = player.RevealCards(peekingSkill);
				peekedHands.Add(peekedHand);
			}
		}
		return peekedHands;
	}
	
	private double GetRaise(double handStrength, Pot pot, double raiseSoFar){
		double higherRaise = raiseSoFar + RAISE_STEP;
		double potOddsForHigher = CalculatePotOdds(pot, higherRaise);
		double rateOfReturnForHigher = handStrength / potOddsForHigher;
		if (raiseSoFar > pot.GetHighestBet()){
			if (D100() < 40){
				return raiseSoFar;
			}
		}
		int d100 = D100();
		if (rateOfReturnForHigher < 1.0){
			if (d100 < 95){
				return raiseSoFar;
			} else {
				return GetRaise(handStrength, pot, higherRaise);
			}
		} else if (rateOfReturnForHigher < 1.3){
			if (d100 < 30){
				return raiseSoFar;
			} else {
				return GetRaise(handStrength, pot, higherRaise);
			}
		} else {
			if (d100 < 5){
				return raiseSoFar;
			} else {
				return GetRaise(handStrength, pot, higherRaise);
			}
		}
	}
	
	private void CallOrRaise(double handStrength, Pot pot){
		double amountToRaise = GetRaise(handStrength, pot, 0.0);
		if (amountToRaise == 0.0){
			player.Call(pot);
		} else {
			player.Raise(pot, (int)amountToRaise, false);
		}
	}
	
	private double GetBluff(Pot pot){
		double multiplier = (double)Random.Range(1.5f, 2.5f);
		return ((int)(multiplier * pot.GetHighestBet() / RAISE_STEP)) * RAISE_STEP;
	}
	
	private void Bluff(Pot pot){
		player.Raise(pot, (int)GetBluff(pot), false);
	}
	
	private void FoldOrBluff(double handStrength, Pot pot){		
		double rateOfReturnForCall = handStrength / CalculatePotOdds(pot, 0);
		int d100 = D100();
		//Debug.Log("PokerAI.FoldOrBluff() rateOfReturnForCall=" + rateOfReturnForCall + " D100=" + d100 + "----------------------------");
		// Trying to make Ed more aggressive bluffer when drunk...
		int defaulfChange=85;
		if (rateOfReturnForCall < 0.8){
			defaulfChange = 95;
		}
		if (d100 >= Mathf.Min(defaulfChange, playingSkill)){
			//Debug.Log("-- Bluff " + defaulfChange + ", playingSkill=" + playingSkill + " using: " + Mathf.Min(defaulfChange, playingSkill));
			Bluff(pot);
		} else {
			//Debug.Log("-- Fold " + defaulfChange);
			player.Fold(pot);
		}
		
		
		/*if (rateOfReturnForCall < 0.8){
			if (d100 < 95){
				player.Fold(pot);
			} else {
				Bluff(pot);
			}
		} else {
			if (d100 < 85){
				player.Fold(pot);
			} else {
				Bluff(pot);
			}
		}*/
	}
	
	private void CallOrNotCall(double handStrength, Pot pot){
		double potOddsForCall = CalculatePotOdds(pot, 0);
		double rateOfReturnForCall = handStrength / potOddsForCall;
		int d100 = D100();
		if (rateOfReturnForCall < 0.8){
			FoldOrBluff(handStrength, pot);
		} else if (rateOfReturnForCall < 1.0){
			// PL: More agressinve player with not so good hands?
			if (d100 >= Mathf.Min(95, playingSkill)){
				FoldOrBluff(handStrength, pot);
			} else if (d100 >= Mathf.Min(85,playingSkill)){
				CallOrRaise(handStrength, pot);
			}
			else {
				Debug.Log("PokerAI.CallOrNotCall(), not calling CallOrRaise() or FoldOrBluff(). Is this OK?");	
			}
			/*
			if (d100 <95){
				FoldOrBluff(handStrength, pot);
			} else if (d100 < 85){
				CallOrRaise(handStrength, pot);
			}
			
			*/
			
		} else if (rateOfReturnForCall < 1.3){
			CallOrRaise(handStrength, pot);
		} else {
			CallOrRaise(handStrength, pot);
		}
	}
	
	public void Bet(double handStrength, Pot pot){
		CallOrNotCall(handStrength, pot);
		//TextInterface.ask("");
	}

	
}

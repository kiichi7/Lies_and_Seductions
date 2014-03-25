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

public abstract class AbstractPokerAction : AbstractNestingAction, PokerPlayer {

	protected ArrayList hand;
	protected int stack;

	public AbstractPokerAction(GameObject actor, string targetName, bool withMoney) : base(actor){
		InitInteractionInfo(true, false);
		InitMovementInfo(GameObject.Find(targetName), DistanceConstants.SIT_DOWN_DISTANCE, true, false, false);
		hand = new ArrayList();
		if (withMoney){
			stack = state.GetMoney();
		} else {
			stack = 1000000000;
		}
	}
	
	public string GetName(){
		return actor.name;
	}
	
	public int GetStack(){
		return stack;
	}
	
	public virtual void Draw(Card card1, Card card2){
		hand.Add(card1);
		hand.Add(card2);
	}
	
	public virtual void ClearHand(){
		Debug.Log("AbstractPokerAction.ClearHand: " + actor.name);
		hand.Clear();
	}

	public abstract void Bet(Deck deck, Board board, Pot pot, ArrayList players);

	public void RaiseMinimumBet(Pot pot, int minimumBet){
		//if (stack < minimumBet){
		//	Fold(pot);
		//} else {
		Raise(pot, minimumBet, true);
		//}		
	}
	
	public void CallMinimumBet(Pot pot){
		//if (stack < pot.GetCallCost(this)){
		//	Fold(pot);
		//} else {
		Call(pot);
		//}	
	}
	
	public abstract void Fold(Pot pot);
	
	public abstract void Call(Pot pot);
	
	public abstract void Raise(Pot pot, int amount, bool isMinimumBet);
	
	public abstract bool IsBusy();
	
	public void EndPokerGame(bool updateMoney){
		if (updateMoney){
			state.SetMoney(stack);
		}
		End();
	}

	public EvaluatedHand GetEvaluatedHand(ArrayList board){
		return HandEvaluator.EvaluateHand(hand, board);
	}
	
	public ArrayList RevealCards(int peekingSkill){
		Debug.Log("AbsttractPokerAction.RevealCards(): Peeking skill=" + peekingSkill + ", hand size=" + hand.Count);
		ArrayList revealedCards = new ArrayList();
		for (int i = 0; i < peekingSkill; i++){
			revealedCards.Add(hand[i]);
		}
		return revealedCards;
	}

	public void Win(int potSize){
		stack += potSize;
	}
}

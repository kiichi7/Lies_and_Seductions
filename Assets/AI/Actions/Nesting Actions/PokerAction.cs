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

public class PokerAction : AbstractPokerAction {

	private const int DRUNKNESS_LIMIT = 10;
	private const double MIN_GOOD_HAND_STRENGTH = 0.7;
	private const double MIN_AVERAGE_HAND_STRENGTH = 0.3;

	//private VictoryAction victoryAction;
	//private DefeatAction defeatAction;
	
	private PokerAI pokerAI;
	
	private Transform[] cardHooks;
	private CardItem[] cardItems;
	
	private bool busy;
	private double currentHandStrength;
	
	public PokerAction(GameObject actor, string chairName, bool withMoney) : base(actor, chairName, withMoney){
		//Debug.Log("Drunkness: " + state.GetDrunkness());
		pokerAI = new PokerAI(this, Mathf.Max(0, 1 - (int)state.GetDrunkness() / DRUNKNESS_LIMIT), Mathf.Max(50, 100-4*(int)state.GetDrunkness()));
		cardHooks = new Transform[2];
		cardHooks[0] = CharacterAnimator.FindChild(actor, "card_hook_1");
		cardHooks[1] = CharacterAnimator.FindChild(actor, "card_hook_2");
		cardItems = new CardItem[cardHooks.Length];
		busy = true;
		currentHandStrength = 0.0;
	}
	
	protected override Action CreateDefaultAction(){
		busy = false;
		if (state.GetDrunkness() < DRUNKNESS_LIMIT){
			return new PokerIdleAction(actor, "idle_poker");
		} else if (currentHandStrength > MIN_GOOD_HAND_STRENGTH){
			return new PokerIdleAction(actor, "good_cards");
		} else if (currentHandStrength > MIN_AVERAGE_HAND_STRENGTH){
			return new PokerIdleAction(actor, "average_cards");
		} else {
			return new PokerIdleAction(actor, "bad_cards");
		}
	}
	
	private void CreateCardItem(int index, Card card){
		//Debug.Log("Creating cardItem");
		cardItems[index] = (CardItem)ItemFactory.CreateItem("card").GetComponent(typeof(CardItem));
		cardItems[index].Setup(cardHooks[index], card, false);
	}
	
	public override void Draw(Card card1, Card card2){
		base.Draw(card1, card2);
		CreateCardItem(0, card1);
		CreateCardItem(1, card2);
	}
	
	public override void ClearHand(){
		base.ClearHand();
		for (int i = 0; i < cardItems.Length; i++){
			if (cardItems[i] != null){
				GameObject.Destroy(cardItems[i].gameObject);
			}
			cardItems[i] = null;
		}
	}
	
	public override void Bet(Deck deck, Board board, Pot pot, ArrayList players){
		ArrayList peekedHands = pokerAI.PeekHands(players);
		currentHandStrength = HandStrengthEvaluator.EvaluateHandStrength(
				hand, board.GetCards(), peekedHands, deck);
		pokerAI.Bet(currentHandStrength, pot);
	}

	public override void Fold(Pot pot){
		if (pot.GetCallCost(this) > 0){
			QueueAction(new NoBetAction(actor, "I fold."), true);
			pot.Fold(this);
			busy = true;
		} else {
			Call(pot);
		}
	}
	
	public override void Call(Pot pot){
		int cost = pot.Call(this);
		if (cost > 0){
			QueueAction(new BetAction(actor, "I call.", pot), true);
		} else {
			QueueAction(new NoBetAction(actor, "I call."), true);
		}
		stack -= cost;
		busy = true;
	}
	
	public override void Raise(Pot pot, int amount, bool isMinimumBet){
		string text;
		amount = (int)Mathf.Min(amount, pot.GetMaximumFairRaise(this));
		if (amount == 0){
			Call(pot);
		} else {
			if (isMinimumBet){
				text = "Starting bets " + amount + ", okay?";
			} else {
				text = "I raise by " + amount + "!";
			}
			int cost = pot.Raise(this, amount);
			QueueAction(new BetAction(actor, text, pot), true);
			stack -= cost;
			busy = true;
		}
	}
	
	public override bool IsBusy(){
		return busy;
	}
	
	private void DestroyCardItems(){
		foreach (CardItem cardItem in cardItems){
			GameObject.Destroy(cardItem.gameObject);
		}
	}
	
	protected override void UpdateLastRound(bool interrupted){
		DestroyCardItems();
		actionRunner.ResetRoutine(new StartDialogueAction(actor, CharacterManager.GetPC(), actor.name, "after_poker"), false);
	}
}

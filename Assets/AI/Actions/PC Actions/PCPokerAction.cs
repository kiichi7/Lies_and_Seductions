/**********************************************************************
 *
 * CLASS CharacterAnimator
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

public class PCPokerAction : AbstractPokerAction, SelectableAction {

	private const float IMPRESSION_PUSH_PER_DOLLAR_WON = 0.005f;

	private GameObject opponent;
	private bool withMoney;
	private bool betting;
	private Pot pot;
	private int raise;
	
	public PCPokerAction(GameObject actor, GameObject opponent, string waypointName, bool withMoney) : base(actor, waypointName, withMoney){
		Debug.Log("PCPokerAction.PCPokerAction(" + actor.name + "," + opponent.name + "," + waypointName + "," + withMoney + ")");
		this.opponent = opponent;
		this.withMoney = withMoney;
		betting = false;
		pot = null;
		raise = 0;
		
	}
		
	public MenuIconSet GetIconSet(PopupMenuGUI popupMenuGUI){
		return popupMenuGUI.playPokerIcon;
	}
	
	protected override void UpdateFirstRound(){
		if(state.currentTask.task == CharacterState.Task.POKER) {
			state.TaskCompleted(); 
		}
		else {
			state.SetTask(CharacterState.Task.NONE, null);
		}
		TaskHelp.RemoveHelp();
		PokerController.StartPoker(this, opponent, withMoney);
		CameraController.SetPokerCameraAngle();
	}
	
	protected override void UpdateLastRound(bool interrupted){
		CameraController.SetDefaultCameraAngle();
		//actionRunner.ResetRoutine(new PCStandUpAction(actor, state.GetCurrentSeat()), false);
		actionRunner.ResetRoutine(new FollowAction(actor, opponent, Mathf.Infinity, 0.0f, FollowAction.Reason.NPC, true), false);
	}
	
	protected override Action CreateDefaultAction(){
		return new IdleAction(actor, 1.0f, null, 0.0f, false);
	}
	
	public override void Bet(Deck deck, Board board, Pot pot, ArrayList players){
		raise = 0;
		this.pot = pot;
		betting = true;
	}
	
	public override void Fold(Pot pot){
		betting = false;
		if (pot.GetCallCost(this) > 0){
			pot.Fold(this);
		} else {
			pot.Call(this);
		}	
	}
	
	public override void Call(Pot pot){
		betting = false;
		stack -= pot.Call(this);
		pot.UpdateChips();
	}
	
	public override void Raise(Pot pot, int amount, bool isMinimumBet){
		betting = false;
		stack -= pot.Raise(this, amount);
		pot.UpdateChips();
	}
	
	public override bool IsBusy(){
		return betting;
	}
	
	public void SendPerception(bool isWinner, int winnings){
		int impressionPushValue = Mathf.Max(1, (int)(winnings * IMPRESSION_PUSH_PER_DOLLAR_WON));
		
		if (!isWinner){
			impressionPushValue = - impressionPushValue;
		}
		
		ImpressionPush impressionPush = new ImpressionPush("GoodAtPoker", new NumericConstant(impressionPushValue), false, null);
		Perception perception = new ImpressionPerception(actor, impressionPush);
		PerceptionManager.BroadcastPerception(perception);
	}
	
	public override void OnActionGUI(){
		PokerGUI.SetSkin();
		if (started){
			PokerController.DrawGUI();
			PokerGUI.DrawOwnHand(hand);
			if (betting){
				PokerGUI.DrawBettingGUI(pot.GetPotSize(), pot.GetCallCost(this), raise, this);
			}
		}
	}
	

	public void PlusPressed(){
		if (raise + pot.GetCallCost(this) + PokerAI.RAISE_STEP <= stack){
			raise += (int)PokerAI.RAISE_STEP;
		}
	}
	
	public void MinusPressed(){
		if (raise > 0){
			raise -= (int)PokerAI.RAISE_STEP;
		}
	}
	
	public void FoldPressed(){
		Fold(pot);
	}
	
	public void BetPressed(){
		if (raise == 0){
			Call(pot);
		} else {
			Raise(pot, raise, false);
		}
	}
}

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

public class PokerController : MonoBehaviour {
	
	private bool withMoney;
	public PokerTable table;
	
	private ArrayList players;
	private PCPokerAction pcAction;
	private PokerAction npcAction;
	private Deck deck;
	private Pot pot;
	private Board board;
	
	private bool running;
	private bool playAgain;
	
	private ArrayList stages;
	
	private static PokerController instance;
	
	public void Awake(){
		instance = this;
		enabled=false;
	}
	
	public static PokerTable GetTable(){
		return instance.table;
	}
	
	public static void StartPoker(PCPokerAction pcAction, GameObject npc, bool withMoney){
		instance.InstanceStartPoker(pcAction, npc, withMoney);
		TaskHelp.Hide();
		MouseHandler.ReleaseCursorXPosition();
	}
	
	private void InstanceStartPoker(PCPokerAction pcAction, GameObject npc, bool withMoney){
		this.withMoney = withMoney;
		players = new ArrayList();
		this.pcAction = pcAction;
		players.Add(pcAction);
		npcAction = new PokerAction(npc, "Poker Chair: Ed", withMoney);
		ActionRunner npcActionRunner = (ActionRunner)npc.GetComponent("ActionRunner");
		npcActionRunner.ResetRoutine(npcAction, false);
		players.Add(npcAction);
		deck = new Deck();
		pot = new Pot(players, table.GetPotHook());
		board = new Board(table.GetBoardHooks());
		InitStages();
		running = true;
		playAgain = false;
		enabled=true;
	}
	
	private void InitStages(){
		PokerPlayer dealer = (PokerPlayer)players[0];
		stages = new ArrayList();
		stages.Add(new SetupStage(deck, pot, board, players));
		stages.Add(new DealHandsStage(deck, players));
		stages.Add(new MinimumBetsStage(pot, players, dealer));
		stages.Add(new BettingRoundStage(deck, board, pot, players, dealer));
		stages.Add(new DealCardStage(deck, board));
		stages.Add(new DealCardStage(deck, board));
		stages.Add(new DealCardStage(deck, board));
		stages.Add(new BettingRoundStage(deck, board, pot, players, dealer));
		stages.Add(new DealCardStage(deck, board));
		stages.Add(new BettingRoundStage(deck, board, pot, players, dealer));
		stages.Add(new DealCardStage(deck, board));
		stages.Add(new BettingRoundStage(deck, board, pot, players, dealer));
		stages.Add(new ShowDownStage(board, pot, players, pcAction));
	}
	
	public void Quit(){
		running = false;
		enabled = false;
	}
	
	public void Update(){
		if (running){
			PokerStage currentStage = (PokerStage)stages[0];
			bool finished = currentStage.UpdateStage();
			if (finished){
				stages.RemoveAt(0);
				if (stages.Count == 0){
					EndPokerGame();
				}
			}
		}
	}
	
	private void EndPokerGame(){
		pot.Clear();
		board.Clear();
		if (playAgain){
			InitStages();
			/*foreach (PokerPlayer player in players){
				player.ClearHand();
			}*/
		} else {
			running = false;
			TaskHelp.Reveal();
			MouseHandler.LockCursorXPosition();
			foreach (PokerPlayer player in players){
				player.EndPokerGame(withMoney);
			}
		}
		//
	}
	
	public static void SetPlayAgain(bool playAgain){
		instance.playAgain = playAgain;
	}
	
	public static void DrawGUI(){
		PokerGUI.DrawStackSizes(instance.pot.GetDisplayedPotSize(), instance.pcAction.GetStack(), instance.npcAction.GetStack(), instance.withMoney);
		if (instance.stages.Count > 0){
			((PokerStage)instance.stages[0]).DrawGUI();
		}
	}
	
}
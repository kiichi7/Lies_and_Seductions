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

public class Pot {
	
	private const int SMALL_POT_MAX_SIZE = 300;
	private const int AVERAGE_POT_MAX_SIZE = 800;
	private const int LARGE_POT_MAX_SIZE = 2000;
	
	private Transform potHook;
	private GameObject potItem;
	
	private Hashtable bets;
	private ArrayList playersIn;
	private PokerPlayer previousRaiser;
	private int displayedPotSize;
	
	public Pot(ArrayList players, Transform potHook){
		this.potHook = potHook;
		Clear(players);
	}
	
	public bool IsIn(PokerPlayer player){
		return playersIn.Contains(player);
	}
	
	public int GetHighestBet(){
		int highestBet = 0;
		foreach (int bet in bets.Values){
			if (bet > highestBet){
				highestBet = bet;
			}
		}
		return highestBet;
	}
	
	public int GetCallCost(PokerPlayer player){
		return GetHighestBet() - (int)bets[player];
	}
	
	public int GetPotSize(){
		int size = 0;
		foreach (int bet in bets.Values){
			size += bet;
		}
		return size;
	}
	
	public int GetDisplayedPotSize(){
		return displayedPotSize;
	}
	
	public float GetMaximumFairRaise(PokerPlayer currentPlayer){
		float maximumFairRaise = Mathf.Infinity;
		foreach (PokerPlayer player in playersIn){
			if (player != currentPlayer){
				int stack = player.GetStack();
				int callCost = GetCallCost(player);
				maximumFairRaise = Mathf.Min(maximumFairRaise, (float)(stack - callCost));
			}
		}
		return maximumFairRaise;
	}
	
	public bool HasOnlyOnePlayerIn(){
		return playersIn.Count == 1;
	}
	
	public void Fold(PokerPlayer player){
		playersIn.Remove(player);
	}
	
	public int Call(PokerPlayer player){
		FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_POKER_DEALING);
		int chipsNeeded = GetCallCost(player);
		bets[player] = GetHighestBet();
		if (previousRaiser == null){
			previousRaiser = player;
		}
		return chipsNeeded;
	}
	
	public int Raise(PokerPlayer player, int raiseBy){
		FModManager.StartEvent(FModLies.EVENTID_LIES_ACTIONS_POKER_DEALING);
		int chipsNeeded = GetCallCost(player) + raiseBy;
		bets[player] = GetHighestBet() + raiseBy;
		previousRaiser = player;
		return chipsNeeded;
	}
	
	public void UpdateChips(){
		string oldPotItemName;
		if (potItem == null){
			oldPotItemName = null;
		} else {
			oldPotItemName = potItem.name;
		}
		string newPotItemName;
		int potSize = GetPotSize();
		if (potSize == 0){
			newPotItemName = null;
		} else if (potSize <= SMALL_POT_MAX_SIZE){
			newPotItemName = "small_pot";
		} else if (potSize <= AVERAGE_POT_MAX_SIZE){
			newPotItemName = "average_pot";
		} else if (potSize <= LARGE_POT_MAX_SIZE){
			newPotItemName = "large_pot";
		} else {
			newPotItemName = "huge_pot";
		}
		if (oldPotItemName != newPotItemName){
			if (potItem != null){
				GameObject.Destroy(potItem);
			}
			if (newPotItemName != null){
				potItem = ItemFactory.CreateItem(newPotItemName);
				potItem.transform.position = potHook.transform.position;
				potItem.transform.rotation = potHook.transform.rotation;
			} else {
				potItem = null;
			}
		}
		displayedPotSize = GetPotSize();
	}

	public bool IsFinished(PokerPlayer playerInTurn){
		if (previousRaiser == playerInTurn){
			previousRaiser = null;
			return true;
		} else {
			return false;
		}
	}
	
	public void Clear(ArrayList players){
		bets = new Hashtable();
		foreach (PokerPlayer player in players){
			bets[player] = 0;
		}
		playersIn = new ArrayList(players);
		previousRaiser = null;
		displayedPotSize = 0;
	}
	
	public void Clear(){
		if (potItem != null){
			GameObject.Destroy(potItem);
			potItem = null;
		}
	}
}

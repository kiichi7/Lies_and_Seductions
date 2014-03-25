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

public interface PokerPlayer {

	string GetName();

	int GetStack();
	
	void Draw(Card card1, Card card2);
	
	void ClearHand();

	void Bet(Deck deck, Board board, Pot pot, ArrayList players);
		
	void RaiseMinimumBet(Pot pot, int minimumBet);
		
	void CallMinimumBet(Pot pot);
		
	void Fold(Pot pot);
	
	void Call(Pot pot);
	
	void Raise(Pot pot, int amount, bool isMinimumBet);
	
	bool IsBusy();
	
	EvaluatedHand GetEvaluatedHand(ArrayList board);
	
	ArrayList RevealCards(int peekingSkill);
	
	void EndPokerGame(bool updateMoney);
	
	void Win(int potSize);

}

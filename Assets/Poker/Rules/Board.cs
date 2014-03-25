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

public class Board {

	private Transform[] boardHooks;
	private CardItem[] cardItems;
	
	private ArrayList cards;
	
	public Board(Transform[] boardHooks){
		cards = new ArrayList();
		this.boardHooks = boardHooks;
		cardItems = new CardItem[boardHooks.Length];
		//Debug.Log("CardItem count: " + cardItems.Length);
	}
	
	public ArrayList GetCards(){
		return cards;
	}
	
	public void Draw(Card card){
		cards.Add(card);
		int index = cards.Count - 1;
		cardItems[index] = (CardItem)ItemFactory.CreateItem("card").GetComponent(typeof(CardItem));
		cardItems[index].Setup(boardHooks[index], card, true);
	}
	
	public void Clear(){
		cards = new ArrayList();
		for (int i = 0; i < cardItems.Length; i++){
			if (cardItems[i] != null){
				GameObject.Destroy(cardItems[i].gameObject);
				cardItems[i] = null;
			}
		}
	}
}

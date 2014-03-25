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

public class SlowDrinkAction : AbstractNestingAction {

	private const int BEER_ALCOHOL_LEVEL = 1;
	private const int WHISKEY_ALCOHOL_LEVEL = 10;
	private const int DRINK_ALCOHOL_LEVEL = 5;

	private float startingTime;
	private float durationInSeconds;
	private string itemType;

	public SlowDrinkAction(GameObject actor, string itemType, string targetName, float durationInSeconds, bool sitDown) : base(actor){
		InitInteractionInfo(true, true);
		if (targetName != null){
			InitMovementInfo(GameObject.Find(targetName), DistanceConstants.SIT_DOWN_DISTANCE, sitDown, false, false);
		}

		this.itemType = itemType;
		startingTime = GameTime.GetRealTimeSecondsPassed();
		this.durationInSeconds = durationInSeconds;
	}
	
	protected override void UpdateFirstRound(){
		animator.StopAnimation("hold_" + itemType);
	}

	protected override void UpdateLastRound(bool interrupted){
		int alcoholLevel = 0;
		if (itemType.Equals("beer")){
			alcoholLevel = BEER_ALCOHOL_LEVEL;
		} else if (itemType.Equals("whiskey")){
			alcoholLevel = WHISKEY_ALCOHOL_LEVEL;
		} else if (itemType.Equals("drink")){
			alcoholLevel = DRINK_ALCOHOL_LEVEL;
		}
		state.Drink(alcoholLevel);
		state.SetItem(null);
	}
	
	protected override Action CreateDefaultAction(){
		if (GameTime.GetRealTimeSecondsPassed() - startingTime < durationInSeconds){
			return new DrinkAction(actor, itemType);
		} else {
			return null;
		}
	}
}

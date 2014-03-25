using UnityEngine;
using System.Collections;

public class RoutineCreatorChris : RoutineCreator {

	private const int MAXIMUM_DRUNKNESS = 10;

	protected override ArrayList GetPossibleRoutines(int hour, int minute){
		ArrayList possibleRoutines = new ArrayList();
		CharacterState state = GetComponent(typeof(CharacterState)) as CharacterState;
		if (state.IsOnBlindDate()){
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
		} else if (state.GetDrunkness() >= MAXIMUM_DRUNKNESS && hour < 22 && hour >= 8){
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
		} else if (hour >= 8 && hour < 9){
			//breakfast
			possibleRoutines.Add(new EatAction(gameObject, "Chair: Chris"));
		} else if (hour >= 9 && hour < 13){
			//deck/lobby
			//possibleRoutines.Add(new WanderAction(gameObject, 3, "Lobby Connecting Waypoints"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			//possibleRoutines.Add(new WanderAction(gameObject, 3, "Deck Connecting Waypoints"));
		} else if (hour >= 13 && hour < 14){
			//lunch
			possibleRoutines.Add(new EatAction(gameObject, "Chair: Chris"));
		} else if (hour >= 14 && hour < 17){
			//deck/lobby
			//possibleRoutines.Add(new WanderAction(gameObject, 3, "Lobby Connecting Waypoints"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			//possibleRoutines.Add(new WanderAction(gameObject, 3, "Deck Connecting Waypoints"));
		} else if (hour >= 17 && hour < 18){
			//dinner
			possibleRoutines.Add(new EatAction(gameObject, "Chair: Chris"));
		} else if (hour >= 18 && hour < 20){
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			possibleRoutines.Add(new BuyItemAction(gameObject, "beer", "Waypoint: Restaurant Shopkeeper"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Restaurant Window Waypoints"));	
		} 
		else if (hour >= 20 && hour < 22) {
			possibleRoutines.Add(new BuyItemAction(gameObject, "beer", "Waypoint: Disco Shopkeeper"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			
		} else {
			possibleRoutines.Add(new IdleAction(gameObject, 1.0f, GameObject.Find("Waypoint: Cabin: Chris"), DistanceConstants.WAYPOINT_RADIUS, false));
		}
		return possibleRoutines;
	}
	
	protected override Seat GetSeat(Area area){
		GameObject seatGO = null;
		if (area.name.Equals("Restaurant")){
			seatGO = GameObject.Find("Chair: Chris");
		} else if (area.name.Equals("Disco")){
			seatGO = GameObject.Find("Lounger: Chris");
		}
		if (seatGO != null){
			return (Seat)seatGO.GetComponent("Seat");
		} else {
			return null;
		}
	}

}

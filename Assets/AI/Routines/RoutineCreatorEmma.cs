using UnityEngine;
using System.Collections;

public class RoutineCreatorEmma : RoutineCreator {
	
	private const int MAXIMUM_DRUNKNESS = 10;
	
	protected override ArrayList GetPossibleRoutines(int hour, int minute){
		ArrayList possibleRoutines = new ArrayList();
		CharacterState state = GetComponent(typeof(CharacterState)) as CharacterState;
		if (state.GetDrunkness() >= MAXIMUM_DRUNKNESS || hour >= 0 && hour < 8){
			//sleep
			possibleRoutines.Add(new IdleAction(gameObject, 1.0f, GameObject.Find("Waypoint: Cabin: Emma"), DistanceConstants.WAYPOINT_RADIUS, false));
		} else if (hour >= 8 && hour < 9){
			//breakfast
			possibleRoutines.Add(new EatAction(gameObject, "Chair: Emma"));
		} else if (hour >= 9 && hour < 13){
			//deck/lobby
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Lobby Display Waypoints"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Chris"), FollowAction.Reason.NPC, true));
			//possibleRoutines.Add(new WanderAction(gameObject, 3, "Deck Connecting Waypoints"));
		} else if (hour >= 13 && hour < 14){
			//lunch
			possibleRoutines.Add(new EatAction(gameObject, "Chair: Emma"));
		} else if (hour >= 14 && hour < 17){
			//restaurant/deck/lobby
			//possibleRoutines.Add(new WanderAction(gameObject, 3, "Deck Connecting Waypoints"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			possibleRoutines.Add(new BuyItemAction(gameObject, "drink", "Waypoint: Restaurant Shopkeeper"));
			possibleRoutines.Add(new BuyItemAction(gameObject, "beer", "Waypoint: Restaurant Shopkeeper"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Restaurant Window Waypoints"));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Chris"), FollowAction.Reason.NPC, true));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Ed"), FollowAction.Reason.NPC, true));
		} else if (hour >= 17 && hour < 18){
			//dinner
			possibleRoutines.Add(new EatAction(gameObject, "Chair: Emma"));
		} else if (hour >= 18 && hour < 20){
			//restaurant/deck
			//possibleRoutines.Add(new WanderAction(gameObject, 3, "Deck Connecting Waypoints"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			//possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Restaurant Window Waypoints"));
			possibleRoutines.Add(new BuyItemAction(gameObject, "drink", "Waypoint: Restaurant Shopkeeper"));
			possibleRoutines.Add(new BuyItemAction(gameObject, "beer", "Waypoint: Restaurant Shopkeeper"));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Chris"), FollowAction.Reason.NPC, true));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Ed"), FollowAction.Reason.NPC, true));
		} else if (hour >= 20 || hour < 22){
			//disco
			possibleRoutines.Add(new BuyItemAction(gameObject, "beer", "Waypoint: Disco Shopkeeper"));
			possibleRoutines.Add(new BuyItemAction(gameObject, "drink", "Waypoint: Disco Shopkeeper"));
			possibleRoutines.Add(new DanceAction(gameObject, 20.0f, "Waypoint: Dance Floor", false));
			possibleRoutines.Add(new DanceAction(gameObject, 20.0f, "Waypoint: Dance Floor", false));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Chris"), FollowAction.Reason.NPC, true));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Ed"), FollowAction.Reason.NPC, true));
		}
		else if(hour >= 22 || hour < 0) {
			//As above minus follow Chris, as he is, very likely, sleeping
			possibleRoutines.Add(new BuyItemAction(gameObject, "beer", "Waypoint: Disco Shopkeeper"));
			possibleRoutines.Add(new BuyItemAction(gameObject, "drink", "Waypoint: Disco Shopkeeper"));
			possibleRoutines.Add(new DanceAction(gameObject, 20.0f, "Waypoint: Dance Floor", false));
			possibleRoutines.Add(new DanceAction(gameObject, 20.0f, "Waypoint: Dance Floor", false));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Chris"), FollowAction.Reason.NPC, true));
		}

		return possibleRoutines;
	}
	
	protected override Seat GetSeat(Area area){
		GameObject seatGO = null;
		if (area.name.Equals("Restaurant")){
			seatGO = GameObject.Find("Chair: Emma");
		} else if (area.name.Equals("Disco")){
			seatGO = GameObject.Find("Lounger: Emma");
		}
		if (seatGO != null){
			return (Seat)seatGO.GetComponent("Seat");
		} else {
			return null;
		}
	}

}

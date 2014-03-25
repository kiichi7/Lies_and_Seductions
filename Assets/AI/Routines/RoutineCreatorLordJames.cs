using UnityEngine;
using System.Collections;

public class RoutineCreatorLordJames : RoutineCreator {

	private const int MAXIMUM_DRUNKNESS = 10;
	
	protected override ArrayList GetPossibleRoutines(int hour, int minute){
		ArrayList possibleRoutines = new ArrayList();
		CharacterState state = GetComponent(typeof(CharacterState)) as CharacterState;
		if (state.GetDrunkness() >= MAXIMUM_DRUNKNESS && hour < 22 && hour >= 9){
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
		} else if (hour >= 9 && hour < 10){
			//breakfast
			possibleRoutines.Add(new EatAction(gameObject, "Chair: Lord James"));
		} else if (hour >= 10 && hour < 11){
			//walk on deck
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Emma"), FollowAction.Reason.NPC, true));
			//possibleRoutines.Add(new StartDialogueAction(gameObject, CharacterManager.GetPC(), "Lord James", "normal"));
		} else if (hour >= 11 && hour < 14){
			//cabin
			possibleRoutines.Add(new IdleAction(gameObject, 20.0f, GameObject.Find("Waypoint: Cabin: Lord James"), DistanceConstants.WAYPOINT_RADIUS, false));
			possibleRoutines.Add(new IdleAction(gameObject, 20.0f, GameObject.Find("Waypoint: Cabin: Lord James"), DistanceConstants.WAYPOINT_RADIUS, false));
			possibleRoutines.Add(new IdleAction(gameObject, 20.0f, GameObject.Find("Waypoint: Cabin: Lord James"), DistanceConstants.WAYPOINT_RADIUS, false));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Emma"), FollowAction.Reason.NPC, true));
			//possibleRoutines.Add(new StartDialogueAction(gameObject, CharacterManager.GetPC(), "Lord James", "normal"));
		} else if (hour >= 14 && hour < 15){
			//lunch
			possibleRoutines.Add(new EatAction(gameObject, "Chair: Lord James"));
		} else if (hour >= 15 && hour < 18){
			//deck/lobby/restaurant/cabin
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			//possibleRoutines.Add(new WanderAction(gameObject, 3, "Deck Connecting Waypoints"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Lobby Display Waypoints"));
			possibleRoutines.Add(new BuyItemAction(gameObject, "whiskey", "Waypoint: Restaurant Shopkeeper"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Restaurant Window Waypoints"));
			possibleRoutines.Add(new IdleAction(gameObject, 10.0f, GameObject.Find("Waypoint: Cabin: Lord James"), DistanceConstants.WAYPOINT_RADIUS, false));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Emma"), FollowAction.Reason.NPC, true));
			//possibleRoutines.Add(new StartDialogueAction(gameObject, CharacterManager.GetPC(), "Lord James", "normal"));
		} else if (hour >= 18 && hour < 19){
			//dinner
			possibleRoutines.Add(new EatAction(gameObject, "Chair: Lord James"));
		} else if (hour >= 19 && hour < 22){
			//deck/lobby/restaurant/disco
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			//possibleRoutines.Add(new WanderAction(gameObject, 3, "Deck Connecting Waypoints"));
			//possibleRoutines.Add(new WanderAction(gameObject, 3, "Lobby Connecting Waypoints"));
			possibleRoutines.Add(new BuyItemAction(gameObject, "whiskey", "Waypoint: Restaurant Shopkeeper"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Restaurant Window Waypoints"));
			if (hour >= 20){
				//???possibleRoutines.Add(new WanderAction(gameObject, 3, "Disco Connecting Waypoints"));
			}
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Emma"), FollowAction.Reason.NPC, true));
			//possibleRoutines.Add(new StartDialogueAction(gameObject, CharacterManager.GetPC(), "Lord James", "normal"));
		} else {
			//sleep
			possibleRoutines.Add(new IdleAction(gameObject, 1.0f, GameObject.Find("Waypoint: Cabin: Lord James"), DistanceConstants.WAYPOINT_RADIUS, false));
		}

		return possibleRoutines;
	}
	
	protected override Seat GetSeat(Area area){
		GameObject seatGO = null;
		if (area.name.Equals("Restaurant")){
			seatGO = GameObject.Find("Chair: Lord James");
		}
		if (seatGO != null){
			return (Seat)seatGO.GetComponent("Seat");
		} else {
			return null;
		}
	}

}

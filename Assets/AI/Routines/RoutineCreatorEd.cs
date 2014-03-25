using UnityEngine;
using System.Collections;

public class RoutineCreatorEd : RoutineCreator {

	private const int MAXIMUM_DRUNKNESS = 50;

	protected override ArrayList GetPossibleRoutines(int hour, int minute){
		ArrayList possibleRoutines = new ArrayList();
		int wakingHour = Random.Range(14, 17);
		CharacterState state = GetComponent(typeof(CharacterState)) as CharacterState;
		if (state.GetDrunkness() >= MAXIMUM_DRUNKNESS && hour < 2 && hour >= wakingHour){
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
		} if (hour >= wakingHour && hour < 17){
			//Lobby/deck
			possibleRoutines.Add(new StandAroundAction(gameObject, 3, "Lobby Display Waypoints"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Deck Railing Waypoints"));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Chris"), FollowAction.Reason.NPC, true));
			//possibleRoutines.Add(new WanderAction(gameObject, 3, "Deck Connecting Waypoints"));
		} else if (hour >= 17 && hour < 18){
			//Dinner (Ed's breakfast)
			possibleRoutines.Add(new EatAction(gameObject, "Chair: Ed"));
		} else if (hour >= 18 && hour < 20){
			//restaurant idle/gamble
			possibleRoutines.Add(new BuyItemAction(gameObject, "beer", "Waypoint: Restaurant Shopkeeper"));
			possibleRoutines.Add(new StandAroundAction(gameObject, 20.0f, "Restaurant Window Waypoints"));
			possibleRoutines.Add(new FollowAction(gameObject, CharacterManager.GetMajorNPC("Chris"), FollowAction.Reason.NPC, true));
			//possibleRoutines.Add(new NPCPokerAction());
		} else if (hour >= 20 || hour < 2){
			//disco
			possibleRoutines.Add(new BuyItemAction(gameObject, "beer", "Waypoint: Disco Shopkeeper"));
		} else {
			//sleep
			possibleRoutines.Add(new IdleAction(gameObject, 1.0f, GameObject.Find("Waypoint: Cabin: Ed"), DistanceConstants.WAYPOINT_RADIUS, false));
		}
		return possibleRoutines;
	}
	
	protected override Seat GetSeat(Area area){
		GameObject seatGO = null;
		if (area.name.Equals("Restaurant")){
			seatGO = GameObject.Find("Chair: Ed");
		} else if (area.name.Equals("Disco")){
			seatGO = GameObject.Find("Lounger: Ed");
		}
		if (seatGO != null){
			return (Seat)seatGO.GetComponent("Seat");
		} else {
			return null;
		}
	}
}

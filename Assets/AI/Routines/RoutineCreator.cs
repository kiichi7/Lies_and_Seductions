using UnityEngine;
using System.Collections;

public abstract class RoutineCreator : MonoBehaviour {
	
	protected CharacterState state;

	public void Start(){
		state = GetComponent(typeof(CharacterState)) as CharacterState;
		enabled=false;
	}

	public virtual Action CreateRoutine(){
		Item item = state.GetItem();
		//int drunkness = state.GetDrunkness();
		Seat seat = GetSeat(state.GetCurrentArea());
		string seatName;
		bool canSitDown;
		if (seat == null){
			seatName = null;
			canSitDown = false;
		} else {
			seatName = seat.name;
			canSitDown = true;
		}
		if (item == null){
			int hour = GameTime.GetDateAndTime().Hour;
			int minute = GameTime.GetDateAndTime().Minute;
			ArrayList possibleRoutines = GetPossibleRoutines(hour, minute);
			return RandomlyPickRoutine(possibleRoutines);
		} else if (item.name.Equals("beer") || item.name.Equals("whiskey") || item.name.Equals("drink")){
			return new SlowDrinkAction(gameObject, item.name, seatName, 20.0f, canSitDown);
		} else if (item.name.Equals("chocolate")){
			return new EatChocolateAction(gameObject, seatName, canSitDown);
		} else if (item.name.Equals("rose")){
			return new DropItemAction(gameObject, "Waypoint: Cabin: " + name);
		} else {
			Debug.LogError("RoutineCreator.CreateRoutine(): Unidentified item in " + name + "'s hand: " + item.name);
			return null;
		}
	}
	
	private Action RandomlyPickRoutine(ArrayList possibleRoutines){
		Debug.Log("RoutineCreator.RandomlyPickRoutine() for: " + name);
		return (Action)possibleRoutines[Random.Range(0, possibleRoutines.Count)];
	}
	
	protected abstract ArrayList GetPossibleRoutines(int hour, int minute);
	
	protected abstract Seat GetSeat(Area area);
}

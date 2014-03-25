using UnityEngine;
using System.Collections;

public class RoutineCreatorPC : RoutineCreator {

	public override Action CreateRoutine(){
		CharacterState state = GetComponent(typeof(CharacterState)) as CharacterState;
		Seat seat = state.GetCurrentSeat();
		string seatType = null;
		if (seat != null){
			seatType = seat.GetType();
		}
		return new PCAction(gameObject, seat != null, seatType);
	}
	
	//Should never be called!
	protected override ArrayList GetPossibleRoutines(int hour, int minute){
		Debug.LogError("RoutineCreatorPC.GetPossibleRoutines() should never be called!");
		return null;
	}
	
	//Should never be called!
	protected override Seat GetSeat(Area area){
		Debug.LogError("RoutineCreatorPC.GetSeat() should never be called!");
		return null;
	}
}

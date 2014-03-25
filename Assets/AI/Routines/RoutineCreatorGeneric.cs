using UnityEngine;
using System.Collections;

public class RoutineCreatorGeneric : RoutineCreator {

	public bool canWalk;
	public bool canSitOnChair;
	public bool canEat;
	public bool canSitOnLounger;
	public bool canSitOnDeckChair;
	public bool canDance;

	private Action routine;
	
	private Area area;
	
	public bool InitRoutine(Area area){
		this.area = area;
		ArrayList possibleRoutines = new ArrayList(); 
 		if (canWalk){
			possibleRoutines.Add(new GenericWanderAction(gameObject, area.wanderWaypointParent));
		}
		if (canSitOnChair && area.chairParent != null){
			possibleRoutines.Add(new GenericSitAction(gameObject, "chair", "sit_chair", area.chairParent));
		}
		if (canEat && area.chairParent != null){
			possibleRoutines.Add(new GenericSitAction(gameObject, "chair", "eat", area.chairParent));
		}
		if (canSitOnLounger && area.loungerParent != null){
			possibleRoutines.Add(new GenericSitAction(gameObject, "lounger", "sit_lounger", area.loungerParent));
		}
		if (canSitOnDeckChair && area.deckChairParent != null){
			possibleRoutines.Add(new GenericSitAction(gameObject, "deck_chair", "sit_deck_chair", area.deckChairParent));
		}
		if (canDance && area.name.Equals("Disco")){
		//	possibleRoutines.Add(new GenericDanceAction());
		}
		if (possibleRoutines.Count == 0){
			routine = new GenericSelfDestructAction(gameObject);
			return false;
		} else {
			routine = (Action)possibleRoutines[Random.Range(0, possibleRoutines.Count)];
			//Debug.Log("Chosen routine: " + routine + " for " + name);
			return true;
		}
	}
	
	public override Action CreateRoutine(){
		//Debug.Log("RoutineCreatorGeneric.CreateRoutine(): " + routine);
		return routine;
	}
	
	protected override ArrayList GetPossibleRoutines(int hour, int minute){
		//
		//if(area.name.Equals("Restaurant") && hour >= 23) {
			// 1. Go to door 
			// 2. Self Destruct
		//}
		return null;
	}
	
	protected override Seat GetSeat(Area area){
		return null;
	}
}

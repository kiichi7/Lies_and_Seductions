using UnityEngine;
using System.Collections;

public class DeckBackdoorSounds : MonoBehaviour {
		
	private static Vector3 thePlace;
	
	void Awake () {
		thePlace = transform.position;
	}

	public static void EnteredRestaurant() {
		FModManager.StartEvent(FModLies.EVENTID_LIES_LOBBY_DECK_NOISE,thePlace);
	}
	
	public static void LeftRestaurant() {
		FModManager.StopEvent(FModLies.EVENTID_LIES_LOBBY_DECK_NOISE);
	}
}

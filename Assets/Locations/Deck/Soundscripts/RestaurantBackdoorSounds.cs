using UnityEngine;
using System.Collections;

public class RestaurantBackdoorSounds : MonoBehaviour {
		
	private static bool atDeck = false;
	
	private static Vector3 thePlace;
	
	void Awake () {
		thePlace = transform.position;
	}
		
	public static void EnteredDeck() {
		if (GameTime.IsRestaurantOpen())
			FModManager.StartEvent(FModLies.EVENTID_LIES_LOBBY_RESTAURANT_NOISE,thePlace);
		atDeck = true;
	}
	
	public static void LeftDeck() {
		FModManager.StopEvent(FModLies.EVENTID_LIES_LOBBY_RESTAURANT_NOISE);
		atDeck = false;
	}
	
	public static void RestaurantOpen(bool o) {
		if (o && atDeck) {
			FModManager.StartEvent(FModLies.EVENTID_LIES_LOBBY_RESTAURANT_NOISE,thePlace);
		} else {
			FModManager.StopEvent(FModLies.EVENTID_LIES_LOBBY_RESTAURANT_NOISE);	
		}
	}
}
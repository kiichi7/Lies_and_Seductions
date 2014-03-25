using UnityEngine;
using System.Collections;

public class RestaurantDoorSounds : MonoBehaviour {
	
	private static bool atLobby = false;
	
	private static Vector3 thePlace;
	
	void Awake () {
		thePlace = transform.position;
	}
		
	public static void EnteredLobby() {
		if (GameTime.IsRestaurantOpen())
			FModManager.StartEvent(FModLies.EVENTID_LIES_LOBBY_RESTAURANT_NOISE,thePlace);
		atLobby = true;
	}
	
	public static void LeftLobby() {
		FModManager.StopEvent(FModLies.EVENTID_LIES_LOBBY_RESTAURANT_NOISE);
		atLobby = false;
	}
	
	public static void RestaurantOpen(bool o) {
		if (o && atLobby) {
			EnteredLobby();
		} else {
			FModManager.StopEvent(FModLies.EVENTID_LIES_LOBBY_RESTAURANT_NOISE);	
		}
	}
}
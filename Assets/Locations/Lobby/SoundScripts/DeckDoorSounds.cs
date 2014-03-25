using UnityEngine;
using System.Collections;

public class DeckDoorSounds : MonoBehaviour {
	
	private static Vector3 thePlace;
	
	void Awake () {
		thePlace = transform.position;
	}

	public static void EnteredLobby() {
		FModManager.StartEvent(FModLies.EVENTID_LIES_LOBBY_DECK_NOISE,thePlace);
	}
	
	public static void LeftLobby() {
		FModManager.StopEvent(FModLies.EVENTID_LIES_LOBBY_DECK_NOISE);
	}
}

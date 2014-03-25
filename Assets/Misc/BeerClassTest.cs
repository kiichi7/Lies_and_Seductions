using UnityEngine;
using System.Collections;

/************************************************************************************
 *
 * Script for testing how to add item to character's hand and attach item to the hand
 *
 * Author: Petri Lankoski
 *
 ************************************************************************************/

public class BeerClassTest : MonoBehaviour {

	public GameObject beer;
	public GameObject hand;
	public GameObject destroyThis;
		
		
	public int buttonX, buttonY;

	// Adds button for adding item to and removing item from hand to these coordinates
	private GameObject itemInHand;

	
	// Use this for initialization
	void Start () {
		itemInHand = null;
		
		// This is needed for Ed model that currently is holding a Beer Glass in his hand
		if(destroyThis) {
			Destroy(destroyThis);
		}
	}
	
	void OnGUI() {
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		if(GUI.Button(new Rect(buttonX, buttonY, 100, 20), "BUY BEER")) {
			
			Debug.Log("BUY BEER Clicked");

			Debug.Log("HAND <" + hand.transform.position.x + ", " + hand.transform.position.y + ", " + hand.transform.position.z + ">");
			if(itemInHand) {
				Destroy(itemInHand);
				itemInHand = null;
				Debug.Log("Old Beer removed");
			}
			else {
				itemInHand = (GameObject)Instantiate(beer, hand.transform.position, Quaternion.identity);
				Debug.Log("New added <" + itemInHand.transform.position.x + ", " + itemInHand.transform.position.y + ", " + itemInHand.transform.position.z + ">");
			}
		}
			
	}
	
	// Update is called once per frame
	void Update () {
		if (itemInHand) {
			itemInHand.transform.position = hand.transform.position;
		}
	}
}

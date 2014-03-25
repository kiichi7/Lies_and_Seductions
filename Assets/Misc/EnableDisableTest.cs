using UnityEngine;
using System.Collections;

public class EnableDisableTest : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("e")){
			Debug.Log("enabled");
			animation["walk"].enabled = true;
		}
	}
}

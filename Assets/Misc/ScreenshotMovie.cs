using UnityEngine;
using System.Collections;

public class ScreenshotMovie : MonoBehaviour {

	public string folder = "ScreenshotFolder";
	public int frameRate = 25;  

	private enum STATES {DEFAULT, CONFIRM, CAPTURING, ACK};
	private STATES state;

	private bool capturing = false;
	private bool startCapturing = false;
	private bool showMessage = false;
	private string message = "";
	private string realFolder = "";
	// Use this for initialization
	void Start () {
    	state = STATES.DEFAULT;
    	realFolder = "";
    	message = "";
   	}
	
	// Update is called once per frame
	void Update () {
		if(state == STATES.CAPTURING) {
			if(Input.GetKeyDown("m")) {
				showMessage = true;
				state = STATES.ACK;
				message = "Capturing finished. Screenshots stored in " + realFolder;
			}
			else {
    			string name = string.Format("{0}/{1:D04}_shot.png", realFolder, Time.frameCount );
    			Application.CaptureScreenshot (name);
			}
		}
		else {
			if(Input.GetKeyDown("m")) {
				state = STATES.CONFIRM;
				message = "Press OK to start capture movie. Quit with 'm'";
			}
		}
	}
	
	void OnGUI() {
		/*GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		string stateString = "";
		switch(state) {
			case STATES.DEFAULT:
				stateString= "DEFAULT";
				break;
			case STATES.CONFIRM:
				stateString= "CONFIRM";
				break;
			case STATES.ACK:
				stateString= "ACK";
				break;
			case STATES.CAPTURING:
				stateString= "CAPTURING";
				break;
		}
		GUI.Label(new Rect(0, 0, 100, 50), stateString);
		*/
		if(state == STATES.CONFIRM || state == STATES.ACK) {
			//GUI.skin=gSkin;
			GUI.matrix = GUIGlobals.GetGUIScaleMatrix();

			GUILayout.BeginArea(new Rect(GUIGlobals.GetCenterX()-300, GUIGlobals.GetCenterY()-200, 600, 400));
			GUILayout.BeginVertical();
		
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(message);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("OK")) {
				if(state == STATES.CONFIRM) {
					state = STATES.CAPTURING;
					StartCapture();	
				}
				else {
					state = STATES.DEFAULT;
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
					
			GUILayout.EndVertical();
			GUILayout.EndArea(); 	
		}
	}
	
	private void StartCapture() {
		// Find a folder that doesn't exist yet by appending numbers!
    	realFolder = folder;
    	int count = 1;
    	while (System.IO.Directory.Exists(realFolder)) {
       	 	realFolder = folder + count;
        	count++;
    	}
    	// Create the folder
    	System.IO.Directory.CreateDirectory(realFolder);
		Time.captureFramerate = frameRate;
	}
}

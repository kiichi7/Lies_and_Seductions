/**********************************************************************
 *
 * CLASS 
 *
 * Copyright 2008 Tommi Horttana, Petri Lankoski, Jari Suominen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License 
 * at http://www.apache.org/licenses/LICENSE-2.0 Unless required 
 * by applicable law or agreed to in writing, software distributed 
 * under the License is distributed on an "AS IS" BASIS, WITHOUT 
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 * See the License for the specific language governing permissions and 
 * limitations under the License.
 *
 ***********************************************************************/
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

/************************************************************************************
 *
 * Class for creating MAIN MENU and PAUSE menu,  
 *
 *
 ************************************************************************************/

public class MainMenu : MonoBehaviour {
	
	/************************************************************************************
	 *
	 * PUBLIC MEMBERS, these can be set in Unity GUI
	 *
	 ************************************************************************************/
	
	// Skin for GUI, defines fonts, font colours, aligments, etc.
	public GUISkin gSkin;
	//public GUIStyle "smallFontBox";
	// The background for menu screens (4:3) 
	public Texture2D mainBG;
	public Texture2D infoBG;

	// The background for menu screens (wide screen) 
	public Texture2D mainBG_wide;
	public Texture2D infoBG_wide;

	// Images to be displayed in credits screen
	public Texture2D []creditsImg;
	
	// Level to load when game is started
	public string StartLevel;
	public string introCutSceneName;

	// Is this START menu 
	public bool startMenu;
	
	public string feedbackUrl="http://mlab.taik.fi/~plankosk/lsfeedback/feedback.php";
	private WWW myWeb = null;

	/************************************************************************************
	 *
	 * PRIVATE MEMBERS, these can be set in Unity GUI
	 *
	 ************************************************************************************/

	// Whether this menu is on
	private bool isOn;
		
	private const string helpText_column1="- Your goals is to seduce Chris before cruise ends.\n- Time and time left if dislayed in the bottom right hand side.\n- It is also possible to seduce and exploit other passangers.\n- Use cursor or ASDW to move and mouse to interact with objects and charaters. Cursor changes to indicate that interaction is possible.\n- Hit the spacebar to skip dialogue\n\n\nHINT\nChris does not volunteer to be seduced, and you need to get his friends help.";

	private const string helpText_column2="- In order to dance in dance floor of night club, use cursor keys (or ASDW) as corresponding with the dropping arrow.\n- To go sleep use elevator. Abby can go to sleep between 20-04 and she wakes 8  hours after that or take a NPC to the elevator whenever sex is in the agenda. \n- Poker table can be found at the Restaurant";
	
	// MainMenu STATES (Menu Screeens)
	private const int MAIN_STATE=0;
	private const int CREDITS_STATE=1;
	private const int QUIT_CONFIRM_STATE=2;
	private const int HELP_STATE=3;
	private const int LOADING_LEVEL_STATE=4;
	private const int INTRO_CUTSCENE_PLAYING=5;
	private const int OPTIONS_STATE=6;
	private const int FEEDBACK_STATE=7;
	private const int SHOW_MESSAGE_STATE=8;
	private const int WWW_REQUEST_STATE=9;
	private const int ARCHIEVEMENTS_STATE=10;
		
	// Default menu screen
	private int state=MAIN_STATE;
	
	// Level to be loaded when QUIT is selected in PAUSE MENU
	private string MainMenuLevel="StartScreen";
	
	// Sounds are played using FMOD Sound and Music System
	private FModManager fmod;
	// 
	private float musicVolume;
	private float effectVolume;
	
	// FeedBack system uses these
	private string performanceDebugText = "";
	private string feedbackString = "";
	private Vector2 scrollPosition = Vector2.zero;
	
	// MesageScreeen shows this message
	private string guiMessage="";
	
	private int loadingIndicator;
	
	private Texture2D defaultCursor;
	
	private bool mouseMoveChanged;
	
	/************************************************************************************
	 *
	 * OVERLOADED METHODS, from MonoBehaviour
	 *
	 ************************************************************************************/
	
	void Update () {
		if(state == INTRO_CUTSCENE_PLAYING) {
			if(CutScenePlayer.IsPlaying() == false) {
				state = LOADING_LEVEL_STATE;
				Application.LoadLevel(StartLevel);
			}
		}
		if (Input.GetKeyDown("escape")){
			SetOn(!isOn);
		}
		if(isOn) {
			if(state == OPTIONS_STATE && (Input.GetKey("left shift") || Input.GetKey("right shift")) && Input.GetKeyDown("c")) {
				SaveLoad.saveEnabled = !SaveLoad.saveEnabled;	
			} 
			if(state == ARCHIEVEMENTS_STATE && (Input.GetKey("left shift") || Input.GetKey("right shift")) && Input.GetKeyDown("r")) {
				Archievements.Reset();	
			}
			if((Input.GetKey("left shift") || Input.GetKey("right shift")) && Input.GetKeyDown("f")) {
				state = FEEDBACK_STATE;
			}	
		}
	}
	
	void Awake() {
		Debug.Log("MainMenu.Awake(): Lies and Seductions version: " + lsInfo.VERSION);
		if (startMenu) {
        	FModManager.Cue(FModLies.MUSICCUE_LIES_STARTSCREEN_MUZAK);
		}
		
		if (!gSkin)
		{
			Debug.LogError("MainMenu.Awake(): gSkin is not set.");
		}
		
		guiMessage="";
		loadingIndicator=0;
		mouseMoveChanged = false;
		// We load PlayerPreferences on the disk, but first we need check if
		// this is the firts time the game is started to get right default values
		int initialized = PlayerPrefs.GetInt("Initialized");
		if(initialized == 0) {
			effectVolume=1.0f;
			musicVolume=1.0f;
			PlayerPrefs.SetFloat("EffectVolume", 1.0f);
			PlayerPrefs.SetFloat("MusicVolume", 1.0f);
			PlayerPrefs.SetInt("ToolTip", 1);
		}
		else {
			effectVolume = PlayerPrefs.GetFloat("EffectVolume");
		}
		//Not implemented...
		//FModManager.SetSoundVolume(effectVolume);
	}
		
	//public void LateUpdate() {
	//	
	//}
	
	void OnApplicationQuit() {
		FModManager.StopAll();	
	}
	
	void OnGUI()
	{
		
		if (startMenu || isOn) 
		{
			Screen.showCursor = true;
		} 
		
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		GUI.depth = 0;
		if (startMenu==false && !isOn) 
		{
			return;
		}
		
		// Set up SKIN for START and PAUSE menus
		
		GUI.skin=gSkin;	
			
		// Creating correct screen
		switch(state)
		{
			case MAIN_STATE:
				DrawBackgroundMain();
				MainMenuScreen();
				break;
			case CREDITS_STATE:
				DrawBackgroundInfo();
				CreditsMenu();
				break;
			case QUIT_CONFIRM_STATE:
				DrawBackgroundInfo();
				ConfirmQuit();
				break;
			case HELP_STATE:
				DrawBackgroundInfo();
				HelpMenu();
				break;
			case LOADING_LEVEL_STATE:
				DrawBackgroundInfo();
				LoadingScreen();
				break;
			case OPTIONS_STATE:
				DrawBackgroundInfo();
				OptionsScreen();
				break;
			case FEEDBACK_STATE:
				DrawBackgroundInfo();
				FeedbackScreen();
				break;
			case INTRO_CUTSCENE_PLAYING:
				// CutScenePlayer handles all drawing, we just make sure that no drawing is made in this state
				break;
			case SHOW_MESSAGE_STATE:
				DrawBackgroundInfo();
				ShowMessageScreen();
				break;
			case  WWW_REQUEST_STATE:
				DrawBackgroundInfo();
				ShowWWWLoadingScreen();
				break;
			case ARCHIEVEMENTS_STATE:
				DrawBackgroundInfo();
				if(Archievements.Draw()) {
					state=MAIN_STATE;	
				}
				break;
			default:
				MainMenuScreen();
				Debug.LogError("Undefined MainMenu STATE: Displaying MAIN_STATE screen");
				break;
		}
	}	
	
	/************************************************************************************
	 *
	 * PRIVATE METHODS
	 *
	 ************************************************************************************/
	
	private void ShowMessage(string msg) {
		state = SHOW_MESSAGE_STATE;
		guiMessage=msg;
	}
	
	private void MainMenuScreen()
	{
		// The main menu, defaut screen
		
		int menuW=550;
		int menuH=50;
		if (!startMenu) {
			GUI.Box(new Rect(0, 0, GUIGlobals.screenWidth,GUIGlobals.screenHeight), "Game Paused");
			menuW=500;	
		}
		
		float menuY;
		if(GUIGlobals.IsWideScreen()) {
			menuY = GUIGlobals.screenHeight-130;
		}
		else {
			menuY = GUIGlobals.screenHeight-110;	
		}
		GUILayout.BeginArea (new Rect (0, menuY, GUIGlobals.screenWidth, menuH));
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (startMenu) 
		{
			if (GUILayout.Button( "New Game") )
			{	
				Debug.Log("New Game selected");
				state=LOADING_LEVEL_STATE;
				
				FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
				SaveLoad.ResetSave();
				IngameHelpMemory.Reset();
				Screen.showCursor = false;
				state = INTRO_CUTSCENE_PLAYING;
				CutScenePlayer.Play(introCutSceneName);
			}
			if (SaveLoad.SaveExits())
			{
				if (GUILayout.Button( "Continue") )
				{
					Application.LoadLevel(StartLevel);
					state=LOADING_LEVEL_STATE;
				}
			} 
		}
		else
		{
			if (GUILayout.Button( "Continue") ) {
					SetOn(false);
					Screen.showCursor = false;
			}
			if(Application.isEditor && SaveLoad.SaveExits()) {
				if (GUILayout.Button( "Reset Save") ) {
					SaveLoad.ResetSave();	
				}
			}
		}
		//if(startMenu==true)
		//{
		if (GUILayout.Button( "Credits") )
		{
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
			state=CREDITS_STATE;
		}
		//}
		if (GUILayout.Button( "Help") )
		{
			state=HELP_STATE;
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
		}
		if (GUILayout.Button( "Options") )
		{
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);

			state=OPTIONS_STATE;
		}
		string quitText="Exit";
		if (startMenu==false) 
		{
			// SAVE GAME HERE!!!
			quitText="Quit";
			
		}
		if (GUILayout.Button( "Achievements") ) {
			
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);

			state=ARCHIEVEMENTS_STATE;
		}
		 
		//if (GUILayout.Button( "Feedback") )
		//{
		//	FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
		//	state=FEEDBACK_STATE;
		//}
		
		if (GUILayout.Button( quitText ) )
		{
			if (startMenu==true) 
			{
				Application.Quit();
			}
			else
			{
				state=QUIT_CONFIRM_STATE;
			}
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);

		}
		GUILayout.FlexibleSpace();

		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
			
	}

	
	private void OptionsScreen()
	{
		int menuH = 1200;
		int menuW = 900;		
		// 
		GUILayout.BeginArea (new Rect (GUIGlobals.GetCenterX()-menuW/3, 250, menuW, menuH));
		
		/*GUILayout.BeginHorizontal();
		GUILayout.Label("Sound Volume:");
		effectVolume = GUILayout.HorizontalSlider(effectVolume, 0.0f, 1.0f);
		GUILayout.EndHorizontal();*/
		
		GUILayout.BeginHorizontal();
		
		GUILayout.BeginVertical();
		GUILayout.Label("Resolution:", "right aligned");
		GUILayout.Label("Full screen", "right aligned");
		GUILayout.Label("Graphics quality:", "right aligned");
		GUILayout.Label("Help:", "right aligned");
		GUILayout.Label("Controls:", "right aligned");
		GUILayout.Label("");
		GUILayout.EndVertical();
		
		
		GUILayout.BeginVertical();
		
		int w = PlayerPrefs.GetInt("Screenmanager Resolution Width");
		int h = PlayerPrefs.GetInt("Screenmanager Resolution Height");
		int n=0;
		foreach (Resolution res in Screen.resolutions) {
			if(res.width == w && res.height == h) {
				break;	
			}
			n++;
		}
		GUILayout.BeginHorizontal();
		bool resChanged = false;
		if(GUILayout.Button("<")) {
			n--;
			if(n>=0) {
				Screen.SetResolution (Screen.resolutions[n].width, Screen.resolutions[n].height, Screen.fullScreen);
			}
				
		}
		GUILayout.Label(w + "x" + h);
		if(GUILayout.Button(">")) {
			n++;
			if(n < Screen.resolutions.Length) {
				Screen.SetResolution (Screen.resolutions[n].width, Screen.resolutions[n].height, Screen.fullScreen);	
			}	
		}
		GUILayout.EndHorizontal();
		
		
		if(resChanged) {
			 PlayerPrefs.SetInt("Screenmanager Resolution Width", Screen.currentResolution.width);
			 PlayerPrefs.SetInt("Screenmanager Resolution Height",Screen.currentResolution.height); 
		}
		
		
		//GUILayout.BeginHorizontal();
		string []onOffText = {"Off", "On"};
		int fullScreenInUse = 0;
		if(Screen.fullScreen) {
			fullScreenInUse = 1;
		} 
		fullScreenInUse = GUILayout.Toolbar(fullScreenInUse, onOffText);
		//GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		if(GUILayout.Button("<")) {
			QualitySettings.DecreaseLevel();
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
			LocationAnimQuality.Changed(QualitySettings.currentLevel);

		}
		
		GUILayout.Label(QualitySettings.currentLevel.ToString());
		if(GUILayout.Button(">")){
			QualitySettings.IncreaseLevel();
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
			LocationAnimQuality.Changed(QualitySettings.currentLevel);
		}
		GUILayout.EndHorizontal();
		
		int oldTooltipVal = PlayerPrefs.GetInt("ToolTip"); 
		int tooltipInUse = GUILayout.Toolbar(oldTooltipVal, onOffText);

		int oldMouseMoveVal = PlayerPrefs.GetInt("mouseMove");
	
		string []controls = {"Default", "Mouse"}; 
		GUILayout.BeginHorizontal();
		int mouseMove = GUILayout.Toolbar(oldMouseMoveVal, controls);
		if(mouseMove==1) { 
			GUILayout.Label("Move: mouse right, mouse position\nInteract: mouse left", "left aligned small");
		}
		else {
			GUILayout.Label("Move: cursor keys (ALT: ASDW),\nInteract: mouse left", "left aligned small");	
		}
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea (); 
		
		// For testing. Remove this when save works 
		if(SaveLoad.saveEnabled) {
			GUI.Label(new Rect(GUIGlobals.screenWidth-250, 30, 250, 100), "Save enabled\ndisable with 'c'");
		}
		
		if (GUI.changed) {
			//Debug.Log("OptionsScreen: User Changed Preferences");
			PlayerPrefs.SetFloat("MusicVolume", musicVolume);
			PlayerPrefs.SetFloat("EffectVolume", effectVolume);
			if(oldMouseMoveVal!=mouseMove) {
				//Debug.Log("MainMenu.OptionsScreen() MoudeMove changed!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
				FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
				PlayerPrefs.SetInt("mouseMove", mouseMove);
				if(mouseMove == 1) {
					MouseHandler.LockCursorXPosition();
				}
				else {
					MouseHandler.ReleaseCursorXPosition();	
				}
				//IngameHelpMemory.Unmark(IngameHelpMemory.START_HELP);
			}
			if(oldTooltipVal != tooltipInUse) {
				FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
				PlayerPrefs.SetInt("ToolTip", tooltipInUse);
				IngameHelpMemory.Reset();	
			}
			if(fullScreenInUse == 0) {
				Screen.fullScreen = false;
			}
			else {
				Screen.fullScreen = true;	
			}
			TaskHelp.PreferenceChanged();
			//Not implemented
			//FModManager.SetSoundVolume(effectVolume);
		}
		
		if (GUI.Button( new Rect(GUIGlobals.GetCenterX()-50, GUIGlobals.screenHeight-80, 100,50 ), "Back") )
		{
			//Debug.Log("Options::Back selected");
			state=MAIN_STATE;
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
		}

	}
	
	private void DrawBackgroundMain() {
		GUIStyle bgStyle = new GUIStyle();
		bool isWide=GUIGlobals.IsWideScreen();
		if (isWide) {
			bgStyle.normal.background=mainBG_wide;
		}
		else {
			bgStyle.normal.background=mainBG;
		}
		GUI.Label( new Rect(0,0,GUIGlobals.screenWidth, GUIGlobals.screenHeight), "", bgStyle);
	} 
	
	private void DrawBackgroundInfo() {
		GUIStyle bgStyle = new GUIStyle();
		bool isWide=GUIGlobals.IsWideScreen();
		if (isWide) {
			bgStyle.normal.background=infoBG_wide;
		}
		else {
			bgStyle.normal.background=infoBG;
		}
		GUI.Label( new Rect(0,0,GUIGlobals.screenWidth, GUIGlobals.screenHeight), "", bgStyle);

	}
		
	private void LoadingScreen()
	{
		
		GUI.Label(new Rect(GUIGlobals.GetCenterX()-100, GUIGlobals.GetCenterY(), 250,100), "Loading...");
	}
	
	
	
	// Checks if a save exits
	/*private bool SaveExists()
	{
		return false;	
	}*/
	
	private void CreditsMenu()
	{		
		//GUI.Label( new Rect(GUIGlobals.GetCenterX()-50, 200, 150,50 ), "Credits\n");
		
		int gutter = 25;
		int menuH = 700;
		//int menuW = (int)(GUIGlobals.GetCenterX()-gutter);
		 int menuXStart= 100;
		int bodyColumnStartY = 200;
		int menuW = (int)GUIGlobals.screenWidth - menuXStart*2;
		
		// Main Credits, column 1
		GUILayout.BeginArea (new Rect (menuXStart, bodyColumnStartY, menuW, menuH));
		GUILayout.BeginHorizontal();
		GUILayout.Label(CreditsText.Column1, "smallFontBoxRightAligned");
		GUILayout.Space(gutter);
		GUILayout.Label(CreditsText.Column2, "smallFontBox");
		GUILayout.Space(gutter);
		GUILayout.Label(CreditsText.additionalCredits, "smallFontBoxWordWrap");
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
		
		//GUI.Label(new Rect(100, 600, GUIGlobals.screenWidth-200,200), CreditsText.additionalCredits, "smallFontBox");
		
		int totalW = 0;
		foreach(Texture2D img in creditsImg) {
			totalW += img.width + gutter;
		}
		//float drawX = GUIGlobals.GetCenterX()- (GUIGlobals.screenWidth - drawW)/2;
		GUILayout.BeginArea (new Rect ((GUIGlobals.screenWidth-totalW)/2, GUIGlobals.screenHeight-150,  totalW, 64));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		foreach(Texture2D img in creditsImg) {
			//GUIStyle tmpStyle=new GUIStyle();
			//tmpStyle.normal.background = img;
			
			GUILayout.Label(img);
			GUILayout.Space(gutter);
			
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
		
		if (GUI.Button(new Rect(GUIGlobals.GetCenterX()-50, GUIGlobals.screenHeight-80, 100,50 ), "Back"))
		{
			Debug.Log("Credits::Back selected");
			state=MAIN_STATE;
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);

		}

		
	}
	
	private void HelpMenu()
	{
		
		GUI.Label( new Rect(GUIGlobals.GetCenterX()-50, 200, 100,50 ), "Help");
		
		int menuH = 700;
		int gutter = 50;
		
		// Main Credits, column 1
		GUILayout.BeginArea (new Rect (0, 400, GUIGlobals.screenWidth, menuH));
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("http://www.liesandseductions.com/help.html", "buttonCenter")) {
				Application.OpenURL("http://www.liesandseductions.com/help.html");
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea (); 
		
		/*
		GUILayout.BeginArea (new Rect (gutter, 250, GUIGlobals.GetCenterX()-gutter, menuH));
		GUILayout.Label(helpText_column1, "smallFontBox");
		GUILayout.EndArea (); // Main Credits ends
		
		// Main Credits, column 2
		GUILayout.BeginArea (new Rect (GUIGlobals.GetCenterX()+gutter, 250, GUIGlobals.GetCenterX()-gutter*2, menuH));
		
		GUILayout.Box(helpText_column2, "smallFontBox");
		
		if(GUILayout.Button("http://www.liesandseductions.com/help.html")) {
				Application.OpenURL("http://www.liesandseductions.com/help.html");
		}
		GUILayout.EndArea (); // Main Credits, column 2
		*/
		if (GUI.Button( new Rect(GUIGlobals.GetCenterX()-50, GUIGlobals.screenHeight-80, 100,50 ), "Back") )
		{	
			state=MAIN_STATE;
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
		}
		
	}
	
	private void FeedbackScreen() {
		
		GUI.Label( new Rect(GUIGlobals.GetCenterX()-100, 200, 200,50 ), "BUILD INFO");
			 
		
		string message = "";
		//if (startMenu) {
		message = "\n\n" + lsInfo.GetSystemInfo() + "\n" + performanceDebugText;
		//}
		//else {
		//	message = "\n\n" + lsInfo.GetSystemInfo() + "\n" + performanceDebugText;
		//}
		
		GUI.Label(new Rect(GUIGlobals.screenWidth-400,300,390,GUIGlobals.screenHeight), message, "debugInfo");
	
		//scrollPosition = GUI.BeginScrollView(new Rect (100,300,GUIGlobals.screenWidth/2+20, GUIGlobals.screenHeight-400),scrollPosition, new Rect (0, 0, GUIGlobals.screenWidth/2, 4000));
		
		//feedbackString = GUI.TextArea(new Rect (0, 0, GUIGlobals.screenWidth/2, 4000), feedbackString, 2000);
	
		//GUI.EndScrollView();
	
		if (GUI.Button( new Rect(GUIGlobals.GetCenterX()-100, GUIGlobals.screenHeight-80, 120,50 ), "Back") )
		{	
			state=MAIN_STATE;
			feedbackString = "";
			scrollPosition = Vector2.zero;
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
		}
		/*
		if (GUI.Button( new Rect(GUIGlobals.GetCenterX()+50, GUIGlobals.screenHeight-80, 120,50 ), "Send") )
		{	
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
			
			// We send feedback using web for
			WWWForm form = new WWWForm();
			form.AddField("data",lsInfo.GetSystemInfo() + "\n" + performanceDebugText);
			form.AddField("feedback", feedbackString);
			myWeb = new WWW(feedbackUrl, form);
			
			scrollPosition = Vector2.zero;
			
			//feedbackString = "";
			
			// And next we will show Sending... screen util we know wether the http query seceeded or not
			state=WWW_REQUEST_STATE;
		}*/
	}
	
	private void ShowWWWLoadingScreen() {
		if(myWeb.isDone) {
			if(myWeb.error != null) {
				Debug.LogError("MainMenu.ShowWWWLoadingScreen(): Error in sending feedback!");
				ShowMessage("Sending Feedback Failed\n" + myWeb.error);
			}
			else {
				ShowMessage("Send OK\n\nThank you for your feedback.");	
			}
		} 
		else {
			GUI.Label(new Rect(GUIGlobals.GetCenterX()-100, GUIGlobals.GetCenterY(), 250,100), "Sending...");
		}
	}
	
	
	private void ShowMessageScreen() {
		
		Debug.Log("MainMenu.ShowMessageScreen() " + guiMessage);
		GUI.Label(new Rect(100,390, GUIGlobals.screenWidth-200, GUIGlobals.screenHeight), guiMessage);
		
		if (GUI.Button( new Rect(GUIGlobals.GetCenterX()-50, GUIGlobals.screenHeight-80, 100,50 ), "OK") )
		{	
			guiMessage="";
			state=MAIN_STATE;
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
		}
	}
	
	private void ConfirmQuit()
	{	
	
		GUILayout.BeginArea  (new Rect (GUIGlobals.GetCenterX()-200, GUIGlobals.GetCenterY()-150, 400, 500));
	
		string confirmText = "";
		if(SaveLoad.SaveExits()) {
			confirmText = "You will lose all your progress since last autosave.";
		}
		else {
			confirmText = "You will lose all your progress.";
		}
		
		
		GUILayout.Label("Really Quit?");
		GUILayout.Label(confirmText, "smallCenter");
		GUILayout.Space(50);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("yes") ) {
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);
			
			if (!Application.isEditor) {
				Application.Quit();	
			}
			//else {
			//	Debug.Log("MainMenu.ConfirmQuit(): OK selected in editor. Ignoring");	
			//}


			// There is some bug that prevent restarting the game from start menu. So we cannot load level
			/*if (startMenu)
			{
				if ( (Application.platform == RuntimePlatform.OSXPlayer) || (Application.platform == RuntimePlatform.WindowsPlayer) )
				{
					Application.Quit();	
				}
			}
			else
			{
				state=LOADING_LEVEL_STATE;
				Application.LoadLevel(MainMenuLevel);
			}*/
			
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("no"))
		{
			FModManager.StartEvent(FModLies.EVENTID_LIES_STARTMENU_MOUSECLICKED);

			state=MAIN_STATE;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea  ();
	}
	
	/*
	 * Handles Pausing the game
	 *
	 */
	private void SetOn(bool isOn){
		this.isOn = isOn;
		Pause.SetPaused(isOn);
		if(isOn) {
			performanceDebugText = PerformanceDebug.GetAsString();
		}
		else if (isOn==false) {
			PerformanceDebug.Reset();
		}
	}

		
}
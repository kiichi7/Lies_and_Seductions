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
using System;

public class PopupMenuGUI : MonoBehaviour {
	
	private class MenuButton {
		
		// This contains icons for button 
		private GUIStyle buttonStyle;

		// The center of the button icon		
		public float x;
		public float y;
		
		private SelectableAction action;

		// Button icon is drawn inside this
		public Rect buttonRect;

		// tooltip text is here
		public string tooltip;
		
		public MenuButton(MenuIconSet iconSet, SelectableAction action){
			this.buttonStyle = new GUIStyle();
			this.buttonStyle.normal.background = iconSet.normal;
			this.buttonStyle.hover.background = iconSet.hover;
			this.buttonStyle.active.background = iconSet.hover;
			this.tooltip = iconSet.tooltip;
			this.action = action;
			
		}
		
		public SelectableAction GetAction(){
			return action;
		}
		
		public void SetPosition(float x, float y, int diameter){
			this.x = x;
			this.y = y;
			this.buttonRect = new Rect(x - diameter / 2, y - diameter / 2, diameter, diameter);
		}
		
		/*
		 * Draws a menu item and returns true if it has been clicked
		 */
		public bool Draw(){
			if (GUI.Button(buttonRect, "", buttonStyle))  {
				return true;
			} else {
				return false;
			}
		}
	}

	
	public MenuIconSet eatDrinkIcon;
	public MenuIconSet giveIcon;
	public MenuIconSet dropIcon;
	public MenuIconSet buyWhiskeyIcon;
	public MenuIconSet buyBeerIcon;
	public MenuIconSet buyDrinkIcon;
	public MenuIconSet buyFlowersIcon;
	public MenuIconSet buyChocolateIcon;
	public MenuIconSet drinkIcon;
	public MenuIconSet flirtIcon;
	public MenuIconSet talkIcon;
	public MenuIconSet playPokerIcon;
	public MenuIconSet sleepIcon;
	public MenuIconSet sexIcon;
	public MenuIconSet sitDownIcon;
	public MenuIconSet standUpIcon;
	public MenuIconSet danceIcon;
	public MenuIconSet closeMenuIcon; 

	public int menuItemDiameter=70;
		
	public RectOffset menuMinDistanceFromBorders;
		
	public GUISkin gSkin;
	
	public int helpScreenWidth = 250;
	public int helpScreenYOffset = 100;
		
	private ArrayList menuButtons;
	private Vector2 menuPos;
	private GUIStyle bgStyle=null; 
	
	private bool finished;
	private SelectableAction selectedAction;
	private Vector3 guiPosition;
	
	private string helpText;
	private string popupName;
	
	private static PopupMenuGUI instance;
		
	void Awake(){
		if(!gSkin) {
			Debug.LogError("PopupMenuGUI.InstanceOpenMenu(): no gSkin set");	
		}
		instance = this;
		enabled=false;
		finished = true;
	}

	public static void OpenMenu(ArrayList availableActions, string helpText, string objName){
		//Debug.Log("Opening popup menu");
		MouseHandler.UseDefaultCursor();
		instance.helpText = helpText;
		instance.InstanceOpenMenu(availableActions);
		instance.popupName=objName;
	}

	private void InstanceOpenMenu(ArrayList availableActions){
		finished = false;
		selectedAction = null;
				
		menuButtons = new ArrayList();
		
		
		if(PlayerPrefs.GetInt("mouseMove") == 1) {
			guiPosition = GUIGlobals.ScreenCoordToGUICoord(new Vector3(Screen.width/2, Input.mousePosition.y, Input.mousePosition.z));
		} else {
			guiPosition = GUIGlobals.ScreenCoordToGUICoord(Input.mousePosition);
		}
		
		MouseHandler.ReleaseCursorXPosition();
		
		MenuButton closeButton = new MenuButton(closeMenuIcon, null);
		menuPos = new Vector2(guiPosition.x, guiPosition.y);
		
		// Making sure that menu stays in the screen.
		if(menuPos.x - menuMinDistanceFromBorders.left < 0) {
			menuPos.x = menuMinDistanceFromBorders.left;
		}
		else if(menuPos.x + menuMinDistanceFromBorders.right > GUIGlobals.screenWidth)  {
			menuPos.x = GUIGlobals.screenWidth - menuMinDistanceFromBorders.right;
		}
		if(menuPos.y - menuMinDistanceFromBorders.top < 0) {
			menuPos.y = menuMinDistanceFromBorders.top;
		}
		else if(menuPos.y + menuMinDistanceFromBorders.bottom > GUIGlobals.screenHeight) {
			menuPos.y = GUIGlobals.screenHeight - menuMinDistanceFromBorders.bottom;
		}
		
		closeButton.SetPosition(menuPos.x, menuPos.y, menuItemDiameter);
		menuButtons.Add(closeButton);
		
		// Building the menu
		foreach (SelectableAction availableAction in availableActions){
			menuButtons.Add(new MenuButton(availableAction.GetIconSet(this), availableAction));
		}
		
		int nPositive = 1;
		int nNegative = 1;
		for (int i = 1; i < menuButtons.Count; i++){
			if(i%2==0) {
				float x=menuPos.x-(float)nNegative*(float)menuItemDiameter;
				float y=menuPos.y;
				nNegative++;
				((MenuButton)menuButtons[i]).SetPosition(x, y, menuItemDiameter);
			}
			else {
				float x=menuPos.x+(float)nPositive*(float)menuItemDiameter;
				float y=menuPos.y;
				nPositive++;
				((MenuButton)menuButtons[i]).SetPosition(x, y, menuItemDiameter);
			}
		}
		
		
		FModManager.StartEvent(FModLies.EVENTID_LIES_POPUPMENU_MENUOPEN);
	}
	
	public static void DrawGUI(){
		instance.InstanceDrawGUI();
	}
	
	private void InstanceDrawGUI(){
		GUI.matrix = GUIGlobals.GetGUIScaleMatrix();
		GUI.depth = 0;

		float menuButtonH = 0.0f;
		if (menuButtons != null && !finished){
			//GUI.Box(new Rect(menuPos.x-menuBGDiameter/2, menuPos.y-menuBGDiameter/2, menuBGDiameter, menuBGDiameter), "", bgStyle);
			MenuButton hover = null;		
			foreach (MenuButton menuButton in menuButtons){
				if(menuButtonH<1) {
					menuButtonH = menuButton.buttonRect.height;
				}
				if(menuButton.buttonRect.Contains(GUIGlobals.ScreenCoordToGUICoord(Input.mousePosition))) {
					// Cursor is on the top of this button, we'll put this aside for drawing tooltip below
					hover = menuButton;
				}
				if (menuButton.Draw()){
					finished = true;
					MouseHandler.LockCursorXPosition();
					selectedAction = menuButton.GetAction();
					FModManager.StartEvent(FModLies.EVENTID_LIES_POPUPMENU_ITEMCLICKED);
				}
			}
			
			GUIStyle tooltipStyle = gSkin.GetStyle("tooltip");
			// Info about object the popup orginated
			/*Vector2 s = tooltipStyle.CalcSize(new GUIContent("Testing") );
			float x =  ((MenuButton)menuButtons[0]).x-s.x/2;
			float y = ((MenuButton)menuButtons[0]).buttonRect.y - s.y - 10;
			GUI.Label(new Rect(x, y, s.x, s.y), "Testing", tooltipStyle);*/

			
			// Drawing help texts player wants them
			if(PlayerPrefs.GetInt("ToolTip") == 1) {
				GUI.skin = gSkin;
				
				// Help text for the menu.
				if(!helpText.Equals("")) {
					GUIContent help = new GUIContent(helpText);
					GUIStyle bgStyle=gSkin.GetStyle("helpBG");
					float height = bgStyle.CalcHeight(help, helpScreenWidth);// + bgStyle.padding.top + bgStyle.padding.bottom;
					GUILayout.BeginArea (new Rect(GUIGlobals.GetCenterY()-helpScreenWidth/2, helpScreenYOffset, helpScreenWidth, height), bgStyle);
					GUILayout.Label(help);
					GUILayout.EndArea(); 
				}
			
				// Drawing tooltip for menu item
			
				if(!popupName.Equals("")) {
					Vector2 popupNameSize = tooltipStyle.CalcSize(new GUIContent(popupName) );
					GUI.Label(new Rect(guiPosition.x-popupNameSize.x/2.0f, guiPosition.y-menuButtonH-5, popupNameSize.x, popupNameSize.y), popupName, tooltipStyle);
				
				}
			
				if(hover!=null) {
					//GUIStyle tooltipStyle = gSkin.GetStyle("tooltip");
					Vector2 tooltipSize = tooltipStyle.CalcSize(new GUIContent(hover.tooltip) );
					float tooltipX =  hover.x-tooltipSize.x/2;
					float tooltipY = hover.buttonRect.y+hover.buttonRect.height+10;
					GUI.Label(new Rect(tooltipX, tooltipY, tooltipSize.x, tooltipSize.y), hover.tooltip, tooltipStyle);
				}
			}
			
			
		}
		
	}
	
	public static bool IsFinished(){
		return instance.finished;
	}
	
	public static SelectableAction GetSelectedAction(){
		return instance.selectedAction;
	}
}

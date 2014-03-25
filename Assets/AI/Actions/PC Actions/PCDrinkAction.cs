using UnityEngine;
using System.Collections;

public class PCDrinkAction : SlowDrinkAction, SelectableAction {

	public PCDrinkAction(GameObject actor, string itemType) : base(actor, itemType, null, 2.0f, false){
	}
		
	public MenuIconSet GetIconSet(PopupMenuGUI popupMenuGUI){
		return popupMenuGUI.drinkIcon;
	}

}

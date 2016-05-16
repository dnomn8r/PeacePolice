using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComponentEditor : AbstractEditor {
	
	void OnPlacedRoomSelected(ShipRoomController roomController){
	
		GameObject componentListPopup = GameObject.Instantiate(Resources.Load("ComponentListPopup")) as GameObject;
		
		ComponentBrowser browser = componentListPopup.GetComponent<ComponentBrowser>();
			
		browser.SetSelectedRoom(roomController);
		
	}
	
}

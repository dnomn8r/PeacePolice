using UnityEngine;
using System.Collections;


public class RoomEditMenu : MonoBehaviour {

	public RoomEditor roomEditor;
		
	void OnGUIButtonClicked(GUIButton b){
	
		if(b.name == "fileButton"){
			
			GameObject fileDialogPopup = GameObject.Instantiate(Resources.Load("RoomEditorDialogPopup")) as GameObject;
		
			RoomEditorFileDialog dialog = fileDialogPopup.GetComponent<RoomEditorFileDialog>();

			dialog.OnSetRoomEditor(roomEditor, roomEditor.CurrentShipName);
			
		}
	}
	
	void OnHullSelected(HullDisplay display){
		
		roomEditor.SetHull(display.CurrentTexture);	
	}
	
	void OnShipRoomSelected(ShipRoomController roomController){
		
		roomEditor.OnSelectShipRoom(roomController, roomController.transform.localEulerAngles, roomController.transform.position);
	}
	
}

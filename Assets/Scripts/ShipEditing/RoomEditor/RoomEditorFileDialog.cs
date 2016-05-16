using UnityEngine;
using System.Collections;

using System.IO;

public class RoomEditorFileDialog : Popup{
	
	private RoomEditor shipEditor;
	
	public GUIButton saveButton;
	public GUIButton loadButton;
	
	private string currentShipFilename = "";
	
	public InputField inputField;
	
	public TextMesh statusText;
	
	private bool isShipValid = true;
	
	public void OnSetRoomEditor(RoomEditor editor, string filename){
		
		currentShipFilename = filename;
			
		shipEditor = editor;

		statusText.text = "";	
		isShipValid = true;

		if(editor.CurrentShipController != null){

			if(!editor.CurrentShipController.ship.AreRoomsValid()){
			
				statusText.text = "Cannot save, ship rooms are not all connected";
				isShipValid = false;
			
			}else if(!editor.CurrentShipController.ship.HasEngineRoom()){

				statusText.text = "Cannot save, ship has no engine room";
				isShipValid = false;
			}

		}
		
		updateButtons(filename);
	}
	
	protected override void Start(){
		
		base.Start();
		
		inputField.setText(currentShipFilename);
	}
	
	private void updateButtons(string newName){
		
		string fullFilename = RoomEditor.HULL_DIRECTORY + newName;
		
		//Debug.Log("new full filename: " + fullFilename);
		
		
		if(newName == "<new ship>" || !isShipValid){
		
			saveButton.DisableButton();
		}else{
			
			saveButton.EnableButton();
		}
		
		if(currentShipFilename == newName || !fileExists(fullFilename)){
			
			loadButton.DisableButton();
		}else{
			
			loadButton.EnableButton();
		}
	}
	
	public override void OnGUIButtonClicked(GUIButton b){
	
		base.OnGUIButtonClicked(b);
		
		if(b.name == "saveButton"){
			
			ShipFactory.SaveShip(shipEditor.CurrentShipController.ship, RoomEditor.HULL_DIRECTORY, currentShipFilename + ".hull");
		
			RemovePopup();
			
		}else if(b.name == "loadButton"){

			shipEditor.SetCurrentShip(ShipFactory.LoadShip(RoomEditor.HULL_DIRECTORY, currentShipFilename + ".hull"));	
		
			shipEditor.CurrentShipController.ShowLocations();
			
			RemovePopup();
			
		}else if(b.name == "newButton"){
			
			shipEditor.OnNewShip();
			
			RemovePopup();
		}
		
	}
	
	void OnInputFieldChanged(InputField f){
	
		updateButtons(f.Value);
		
		currentShipFilename = f.Value;
		
	}
	
	private bool fileExists(string filename){
	
		return File.Exists(filename + ".hull");
	}
	
}

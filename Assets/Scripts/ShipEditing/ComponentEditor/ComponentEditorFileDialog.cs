using UnityEngine;
using System.Collections;

using System.IO;

public class ComponentEditorFileDialog : Popup{
	
	private ComponentEditor shipEditor;
	
	public GUIButton saveButton;
	public GUIButton loadButton;
	
	private string currentShipFilename = "";
	
	private bool isShipValid = false;
	
	public InputField inputField;
	
	public TextMesh statusText;
	
	public void OnSetComponentEditor(ComponentEditor editor, string filename){

		currentShipFilename = filename;
			
		shipEditor = editor;
		
		if(editor.CurrentShipController != null){
		
			if(editor.CurrentShipController.ship.ComponentTypeCount("Bridge") != 1){
				
				statusText.text = "Cannot save, your ship needs exactly one bridge";
				isShipValid = false;
			
			}else if(editor.CurrentShipController.ship.ComponentTypeCount("Engine") == 0){
				
				statusText.text = "Cannot save, your ship needs at least one engine";
				isShipValid = false;
			
			}else{
				
				isShipValid = true;
				statusText.text = "";
			}
		}else{
			
			isShipValid = false;	
			statusText.text = "Cannot save, you have no current ship";	
		}
		
		updateButtons(filename);
	}
	
	protected override void Start(){
		
		base.Start();
		
		inputField.setText(currentShipFilename);
		
	}
	
	private void updateButtons(string newName){
		
		string fullFilename = RoomEditor.SHIP_DIRECTORY + newName;
		
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
			
			ShipFactory.SaveShip(shipEditor.CurrentShipController.ship, RoomEditor.SHIP_DIRECTORY, currentShipFilename + ".ship");
		
			RemovePopup();
			
		}else if(b.name == "loadButton"){
			
			shipEditor.SetCurrentShip(ShipFactory.LoadShip(RoomEditor.SHIP_DIRECTORY, currentShipFilename + ".ship"));	
		
			shipEditor.CurrentShipController.ShowLocations();
			
			shipEditor.CurrentShipController.CleanupDoors();
			
			RemovePopup();
			
		}
		
	}
	
	void OnInputFieldChanged(InputField f){
	
		updateButtons(f.Value);
		
		currentShipFilename = f.Value;
		
	}
	
	private bool fileExists(string filename){
	
		return File.Exists(filename + ".ship");
	}
	
}

using UnityEngine;
using System.Collections;

using System.IO;

public class HullLoadPopup : Popup{
	
	private ComponentEditor componentEditor;
	
	public GUIButton loadButton;
	
	private string currentShipFilename = "";

	public InputField inputField;
	
	public void OnSetComponentEditor(ComponentEditor editor){
		
		componentEditor = editor;
	}
	
	protected override void Start(){
		
		base.Start();
		
		inputField.setText(currentShipFilename);
		
		updateButtons(currentShipFilename);
	}
	
	private void updateButtons(string newName){
		
		string fullFilename = ComponentEditor.HULL_DIRECTORY + newName;
		
		if(!fileExists(fullFilename)){
			
			loadButton.DisableButton();
		}else{
			
			loadButton.EnableButton();
		}
	}
	
	public override void OnGUIButtonClicked(GUIButton b){
	
		base.OnGUIButtonClicked(b);

		if(b.name == "loadButton"){
			
			componentEditor.SetCurrentShip(ShipFactory.LoadShip(RoomEditor.HULL_DIRECTORY, currentShipFilename));
		
			componentEditor.CurrentShipController.ShowLocations();
			
			componentEditor.CurrentShipController.CleanupDoors();
			
			RemovePopup();
		}

	}
	
	void OnInputFieldChanged(InputField f){
		
		currentShipFilename = f.Value + ".hull";
		
		updateButtons(currentShipFilename);
	}
	
	private bool fileExists(string filename){
	
		return File.Exists(filename);
	}
	
}

using UnityEngine;
using System.Collections;


public class ComponentEditMenu : MonoBehaviour {

	public ComponentEditor componentEditor;
		
	void OnGUIButtonClicked(GUIButton b){
	
		if(b.name == "fileDialog"){
			
			GameObject fileDialogPopup = GameObject.Instantiate(Resources.Load("ComponentEditorDialogPopup")) as GameObject;
		
			ComponentEditorFileDialog dialog = fileDialogPopup.GetComponent<ComponentEditorFileDialog>();
			
			dialog.OnSetComponentEditor(componentEditor, componentEditor.CurrentShipName);
			
		}else if(b.name == "loadHullButton"){
			
			GameObject fileDialogPopup = GameObject.Instantiate(Resources.Load("HullLoadPopup")) as GameObject;
		
			HullLoadPopup dialog = fileDialogPopup.GetComponent<HullLoadPopup>();
			
			dialog.OnSetComponentEditor(componentEditor);
			
		}
	}
	
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIControllerMouse : GUIController{
		
	protected void Update(){
		
		GUIObject touchedObject = null;
			
		if(Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)){
		
			touchedObject = GetSelectedGUIObject(Input.mousePosition);
		
		}else{
		
			releaseGUIObject();
		}
		
		// first, check if we have a new active GUI object
		if(Input.GetMouseButtonDown(0)){
			
			if(touchedObject != null && activeGUIObject == null){
				
				activeGUIObject = touchedObject;
				activeGUIObject.OnTouched(); 			
			}
		
		}else{ // if we've got an active object, we check if we're still touching it, or if we just let go of it, or moused away from it
		
			if(activeGUIObject != null){
				
				if(touchedObject == activeGUIObject){ // if we're touching the active object
					
					if(Input.GetMouseButtonUp(0)){
						
						if(touchedObject == activeGUIObject){
							
							activeGUIObject.OnClicked();	//if the originally pressed button is the one we released on, it sends a click command to it
							
							activeGUIObject = null;
						}
					}else{ // otherwise, just let the button know it's now being touched
						
						activeGUIObject.OnTouched();
					}
				}else{ // the active button is now not moused over anymore
					
					activeGUIObject.OnReleased();
				}
			}
		}
	}
	
}
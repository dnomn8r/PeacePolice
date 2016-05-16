using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIControllerTouch : GUIController{

	private Touch getActiveTouch(int fingerID){
		
		for(int i = 0; i < Input.touchCount; ++i){ 
		
			if(Input.GetTouch(i).fingerId == fingerID){
				
				return Input.GetTouch(i);
			}
		}
		
		Debug.LogError("should never get to here!");
		return new Touch();
	}
	
	private GUIObject getObjectFromFingerTouch(){ // get the object we are touching, but with a preference to the active object if multiple touches
		
		GUIObject touchedObject = null;
		
		for(int i = 0; i < Input.touchCount; ++i){ 
		
			Touch currentTouch = Input.GetTouch(i);
			
			touchedObject = GetSelectedGUIObject(currentTouch.position);
			
			if(currentTouch.phase == TouchPhase.Began){
				
				if(touchedObject != null && activeGUIObject == null){ // if we've clicked on a gui object for the first time, no need to check the rest of the touches
										
					activeGUIObject = touchedObject;
					activeGUIObject.OnTouched();
					
					return touchedObject;
				}
			}else if(activeGUIObject != null && touchedObject == activeGUIObject){ // if we've got a touch on our active object, that's the one we care about, ignore the rest
				
				return touchedObject;
			}
			
		}

		return null;
	}
	
}
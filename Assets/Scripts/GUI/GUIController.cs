using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GUIController : MonoBehaviour{
	
	protected GUIObject activeGUIObject = null;
	
	private static GUIController instance = null;
	
	//check to see if there's already an instance of the guicontroller in the scene
	public static GUIController Instance{ 
		
		get{
			if(instance == null){
				
				GameObject guiController = new GameObject(); //create the empty gameobject
				guiController.name = "CPX_GUIController(Auto Generated)"; //rename the gameobject to our default name
				
				if(Input.mousePresent){
				
					instance = guiController.AddComponent<GUIControllerMouse>();
				}else{
					
					instance = guiController.AddComponent<GUIControllerTouch>();
				}
			}
			
			return instance; //return the instance if it exists prior, or the new one we just created
		}
	}
		
	public void Activate(){
		
		enabled = true;
		releaseGUIObject();		
	}
	
	public void Deactivate(){
		
		enabled = false;	
		releaseGUIObject();
	}

	protected void releaseGUIObject(){
		
		if(activeGUIObject != null){
			
			activeGUIObject.OnReleased(); //button is released, but not clicked
			activeGUIObject = null;
		}
	}
	
	protected GUIObject GetSelectedGUIObject(Vector2 selectionPos){
		
		List<Camera> sortedCameras = new List<Camera>();
			
		foreach(Camera cam in Camera.allCameras){
			
			if(cam.enabled && cam.gameObject.activeInHierarchy){ // ignore disabled cameras
				
				sortedCameras.Add(cam);
			}
		}
		
		sortedCameras.Sort(delegate (Camera c1, Camera c2){ 
			
			if(c1.depth > c2.depth) return -1;
			else if(c1.depth < c2.depth) return 1;
		
			return 0;
		});
		
		foreach(Camera cam in sortedCameras){ //filter through the cameras in our list from top to bottom, taking the highest level camera as top priority
			
			Ray ray = cam.ScreenPointToRay(selectionPos);
		
			RaycastHit hit;
			
			if(Physics.Raycast(ray.origin, ray.direction, out hit, cam.farClipPlane, cam.cullingMask)){ // if the raycast hits something, check if it's a gui object
				
				GUIObject obj = hit.transform.GetComponent<GUIObject>();
			
				if(obj != null){
				
					return obj;	
				}
			}
		}
		
		return null;
	}
}
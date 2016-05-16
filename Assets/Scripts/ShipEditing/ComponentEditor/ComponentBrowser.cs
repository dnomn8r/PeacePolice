using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ComponentBrowser : Popup{
	
	private int currentPage = 0;
	
	private int maxPages = 0;
	
	public GameObject componentDisplayPrefab;
	
	public TextMesh statusText;
	
	public Transform[] componentDisplays;
	
	private ShipRoomController selectedRoomController;
	
	private List<GameObject> currentDisplays = new List<GameObject>();
	
	private List<ShipComponentData> availableComponents;
	
	public void SetSelectedRoom(ShipRoomController roomController){
		
		selectedRoomController = roomController;
		
		availableComponents = filterComponents(selectedRoomController.room);
		
		if(componentDisplays.Length > 0 && availableComponents.Count > 0){
		
			maxPages = Mathf.CeilToInt((availableComponents.Count) / (float)componentDisplays.Length);
		}else{
			
			maxPages = 0;	
		}
		
		refreshDisplay();
	}
	
	private List<ShipComponentData> filterComponents(ShipRoom room){
		
		List<ShipComponentData> filteredComponents = new List<ShipComponentData>();
		
		foreach(ShipComponentData data in Definitions.Instance.Components){
		
			if(data.placementRestrictions.Count > 0){
				
				foreach(ShipLocation placementRestriction in data.placementRestrictions){
				
					if(placementRestriction == room.Location){
						
						filteredComponents.Add(data);
						break;
					}
				}
			}else{ // if no restrictions, we can add it no problem if it fits our room
				
				if(data.size == room.roomData.size){
				
					filteredComponents.Add(data);
				}
			}
		}
		
		return filteredComponents;
	}

	private void refreshDisplay(){
		
		foreach(GameObject display in currentDisplays){
			
			Destroy(display);	
		}
	
		currentDisplays.Clear();
		
		
		if(maxPages == 0){
			
			statusText.text = "0/0";	
		
			return;
		}
		
		statusText.text = (currentPage+1) + "/" + maxPages;
		
		for(int i=0;i<componentDisplays.Length;++i){
			
			int targetIndex = (currentPage*componentDisplays.Length + i);
			
			if(targetIndex < availableComponents.Count){
			
				GameObject newComponentDisplay = GameObject.Instantiate(componentDisplayPrefab) as GameObject;
				
				newComponentDisplay.transform.parent = componentDisplays[i];
				newComponentDisplay.transform.localScale = Vector3.one;
				newComponentDisplay.transform.localPosition = Vector3.zero;
				newComponentDisplay.transform.localEulerAngles = Vector3.zero;
				
				ComponentDisplay display = newComponentDisplay.GetComponent<ComponentDisplay>();
				
				display.SetComponentData(availableComponents[targetIndex]);

				currentDisplays.Add(display.gameObject);

			}else{
			
				foreach(Transform child in componentDisplays[i]){
					
					Destroy(child.gameObject);
				}
			}
		}
	}
	
	private void setPage(int newPage){
		
		int previousPage = currentPage;
		
		if(newPage >= maxPages){
		
			newPage = 0;
			
		}else if(newPage < 0){
			
			newPage = (maxPages-1);
		}
		
		currentPage = newPage;
		
		if(currentPage != previousPage){
		
			refreshDisplay();
		}
	}
	
	
	public void OnGUIButtonPressed(GUIButton b){
		
		if(b.name == "nextButton"){
		
			setPage(currentPage+1);
			
		}else if(b.name == "previousButton"){
			
			setPage(currentPage-1);
		
		}else if(b.name == "removeButton"){
			
			if(selectedRoomController != null){
			
				selectedRoomController.RemoveComponent();
				
				PopupManager.Instance.OnRemovePopup(this.gameObject);
				
			}else{
			
				Debug.LogError("no room selected, this shouldn't happen");
			}
		}
	}
	
	
	void OnComponentSelected(ShipComponentData data){
		
		//Debug.Log("component selected: " + data.title);	
		
		if(selectedRoomController != null){

			selectedRoomController.SetComponent(new ShipComponent(selectedRoomController.room, data));
			
			PopupManager.Instance.OnRemovePopup(this.gameObject);
			
		}else{
		
			Debug.LogError("no room selected, this shouldn't happen");
		}
		
	}
	
}

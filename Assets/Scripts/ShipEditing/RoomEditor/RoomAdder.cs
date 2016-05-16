using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RoomAdder : MonoBehaviour {
	
	private ShipRoomController currentRoomController;
	
	private Transform myTransform;
	
	private Camera selectionCamera;
	
	private Collider hullCollider;
	
	private RoomEditor shipEditor;
	
	private bool isValid = false;
	
	private bool isOnShip = false;
	
	private ShipDoor[] doors;
	
	private List<DoorPair> currentCollidingDoorsWithDoors = new List<DoorPair>();
	
	private List<Collider> currentRoomCollisions = new List<Collider>();
	
	void Awake(){
		
		myTransform = transform;
		
		enabled = false;
	}
	
	public void AddRoom(RoomEditor editor, ShipRoomController roomController, Camera cam, Collider col){
	
		shipEditor = editor;
		selectionCamera = cam;
		hullCollider = col;
		
		currentRoomController = roomController;
		
		doors = GetComponentsInChildren<ShipDoor>();
		
		currentRoomController.OnToggleCollider(true);
		
		toggleLongRoomColliders(true);
			
		updateHighlights(!editor.CurrentShipController.ship.HasRooms);
		
		enabled = true;
	}
	
	private void toggleLongRoomColliders(bool expand){
		
		BoxCollider roomCollider = currentRoomController.Collider as BoxCollider;
		
		Vector3 roomSize = roomCollider.size;
		
		roomSize.z = expand ? 100.0f : 1.0f;
		
		roomCollider.size = roomSize;
		
		foreach(ShipDoor door in doors){
		
			BoxCollider doorCollider = door.GetComponent<BoxCollider>();
			
			Vector3 doorSize = doorCollider.size;
			
			doorSize.z = expand ? 100.0f : 1.0f;
			
			doorCollider.size = doorSize;
			
		}
	}
	
	void Update(){
	
		if(hullCollider == null){
			
			Destroy(this.gameObject);	
		}
		
		if(Input.GetMouseButtonUp(0)){
			
			if(isValid && isOnShip){
			
				placePiece();
			}
		}else if(Input.GetMouseButtonDown(1)){
		
			myTransform.Rotate(new Vector3(0,0,-90));
			
			currentRoomController.DisplayLocation();
			
		}else if(Input.GetKeyDown(KeyCode.Escape)){
			
			Destroy(this.gameObject);
		
		}else if(Input.GetKeyDown(KeyCode.E)){
		
			if(currentRoomController.room.Location == ShipLocation.AFT){
				
				currentRoomController.SetLocation(ShipLocation.CENTER);
			}else{
				
				currentRoomController.SetLocation(ShipLocation.AFT);
			}
			
		}else{
			
			// first, move us anywhere the mouse is, but if we hit the collider, we move to a specific spot
			// so all rooms are aligned
			Vector3 position = selectionCamera.ScreenToWorldPoint(Input.mousePosition);
			
			position.z += 50;
			
			myTransform.position = position;
			
			
			Ray screenRay = selectionCamera.ScreenPointToRay(Input.mousePosition);
			
			RaycastHit hit;
			
			if(hullCollider.Raycast(screenRay, out hit, selectionCamera.farClipPlane)){
	
				isOnShip = true;
				
				Vector3 pos = hit.point;
				
				pos.z -= 50.0f;
				
				myTransform.position = pos;
				
			}
		}
		
	}
	
	private void updateHighlights(bool validity){

		isValid = validity;
		
		currentRoomController.OnHighlightRoom(isValid, Color.green);
		
		foreach(DoorPair pair in currentCollidingDoorsWithDoors){
			
			pair.door2.ShipRoomController.OnHighlightRoom(isValid, new Color(0.4f,0.4f,0.4f));
		}

	}
	
	private void placePiece(){
		
		Vector3 pos = myTransform.position;
		pos.z += 20.0f;
		myTransform.position = pos;
		
		toggleLongRoomColliders(false);
		
		updateHighlights(false);
		
		currentRoomController.OnPlaceInShip();
		
		shipEditor.OnAddPiece(currentRoomController, currentCollidingDoorsWithDoors);
		
		Destroy(this);
	}
	
	void OnDoorHitDoor(DoorPair newPair){
		
		foreach(DoorPair pair in currentCollidingDoorsWithDoors){
			
			if(pair.door1 == newPair.door1 && pair.door2 == newPair.door2){
				
				Debug.LogError("pair already exists!");
				return;
			}
		}

		currentCollidingDoorsWithDoors.Add(newPair);
		
		checkValidity();
	}
	
	void OnDoorLeaveDoor(DoorPair newPair){
		
		foreach(DoorPair pair in currentCollidingDoorsWithDoors){
			
			if(pair.door1 == newPair.door1 && pair.door2 == newPair.door2){
				
				currentCollidingDoorsWithDoors.Remove(pair);
				
				pair.door2.ShipRoomController.OnHighlightRoom(false, Color.white);
				
				checkValidity();
				
				return;
			}
		}
		
		Debug.LogError("trying to remove door that doesn't exist");
	}
	
	void OnRoomHitCollider(Collider c){
		
		if(currentRoomCollisions.Contains(c)){
		
			Debug.LogError("trying to add a collider twice!");
		}else{
			
			currentRoomCollisions.Add(c);	
		}
		
		checkValidity();
	}
	
	void OnRoomLeaveCollider(Collider c){
		
		if(currentRoomCollisions.Contains(c)){
		
			currentRoomCollisions.Remove(c);	
		}else{
			
			Debug.LogError("trying to remove a collider which wasn't added");	
		}
		
		checkValidity();
	}
	
	private void checkValidity(){
		
		if(currentRoomCollisions.Count == 0 && 
			currentCollidingDoorsWithDoors.Count > 0){
			
			updateHighlights(true);	
		}else{
			
			updateHighlights(false);	
		}
		
	}
	
	void OnGUI(){

		GUI.Label(new Rect(10, 10, 200, 20), "Door to door: " + currentCollidingDoorsWithDoors.Count);
		GUI.Label(new Rect(10, 30, 200, 20), "Room Collisions: " + currentRoomCollisions.Count);
		GUI.Label(new Rect(10, 50, 200, 20), "Valid: " + isValid);
	}
	
}




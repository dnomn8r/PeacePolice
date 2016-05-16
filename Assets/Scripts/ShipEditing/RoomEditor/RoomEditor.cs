using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomEditor : AbstractEditor {
	
	private RoomAdder currentRoomAdder = null;
	
	public RoomEditMenu roomEditMenu;
	
	
	public void OnSelectShipRoom(ShipRoomController roomController, Vector3 rotation, Vector3 position){
		
		if(currentShipController != null){

			if(currentRoomAdder != null){
				
				Destroy(currentRoomAdder.gameObject);	
			}
			
			GameObject newRoom = GameObject.Instantiate(roomController.gameObject) as GameObject;
			
			newRoom.transform.localPosition = position;
			newRoom.transform.localEulerAngles = rotation;
			newRoom.transform.localScale = Vector3.one;

			newRoom.transform.parent = transform;
			newRoom.name = roomController.name;
			
			currentRoomAdder = newRoom.AddComponent<RoomAdder>();

			ShipRoomController newShipRoomController = newRoom.GetComponent<ShipRoomController>();

			newShipRoomController.room = new ShipRoom(roomController.room);

			currentRoomAdder.AddRoom(this, newShipRoomController, editorCamera, currentShipController.HullCollider);

		}else{
			
			Debug.LogWarning("we have no selected ship");	
		}
	}
	
	protected override void createNewShip(){
	
		if(currentRoomAdder != null){
			
			Destroy(currentRoomAdder.gameObject);	
		}
		
		base.createNewShip();
	}
	
	public void SetHull(Texture2D tex){
		
		if(currentShipController != null){
			
			currentShipController.SetHull(tex);
		}else{
			
			Debug.LogWarning("can't set the hull on a non-existent ship");	
		}
		
	}
	
	public void OnAddPiece(ShipRoomController roomController, List<DoorPair> collidingDoors){
		
		if(currentShipController != null){
			
			Vector3 totalDifference = Vector3.zero;
			
			foreach(DoorPair pair in collidingDoors){
				
				totalDifference += (pair.door2.transform.position - pair.door1.transform.position);	
			}

			if(collidingDoors.Count > 0){
			
				roomController.transform.position += totalDifference / collidingDoors.Count;
			}

			roomController.transform.parent = currentShipController.transform;

			roomController.room.position = new XnaGeometry.Vector3(
																	(double)roomController.transform.localPosition.x, 
																	(double)roomController.transform.localPosition.y, 
																	(double)roomController.transform.localPosition.z);

			roomController.room.rotation = roomController.transform.localEulerAngles.z;

			roomController.ShipController = currentShipController;

			foreach(DoorPair pair in collidingDoors){

				pair.door1.ShipRoomController.room.AddConnection(
					new Connection(pair.door1.id, pair.door2.ShipRoomController.room, 

				               new XnaGeometry.Vector3((float)pair.door1.transform.localPosition.x, 
				                        				(float)pair.door1.transform.localPosition.y, 
				                        				(float)pair.door1.transform.localPosition.z)));
				
				pair.door2.ShipRoomController.room.AddConnection(
					new Connection(pair.door2.id, pair.door1.ShipRoomController.room, 
				               new XnaGeometry.Vector3((float)pair.door2.transform.localPosition.x,
				                        				(float)pair.door2.transform.localPosition.y,
				                        				(float)pair.door2.transform.localPosition.z)));
			}

			currentShipController.ship.AddRoom(roomController.room);
		
		}else{
			
			Debug.LogWarning("can't add a piece to a non-existent ship");	
		}
	}
	
	public void OnPlacedRoomSelected(ShipRoomController roomController){
		
		if(currentShipController != null){
			
			currentShipController.ship.Rooms.Remove(roomController.room);
		
			OnSelectShipRoom(roomController, roomController.transform.localEulerAngles, roomController.transform.position);
		
			roomController.room.ClearConnections();
			
			Destroy(roomController.gameObject);
			
		}else{
			
			Debug.LogWarning("no ship selected");	
		}
	}
	
	
}

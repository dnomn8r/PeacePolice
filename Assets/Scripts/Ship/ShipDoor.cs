using UnityEngine;
using System.Collections;

public struct DoorPair{
	
	public ShipDoor door1;
	public ShipDoor door2;
	
	public DoorPair(ShipDoor firstDoor, ShipDoor secondDoor){
		
		door1 = firstDoor;
		door2 = secondDoor;
	}
}


public class ShipDoor : MonoBehaviour {

	public int id;
	
	private ShipRoomController shipRoomController;
	
	public ShipRoomController ShipRoomController{
		get{
			return shipRoomController;	
		}
	}
	
	void Awake(){
	
		shipRoomController = transform.parent.GetComponent<ShipRoomController>();
	}
	
	void OnTriggerEnter(Collider c){
		
		ShipDoor door = c.GetComponent<ShipDoor>();
		
		if(door != null){

			SendMessageUpwards("OnDoorHitDoor", new DoorPair(this, door), SendMessageOptions.DontRequireReceiver);
		}
	}

	void OnTriggerExit(Collider c){
		
		ShipDoor door = c.GetComponent<ShipDoor>();
		
		if(door != null){
		
			SendMessageUpwards("OnDoorLeaveDoor", new DoorPair(this, door), SendMessageOptions.DontRequireReceiver);
		}
	}
	
}

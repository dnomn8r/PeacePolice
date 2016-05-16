using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PeaceServerClientCommon;

public class ShipDisplay : MonoBehaviour {

	public Renderer hullRenderer;

	public Ship ship;

	private Transform myTransform;
	private Collider hullCollider;

	private List<ShipRoomController> roomControllers = new List<ShipRoomController>();

	private Dictionary<int, ShipRoomController> roomControllerMap = new Dictionary<int, ShipRoomController>();

	public List<ShipRoomController> RoomControllers{

		get{return roomControllers;}
	}

	void Awake(){
		
		myTransform = transform;
		
		hullCollider = hullRenderer.GetComponent<Collider>();
	}

	public ShipRoomController GetRoomController(int roomID){

		if(roomControllerMap.ContainsKey(roomID)){

			return roomControllerMap[roomID];
		}

		return null;
	}

	public Collider HullCollider{
		
		get{return hullCollider;}	
	}

	public void ClearHighlights(){

		foreach(ShipRoomController controller in roomControllers){

			controller.SetHighlight(HighlightType.NONE);
		}
	}

	public void HighlightActivatedRooms(List<CombatAction> combatActions){

		ClearHighlights();

		foreach(CombatAction combatAction in combatActions){

			if(combatAction.playerID == ship.OwnerID){

				foreach(ShipRoomController currentRoomController in roomControllers){

					if(currentRoomController.room.ID == combatAction.roomID){

						currentRoomController.SetHighlight(HighlightType.ACTIVATED);
						break;
					}
				}
			}
		}
	}

	public void ShowLocations(){

		foreach(ShipRoomController currentRoomController in roomControllers){
			
			currentRoomController.DisplayLocation();
		}
	}

	public void CleanupDoors(){

		if(ship == null) return;

		foreach(ShipRoomController currentRoomController in roomControllers){
		
			List<ShipDoor> doorsToRemove = new List<ShipDoor>();
			
			foreach(ShipDoor door in currentRoomController.Doors){
				
				Destroy(door.GetComponent<Collider>()); // remove the colliders, we don't need them here
				
				bool isConnected = false;
			
				foreach(Connection currentConnection in currentRoomController.room.connections){
				
					if(currentConnection.doorID == door.id){
					
						isConnected = true;
						break;
					}
				}
				
				if(!isConnected){
				
					doorsToRemove.Add(door);	
				}
			}

			foreach(ShipDoor door in doorsToRemove){
				
				currentRoomController.RemoveDoor(door);	
			}
		}
	}

	public void SetShip(Ship ship){

		this.ship = ship;

		roomControllers.Clear();

		Debug.Log("ship hull name: " + ship.HullName);

		SetHull(Resources.Load("ShipHulls/" + ship.HullName) as Texture2D);

		foreach(ShipRoom room in ship.Rooms){

			GameObject 
				roomPrefab = Resources.Load("ShipRooms" +
				"/" + room.roomData.name) as GameObject;

			GameObject newRoomPrefab = GameObject.Instantiate(roomPrefab) as GameObject;
			newRoomPrefab.transform.parent = myTransform;
			newRoomPrefab.transform.localPosition = new Vector3((float)room.position.X, (float)room.position.Y, (float)room.position.Z);
			newRoomPrefab.transform.localEulerAngles = new Vector3(0,0,room.rotation);
			newRoomPrefab.name = room.roomData.name + " ID: " + room.ID;

			ShipRoomController roomController = newRoomPrefab.GetComponent<ShipRoomController>();
			roomController.SetRoom(room);
			roomController.OnPlaceInShip();

			roomController.SetLocation(room.Location);
			roomController.ShipController = this;
			roomControllers.Add(roomController);

			roomControllerMap.Add(room.ID, roomController);
		
			if(room.ComponentName != ""){

				roomController.SetComponent(room.CurrentComponent);
			}
				
			newRoomPrefab.transform.parent = myTransform;
		}
	}


	public void SetHull(Texture2D hull){
		
		if(hullRenderer != null && ship != null){
			
			ship.HullName = hull.name;
			
			hullRenderer.transform.localScale = new Vector3(hull.width, hull.height, 1);
			
			hullRenderer.material.mainTexture = hull;
		}else{
			
			Debug.LogError("ship has no hull renderer!");	
		}
	}


	public static ShipDisplay CreateNewShip(){
		
		GameObject shipBase = GameObject.Instantiate(Resources.Load("Ship/Base")) as GameObject;
		
		shipBase.transform.position = Vector3.zero;
		shipBase.transform.eulerAngles = Vector3.zero;
		shipBase.transform.localScale = Vector3.one;

		ShipDisplay controller = shipBase.GetComponent<ShipDisplay>();

		controller.ship = new Ship();

		return controller;
	}
	
}



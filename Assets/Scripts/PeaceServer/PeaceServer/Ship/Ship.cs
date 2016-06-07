using System;
using System.Collections;
using System.Collections.Generic;
using XnaGeometry;


public class RoomDoorData{
	
	public int id;
	public Vector3 position;
	
}

public class ShipRoomData{
	
	public string name;
	public string size;
	
	public List<RoomDoorData> doors;
	
	public override string ToString(){
		
		return "Room Name: " + name + " size: " + size; 
	}
}

public class ShipComponentData{
	
	public string identifier;
	
	public string prefab;
	
	public string size;
	
	public string title;
	
	public string desc;
	
	public int system;
	public int armour;
	public int energy = 0;


	public List<ShipLocation> placementRestrictions = new List<ShipLocation>();
	
	public List<AbilityData> abilities;
	
	/*
	public string size{
		
		set{
			
			roomSize = (RoomSize)System.Enum.Parse(typeof(RoomSize), value);
		}
	}
	*/
	
	public List<string> restrictions{
		
		set{
			
			placementRestrictions.Clear();
			
			foreach(string restriction in value){
				
				placementRestrictions.Add((ShipLocation)System.Enum.Parse(typeof(ShipLocation), restriction));	
			}
		}
		
	}
	
	public override string ToString(){
		
		string result = "";
		
		result += "Identifier: " + identifier + "\n";
		result += "Prefab: " + prefab + "\n";
		result += "Size: " + size + "\n";
		result += "Title: " + title + "\n";
		result += "Description: " + desc + "\n";
		result += "System HP: " + system + "\n";
		result += "Armour HP: " + armour + "\n";
		result += "Energy: " + energy + "\n";
		
		result += "Restrictions: ";
		
		foreach(ShipLocation restriction in placementRestrictions){
			
			result += restriction.ToString() + ",";	
		}
		
		result += "\nAbilities: ";
		
		foreach(AbilityData ability in abilities){
			
			result += ability.ToString();	
		}
		
		return result;
	}	
	
	public bool HasAbilityType(string type){
		
		foreach(AbilityData ability in abilities){
			
			if(ability.type == type) return true;
		}
		
		return false;
	}
}


public class ConnectionEntry{
	public int roomID;
	public int doorID;
	public Vector3 position;
}

public class RoomEntry{

	public string name;
	public int roomID;
	
	public Vector3 position;
	public float rotation;
	
	public string location;
	
	public List<ConnectionEntry> connections = new List<ConnectionEntry>();
	
	public string componentName;
	
}

public class ShipData{
	
	public string shipName = "";
	
	public string hullName;	
	
	public List<RoomEntry> roomEntries = new List<RoomEntry>();
}


public class Ship{

	private ShipData shipData;
	
	private List<ShipRoom> roomList = new List<ShipRoom>();

	private string currentHullName = "";	
	private string currentShipName = "";

	private string ownerID;

	private int currentThrust = 0;

	public string OwnerID{
		get{return ownerID;}
	}

	public string HullName{

		get{return currentHullName;}
		set{currentHullName = value;}
	}

	public string ShipName{
		
		get{return currentShipName;}

		set{currentShipName = value;}
	}

	public void AdvanceTurn(){

		foreach(ShipRoom room in roomList){

			if(room.CurrentComponent == null) continue;

			List<ShipComponentStatus> expiredStatuses = new List<ShipComponentStatus>();

			foreach(ShipComponentStatus status in room.CurrentComponent.Statuses){

				status.turnsLeft--;

				if(status.turnsLeft == 0){

					Console.WriteLine("status: " + status.statusType + " has expired on room: " + room.ID);
					expiredStatuses.Add(status);
				}
			}

			foreach(ShipComponentStatus expiredStatus in expiredStatuses){
				
				room.CurrentComponent.Statuses.Remove(expiredStatus);
			}
		}
	}

	public ShipRoom GetRandomRoom(){

		if(roomList.Count > 0){

			return roomList[RandomNumber.Range(0, roomList.Count-1)];
		}else{
			return null;
		}
	}

	public List<ShipRoom> Rooms{
		
		get{return roomList;}
	}

	public Ship(){

		shipData = null;

		Console.WriteLine("using default ship constuector");
	}

	public Ship(ShipData data){

		LoadShip(data);

		Console.WriteLine("ship consutrctor with no plauyr");
	}

	public Ship(ShipData data, string playerID){
		
		LoadShip(data);

		Console.WriteLine("setting owner id to: " + playerID);
		ownerID = playerID;

		Console.WriteLine("owner id right after: " + OwnerID);
	}
	
	public bool HasRooms{
		
		get{return roomList.Count > 0;}	
	}
	
	public ShipRoom GetRoom(int id){

		foreach(ShipRoom room in roomList){

			if(room.ID == id) return room;
		}

		Console.WriteLine("could not find room with id: " + id);
		return null;
	}

	public void AddRoom(ShipRoom room){
		
		roomList.Add(room);

		room.Ship = this;

	}

	public List<Shield> GetShieldsProtectingRoom(ShipRoom targetedRoom){

		List<Shield> shields = new List<Shield>();

		foreach(ShipRoom room in roomList){

			if(room.CurrentComponent == null) continue;

			foreach(ComponentAbility ability in room.CurrentComponent.Abilities){

				Shield shield = ability as Shield;

				if(shield != null){
				
					Console.WriteLine("shield strenght is: " + shield.CurrentStrength);

					if(shield.CurrentStrength > 0){

						Console.WriteLine("distance is: " + Vector3.Distance(room.position, targetedRoom.position) + 
						                  " and range: " + shield.CurrentRange);

						if(Vector3.Distance(room.position, targetedRoom.position) <= shield.CurrentRange){

							Console.WriteLine("adding shield!!!!");
							shields.Add(shield);

						}
					}
				}
			}
		}
			
		return shields;
	}

	
	public void LoadShip(ShipData data){
		
		this.shipData = data;

		currentShipName = shipData.shipName;

		currentHullName = shipData.hullName;

		Dictionary<int, ShipRoom> roomDictionary = new Dictionary<int, ShipRoom>();

		foreach(RoomEntry entry in shipData.roomEntries){

			ShipRoom newRoom = new ShipRoom(this, Definitions.Instance.GetRoomByID(entry.name), entry.roomID);
			newRoom.position = entry.position;
			newRoom.rotation = entry.rotation;
			newRoom.Location = (ShipLocation)System.Enum.Parse(typeof(ShipLocation),entry.location);

			roomList.Add(newRoom);
			
			roomDictionary.Add(newRoom.ID, newRoom);
		}
		
		// now that rooms are loaded, make connections
		foreach(RoomEntry entry in shipData.roomEntries){
		
			ShipRoom currentRoom = roomDictionary[entry.roomID];

			if(entry.componentName != ""){
			
				currentRoom.SetComponent(new ShipComponent(currentRoom, Definitions.Instance.GetComponentByID(entry.componentName)));
			}

			foreach(ConnectionEntry connectionEntry in entry.connections){

				ShipRoom targetRoom = roomDictionary[connectionEntry.roomID];

				Connection newConnection = new Connection(connectionEntry.doorID, targetRoom, connectionEntry.position);

				currentRoom.AddConnection(newConnection);
			}			
		}

		ActivatePassiveAbilities();

	}
	
	public void ActivatePassiveAbilities(){

		foreach(ShipRoom currentRoom in roomList){
			
			currentRoom.ActivatePassiveAbilities();
		}
	}

	public ShipData SaveShip(){
		
		int currentRoomID = 0;
		
		// first, let's give each room an ID, we need to do this to do connections properly, it can be arbitrary
		
		foreach(ShipRoom s in roomList){
			
			s.ID = currentRoomID++;
		}
		
		ShipData shipData = new ShipData();
		
		shipData.shipName = currentShipName.Replace(".hull", "");
		shipData.hullName = currentHullName;
		
		// now we can go and properly do the connections
		foreach(ShipRoom currentRoom in roomList){
			
			RoomEntry newEntry = new RoomEntry();
			newEntry.roomID = currentRoom.ID;
			newEntry.name = currentRoom.roomData.name;

			newEntry.position = currentRoom.position;
			newEntry.rotation = currentRoom.rotation;

			newEntry.location = currentRoom.Location.ToString();
			
			newEntry.componentName = currentRoom.ComponentName;
			
			foreach(Connection currentConnection in currentRoom.connections){
			
				ConnectionEntry newConnectionEntry = new ConnectionEntry();
				
				newConnectionEntry.doorID = currentConnection.doorID;
				newConnectionEntry.roomID = currentConnection.room.ID;	
				newConnectionEntry.position = currentConnection.position;
				
				newEntry.connections.Add(newConnectionEntry);
			}
			
			shipData.roomEntries.Add(newEntry);
		}
	
		return shipData;
	}


	public int ComponentTypeCount(string typeName){
		
		int count = 0;

		foreach(ShipRoom currentRoom in roomList){
		
			if(currentRoom.CurrentComponent != null){
				
				foreach(AbilityData ability in currentRoom.CurrentComponent.ComponentData.abilities){
				
					if(ability.type == typeName){
						
						count++;
					}	
				}
			}
		}
		
		return count;
	}

	public bool HasFunctionalBridge(){

		foreach(ShipRoom currentRoom in roomList){
			
			if(currentRoom.CurrentComponent != null && !currentRoom.CurrentComponent.IsDestroyed()){

				foreach(ComponentAbility ability in currentRoom.CurrentComponent.Abilities){

					if(ability.AbilityData.type == "Bridge"){

						return true;
					}
				}
			}
		}


		return false;
	}

	public bool HasEngineRoom(){

		foreach(ShipRoom room in roomList){

			if(room.Location == ShipLocation.AFT) return true;
		}

		return false;
	}

	public bool AreRoomsValid(){
	
		List<ShipRoom> connectedRooms = new List<ShipRoom>();
		
		if(roomList.Count > 0){

			int connectedRoomCount = roomList[0].NumConnectedRooms(ref connectedRooms);

			return (connectedRoomCount == roomList.Count);
		}else{
			
			return false;
		}
	}

	public void AddThrust(int amt){
		
		currentThrust += amt;	

		Console.WriteLine("new thrust is: " + currentThrust);

		Console.WriteLine("new dodge change is: " + CurrentDodgeChance);
	}
	
	public void ResetCombatTurn(){
		
		currentThrust = 0;
	}

	public float CurrentDodgeChance{

		get{

			float adjustedThrust = currentThrust / (float)roomList.Count;

			return adjustedThrust / (adjustedThrust + PeaceServerClientCommon.PeaceConstants.THRUST_DIVISOR);
		
		}
	}

}


public class ShipFactory{
	
	public static Ship LoadShip(string directory, string filename){

		string loadedString = SaveAndLoad.Load(directory, filename);

		ShipData shipData = Newtonsoft.Json.JsonConvert.DeserializeObject<ShipData>(loadedString);
		                                           
		shipData.shipName = filename;
		
		return new Ship(shipData);
	}
	
	public static void SaveShip(Ship shipToSave, string directoryName, string filename){
	
		if(shipToSave != null){
	
			shipToSave.ShipName = filename.Replace(".ship", "");

			string saveString = Newtonsoft.Json.JsonConvert.SerializeObject(shipToSave.SaveShip());

			SaveAndLoad.Save(directoryName, filename, saveString);
		}else{
			
			Console.WriteLine("you have no ship to save.");	
		}	
	}

}

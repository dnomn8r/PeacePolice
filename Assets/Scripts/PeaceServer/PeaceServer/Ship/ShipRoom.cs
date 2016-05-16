using System.Collections;
using System.Collections.Generic;
using XnaGeometry;

public struct Connection{

	public int doorID;
	public ShipRoom room;
	public Vector3 position;
	
	public Connection(int doorID, ShipRoom r, Vector3 pos){
		
		this.doorID = doorID;
		room = r;
		this.position = pos;
	}
}

public enum HighlightType{NONE, ENEMY, FRIENDLY, SELECTED, ACTIVATED}

public enum ShipLocation {STERN, AFT, CENTER, STARBOARD, PORT} 

public delegate void DamageEventDelegate(int damage);
public delegate void MissEventDelegate();

public class ShipRoom{

	public ShipRoomData roomData;
	
	private int id;

	//private ShipComponentData componentData;
	private ShipComponent component;

	public static int MAX_STRUCTURE = 1000;
	public int currentStructure;

	public Vector3 position;
	public float rotation;

	public List<Connection> connections = new List<Connection>();

	private ShipLocation location = ShipLocation.CENTER;

	public event DamageEventDelegate OnDamageEvent;
	public event MissEventDelegate OnMissEvent;

	private Ship ship = null;

	public ShipRoom(ShipRoom room){

		id = room.id;
		currentStructure = room.currentStructure;
		//maxStructure = room.maxStructure;
		position = room.position;
		rotation = room.rotation;
		roomData = room.roomData;
	}

	public ShipRoom(ShipRoomData data){

		this.roomData = data;

		currentStructure = MAX_STRUCTURE;
	}

	public ShipRoom(Ship ship, ShipRoomData data, int id){

		this.ship = ship;
		this.roomData = data;
		this.id = id;

		currentStructure = MAX_STRUCTURE;
	}

	public Ship Ship{
		
		get{return ship;}
		
		set{ship = value;}
	}

	public string OwnerID{

		get{return ship.OwnerID;}
	}

	public int ID{
		
		get{return id;}	
		
		set{id = value;}
	}

	public ShipLocation Location{
	
		get{return location;}
		
		set{location = value;}
	}
	
	public ShipComponent CurrentComponent{
		
		get{return component;}	
		set{component = value;}
	}
	
	public string ComponentName{
			
		get{
			string compName = "";
						
			if(component != null){
				
				compName = component.ComponentData.identifier;
			}
			
			return compName;
		}
	}
	

	public void SetComponent(ShipComponent component){

		this.component = component;

	}
	
	public void CreateComponent(){
	
		// let's initialize the structure of each room here as well
		/*

		switch(roomData.size){
		
			case "1x1":
			
				maxStructure = IxI_structure;
				break;
			
			case "2x1":
			
				maxStructure = IIxI_structure;
				break;
			
			case "2x2":
			
				maxStructure = IIxII_structure;
				break;
			
		}
		
		currentStructure = maxStructure;
		
		if(componentData != null){
		
			component = currentComponentModel.AddComponent<ShipComponent>();
			
			component.InitializeComponent(this, componentData);
		}
		*/
	}


	public void ActivatePassiveAbilities(){

		if(component != null){
			
			component.ActivatePassives();	
		}
	}
	
	public void AddConnection(Connection connection){

		//Debug.Log("adding connection door id: " + connection.position + " room: " + connection.room.position);
		connections.Add(connection); 
	}
	
	public void ClearConnections(){
		
		foreach(Connection c in connections){
			
			c.room.removeConnections(this);
		}
		
		connections.Clear();	
	}
	
	private void removeConnections(ShipRoom room){
		
		List<Connection> connectionsToRemove = new List<Connection>();
		
		foreach(Connection c in connections){
		
			if(c.room == room){
				
				connectionsToRemove.Add(c);
			}
		}
		
		foreach(Connection c in connectionsToRemove){
		
			connections.Remove(c);
		}	
	}
	
	public List<ShipRoom> GetAdjacentRooms(){
		
		List<ShipRoom> adjacentRooms = new List<ShipRoom>();
		
		foreach(Connection c in connections){
		
			adjacentRooms.Add(c.room);
		}
		
		return adjacentRooms;
	}

	public int NumConnectedRooms(ref List<ShipRoom> previouslyVisitedRooms){
	
		int roomCount = 1;
		
		previouslyVisitedRooms.Add(this);
		
		foreach(Connection c in connections){
			
			if(!previouslyVisitedRooms.Contains(c.room)){
				
				roomCount += c.room.NumConnectedRooms(ref previouslyVisitedRooms);
			}
		}
		
		return roomCount;
	}
	
	public void OnTakeDamage(int damage){

		if(component != null && !component.IsDestroyed()){

			component.OnTakeDamage(damage);
		
		}else{

			currentStructure = System.Math.Min(System.Math.Max(0, currentStructure - damage), MAX_STRUCTURE);
		}

		if(OnDamageEvent != null){

			OnDamageEvent(damage);
		}
	}

	public void OnDodge(){

		if(OnMissEvent != null){

			OnMissEvent();
		}
	}

	
}

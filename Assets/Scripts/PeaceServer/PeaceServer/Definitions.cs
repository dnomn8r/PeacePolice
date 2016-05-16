using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class Definitions{
	
	private static Definitions instance;
	
	public static Definitions Instance{
		
		get{

			if(instance == null){

				new Definitions();
			}

			return instance;
		
		}	
	}
	
	private List<ShipRoomData> rooms = new List<ShipRoomData>();
	private Dictionary<string, ShipRoomData> roomDictionary = new Dictionary<string, ShipRoomData>();

	private List<ShipComponentData> components = new List<ShipComponentData>();
	private Dictionary<string, ShipComponentData> componentDictionary = new Dictionary<string, ShipComponentData>();


#if !PEACE_SERVER
	string path = "Assets/Scripts/PeaceServer/PeaceServer/Data/";
#else
	string path = "../../Data/";
#endif

	public List<ShipRoomData> Rooms{

		get{return rooms;}
	}

	public List<ShipComponentData> Components{
	
		get{return components;}
	}

	public ShipRoomData GetRoomByID(string identifier){

		if(identifier != null && roomDictionary.ContainsKey(identifier)){

			return roomDictionary[identifier];
		}

		return null;
	}

	public ShipComponentData GetComponentByID(string identifier){
		
		if(identifier != null && componentDictionary.ContainsKey(identifier)){
			
			return componentDictionary[identifier];	
		}
		
		return null;
	}
	

	 public Definitions(){
		
		instance = this;
		
		loadDefinitions();
	}
	
	
	private void loadDefinitions(){

		loadRooms();

		loadComponents();	
	}

	private void loadRooms(){
	
		//Object[] roomFiles = Resources.LoadAll("Ship/RoomDefinitions", typeof(TextAsset)) as Object[];

		Console.WriteLine("path is: " + path);

		string roomText = System.IO.File.ReadAllText(path + "RoomDefinitions.txt");

		Console.WriteLine("reading room definition: " + roomText);

		rooms = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ShipRoomData>>(roomText);

		foreach(ShipRoomData roomData in rooms){

			//Debug.Log ("reading data: " + roomData);
			roomDictionary.Add(roomData.name, roomData);
		}
	}
	
	private void loadComponents(){

		string [] componentFiles = Directory.GetFiles(path + "Components/Definitions/", "*.txt");

		//Object[] componentFiles = Resources.LoadAll("Ship/Components/Definitions", typeof(TextAsset)) as Object[];
		
		foreach(string componentFile in componentFiles){
	
			//if(!componentFile.EndsWith(".meta") && !componentFile.EndsWith(".DS_Store")){

				Console.WriteLine("trying to read compnent file: " + componentFile);

				string componentText = System.IO.File.ReadAllText(componentFile);

				Console.WriteLine("reading definition: " + componentText);

				ShipComponentData currentData = Newtonsoft.Json.JsonConvert.DeserializeObject<ShipComponentData>(componentText);

				components.Add(currentData);
				
				componentDictionary.Add(currentData.identifier, currentData);
			//}
		}
		
	}
	
	private void printShipComponents(){
		
		foreach(ShipComponentData component in components){
			
			Console.WriteLine(component);
		}
	}
}

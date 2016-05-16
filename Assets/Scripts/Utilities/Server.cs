using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
public class ServerResult{
	
	public List<ComponentActionResult> actionResults;

	public List<CombatAction> combatActions;

	public Dictionary<int, PlayerData> playerStates = new Dictionary<int, PlayerData>();

	public override string ToString(){

		string resultString = "";

		resultString += "\nActions:\n";

		foreach(CombatAction action in combatActions){

			resultString += "\n\t" + action;
		}

		resultString += "\n\nAction Results:";

		foreach(ComponentActionResult result in actionResults){

			resultString += "\n\t" + result;
		}

		return resultString;
	}
}


public class Server : MonoBehaviour{

	private const float distanceBetweenPlayerShips = 1000.0f; // this will be replaced with proper mountpoints in the future

	private const int startReactorEnergy = 500;
	private const int maxReactorEnergy = 1000;
	private const int energyIncrement = 25;

	private int currentTurn = 0;

	private Dictionary<int, Player> players = new Dictionary<int, Player>();

	private List<CombatAction> receivedActions = new List<CombatAction>();

	private static Server instance;

	public static Server Instance{
		get{
			return(instance);
		}
	}
	
	void Awake(){

		instance = this;

		new Definitions();
	}

	public float DistanceBetweenShips{

		get{return distanceBetweenPlayerShips;}
	}

	public Player GetPlayer(int playerID){

		Player player = null;

		if(players.TryGetValue(playerID, out player)){

			return player;
		}else{

			Debug.LogWarning("there is no player with id: " + playerID);
			return null;
		}
	}

	public void ConnectPlayer(int playerID, string shipName){

		if(!players.ContainsKey(playerID)){

			Player newPlayer = new Player(playerID, loadShip(shipName));

			newPlayer.CurrentShip = new Ship(newPlayer.playerData.shipData, playerID);

			newPlayer.AvailableEnergy = startReactorEnergy;
			newPlayer.MaxEnergy = maxReactorEnergy;
			newPlayer.RechargeIncreaseRate = energyIncrement;
			newPlayer.CurrentRechargeRate = energyIncrement;

			players.Add(playerID, newPlayer);
		}else{

			Debug.LogError("we already have a player with id: " + playerID);
		}
	}

	public string GetPlayerData(){

		Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
		settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;

		return Newtonsoft.Json.JsonConvert.SerializeObject(players, settings);
	}

	public void ReceiveActions(string actionString){

		List<CombatAction> combatActions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CombatAction>>(actionString);

		receivedActions.AddRange(combatActions);
	}
	
	public string ProcessTurn(){

		foreach(KeyValuePair<int, Player> kvp in players){
			
			kvp.Value.AvailableEnergy += energyIncrement*currentTurn;
		}

		
		Debug.Log("server has actions:");
		
		foreach(CombatAction action in receivedActions){
			
			Debug.Log("action: " + action);
		}
	
		ServerResult newResult = new ServerResult();
		newResult.combatActions = receivedActions;
		newResult.actionResults = ProcessActions(receivedActions);

		Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
		settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;

		// add the new combat state which has the new energy and whatnot
		foreach(KeyValuePair<int, Player> kvp in players){

			Debug.Log ("playerid: " + kvp.Key + "player: " + kvp.Value);

			newResult.playerStates.Add(kvp.Key, kvp.Value.playerData);
		}

		string returnString = Newtonsoft.Json.JsonConvert.SerializeObject(newResult, settings);

		receivedActions.Clear();

		currentTurn++;

		return returnString;
	}
	
	public List<ComponentActionResult> ProcessActions(List<CombatAction> actions){

		List<ComponentActionResult> allResults = new List<ComponentActionResult>();

		foreach(CombatAction action in actions){
	
			Player player = GetPlayer(action.playerID);

			Debug.Log ("player: " + player.ID);

			ShipRoom room = player.CurrentShip.GetRoom(action.roomID);

			if(player.AvailableEnergy >= room.CurrentComponent.currentCost){

				player.AvailableEnergy -= room.CurrentComponent.currentCost;

				foreach(AbilityEntry abilityEntry in action.abilityEntryList){

					ComponentAbility ability = room.CurrentComponent.GetAbility(abilityEntry.abilityName);
				
					List<ShipRoom> rooms = new List<ShipRoom>();

					foreach(AbilityTarget target in abilityEntry.targets){

						Player targetPlayer = GetPlayer(target.playerID);
						ShipRoom targetRoom = targetPlayer.CurrentShip.GetRoom(target.roomID);

						rooms.Add(targetRoom);
					}

					List<ComponentActionResult> actionResults = ability.ExecuteAction(rooms);

					allResults.AddRange(actionResults);
				}
			}else{

				Debug.LogError("tried to execute an action for which we didn't have enough energy for!");
			}
		}

		return allResults;
	}


	private ShipData loadShip(string filename){
	
		string shipString = SaveAndLoad.Load("ShipDesigns", filename + ".ship");

		return Newtonsoft.Json.JsonConvert.DeserializeObject<ShipData>(shipString);
	}
}
*/

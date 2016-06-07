using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PeaceServerClientCommon;


public delegate void UnityCombatStatusChanged();

public class UnityCombatManager{

	public event UnityCombatStatusChanged OnRefresh;

	private static UnityCombatManager instance;

	public static UnityCombatManager Instance{

		get{ return instance; }
	}

	private PeaceGame game;

	public PeaceGame Game{
		get{ return game; }
	}

	private string localPlayerName;

	public string PlayerName{
		get{ return localPlayerName; }
	}

	private bool opponentLeft = false;

	public bool OpponentHasLeft{ get { return opponentLeft; } }

	private PeaceClient client;

	private List<PlayerController> playerControllers = new List<PlayerController>();

	private List<PeaceMessage> receivedMessages = new List<PeaceMessage>();

	public bool WaitingForOtherPlayers{ get; private set; }

	private bool isGoingToWin = false;
	private bool isGoingToLose = false;

	private bool hasLost = false;
	private bool hasWon = false;

	OurPlayerController localController;


	public bool HasLost{
		get{ return hasLost; }
	}
	public bool HasWon{
		get{ return hasWon; }
	}

	void OnDestroy(){

		instance = null;
	}

	public void UpdateGameStatus(){

		foreach(PlayerController controller in playerControllers){

			controller.Display.UpdateDodgeChange(0);
			controller.Display.UpdateEnergy();
		}

		//game.AdvanceTurn();

		if(isGoingToWin){

			hasWon = true;
		} else if(isGoingToLose){

			hasLost = true;
		}
	}

	public UnityCombatManager(PeaceClient networkClient, string localPlayer, PeaceGame game){

		instance = this;

		opponentLeft = false;
		client = networkClient;

		localPlayerName = localPlayer;

		this.game = game;

		//UnityCombatScheduler.Instance.SetCombatScheduler(game.Scheduler);

//		if(UnityCombatScheduler.Instance.CombatScheduler == null){
//			UnityCombatScheduler.Instance.CombatScheduler = this.game.Scheduler;
//		}
			
		Debug.Log("our player is: " + localPlayerName);

		Debug.Log("result: " + game.PrintPlayers());

		client.OnMessageReceived += HandleMessage;

		WaitingForOtherPlayers = false;

	}

	public Player GetFirstEnemy(){

		return game.OpposingPlayers(localPlayerName)[0];
	}

	public Player GetLocalPlayer(){

		return game.GetPlayer(localPlayerName);
	}

	public void AddPlayerController(PlayerController controller){

		playerControllers.Add(controller);
	}

	public List<PlayerController> GetEnemyPlayerControllers(string playerName){

		return playerControllers.Where(x => x.Player.Name != playerName).ToList();
	}

	public PlayerController GetPlayerController(string playerName){

		return playerControllers.Find(x => x.Player.Name == playerName);
	}
		
	public OurPlayerController GetOurPlayerController(){

		return playerControllers.Find(x => x is OurPlayerController) as OurPlayerController;
	}

	public ShipRoomController GetRoomController(string playerName, int roomID){

		PlayerController controller = GetPlayerController(playerName);

		return controller.Display.ShipDisplay.GetRoomController(roomID);
	}

	public void Refresh(){

		if(OnRefresh != null){

			OnRefresh();
		}
	}

	public void EndLocalPlayerTurn(){

		if(localController == null){

			localController = GetOurPlayerController();
		}
		//localController = GameObject.FindObjectOfType(typeof(OurPlayerController)) as OurPlayerController;

		Debug.Log("ending our turn with actions: " + localController.PrintActions());

		string actionString =  Newtonsoft.Json.JsonConvert.SerializeObject(localController.CombatActions);

		Debug.Log("action string: " + actionString);

		client.SendPeaceMessage(new PeaceMessage(PeaceMessageType.ACTIONS, actionString));

		WaitingForOtherPlayers = true;

		cleanupLocalPlayer();

	}
		
	public void Cleanup(){

		if(client != null){
		
			client.Disconnect();
		}

		instance = null;
	}

	private void cleanupLocalPlayer(){

		if(localController == null){

			localController = GetOurPlayerController();
		}

		if(localController != null){
		
			localController.CombatActions.Clear();

			localController.Display.ShipDisplay.ClearHighlights();
		} else{

			Debug.Log("meow?");
		}
	}

	void HandleMessage(PeaceMessage message){

		Debug.Log("got meow message: " + message.messageType);

		receivedMessages.Add(message);

		if(!opponentLeft){
			
			if(message.messageType == PeaceMessageType.GAME_WIN_DISCONNECT){

				opponentLeft = true;

			} else if(message.messageType == PeaceMessageType.TURN){

				WaitingForOtherPlayers = false;

				Debug.Log("YAY! received turn message: " + message.messageContent);

				PeaceTurn newTurn = null;
				TypeNameSerializationBinder binder = new TypeNameSerializationBinder("{0}");

				Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
				settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
				settings.Binder = binder;

				try{
					
					newTurn = Newtonsoft.Json.JsonConvert.DeserializeObject<PeaceTurn>(message.messageContent, settings);
				
				} catch(System.Exception e){

					Debug.Log("got exception in dserialize: " + e.Message);
				}

				foreach(KeyValuePair<string, int> kvp in newTurn.energy){

					PlayerController controller = GetPlayerController(kvp.Key);
					controller.Player.SetEnergy(kvp.Value);

				}


//				if(UnityCombatScheduler.Instance.CombatScheduler == null){
//					UnityCombatScheduler.Instance.CombatScheduler = game.Scheduler;
//				}

				//ADD NEW ENERGY VALUES SENT AS PART OF TURN
					
				UnityCombatScheduler.Instance.ExecuteTurn(newTurn.results);

			}else if(message.messageType == PeaceMessageType.WIN){

				isGoingToWin = true;

			}else if(message.messageType == PeaceMessageType.LOSS){

				isGoingToLose = true;

			}else{
				Debug.LogError("got invalid message of type: " + message.messageType + " with content: " + message.messageContent);
			}
		}else{
			Debug.Log("received message: " + message + " after game was over!");
		}
	}
		

//	private Dictionary<int, PlayerController> playerControllers = new Dictionary<int, PlayerController>();
//
//	private static UnityCombatManager instance;
//	
//	public static UnityCombatManager Instance{
//		
//		get{return instance;}	
//	}
//	
//	public UnityCombatManager(){
//		
//		instance = this;	
//	}
//
//
//	/*
//	void Start(){
//
//		string playerData = Server.Instance.GetPlayerData();
//
//		Dictionary<int, Player> players = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, Player>>(playerData);
//
//		players[0].CurrentShip = new Ship(players[0].playerData.shipData, 0);
//
//		playerControllers[0].Player = players[0];
//		playerControllerMap.Add (0, playerControllers[0]);
//
//		players[1].CurrentShip = new Ship(players[1].playerData.shipData, 1);
//
//		playerControllers[1].Player = players[1];
//		playerControllerMap.Add (1, playerControllers[1]);
//
//
//		playerControllers[0].ShipController.CleanupDoors();
//		playerControllers[1].ShipController.CleanupDoors();
//	}
//	
//	void OnGUIButtonClicked(GUIButton b){
//
//		if(b.name == "endTurnButton"){
//
//			Server.Instance.ReceiveActions(Newtonsoft.Json.JsonConvert.SerializeObject(playerControllers[0].CombatActions));
//			Server.Instance.ReceiveActions(Newtonsoft.Json.JsonConvert.SerializeObject(playerControllers[1].CombatActions));
//
//			playerControllers[0].CombatActions.Clear();
//			playerControllers[1].CombatActions.Clear();
//
//			string resultString = Server.Instance.ProcessTurn();
//
//			Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
//			settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
//
//			ServerResult serverResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerResult>(resultString, settings);
//
//
//			UnityCombatScheduler.Instance.AddActionResults(serverResult.actionResults);
//
//			foreach(CombatAction action in serverResult.combatActions){
//
//				PlayerController playerController = CombatManager.Instance.GetPlayerController(action.playerID);
//
//				ShipRoomController roomController = playerController.ShipController.GetRoomController(action.roomID);
//			
//				ShipComponentController componentController = roomController.ComponentController;
//
//				foreach(AbilityEntry entry in action.abilityEntryList){
//				
//					AbilityData data = componentController.CurrentComponent.GetAbility(entry.abilityName).AbilityData;
//
//					componentController.Activate(entry.targets, data);
//				}
//			}
//
//			UnityCombatScheduler.Instance.ExecuteTurn();
//
//			foreach(PlayerController p in playerControllers){
//
//				p.Player.playerData = serverResult.playerStates[p.Player.ID];
//			
//				p.ShipController.ClearHighlights();
//			}
//
//		}
//	}
//	*/
//
//	public PlayerController GetPlayerController(int playerID){
//
//		PlayerController controller = null;
//
//		if(playerControllers.TryGetValue(playerID, out controller)){
//
//			Debug.LogWarning("could not find player with ID: " + playerID);
//		}
//
//		return controller;
//	}
//
//	public List<PlayerController> GetEnemyPlayerControllers(int playerID){
//
//		List<PlayerController> enemyControllers = new List<PlayerController>();
//
//		foreach(KeyValuePair<int, PlayerController> kvp in playerControllers){
//
//			if(kvp.Key != playerID){
//				enemyControllers.Add(kvp.Value);
//			}
//		}
//		       
//		return enemyControllers;
//	}
//	
//	public void Refresh(){
//
//		foreach(PlayerController p in playerControllers.Values){
//			
//			p.Refresh();
//
//			p.PrintActions();
//		}
//	}

}

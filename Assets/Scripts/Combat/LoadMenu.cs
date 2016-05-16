using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PeaceServerClientCommon;

public class LoadResult{

	public ShipData ship;
}

public class LoadMenu : MonoBehaviour {

	public InputField playerName;
	public TextMesh playerShipName;
	public InputField serverIP;

	public TextMesh statusText;

	public GUIButton startGameButton;

	private LoadResult playerLoadResult = new LoadResult();
	//private ShipData shipData = new ShipData();

	private PeaceClient client; 

	PeaceServerClientCommon.PeacePlayerData playerData = new PeaceServerClientCommon.PeacePlayerData();

	void Awake(){

		if(startGameButton != null){
		
			startGameButton.gameObject.SetActive(false);
		}

		client = new PeaceClient();

		client.OnStatusChanged += OnClientStatusChanged;
		client.OnMessageReceived += OnMessageReceived;

		//new Definitions();
	}

	void OnGUIButtonClicked(GUIButton b){

		if(b.name == "loadPlayerShipButton"){
			
			GameObject fileDialogPopup = GameObject.Instantiate(Resources.Load("ShipLoadPopup")) as GameObject;
			
			ShipLoadPopup dialog = fileDialogPopup.GetComponent<ShipLoadPopup>();
			
			dialog.SetLoadResult(playerLoadResult);

			dialog.SetCallbackFunctions(OnPopupClosed, OnPopupClosed);

			dialog.LoadFiles(RoomEditor.SHIP_DIRECTORY, RoomEditor.SHIP_EXTENSION);

		}

		if(b == startGameButton){

			playerData.name = playerName.Value;
			playerData.ship = playerLoadResult.ship;

			client.Connect(serverIP.Value, 3345, 
			               Newtonsoft.Json.JsonConvert.SerializeObject(playerData));

			//Server.Instance.ConnectPlayer(0, player1Result.playerData.shipName);
			//Server.Instance.ConnectPlayer(1, player2Result.playerData.shipName);

			//Application.LoadLevel("Combat");
		}
	}

	void OnClientStatusChanged(string message){

		statusText.text = message;
	}

	void OnMessageReceived(PeaceMessage message){

		if(message.messageType == PeaceMessageType.GAME_START){

			Debug.Log("game starting: " + message.messageContent);

			PeaceGame newGame = new PeaceGame(
				Newtonsoft.Json.JsonConvert.DeserializeObject<List<PeacePlayerData>>(message.messageContent)
			);

			new UnityCombatManager(client, playerData.name, newGame);

		}else{

			Debug.Log("the only type of message we should be gettign here is a Game Start one, but we got a message" +
			"of type: " + message.messageType);
		}
	}
		
	void Update(){

		// have to do this loading here, not right when we received this message.  This is due to Unity Threading.
		if(UnityCombatManager.Instance != null){

			Application.LoadLevel("Combat");
		}

	}


	void OnInputFieldChanged(InputField f){

		checkForGameStart();
	}

	private void OnPopupClosed(){

		if(playerLoadResult.ship != null){

			playerShipName.text = playerLoadResult.ship.shipName;
		}

		checkForGameStart();
	}

	void checkForGameStart(){

		//Debug.Log ("meow: " + playerName.Value);

		startGameButton.gameObject.SetActive(playerLoadResult.ship != null && 
			!string.IsNullOrEmpty(playerName.Value) &&
		                                     serverIP.Value != "");
	}

	void OnDisable(){

		client.OnStatusChanged -= OnClientStatusChanged;
		client.OnMessageReceived -= OnMessageReceived;
	}

}

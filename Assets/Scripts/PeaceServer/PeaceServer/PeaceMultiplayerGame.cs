using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

using PeaceServerClientCommon;


public class PeaceMultiplayerGame{

	private List<RemotePeacePlayer> players;
		
	private PeaceGame game;

	public PeaceMultiplayerGame(List<RemotePeacePlayer> players){

		this.players = players;
	}

	private List<PeaceMessage> actionMessages = new List<PeaceMessage>();

	public void StartGame(){


		Console.WriteLine("SERVER: Starting game with players: ");

		List<PeacePlayerData> playerDataList = new List<PeacePlayerData>();

		foreach(RemotePeacePlayer player in players){
			
			playerDataList.Add(player.PlayerData);
		}
			

		foreach(RemotePeacePlayer player in players){

			string messageContent = Newtonsoft.Json.JsonConvert.SerializeObject(playerDataList);

			// send other players the list of other players
			player.QueueOutgoingMessage(
				new PeaceMessage(PeaceMessageType.GAME_START, 
			   	messageContent
				));

			//Console.WriteLine("Sending out content to: " + player.PlayerData.name + " content: " + messageContent);

		}

		game = new PeaceGame(playerDataList);

		bool done = false;

		int serverCheckDelayMS = 500;

		int timeLeftInTurn = PeaceConstants.SECONDS_PER_TURN * 1000; // so we can deal in milliseconds

		while(!done){

			/*
			Console.WriteLine("---- game state -----");

			foreach(RemotePeacePlayer player in players){
				
				Console.WriteLine("player: " + player.PlayerData.name + " state: " + player.PlayerState + " i/o: (" + 
				                  player.IncomingMessageCount() + "/" + player.OutgoingMessageCount() + ")");
			}
			*/

			foreach(RemotePeacePlayer player in players){

				if(player.HasIncomingMessages()){

					PeaceMessage newMessage = player.GetIncomingMessage();

					if(newMessage.messageType == PeaceMessageType.ACTIONS){

						actionMessages.Add(newMessage);
					}
				}

			}

			// once all players have sent out their action messages, we process everything and send all players the results
			if(actionMessages.Count == players.Count || timeLeftInTurn < 0){

				if(timeLeftInTurn < 0){

					Console.WriteLine("TIME RAN OUT ON TURN! SENDING WHAT WE HAVE NOW");
				}

				Console.WriteLine("Server is processing turn: ");

				// first, convert all actions we've got from all players to combat actions
				List<CombatAction> combatActions = new List<CombatAction>();

				foreach(PeaceMessage message in actionMessages){

					List<CombatAction> playerActions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CombatAction>>(message.messageContent);

					/*
					foreach(CombatAction combatAction in combatActions){

						Console.WriteLine("avtion has player id: " + combatAction.playerID + " adn room id: " + combatAction.roomID);

						Console.WriteLine("action tostirng() " + combatAction); 
					}*/

					combatActions.AddRange(playerActions);
				}

				// now execute all the actions, and get the list of action results(damage events, etc...)

				List<ComponentActionResult> results = ProcessActions(combatActions);

				game.Scheduler.AddActionResults(results);

				game.Scheduler.CurrentSlice = 0;

				// loop through all the actions
				while(!game.Scheduler.ExecuteNextSlice()){

					Console.Write(".");
				}



				Dictionary<string, int> energies = game.AdvanceTurn();


				// now we've got the results, let's send out the actions and the results to all the players
				PeaceTurn newTurn = new PeaceTurn();
				//newTurn.actionMessages = actionMessages;
				newTurn.energy = energies;
				newTurn.results = results;

				TypeNameSerializationBinder binder = new TypeNameSerializationBinder("{0}");

				//"$type":"AimEvent, PeaceServer"

				Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
				//settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
				//settings.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full;

				settings.Binder = binder;

				string turnString = Newtonsoft.Json.JsonConvert.SerializeObject(newTurn);

				Console.WriteLine("SERVER HAS A NEW TURN:\n" + turnString);

				PeaceMessage newTurnMessage = new PeaceMessage(PeaceMessageType.TURN, turnString);

				foreach(RemotePeacePlayer player in players){

					player.QueueOutgoingMessage(newTurnMessage);
				}
			
				actionMessages.Clear();

				// add time it will take for the client simulation to finish executing

				Console.WriteLine("basic time is: " + (PeaceConstants.SECONDS_PER_TURN * 1000));

				Console.WriteLine("current slcie: " + game.Scheduler.CurrentSlice);
				Console.WriteLine("extra time is: " +  (int)(game.Scheduler.CurrentSlice * CombatScheduler.TIME_SLICE_LENGTH*1000));


				timeLeftInTurn = (PeaceConstants.SECONDS_PER_TURN * 1000) +
								 (int)(game.Scheduler.CurrentSlice * CombatScheduler.TIME_SLICE_LENGTH * 1000);
			}

			Thread.Sleep(serverCheckDelayMS);

			timeLeftInTurn -= serverCheckDelayMS;

			Console.WriteLine("Time left in turn: " + timeLeftInTurn);

			done = CheckForGameEnd();

		}

		Thread.CurrentThread.Join();
	}

	private bool CheckForGameEnd(){

		players.RemoveAll(p => p.PlayerState == PeacePlayerState.DISCONNECTED);

		if(players.Count == 1){ // if we're the only player left, we win!

			players[0].QueueOutgoingMessage(new PeaceMessage(PeaceMessageType.GAME_WIN_DISCONNECT));

			Console.WriteLine("player: " + players[0].PlayerData.name + " has won due to everyone else disconnecting!");

			return true;
			//Thread.CurrentThread.Join();

		}else if(players.Count == 0){

			Console.WriteLine("Game has ended due to everyone leaving!  No one wins?!!? This should be rare...");

			return true;
			//Thread.CurrentThread.Join();
		}

		List<RemotePeacePlayer> losingPlayers = new List<RemotePeacePlayer>();

		foreach(RemotePeacePlayer rpp in players){

			Player player = game.Scheduler.GetPlayer(rpp.PlayerData.name);

			if(player.IsDead()){

				Console.WriteLine("oh no, player " + player.Name + " is dead!");

				rpp.QueueOutgoingMessage(new PeaceMessage(PeaceMessageType.LOSS));

				losingPlayers.Add(rpp);
			}
		}

		foreach(RemotePeacePlayer losingPlayer in losingPlayers){

			players.Remove(losingPlayer);

			losingPlayer.Disconnect();
		}

		if(losingPlayers.Count > 0){

			foreach(RemotePeacePlayer winningPlayer in players){

				Console.WriteLine("yeah! player " + winningPlayer.PlayerData.name + " has won!");

				winningPlayer.QueueOutgoingMessage(new PeaceMessage(PeaceMessageType.WIN));

				winningPlayer.Disconnect();
			}

			return true;
		}

		return players.Count == 0; // if we have no players left, the game is over, this can happen if players kill each other
	}
		

	public RemotePeacePlayer GetPlayer(string playerID){

		foreach(RemotePeacePlayer player in players){

			if(player.PlayerData.name == playerID){

				return player;
			}
		}

		return null;
	}

	public List<ComponentActionResult> ProcessActions(List<CombatAction> actions){
		
		List<ComponentActionResult> allResults = new List<ComponentActionResult>();
		
		foreach(CombatAction action in actions){

			Player player = game.Scheduler.GetPlayer(action.playerID);
	
			ShipRoom room = player.CurrentShip.GetRoom(action.roomID);

			if(room.CurrentComponent != null){

				if(player.CurrentEnergy >= room.CurrentComponent.currentEnergy){

					player.UseEnergy(room.CurrentComponent.currentEnergy);

					foreach(AbilityEntry abilityEntry in action.abilityEntryList){

						ComponentAbility ability = room.CurrentComponent.GetAbility(abilityEntry.abilityName);

						List<ShipRoom> rooms = new List<ShipRoom>();

						foreach(AbilityTarget target in abilityEntry.targets){

							Player targetPlayer = game.Scheduler.GetPlayer(target.playerID);
							ShipRoom targetRoom = targetPlayer.CurrentShip.GetRoom(target.roomID);
							
							rooms.Add(targetRoom);
						}

						Console.WriteLine("executin action for room: " + action.roomID);

						List<ComponentActionResult> actionResults = ability.ExecuteAction(game, rooms);
						
						allResults.AddRange(actionResults);
					}
				}else{

					Console.WriteLine("uhoh, someone tried to use more energy than they have!");
				}
			}
		}
		
		return allResults;
	}
		

	/*
	private void OnPlayerStateChanged(RemotePeacePlayer player){

		if(player.PlayerState == PeacePlayerState.DISCONNECTED){

			OnGameEndedDueToDisconnect();
		}
	}
	*/
	
}

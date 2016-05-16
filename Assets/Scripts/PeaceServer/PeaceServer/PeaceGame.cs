using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PeaceServerClientCommon{
	
	public class PeaceGame{

		private CombatScheduler scheduler;

		private Dictionary<string, Player> players;


		public CombatScheduler Scheduler{
			get{return scheduler;}
		}

		public PeaceGame(List<PeacePlayerData> currentPlayers){
			
			players = new Dictionary<string, Player>();

			foreach(PeacePlayerData playerData in currentPlayers){

				Player newPlayer = new Player(playerData);

				players.Add(playerData.name, newPlayer);

				Console.WriteLine("adding player with name:" + playerData.name + " to game ship id: " + newPlayer.CurrentShip.OwnerID);
			}

			scheduler = new CombatScheduler(players);
		}

		public Player GetPlayer(string playerName){

			Player player = null;

			if(!players.TryGetValue(playerName, out player)){

				Console.WriteLine("could not find player: " + playerName);
			}

			return player;
		}

		public List<Player> OpposingPlayers(string playerName){

			return (players.Where(e => e.Key != playerName)).Select(x => x.Value).ToList();
		}

		public string PrintPlayers(){

			string result = "";

			foreach(KeyValuePair<string, Player> kvp in players) {

				result += "Player: " + kvp.Value.Name + " with ship name: " + kvp.Value.CurrentShip.ShipName + "\n";
			}

			return result;
		}

		public Dictionary<string, int> AdvanceTurn(){

			Dictionary<string, int> energies = new Dictionary<string, int>();

			foreach(KeyValuePair<string, Player> kvp in players){

				Console.WriteLine("adding energy for player: " +kvp.Key + " val: " + kvp.Value);
				energies.Add(kvp.Key, kvp.Value.AdvanceTurn());
			}

			return energies;
		}
	
	}
}

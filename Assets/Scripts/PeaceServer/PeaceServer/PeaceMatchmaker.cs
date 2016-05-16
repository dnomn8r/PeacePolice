using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using PeaceServerClientCommon;

public class PeaceMatchmaker{

	private List<RemotePeacePlayer> pendingPlayers;

	public PeaceMatchmaker(){

		pendingPlayers = new List<RemotePeacePlayer>();
	}

	public void StartMatchmaker(){

		while(true){

			RunMatchmaker();

			Thread.Sleep(10);
		}
	}

	public void AddPlayer(RemotePeacePlayer newPlayer){

		pendingPlayers.Add(newPlayer);

		Console.WriteLine("new player connected: " + newPlayer.EndPointName + 
			" num players: " + pendingPlayers.Count);

	}

	// remove players that are in the matchmaking queue, but disconnect before being matched
	private void RemoveDisconnectedPlayers(){

		List<RemotePeacePlayer> disconnectedPlayers = pendingPlayers.Where (p => p.PlayerState == PeacePlayerState.DISCONNECTED).ToList();
		
		foreach(RemotePeacePlayer disconnectedPlayer in disconnectedPlayers){
			
			pendingPlayers.Remove(disconnectedPlayer);
		}
	}

	private void RunMatchmaker(){

		RemoveDisconnectedPlayers();

		//List<RemotePeacePlayer> joinedPlayers = pendingPlayers.Where(p => p.PlayerState == PeacePlayerState.JOINED).ToList();

		List<RemotePeacePlayer> joinedPlayers = new List<RemotePeacePlayer>();

		for(int i=0;i<pendingPlayers.Count;++i){

			if(pendingPlayers[i].PlayerState == PeacePlayerState.JOINED){

				joinedPlayers.Add(pendingPlayers[i]);
			}
		}


		for(int i=0;i<joinedPlayers.Count;i+=2){

			if((i+1) <= joinedPlayers.Count - 1){

				StartGame(joinedPlayers[i], joinedPlayers[i+1]);

			}
		}
	}

	private void StartGame(RemotePeacePlayer player1, RemotePeacePlayer player2){

		pendingPlayers.Remove(player1);
		pendingPlayers.Remove(player2);
		
		PeaceMultiplayerGame newGame = new PeaceMultiplayerGame(new List<RemotePeacePlayer>{player1, player2});
		
		Thread newGameThread = new Thread(newGame.StartGame);
		newGameThread.Start();

		//while(!newGameThread.IsAlive);
	}

}

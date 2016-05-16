using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Collections.Generic;

using PeaceServerClientCommon;

public class PeaceServer{

	private void StartServer(){

		new Definitions();

		PeaceMatchmaker matchmaker = new PeaceMatchmaker();
		new Thread(matchmaker.StartMatchmaker).Start();
	

		IPAddress localAddress = IPAddress.Parse("127.0.0.1");
		
		TcpListener server = new TcpListener(localAddress, 3345);
		
		server.Start();
		
		while(true){
			
			TcpClient newPlayer = server.AcceptTcpClient();

			RemotePeacePlayer newPeacePlayer = new RemotePeacePlayer(newPlayer/*, this*/);

			//newPeacePlayer.OnPlayerStateChanged += OnPlayerStateChangedHandler;

			//pendingPlayers.Add(newPeacePlayer);

			matchmaker.AddPlayer(newPeacePlayer);

			Thread.Sleep(1);
		}
	}


	/*
	public void OnPlayerStateChangedHandler(RemotePeacePlayer player){

		if(player.PlayerState == PeacePlayerState.DISCONNECTED){

			pendingPlayers.Remove(player);
			
			Console.WriteLine("player has left, player count now: " + pendingPlayers.Count);

			player.OnPlayerStateChanged -= OnPlayerStateChangedHandler;
		
		}else if(player.PlayerState == PeacePlayerState.JOINED){

			CheckMatchmaker();
		}else{

			Console.WriteLine("uhoh, server got a remote player update with state: " + player.PlayerState + " but it can't handle that!");
		}
	}
	*/

	/*
	private void CheckMatchmaker(){

		List<RemotePeacePlayer> joinedPlayers = new List<RemotePeacePlayer>();

		foreach(RemotePeacePlayer remotePlayer in pendingPlayers){

			if(remotePlayer.PlayerState == PeacePlayerState.JOINED){

				joinedPlayers.Add(remotePlayer);
			}
		}

		for(int i=0;i<joinedPlayers.Count;i+=2){

			if( (i+1) <= joinedPlayers.Count - 1){
				
				joinedPlayers[i].OnPlayerStateChanged -= OnPlayerStateChangedHandler;
				joinedPlayers[i+1].OnPlayerStateChanged -= OnPlayerStateChangedHandler;

				pendingPlayers.Remove(joinedPlayers[i]);
				pendingPlayers.Remove(joinedPlayers[i+1]);

				PeaceMultiplayerGame newGame = new PeaceMultiplayerGame(new RemotePeacePlayer[]{joinedPlayers[i], joinedPlayers[i+1]});
				
				Thread newGameThread = new Thread(newGame.StartGame);
				newGameThread.Start();
				
				while(!newGameThread.IsAlive);
				
				Thread.Sleep(1);
			}
		}
	}
	*/

	public static void Main(){ 

		PeaceServer newServer = new PeaceServer();

		Thread newServerThread = new Thread(newServer.StartServer);
		newServerThread.Start();

		while(newServerThread.IsAlive){

			Thread.Sleep(10);
		}
	
		Console.WriteLine("Main Server Killed");
		/*
		while(true){

			Console.WriteLine("threads! " +  System.Diagnostics.Process.GetCurrentProcess().Threads.Count);

			int count = 0;

			foreach(Thread thread in System.Diagnostics.Process.GetCurrentProcess().Threads){
			
				Console.WriteLine("thread: " + thread.ThreadState);

				if(thread.ThreadState == ThreadState.Running){
					count++;
				}
			}

			Console.WriteLine("number of active threads: " + count);

			//Console.WriteLine("thread count: " + 
			  //                .OfType<System.Diagnostics.ProcessThread>().Where(t => t. == System.Diagnostics.ThreadState.Running).Count());

			Console.WriteLine("Waiting for first player...");

			TcpClient player1 = server.AcceptTcpClient();

			Console.WriteLine("First player connected");

			Console.WriteLine("Waiting for second player... ");

			TcpClient player2 = server.AcceptTcpClient();

			PeaceMultiplayerGame newGame = new PeaceMultiplayerGame(player1, player2);

			Thread newGameThread = new Thread(newGame.StartGame);
			newGameThread.Start();

			Console.WriteLine("Start game with players: " + 
			              player1.Client.RemoteEndPoint.ToString() + " and " + 
			              player2.Client.RemoteEndPoint.ToString());


			while(!newGameThread.IsAlive);

			Thread.Sleep(1);
		}*/
	}

	

}   


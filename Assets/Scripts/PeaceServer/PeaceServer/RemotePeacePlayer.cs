using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using PeaceServerClientCommon;

public delegate void PlayerStateChange(RemotePeacePlayer p);

public class RemotePeacePlayer{

	private Queue<PeaceMessage> incoming = new Queue<PeaceMessage>();
	private Queue<PeaceMessage> outgoing = new Queue<PeaceMessage>();

	//public event PlayerStateChange OnPlayerStateChanged;

	private TcpClient remotePlayer;

	private Thread connectionThread;

	private PeacePlayerData playerData;

	public PeacePlayerData PlayerData{

		get{return playerData;}
	}

	private bool shouldDisconnect = false;
	//private Ship ship;

	//public Ship Ship{
	//	get{return ship;}
	//}

	public PeacePlayerState PlayerState {get; private set;}

	public string EndPointName{
	
		get{return remotePlayer.Client.RemoteEndPoint.ToString();}
	}

	public void QueueOutgoingMessage(PeaceMessage message){

		if(message != null){
			outgoing.Enqueue(message);
		}else{

			Console.WriteLine("Server sent a null message to: " + remotePlayer.Client.RemoteEndPoint);
		}
	}

	public int IncomingMessageCount(){

		return incoming.Count;
	}
	public int OutgoingMessageCount(){

		return outgoing.Count;
	}

	public bool HasIncomingMessages(){

		return incoming.Count > 0;
	}

	public PeaceMessage GetIncomingMessage(){

		if(incoming.Count > 0){

			return incoming.Dequeue();
		}else{

			Console.WriteLine("Client: " + remotePlayer.Client.RemoteEndPoint + " tried to to grab a message when queue is empty");
			return null;
		}
	}

	public RemotePeacePlayer(TcpClient listener/*, PeaceServer server*/){

		Console.WriteLine("created player connection of: " + listener.Client.RemoteEndPoint.ToString());

		//this.server = server;
		this.remotePlayer = listener;

		connectionThread = new Thread(PeaceConnection);
		connectionThread.Start();
	}


	public void PeaceConnection(){
	
		Console.WriteLine("started player connection of: " + remotePlayer.Client.RemoteEndPoint.ToString());

		DateTime lastMessageSendTime = DateTime.Now;

		NetworkStream networkStream = null;
		BinaryReader reader = null;
		BinaryWriter writer = null;

		try{
			networkStream = remotePlayer.GetStream();
			reader = new BinaryReader(networkStream);
			writer = new BinaryWriter(networkStream);
				
			while(!shouldDisconnect){

				Thread.Sleep(50);

				if(networkStream.DataAvailable){

					string messageString = reader.ReadString();

					Console.WriteLine("got message string: " + messageString);

					HandleMessage(Newtonsoft.Json.JsonConvert.DeserializeObject<PeaceMessage>(messageString));

					/*
					Console.WriteLine("CLIENT GOT MESSAGE: " + message);

					if(message.StartsWith("meow")){

						writer.Write("hey, you a cat?" + message);

					}else{

						writer.Write("i don't know what you are mr: " + message);
					}

					Console.WriteLine("Waiting for next message");
					*/

				}else if(outgoing.Count > 0){ // send any pending messages to players
					

					while(outgoing.Count > 0){
					
						writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(outgoing.Dequeue()));
					}

				}else if(DateTime.Now > lastMessageSendTime.AddSeconds(5)){

					PeaceMessage keepaliveMessage = new PeaceMessage(PeaceMessageType.KEEPALIVE, "");

					writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(keepaliveMessage));

					lastMessageSendTime = DateTime.Now;
				}
					
			
			}
		}catch(Exception e){

			//Console.WriteLine("player disconnected with exception: " + e);
			Console.WriteLine("player disconnected");

		}finally{

			ChangeState(PeacePlayerState.DISCONNECTED);

			//QueueOutgoingMessage(new PeaceMessage(PeaceMessageType.DISCONNECT));

			Console.WriteLine("We lost connection to player");

			writer.Close();
			reader.Close();
			networkStream.Close();

			remotePlayer.Close();

			connectionThread.Join();
		}
	}

	public void Disconnect(){

		Console.WriteLine("Disconnecting normally");

		shouldDisconnect = true;
	}


	private void ChangeState(PeacePlayerState newState){

		PlayerState = newState;

		/*
		if(OnPlayerStateChanged != null){

			OnPlayerStateChanged(this);
		}
		*/
	}

	private void HandleMessage(PeaceMessage message){

		if(message.messageType == PeaceMessageType.JOIN){

			//Console.WriteLine("going into join with data: " + messageSplit[1]);
			HandleJoin(message.messageContent);
		}else{

			if(message != null){
				incoming.Enqueue(message);
			}
		}
		
	}

	private void HandleJoin(string playerDataString){
	
		//Console.WriteLine("in join with string: " + playerDataString);

		playerData = Newtonsoft.Json.JsonConvert.DeserializeObject<PeacePlayerData>(playerDataString);

		//ship = new Ship(playerData.ship);

		Console.WriteLine("JOINING player name is: " + playerData.name);
		Console.WriteLine("player ship name: " + playerData.ship.shipName);

		ChangeState(PeacePlayerState.JOINED);
	}
}


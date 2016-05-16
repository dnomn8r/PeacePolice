using UnityEngine;
using System.Collections;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

using PeaceServerClientCommon;

public delegate void MessageReceivedHandler(PeaceMessage message);
public delegate void StatusChangeHandler(string status);

public class PeaceClient {

	public event StatusChangeHandler OnStatusChanged;
	public event MessageReceivedHandler OnMessageReceived;

	public string message;

	TcpClient client;
	NetworkStream networkStream;
	BinaryWriter writer;
	BinaryReader reader;

	Thread connectionThread;

	public PeaceClient(){

		client = new TcpClient();
	}

	public void Connect(string ip, int port, string playerData){

		Debug.Log("Attempting to connect to: " + ip + " on port: " + port);

		try{
			client.Connect(IPAddress.Parse(ip), port);
		
			networkStream = client.GetStream();
			
			writer = new BinaryWriter(networkStream);
			reader = new BinaryReader(networkStream);
	
		 	connectionThread = new Thread(ConnectionUpdate);
			connectionThread.Start();

			if(OnStatusChanged != null){
				OnStatusChanged("Connected to " + ip + ":" + port + ", waiting to be matched");
			}

			SendPeaceMessage(new PeaceMessage(PeaceMessageType.JOIN, playerData));

		}catch(SocketException){

			if(OnStatusChanged != null){
				OnStatusChanged("Could not connect to: " + ip + ":" + port);
			}
		}
	}

	public void SendPeaceMessage(PeaceMessage message){

		if(writer == null){

			Debug.LogError("can't send message with content: " + message.messageContent + " not connected");
			return;
		}

		writer.Write(Newtonsoft.Json.JsonConvert.SerializeObject(message));

	}

	void ConnectionUpdate(){

		while(true){

			if(networkStream.DataAvailable){

				string responseString = reader.ReadString();

				HandleServerMessage(Newtonsoft.Json.JsonConvert.DeserializeObject<PeaceMessage>(responseString));
			}

			Thread.Sleep(20);
		}
	}

	private void HandleServerMessage(PeaceMessage serverMessage){

		if(serverMessage.messageType == PeaceMessageType.KEEPALIVE) {

			HandleKeepAlive();

		}else{

			OnMessageReceived(serverMessage);

			/*
			NEED TO MOVE THIS TO LOADMENU, CHANGE ONSTATUSCHANGED TO INCLUDE PEACEMESSAGE
			if(response.messageType == PeaceMessageType.GAME_START) {

				Application.LoadLevel ("Combat");
			}

			Debug.Log("client got message: " + response);
*/
		}
	}

	private void HandleKeepAlive(){

		//Debug.Log("got a keepalive message");
	}

	public void Disconnect(){

		if(reader != null){
			reader.Close();
		}
		if(writer != null){
			writer.Close();
		}
		if(networkStream != null){
			networkStream.Close();
		}
		if(client != null){
			client.Close();
		}
		if(connectionThread != null){
			connectionThread.Join();
		}
	}

	/*
	void Update(){

		if(Input.GetKeyDown(KeyCode.A)){

			writer.Write(message + "\n weee" + (count++));
		}

		if(networkStream.DataAvailable){
		
			Debug.Log ("response is: " + reader.ReadString());
		}
	}

	void OnDisable(){

		writer.Close();
		reader.Close();
		networkStream.Close();
		client.Close();

	}
	*/

}

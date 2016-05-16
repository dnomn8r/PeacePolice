using System;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;


using System.Runtime.Serialization;

namespace PeaceServerClientCommon{

	public enum PeaceMessageType {INVALID, KEEPALIVE, JOIN, DISCONNECT, GAME_START, 
		GAME_WIN_DISCONNECT, WIN, LOSS, ACTIONS, TURN}

	public enum PeacePlayerState {NONE, DISCONNECTED, JOINED, IN_GAME}

	public class PeaceConstants{

		public static double DISTANCE_BETWEEN_SHIPS = 1000;

		public static int SECONDS_PER_TURN = 60;
	
		public static int THRUST_DIVISOR = 40;
	}

	public class TypeNameSerializationBinder : SerializationBinder
	{
		public string TypeFormat { get; private set; }

		public TypeNameSerializationBinder(string typeFormat)
		{
			TypeFormat = typeFormat;
		}
		/*
	public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
	{
		assemblyName = null;
		typeName = serializedType.Name;
	}
	*/

		public override Type BindToType(string assemblyName, string typeName)
		{
			//string resolvedTypeName = string.Format(TypeFormat, typeName);

			//return Type.GetType(resolvedTypeName, true);

			return Type.GetType(typeName);
		}
	}

	public class PeaceCommunicationConstants{

		//public static string MESSAGE_TYPE_SEPARATOR = "###";

	}

	// what the server sends out to all players once everyone has sent their actions
	public class PeaceTurn{

		//public List<PeaceMessage> actionMessages;

		public Dictionary<string, int> energy;

		public List<ComponentActionResult> results;
	}

	public class PeaceMessage{

		[JsonConverter(typeof(StringEnumConverter))]
		public PeaceMessageType messageType{
			get;
			set;
		}

		public string messageContent;

		public PeaceMessage(){}

		public PeaceMessage(PeaceMessageType type){

			messageType = type;
			messageContent = null;
		}

		public PeaceMessage(PeaceMessageType type, string content){

			messageType = type;
			messageContent = content;
		}

		public override string ToString(){
			return string.Format ("[PeaceMessage: messageType={0}]", messageType + " with content: " +
			messageContent);
		}
	}

	public class PeacePlayerData{

		public string name;
		public ShipData ship;
	}


	public class AbilityEntry{

		public string abilityName;

		public List<AbilityTarget> targets = new List<AbilityTarget>();

		public AbilityEntry(){}

		public AbilityEntry(string abilityName){

			this.abilityName = abilityName;
		}
	}

	public class AbilityTarget{

		public string playerID;
		public int roomID;

		public AbilityTarget(){}

		public AbilityTarget(string playerID, int roomID){

			this.playerID = playerID;
			this.roomID = roomID;
		}
	}

	public class CombatAction{

		public string playerID;

		public int roomID;

		public List<AbilityEntry> abilityEntryList = new List<AbilityEntry>();

		public CombatAction(){}

		public CombatAction(string playerID, int roomID){

			this.playerID = playerID;
			this.roomID = roomID;
		}

		public override string ToString(){

			string resultString = "";

			resultString += "\n\tplayerID: " + playerID;
			resultString += "\n\troomID: " + roomID;

			foreach(AbilityEntry entry in abilityEntryList){

				resultString += "\n\t\tability: " + entry.abilityName;

				foreach(AbilityTarget target in entry.targets){

					resultString += "\n\t\t\troom: " + target.roomID + " of player: " + target.playerID;	
				}
			}

			return resultString;
		}	
	}

}


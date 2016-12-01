using System;
using System.Collections;
using System.Collections.Generic;
using PeaceServerClientCommon;
using XnaGeometry;

public abstract class ComponentActionResult{

	public abstract void ProcessResult(CombatScheduler scheduler);

	//public virtual override string ToString(){return "";}

	public int slice;
}

public class InstantDamageEvent : DamageEvent{

	public string sourcePlayerID;
	public int sourceRoomID;

	public InstantDamageEvent(int slice, string sourcePlayerID, int sourceRoomID,
	                          string targetPlayerID, int targetRoom, int damage) :
			base(slice, targetPlayerID, targetRoom, damage){


		this.sourcePlayerID = sourcePlayerID;
		this.sourceRoomID = sourceRoomID;
	}

	// for instant damage event, make sure we are still alive when we fire, since we could have been destroyed before
	// we fired by something else

	public override void ProcessResult(CombatScheduler scheduler){

		//Console.WriteLine("Source player ID: " + sourcePlayerID + " with room ID: " + sourceRoomID + " deals instant damage of: " + 
		  //                damage + " to player: " + targetPlayerID + " room ID: " + targetRoom);


		Player sourcePlayer = scheduler.GetPlayer(sourcePlayerID);

		ShipRoom sourceRoom = sourcePlayer.CurrentShip.GetRoom(sourceRoomID);


		if(!sourceRoom.CurrentComponent.IsDestroyed()){

			Player targetPlayer = scheduler.GetPlayer(targetPlayerID);
			
			ShipRoom room = targetPlayer.CurrentShip.GetRoom(targetRoomID);
			
			room.OnTakeDamage(damage);

			//Console.WriteLine("Source player: " + sourcePlayer.Name + " with room ID: " + sourceRoom.ID + " deals instant damage of: " + 
			  //                damage + " to player: " + targetPlayer.Name + " room ID: " + room.ID);
		}

	}
}

public class ActivationEvent : ComponentActionResult{

	public string sourcePlayerID;
	public int sourceRoomID;

	public ActivationEvent(int slice, string sourcePlayerID, int sourceRoomID){

		this.slice = slice;
		this.sourcePlayerID = sourcePlayerID;
		this.sourceRoomID = sourceRoomID;
	}

	public override string ToString(){
		
		string resultString = "";
		
		resultString += "\tType: ACTIVATION";
		resultString += "\n\tsource player id: " + sourcePlayerID;
		resultString += "\n\tsource room id: " + sourceRoomID;
		resultString += "\n\tslice: " + slice;
		
		return resultString;
	}

	public override void ProcessResult(CombatScheduler scheduler){
		
		Player sourcePlayer = scheduler.GetPlayer(sourcePlayerID);
		
		ShipRoom sourceRoom = sourcePlayer.CurrentShip.GetRoom(sourceRoomID);
		
		if(!sourceRoom.CurrentComponent.IsDestroyed()){

			sourceRoom.CurrentComponent.OnActivate();
		}
	}
}

public class AimEvent : ComponentActionResult{

	public string sourcePlayerID;
	public int sourceRoomID;
	public string targetPlayerID;
	public int targetRoomID;

	public AimEvent(int slice, string sourcePlayerID, int sourceRoomID, string targetPlayerID, int targetRoom){

		this.slice = slice;

		this.sourcePlayerID = sourcePlayerID;
		this.sourceRoomID = sourceRoomID;
		this.targetPlayerID = targetPlayerID;
		this.targetRoomID = targetRoom;
	}

	public override string ToString(){

		string resultString = "";

		resultString += "\tType: DAMAGE";
		resultString += "\n\ttarget player id: " + targetPlayerID;
		resultString += "\n\ttarger room id: " + targetRoomID;
		resultString += "\n\tslice: " + slice;

		return resultString;
	}
		
	public override void ProcessResult(CombatScheduler scheduler){

		Player sourcePlayer = scheduler.GetPlayer(sourcePlayerID);

		ShipRoom sourceRoom = sourcePlayer.CurrentShip.GetRoom(sourceRoomID);

		if(!sourceRoom.CurrentComponent.IsDestroyed()){

			Player targetPlayer = scheduler.GetPlayer(targetPlayerID);

			ShipRoom targetRoom = targetPlayer.CurrentShip.GetRoom(this.targetRoomID);

			sourceRoom.CurrentComponent.OnTakeAim(targetRoom);
		}
	}
}

public class ChargeEvent : ComponentActionResult{

	public string sourcePlayerID;
	public int sourceRoomID;

	public ChargeEvent(int slice, string sourcePlayerID, int sourceRoomID){

		this.slice = slice;

		this.sourcePlayerID = sourcePlayerID;
		this.sourceRoomID = sourceRoomID;
	}

	public override string ToString(){

		string resultString = "";

		resultString += "\tType: DAMAGE";
		resultString += "\n\tsource player id: " + sourcePlayerID;
		resultString += "\n\tsource room id: " + sourceRoomID;
		resultString += "\n\tslice: " + slice;

		return resultString;
	}

	public override void ProcessResult(CombatScheduler scheduler){

		Player sourcePlayer = scheduler.GetPlayer(sourcePlayerID);

		ShipRoom sourceRoom = sourcePlayer.CurrentShip.GetRoom(sourceRoomID);

		if(!sourceRoom.CurrentComponent.IsDestroyed()){
			
			sourceRoom.CurrentComponent.OnCharge();
		}
	}
}

public class FireEvent : ComponentActionResult{

	public string sourcePlayerID;
	public int sourceRoomID;
	public string targetPlayerID;
	public int targetRoomID;
	public int currentProjectileSpeed;
	public int damage;

	public FireEvent(int slice, string sourcePlayerID, int sourceRoomID, string targetPlayerID,
		int currentProjectileSpeed, int targetRoom, int damage){

		this.slice = slice;

		this.sourcePlayerID = sourcePlayerID;
		this.sourceRoomID = sourceRoomID;
		this.targetPlayerID = targetPlayerID;
		this.targetRoomID = targetRoom;
		this.currentProjectileSpeed = currentProjectileSpeed;
		this.damage = damage;
	}

	public override string ToString(){

		string resultString = "";

		resultString += "\tType: FIRE";
		resultString += "\n\ttarget player id: " + targetPlayerID;
		resultString += "\n\ttarger room id: " + targetRoomID;
		resultString += "\n\tslice: " + slice;

		return resultString;
	}

	public override void ProcessResult(CombatScheduler scheduler){

		Player sourcePlayer = scheduler.GetPlayer(sourcePlayerID);

		ShipRoom sourceRoom = sourcePlayer.CurrentShip.GetRoom(sourceRoomID);

		// if we are still alive when this event fires, then schedule our damage event
		if(!sourceRoom.CurrentComponent.IsDestroyed()){

			Player targetPlayer = scheduler.GetPlayer(targetPlayerID);

			ShipRoom targetRoom = targetPlayer.CurrentShip.GetRoom(this.targetRoomID);

			int hitNumber = RandomNumber.Range(0, 10000);

			int dodgeChance = (int)(targetRoom.Ship.CurrentDodgeChance * 10000);

			bool isHit = hitNumber > dodgeChance;

			Console.WriteLine("hit num: " + hitNumber + " dodge chance: " + dodgeChance + " hit? " + isHit);

			sourceRoom.CurrentComponent.OnFire(targetRoom, currentProjectileSpeed, isHit);

			int targetSlice = slice;

			// if it's not an instant attack
			if(currentProjectileSpeed != -1){
				
				float distance = (float)((targetRoom.position - sourceRoom.position) +
				                 new Vector3(PeaceConstants.DISTANCE_BETWEEN_SHIPS, 0, 0)).Length();


				float speedPerSlice = currentProjectileSpeed * CombatScheduler.TIME_SLICE_LENGTH;

				if(speedPerSlice == 0){

					System.Console.WriteLine("projectile speed is 0!");
				}

				int slicesNeededToReachTarget = (int)(distance / speedPerSlice) + 1;

				targetSlice += slicesNeededToReachTarget;
			}

			//int targetSlice = slice + slicesNeededToReachTarget;

			//Console.WriteLine("room owner id: " + component.Room.OwnerID + " room id: " + component.Room.ID);

			if(isHit){

//				float damageMultiplier = sourceRoom.CurrentComponent.MaxValueOfStatus("enhance");
//
//				damage = (int)(damage * ((100.0f + damageMultiplier) / 100.0f));
//
//				Console.WriteLine("damage miltipleu: " + damageMultiplier + " new dmg: " + damage);
				int adjustedDamage = sourceRoom.CurrentComponent.GetAdjustedDamage(damage);


				DamageEvent newDamageEvent = new DamageEvent(targetSlice, targetRoom.OwnerID, targetRoom.ID, adjustedDamage);

				System.Console.WriteLine("added damage event with owner: " + sourceRoom.OwnerID + 
					" and damage: " + damage);

				scheduler.AddActionResult(newDamageEvent);
			}else{

				MissEvent newMissEvent = new MissEvent(targetSlice, targetRoom.OwnerID, targetRoom.ID);

				System.Console.WriteLine("added miss event with owner: " + sourceRoom.OwnerID);

				scheduler.AddActionResult(newMissEvent);
			}



		}
	}
}

public class AOEFireEvent : FireEvent{

	public int aoe;

	public AOEFireEvent(int slice, string sourcePlayerID, int sourceRoomID, string targetPlayerID,
		int currentProjectileSpeed, int targetRoom, int damage, int aoe)
		: base(slice, sourcePlayerID, sourceRoomID, targetPlayerID, currentProjectileSpeed, targetRoom, damage){
	
		this.aoe = aoe;

	}

	public override string ToString(){

		string resultString = base.ToString();

		resultString += "\n\aoe: " + aoe;

		return resultString;
	}

	public override void ProcessResult(CombatScheduler scheduler){

		Player sourcePlayer = scheduler.GetPlayer(sourcePlayerID);

		ShipRoom sourceRoom = sourcePlayer.CurrentShip.GetRoom(sourceRoomID);

		// if we are still alive when this event fires, then schedule our damage event
		if(!sourceRoom.CurrentComponent.IsDestroyed()){

			Player targetPlayer = scheduler.GetPlayer(targetPlayerID);

			ShipRoom targetRoom = targetPlayer.CurrentShip.GetRoom(this.targetRoomID);

			sourceRoom.CurrentComponent.OnFire(targetRoom, currentProjectileSpeed, true);

			int targetSlice = slice;

			// if it's not an instant attack
			if(currentProjectileSpeed != -1){

				float distance = (float)((targetRoom.position - sourceRoom.position) +
					new Vector3(PeaceConstants.DISTANCE_BETWEEN_SHIPS, 0, 0)).Length();


				float speedPerSlice = currentProjectileSpeed * CombatScheduler.TIME_SLICE_LENGTH;

				if(speedPerSlice == 0){

					System.Console.WriteLine("projectile speed is 0!");
				}

				int slicesNeededToReachTarget = (int)(distance / speedPerSlice) + 1;

				targetSlice += slicesNeededToReachTarget;
			}

		
			foreach(ShipRoom room in targetPlayer.CurrentShip.GetRoomsWithinRange(targetRoom, aoe)){

				int adjustedDamage = sourceRoom.CurrentComponent.GetAdjustedDamage(damage);

				MissileDamageEvent newDamageEvent = new MissileDamageEvent(targetSlice, room.OwnerID, room.ID, adjustedDamage);

				System.Console.WriteLine("added missile damage event with owner: " + sourceRoom.OwnerID + 
				                         " and damage: " + damage + " to room: " + room.ID);

				scheduler.AddActionResult(newDamageEvent);
			}

		}
	}
}



public class DamageEvent : ComponentActionResult{

	public string targetPlayerID;
	public int targetRoomID;

	public int damage;

	public DamageEvent(int slice, string targetPlayerID, int targetRoom, int damage){

		this.slice = slice;
		this.targetPlayerID = targetPlayerID;
		this.targetRoomID = targetRoom;
		this.damage = damage;
	}

	public override string ToString(){

		string resultString = "";

		resultString += "\tType: DAMAGE";
		resultString += "\n\ttarget player id: " + targetPlayerID;
		resultString += "\n\ttarger room id: " + targetRoomID;
		resultString += "\n\tdamage: " + damage;
		resultString += "\n\tslice: " + slice;

		return resultString;
	}

	// for regular damage events, we don't care who the source was, we deal the damage no matter what.
	// used for things like missiles, etc..
	public override void ProcessResult(CombatScheduler scheduler){
	
		Player targetPlayer = scheduler.GetPlayer(targetPlayerID);
		ShipRoom room = targetPlayer.CurrentShip.GetRoom(targetRoomID);

		List<Shield> protectiveShields = targetPlayer.CurrentShip.GetShieldsProtectingRoom(room);

		Console.WriteLine("taking damage, shield count: " + protectiveShields.Count);

		if(protectiveShields.Count > 0 && damage > 0){ // if damage is negative, don't use shields

			for(int i = 0; i < protectiveShields.Count; ++i){

				protectiveShields[i].OnAbsorbDamage(damage / protectiveShields.Count);
			}
		}else{
			room.OnTakeDamage(damage);
		}
	}
}

// missiles can be stopped by "flak" which checks for upcoming damage events and may remove them from the list
public class MissileDamageEvent : DamageEvent{

	public MissileDamageEvent(int slice, string targetPlayerID, int targetRoom, int damage) : 
		base(slice, targetPlayerID, targetRoom, damage){

	}
}

public class MissEvent : ComponentActionResult{
	
	public string targetPlayerID;
	public int targetRoomID;

	public MissEvent(int slice, string targetPlayerID, int targetRoom){
		
		this.slice = slice;
		this.targetPlayerID = targetPlayerID;
		this.targetRoomID = targetRoom;
	}
	
	public override string ToString(){
		
		string resultString = "";
		
		resultString += "\tType: " + this.GetType().ToString();
		resultString += "\n\ttarget player id: " + targetPlayerID;
		resultString += "\n\ttarger room id: " + targetRoomID;
		resultString += "\n\tslice: " + slice;
		
		return resultString;
	}

	public override void ProcessResult(CombatScheduler scheduler){
		
		Player targetPlayer = scheduler.GetPlayer(targetPlayerID);
		ShipRoom room = targetPlayer.CurrentShip.GetRoom(targetRoomID);
		
		room.OnDodge();

	}
}

public class CombatScheduler{

	private Dictionary<string, Player> players = new Dictionary<string, Player>();

	public const float TIME_SLICE_LENGTH = 0.1f;

	private int currentSlice = 0;

//	public static float TimeSliceLength{
//		
//		get{return timeSliceLength;}
//	}
	
	private Dictionary<int, List<ComponentActionResult>> actionEvents = new Dictionary<int, List<ComponentActionResult>>();

	private Dictionary<int, List<IEnumerator<int>>> scheduledBehaviours = new Dictionary<int, List<IEnumerator<int>>>();

	public int CurrentSlice{

		get{return currentSlice;}
		set{currentSlice = value;}
	}

	public CombatScheduler(Dictionary<string, Player> players){

		this.players = players;

		foreach(KeyValuePair<string, Player> kvp in players){

			Console.WriteLine("in combat scheduler player owner id: " + kvp.Value.CurrentShip.OwnerID);

		}
	}

	public Player GetPlayer(string playerName){

		return players[playerName];
	}

	public bool HasEvents(){

		return actionEvents.Count != 0 || scheduledBehaviours.Count != 0;
	}

	public bool ExecuteNextSlice(){

		//Debug.Log ("executing slice: " + currentSlice);

		if(actionEvents.ContainsKey(currentSlice)){
			
			foreach(ComponentActionResult actionResult in actionEvents[currentSlice]){

				actionResult.ProcessResult(this);
				//damageEvent.targetRoom.OnTakeDamage(damageEvent.damage);
			}
			
			actionEvents[currentSlice].Clear();
			
			actionEvents.Remove(currentSlice);
		}

		if(scheduledBehaviours.ContainsKey(currentSlice)){

			foreach(IEnumerator<int> behaviour in scheduledBehaviours[currentSlice]){

				if(behaviour.MoveNext()){

					AddBehaviour(currentSlice + behaviour.Current, behaviour);
				}
			}

			scheduledBehaviours[currentSlice].Clear();

			scheduledBehaviours.Remove(currentSlice);
		}

		currentSlice++;

		Console.WriteLine("action event count: " + actionEvents.Count + " behaciur count: " + scheduledBehaviours.Count);


		if(actionEvents.Count == 0 && scheduledBehaviours.Count == 0){

			foreach(Player player in players.Values){

				player.CurrentShip.ResetCombatTurn();
			}

			return true;
		}else{

			return false;
		}
	}


	public void AddBehaviour(int slice, IEnumerator<int> behaviour){

		if(!scheduledBehaviours.ContainsKey(slice)){

			scheduledBehaviours.Add(slice, new List<IEnumerator<int>>());
		}
	
		scheduledBehaviours[slice].Add(behaviour);
	}

	public void AddActionResult(ComponentActionResult actionEvent){

		// if we are trying to schedule to the current execution slice or a previous slice, we can't, 
		// so we schedule it one slice in the future
		if(actionEvent.slice <= currentSlice){

			actionEvent.slice = currentSlice + 1; 
		}

		if(!actionEvents.ContainsKey(actionEvent.slice)){

			actionEvents.Add(actionEvent.slice, new List<ComponentActionResult>());
		}

		actionEvents[actionEvent.slice].Add(actionEvent);

	}

	public void AddActionResults(List<ComponentActionResult> events){

		foreach(ComponentActionResult actionEvent in events){

			if(!actionEvents.ContainsKey(actionEvent.slice)){

				actionEvents.Add(actionEvent.slice, new List<ComponentActionResult>());
			}

			actionEvents[actionEvent.slice].Add(actionEvent);
		
		}     
	}


	/*
	public void AddDamageEvent(DamageEvent damageEvent){
		Debug.Log ("adding damage event at slice: " + damageEvent.slice);

		if(!damageEvents.ContainsKey(damageEvent.slice)){
			
			damageEvents.Add(damageEvent.slice, new List<DamageEvent>());
		}
		
		damageEvents[damageEvent.slice].Add(damageEvent);

	}
	*/


}

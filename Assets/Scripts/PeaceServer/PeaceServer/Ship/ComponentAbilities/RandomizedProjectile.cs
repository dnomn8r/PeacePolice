using System.Collections;
using System.Collections.Generic;
using XnaGeometry;
using PeaceServerClientCommon;
using System;

public class RandomizedProjectileData : AbilityData{
	
	public int minDamage = 0;
	public int maxDamage = 0;
	public int shots = 1;
	public int delayBetweenShots = 0;
	public int projectileSpeed = 200;
}

public class RandomizedProjectile : ComponentAbility{
	
	private int startingMinDamage;
	private int startingMaxDamage;
	private int startShots;
	private int startDelayBetweenShots;
	private int startProjectileSpeed; 
	
	private int currentMinDamage;
	private int currentMaxDamage;
	private int currentShots;
	private int currentDelayBetweenShots;
	private int currentProjectileSpeed;

	private ShipRoom previousRoom = null;

	public override TargetInfo GetTargetInfo(){
		
		return new TargetInfo(TargetType.SELF);	
	}

	public override string ToString(){
		
		string result = "";
		
		result += "Damage: " + startingMinDamage + "-" + startingMaxDamage + "\n";
		result += "Shots: " + startShots + "\n";
		result += "Shot time: " + startDelayBetweenShots + "\n";
		
		return result;
	}
	
	protected override void ParseDefinition(string definitionString){
		
		RandomizedProjectileData data = Newtonsoft.Json.JsonConvert.DeserializeObject<RandomizedProjectileData>(definitionString);

		startingMinDamage = data.minDamage;
		startingMaxDamage = data.maxDamage;
		startShots = data.shots;
		startDelayBetweenShots = data.delayBetweenShots;
		startProjectileSpeed = data.projectileSpeed;
		
		currentMinDamage = startingMinDamage;
		currentMaxDamage = startingMaxDamage;

		currentShots = startShots;
		currentDelayBetweenShots = startDelayBetweenShots;
		currentProjectileSpeed = startProjectileSpeed;
	
	}
	
	public override List<ComponentActionResult> ExecuteAction(PeaceGame game, List<ShipRoom> targets){

		int currentSlice = 0;

		System.Console.WriteLine("executing action for randomized projectiles");

		List<ComponentActionResult> newResult = new List<ComponentActionResult>();

		List<Player> opposingPlayers = game.OpposingPlayers(component.Room.OwnerID);

		System.Console.WriteLine("num opposing players: " + opposingPlayers.Count);

		for(int i=0;i<currentShots;++i){

			ShipRoom target = opposingPlayers[RandomNumber.Range(0, opposingPlayers.Count - 1)].CurrentShip.GetRandomRoom();

			// if we need to change targets, add an aim delay and send an aim event
			if(target != previousRoom){

				AimEvent newAimEvent = new AimEvent(currentSlice, component.Room.OwnerID, 
					component.Room.ID, target.OwnerID, target.ID);

				newResult.Add(newAimEvent);

				System.Console.WriteLine("adding aim event with target: " + component.Room.OwnerID + " and target id: " + target.ID);

				currentSlice += 5; // TODO: make this data driven

				previousRoom = target;
			}
				

			int damage = RandomNumber.Range(currentMinDamage, currentMaxDamage + 1);

			//Console.WriteLine("original random dmg: " + damage);

			currentSlice += currentDelayBetweenShots; 

			//float damageMultiplier = component.MaxValueOfStatus("enhance");

			//damage = damage * ((100.0f + damageMultiplier) / 100.0f);

			//Console.WriteLine("damage multiplier: " + damageMultiplier + " new dmg: " + damage);

			FireEvent newFireEvent = new FireEvent(currentSlice, component.Room.OwnerID, 
				component.Room.ID, target.OwnerID, currentProjectileSpeed, target.ID, damage);


			System.Console.WriteLine("added fire event with owner: " + component.Room.OwnerID + " and damage: " + damage);
		
			newResult.Add(newFireEvent);
			

		}

		return newResult;
	}

}

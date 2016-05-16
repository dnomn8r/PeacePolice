using System.Collections;
using System.Collections.Generic;
using XnaGeometry;
using PeaceServerClientCommon;
using System;

public class LaserData : AbilityData{
	
	public int minDamage = 0;
	public int maxDamage = 0;
	public int targets = 1;
	public int shotsPerTarget = 1;
	public int delayBetweenShots = 0;
	public int projectileSpeed = 200;
}

public class Laser : ComponentAbility{
	
	private int startingMinDamage;
	private int startingMaxDamage;
	private int startingTargets;
	private int startShotsPerTarget;
	private int startDelayBetweenShots;
	private int startProjectileSpeed; 
	
	private int currentMinDamage;
	private int currentMaxDamage;
	private int currentShotsPerTarget;
	private int currentDelayBetweenShots;
	private int currentProjectileSpeed;

	private ShipRoom previousRoom = null;

	public override TargetInfo GetTargetInfo(){
		
		return new TargetInfo(TargetType.ENEMY, startingTargets);	
	}

	public override string ToString(){
		
		string result = "";
		
		result += "Damage: " + startingMinDamage + "-" + startingMaxDamage + "\n";
		result += "Targets: " + startingTargets + "\n";
		result += "Shot time: " + startDelayBetweenShots + "\n";
		
		return result;
	}
	
	protected override void ParseDefinition(string definitionString){
		
		LaserData data = Newtonsoft.Json.JsonConvert.DeserializeObject<LaserData>(definitionString);

		startingMinDamage = data.minDamage;
		startingMaxDamage = data.maxDamage;
		startingTargets = data.targets;
		startShotsPerTarget = data.shotsPerTarget;
		startDelayBetweenShots = data.delayBetweenShots;
		startProjectileSpeed = data.projectileSpeed;
		
		currentMinDamage = startingMinDamage;
		currentMaxDamage = startingMaxDamage;

		currentShotsPerTarget = startShotsPerTarget;
		currentDelayBetweenShots = startDelayBetweenShots;
		currentProjectileSpeed = startProjectileSpeed;
	
	}
	
	public override List<ComponentActionResult> ExecuteAction(PeaceGame game, List<ShipRoom> targets){

		int currentSlice = 0;

		System.Console.WriteLine("executing action for laser");

		List<ComponentActionResult> newResult = new List<ComponentActionResult>();

		foreach(ShipRoom target in targets){

			// if we need to change targets, add an aim delay and send an aim event
			if(target != previousRoom){

				AimEvent newAimEvent = new AimEvent(currentSlice, component.Room.OwnerID, 
					component.Room.ID, target.OwnerID, target.ID);

				newResult.Add(newAimEvent);

				System.Console.WriteLine("adding aim event with target: " + component.Room.OwnerID + " and target id: " + target.ID);

				currentSlice += 5; // TODO: make this data driven

				previousRoom = target;
			}


			for(int i=0;i<currentShotsPerTarget;++i){

				int damage = RandomNumber.Range(currentMinDamage, currentMaxDamage + 1);

				currentSlice += currentDelayBetweenShots; 

				FireEvent newFireEvent = new FireEvent(currentSlice, component.Room.OwnerID, 
					component.Room.ID, target.OwnerID, currentProjectileSpeed, target.ID, damage);


				System.Console.WriteLine("added fire event with owner: " + component.Room.OwnerID + " and damage: " + damage);
			
				newResult.Add(newFireEvent);
			
			}
		}

		return newResult;
	}

}

using System.Collections;
using System.Collections.Generic;
using PeaceServerClientCommon;

public class Beam : ComponentAbility{

	private class BeamData{
	
		public int minDamage = 0;
		public int maxDamage = 0;
		public int targets = 1;

		public int chargeTime = 0;	
	}

	public int startingMinDamage;
	public int startingMaxDamage;
	public int startingTargets;
	public int startChargeTime;
	
	private int currentMinDamage;
	private int currentMaxDamage;

	private ShipRoom previousRoom = null;
	
	public override TargetInfo GetTargetInfo(){
		
		return new TargetInfo(TargetType.ENEMY, startingTargets);	
	}
	

	public override string ToString(){
		
		string result = "";
		
		result += "Damage: " + startingMinDamage + "-" + startingMaxDamage + "\n";
		result += "Targets: " + startingTargets + "\n";
		result += "Fire time: " + startChargeTime + "\n";
		
		return result;
	}
	
	protected override void ParseDefinition(string definitionString){
		
		BeamData data = Newtonsoft.Json.JsonConvert.DeserializeObject<BeamData>(definitionString);
		
		startingMinDamage = data.minDamage;
		startingMaxDamage = data.maxDamage;
		startingTargets = data.targets;
		startChargeTime = data.chargeTime;
		
		currentMinDamage = startingMinDamage;
		currentMaxDamage = startingMaxDamage;
	}
		
	public override List<ComponentActionResult> ExecuteAction(PeaceGame game, List<ShipRoom> targets){

		int currentSlice = 0;

		System.Console.WriteLine("executing action for beam weapon");

		List<ComponentActionResult> newResult = new List<ComponentActionResult>();

		foreach(ShipRoom target in targets){

			// if we need to change targets, add an aim delay and send an aim event
			if(target != previousRoom){

				AimEvent newAimEvent = new AimEvent(currentSlice, component.Room.OwnerID, 
					component.Room.ID, target.OwnerID, target.ID);

				newResult.Add(newAimEvent);

				System.Console.WriteLine("adding aim event with target: " + component.Room.OwnerID + " and target id: " + target.ID);

				currentSlice += 25; // TODO: make this data driven, beam takes longer than laser :P

				previousRoom = target;
			}


			float damage = RandomNumber.Range(currentMinDamage, currentMaxDamage + 1);

			newResult.Add(new ChargeEvent(currentSlice, component.Room.OwnerID, component.Room.ID));

			currentSlice += startChargeTime; 

			float damageMultiplier = component.MaxValueOfStatus("enhance");

			damage = damage * ((100.0f + damageMultiplier) / 100.0f);


			FireEvent newFireEvent = new FireEvent(currentSlice, component.Room.OwnerID, 
				component.Room.ID, target.OwnerID, -1, target.ID, (int)damage); // -1 = SUPER SPEED FOR BEAM WEAPONS!

			System.Console.WriteLine("added fire event with owner: " + component.Room.OwnerID + " and damage: " + damage);

			newResult.Add(newFireEvent);


		}

		return newResult;
	}
	
}

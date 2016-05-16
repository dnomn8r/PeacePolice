using System.Collections;
using System.Collections.Generic;
using PeaceServerClientCommon;

public class Heal : ComponentAbility{

	private class HealData{
	
		public int heal = 0;
		public int healTime = 0;
		public int targets = 1;			
	}

	public int startingHeal;
	public int startingHealTime;
	public int startingTargets;

	private ShipRoom previousRoom;

	public override TargetInfo GetTargetInfo(){
		
		return new TargetInfo(TargetType.FRIENDLY, startingTargets);	
	}

	public override string ToString(){
		
		string result = "";
		
		result += "Heal: " + startingHeal + "\n";
		result += "Heal Time: " + startingHealTime + "\n";
		result += "Targets: " + startingTargets + "\n";
		
		return result;
	}
	
	protected override void ParseDefinition(string definitionString){
		
		HealData data = Newtonsoft.Json.JsonConvert.DeserializeObject<HealData>(definitionString);
		
		startingHeal = data.heal;
		startingHealTime = data.healTime;
		startingTargets = data.targets;
	}
	

	public override List<ComponentActionResult> ExecuteAction(PeaceGame game, List<ShipRoom> targets){

		int currentSlice = 0;

		System.Console.WriteLine("executing action for heal weapon");

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
				

			newResult.Add(new ChargeEvent(currentSlice, component.Room.OwnerID, component.Room.ID));

			currentSlice += startingHealTime; 

			FireEvent newFireEvent = new FireEvent(currentSlice, component.Room.OwnerID, 
				component.Room.ID, target.OwnerID, -1, target.ID, -startingHeal); // -1 = SUPER SPEED FOR BEAM WEAPONS!

			System.Console.WriteLine("added heal event with owner: " + component.Room.OwnerID + " and amount: " + startingHeal);

			newResult.Add(newFireEvent);


		}

		return newResult;
	}


}

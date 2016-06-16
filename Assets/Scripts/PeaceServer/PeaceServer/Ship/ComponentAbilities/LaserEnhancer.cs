using System.Collections;
using System.Collections.Generic;
using PeaceServerClientCommon;
using System;

public class LaserEnhancer : ComponentAbility{

	private class LaserEnhanceData{

		public string status = "";
		public int bonusPercent = 0;
	}
	
	public int startingBonusPercent = 0;

	public string status;
	
	public override TargetInfo GetTargetInfo(){
		
		return new TargetInfo(TargetType.SELF);	
	}
	
	public override string ToString(){
		
		string result = "";
		
		result += "Damage Bonus: " + startingBonusPercent + "%" + "\n";
		
		return result;
	}
	
	protected override void ParseDefinition(string definitionString){
		
		LaserEnhanceData data = Newtonsoft.Json.JsonConvert.DeserializeObject<LaserEnhanceData>(definitionString);

		startingBonusPercent = data.bonusPercent;

		status = data.status;

		Console.WriteLine("parsing laser enhance: bonus: " + startingBonusPercent + " status: " + status);
	}


	public override void Initialize(ShipComponent c){

		base.Initialize(c);

		component.OnActivationEvent += OnActivate;
	}

	public void OnActivate(){

		//Console.WriteLine("*********ACTIVATING ENHANCE");
		foreach(ShipRoom room in component.Room.GetAdjacentRooms()){

			if(room.CurrentComponent != null){

				room.CurrentComponent.AddStatus(new ShipComponentStatus(status, startingBonusPercent, 1));
			}
		}
	}
		
	public override List<ComponentActionResult> ExecuteAction(PeaceGame game, List<ShipRoom> targets){

		List<ComponentActionResult> actions = new List<ComponentActionResult>();

		// we have an activation event, but it does nothing, except for visuals
		ActivationEvent newActivationEvent = new ActivationEvent(1, component.Room.OwnerID, component.Room.ID);

		actions.Add(newActivationEvent);

		return actions;
	}
	
}

using System.Collections;
using System.Collections.Generic;
using PeaceServerClientCommon;

public class Engine : ComponentAbility{

	private class EngineData{
	
		public int thrust = 0;
	}
	
	public int startingThrust;
	private int currentThrust;

	public int CurrentThrust{
		get{return currentThrust;}
	}

	public override TargetInfo GetTargetInfo(){
		
		return new TargetInfo(TargetType.SELF);
	}

	public override void Initialize(ShipComponent c){
		
		base.Initialize(c);

		component.OnActivationEvent += OnActivate;
	}
	
	public override string ToString(){
		
		string result = "";
		
		result += "Thrust: " + startingThrust + "\n";
		
		return result;
	}
	
	protected override void ParseDefinition(string definitionString){
		
		EngineData data = Newtonsoft.Json.JsonConvert.DeserializeObject<EngineData>(definitionString);
		
		startingThrust = data.thrust;
		currentThrust = startingThrust;
	}

	public void OnActivate(){
		
		CurrentComponent.Room.Ship.AddThrust(currentThrust);
	}

	public override List<ComponentActionResult> ExecuteAction(PeaceGame game, List<ShipRoom> targets){

		int currentSlice = 0;

		System.Console.WriteLine("executing action for engine");
		
		List<ComponentActionResult> actions = new List<ComponentActionResult>();
		
		// TODO: make the 1 data driven

		ActivationEvent newActivationEvent = new ActivationEvent(1, component.Room.OwnerID, component.Room.ID);
		
		actions.Add(newActivationEvent);
		
		return actions;
	}
	
}

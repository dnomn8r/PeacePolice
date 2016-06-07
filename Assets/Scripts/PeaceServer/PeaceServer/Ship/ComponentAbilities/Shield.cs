using System.Collections;
using System.Collections.Generic;
using System;
using PeaceServerClientCommon;

public class Shield : ComponentAbility{
	
	private class ShieldData{
	
		public int gain = 0;
		public int max = 0;
		public int range = 0;	
	}

	public int startingGain;
	public int startingMax;
	public int startingRange;

	private int currentGain;
	private int currentMax;
	private int currentRange;
	private int currentStrength = 0;
	
	//private List<ShieldEffect> coveredRooms = new List<ShieldEffect>();
	//private BasicShield shieldController;

	public int CurrentRange{
		get{return currentRange;}
	}

	public int CurrentStrength{
		get{return currentStrength;}

		set{
			//int previousStrength = currentStrength;
			currentStrength = value;
		
			//updateShieldStrength(previousStrength);
			//updateShieldStrength(currentStrength);
		}
	}

	public override TargetInfo GetTargetInfo(){
		
		return new TargetInfo(TargetType.SELF);	
	}

	public override string ToString(){
		
		string result = "";
		
		result += "Shield Gain/Use: " + startingGain + "\n";
		result += "Max Shield: " + startingMax + "\n";
		result += "Range: " + startingRange + "\n";
		
		return result;
	}

	protected override void ParseDefinition(string definitionString){
		
		ShieldData data = Newtonsoft.Json.JsonConvert.DeserializeObject<ShieldData>(definitionString);
		
		startingGain = data.gain;
		startingMax = data.max;
		startingRange = data.range;

		currentGain = startingGain;
		currentMax = startingMax;
		currentRange = startingRange;
	}

	public override void Initialize(ShipComponent c){

		base.Initialize(c);

		//shieldController = GetComponentInChildren<BasicShield>();

		component.OnActivationEvent += OnActivate;

		CurrentStrength = startingMax / 2;

		//shieldController.SetShield(this);
	}

	public void OnAbsorbDamage(int damage){

		Console.WriteLine("ABSORBING DAMAGE: " + damage);

		CurrentStrength = Math.Max(0, CurrentStrength - damage);

		Console.WriteLine("STRENGTH NOW: " + CurrentStrength);


		component.OnShieldDamage(damage);
	}


	public void OnActivate(){

		CurrentStrength = Math.Min(currentMax, currentStrength + currentGain);

		Console.WriteLine("ADDING SHIELD STRENGTH, TOTAL IS: " + CurrentStrength);
	}

	public override List<ComponentActionResult> ExecuteAction(PeaceGame game, List<ShipRoom> targets){

		//CurrentStrength = Math.Min(currentMax, currentStrength + currentGain);
		List<ComponentActionResult> actions = new List<ComponentActionResult>();

		// TODO: make the 20 data driven
		ActivationEvent newActivationEvent = new ActivationEvent(20, component.Room.OwnerID, component.Room.ID);

		actions.Add(newActivationEvent);

		return actions;
	}

}

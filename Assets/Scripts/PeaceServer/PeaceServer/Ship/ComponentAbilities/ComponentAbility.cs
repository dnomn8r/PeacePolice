using System.Collections;
using System.Collections.Generic;
using PeaceServerClientCommon;

public enum TargetType {NONE, SELF, FRIENDLY, ENEMY}


public class TargetInfo{
	
	public TargetType type = TargetType.NONE;
	
	public int numTargets = 1;
	
	public TargetInfo(){
		
		type = TargetType.NONE;
		numTargets = 0;
	}
	
	public TargetInfo(TargetType t){
		
		type = t;
		numTargets = 1;
	}
	
	public TargetInfo(TargetType t, int num){
	
		type = t;
		numTargets = num;
	}
}

public class AbilityData{
	
	public string type;
	public string details;

	public override string ToString(){
	
		string result = "";
		
		result += "Type: " + type + "\n";
		result += "Details: " + details + "\n";
		
		return result;
	}
}



public abstract class ComponentAbility{
	
	protected ShipComponent component;

	protected AbilityData abilityData;

	public virtual void ActivatePassives(){}
	
	public abstract TargetInfo GetTargetInfo();
	
	protected abstract void ParseDefinition(string definition);

	public AbilityData AbilityData{

		set{

			//System.Console.WriteLine("setting ability data: " + value.type + " and details: " + value.details);
			abilityData = value;
			ParseDefinition(abilityData.details);
		}

		get{return abilityData;}
	}

	public ShipComponent CurrentComponent{

		get{return component;}
	}

	public virtual void Initialize(ShipComponent c){
	
		component = c;
	}
	
	public virtual List<ComponentActionResult> ExecuteAction(PeaceGame game, List<ShipRoom> targetRooms){

		return new List<ComponentActionResult>();
	}

}



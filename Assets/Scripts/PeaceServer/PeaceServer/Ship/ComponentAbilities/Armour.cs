using System.Collections;

public class Armour : ComponentAbility{

	private class ArmourData{
	
		public int armourBonus = 0;
	}
	
	public int armourBonus;
	
	
	public override TargetInfo GetTargetInfo(){
		
		return new TargetInfo(TargetType.NONE);
	}
	
	public override void ActivatePassives(){

		foreach(ShipRoom room in component.Room.GetAdjacentRooms()){
		
			if(room.CurrentComponent != null){
				
				room.CurrentComponent.AddArmour(armourBonus);
			}
		}
	}
	
	 
	public override string ToString(){
		
		string result = "";
		
		result += "Adjacent Bonus Armour: " + armourBonus + "\n";
		
		return result;
	}
	
	
	protected override void ParseDefinition(string definitionString){
		
		ArmourData data = Newtonsoft.Json.JsonConvert.DeserializeObject<ArmourData>(definitionString);
		
		armourBonus = data.armourBonus;
	}
	
	
}

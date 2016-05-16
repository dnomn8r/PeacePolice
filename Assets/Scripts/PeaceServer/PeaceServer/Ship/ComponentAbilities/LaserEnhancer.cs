using System.Collections;

public class LaserEnhancer : ComponentAbility{

	private class LaserEnhanceData{
	
		public int bonusPercent = 0;
	}
	
	public int startingBonusPercent = 0;
	
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
	}
	
}

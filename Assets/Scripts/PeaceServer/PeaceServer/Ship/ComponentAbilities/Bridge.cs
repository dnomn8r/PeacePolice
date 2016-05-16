using System.Collections;


public class Bridge : ComponentAbility{
	
	
	public override TargetInfo GetTargetInfo(){
		
		return new TargetInfo();
	}
	
	public override string ToString(){
		
		return "";
	}	
	
	protected override void ParseDefinition(string definitionString){
		
	}
	
}

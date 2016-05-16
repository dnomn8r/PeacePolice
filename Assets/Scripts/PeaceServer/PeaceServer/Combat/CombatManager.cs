using System.Collections;
using System.Collections.Generic;


public class CombatManager{

	private Dictionary<int, Player> players = new Dictionary<int, Player>();

	private static CombatManager instance;
	
	public static CombatManager Instance{
		
		get{return instance;}	
	}
	
	public CombatManager(){
		
		instance = this;	
	}
	


}

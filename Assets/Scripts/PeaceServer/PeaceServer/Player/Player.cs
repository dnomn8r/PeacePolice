using System.Collections;
using System.Collections.Generic;
using XnaGeometry;

using PeaceServerClientCommon;

using System;

public class Player{

	private const int BASE_MAX_ENERGY = 1000;
	private const int BASE_RECHARGE = 200;

	private int currentEnergy;
	private int maxEnergy;
	private int currentRecharge;

	[Newtonsoft.Json.JsonIgnore] private Ship currentShip;

	private PeacePlayerData playerData;

	[Newtonsoft.Json.JsonIgnore]public string Name{
		
		get{return playerData.name;}
	}

	[Newtonsoft.Json.JsonIgnore] public Ship CurrentShip{
		
		get{return currentShip;}	
	}
		
	public bool IsDead(){

		return !currentShip.HasFunctionalBridge();
	}

	public int CurrentRecharge{

		get{return currentRecharge;}
	}

	public int CurrentEnergy{
		
		get{return currentEnergy;}
	}
	
	public int MaxEnergy{
		
		get{return maxEnergy;}	
	}

	public int NextEnergy{

		get{return Math.Min(currentEnergy + currentRecharge, maxEnergy);}
	}

	public void UseEnergy(int amount){

		currentEnergy = Math.Max(currentEnergy - amount, 0);
	}

	public int AdvanceTurn(){

		currentEnergy = NextEnergy;

		currentShip.AdvanceTurn();

		return currentEnergy;
	}

	public void SetEnergy(int energy){

		currentEnergy = energy;
	}

	public Player(PeacePlayerData playerData){

		this.playerData = playerData;

		Console.WriteLine("creating player with name: " + playerData.name);
		currentShip = new Ship(playerData.ship, playerData.name);

		maxEnergy = BASE_MAX_ENERGY;
		currentEnergy = maxEnergy / 2;
		currentRecharge = BASE_RECHARGE;

	}
	
}

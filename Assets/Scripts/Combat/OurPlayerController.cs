using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PeaceServerClientCommon;

public class OurPlayerController : PlayerController{

	private ShipComponent selectedComponent = null;

	public ShipRoomController selectedRoomController = null;

	private TargetChooser currentTargetChooser = null;

	private List<CombatAction> combatActions = new List<CombatAction>();

	public List<CombatAction> CombatActions{
		get{return combatActions;}
	}

	public ShipComponent SelectedComponent{

		get{return selectedComponent;}
	}
		

	void OnEnable(){

		UnityCombatManager.Instance.OnRefresh += Refresh;
	}

	void OnDisable(){

		if(UnityCombatManager.Instance != null){

			UnityCombatManager.Instance.OnRefresh -= Refresh;

			UnityCombatManager.Instance.Cleanup();
		}
	}

	void Start(){

		UnityCombatManager.Instance.AddPlayerController(this);

		player = UnityCombatManager.Instance.GetLocalPlayer();

		display.SetPlayer(player);
	}



	void OnPlacedRoomSelected(ShipRoomController roomController){

		ShipComponent roomComponent = roomController.room.CurrentComponent;

		if(roomComponent != null && roomComponent.IsSelectable()){

			// deselect if you click on a selected component
			if(selectedComponent != null && roomComponent == selectedComponent){
			
				currentTargetChooser.Cleanup();
				Destroy(currentTargetChooser.gameObject);

				UnityCombatManager.Instance.Refresh();

				DeselectCurrentComponent();

			}else{

				int energyLeft = player.CurrentEnergy - CurrentEnergyUsed();

				// if we have an action here already, we remove it

				CombatAction existingAction = null;

				foreach(CombatAction action in combatActions){

					if(action.playerID == roomController.room.Ship.OwnerID && action.roomID == roomController.room.ID){
						existingAction = action;
						break;
					}
				}

				if(existingAction != null){

					combatActions.Remove(existingAction);
					display.UpdateEnergy(CurrentEnergyUsed());

					UpdateDodgeChance();

					UnityCombatManager.Instance.Refresh();


					DeselectCurrentComponent();
				
				}else if(roomComponent.currentEnergy <= energyLeft){
				
					selectedComponent = roomComponent;
					selectedRoomController = roomController;
					selectedRoomController.Select();


					//Debug.Log("we selected componewnt with abilities: " + selectedComponent.Abilities);

					GameObject chooserObject = GameObject.Instantiate(Resources.Load("TargetChooser")) as GameObject;
					
					currentTargetChooser = chooserObject.GetComponent<TargetChooser>();
					
					currentTargetChooser.Initialize(this, selectedComponent);
				}

			}
		}

		//display.UpdateEnergy(CurrentEnergyUsed());
	}

	public void UpdateDodgeChance(){

		float potentialThrust = 0.0f;

		foreach(CombatAction action in combatActions){

			ShipRoom room = player.CurrentShip.GetRoom(action.roomID);

			if(room.CurrentComponent != null){

				foreach(ComponentAbility ability in room.CurrentComponent.Abilities){

					Engine engine = ability as Engine;

					if(engine != null){

						potentialThrust += engine.CurrentThrust;
					}
				}
			}
		}

		potentialThrust = potentialThrust / player.CurrentShip.Rooms.Count;

		float thrust = potentialThrust / (potentialThrust + PeaceServerClientCommon.PeaceConstants.THRUST_DIVISOR);

		display.UpdateDodgeChange(thrust);
	}

	public void DeselectCurrentComponent(){

		if(selectedRoomController != null){

			selectedRoomController.Deselect();
			selectedComponent = null;
			selectedRoomController = null;

			//display.UpdateEnergy(CurrentEnergyUsed());
		}
	}
	
	public void AddCombatAction(CombatAction action){
		
		combatActions.Add(action);
		display.UpdateEnergy(CurrentEnergyUsed());
		UpdateDodgeChance();

		PrintActions();

		UnityCombatManager.Instance.Refresh();


	}

	public string PrintActions(){
		
		//Debug.Log("current actions list for player: " + player.Name + " with count: " + combatActions.Count);

		string actionString = "";

		foreach(CombatAction action in combatActions){

			actionString += action + "\n";
			//Debug.Log("action: " + action);
			/*
			Debug.Log("\tplayerID: " + action.playerID);
			Debug.Log("\troomID: " + action.roomID);


			foreach(AbilityEntry entry in action.abilityEntryList){
		
				Debug.Log("\t\tability: " + entry.abilityName);
				
				foreach(AbilityTarget target in entry.targets){
				
					Debug.Log("\t\t\troom: " + target.roomID + " of player: " + target.playerID);	
				}
			}
			*/
		}

		return actionString;
		//CombatManager.Instance.RefreshDisplays();
	}
	
	public void Refresh(){

		DeselectCurrentComponent();

		if(currentTargetChooser != null){
			currentTargetChooser.Cleanup();
			Destroy(currentTargetChooser.gameObject);
		}

		display.ShipDisplay.ClearHighlights();

		display.ShipDisplay.HighlightActivatedRooms(combatActions);

	}

//	/*
//	public void ExecuteActions()
//	{
//		ServerResult sr = Server.Instance.DoTurn(combatActions, availableEnergy);
//
//		foreach(KeyValuePair<ShipComponent, CombatAction> kvp in combatActions){
//		
//			foreach(KeyValuePair<ComponentAbility, List<ShipRoom>> kvp2 in kvp.Value.abilityTargetList){
//				
//				if(availableEnergy >= kvp.Key.currentCost){
//				
//					kvp2.Key.ExecuteAction(kvp2.Value);
//					
//					availableEnergy -= kvp.Key.currentCost;
//				}
//			}
//		}
//		
//		combatActions.Clear();
//
//		foreach(var kvp in sr.actionResults)
//		{
//			foreach(var kvp2 in kvp.Value)
//			{
//				kvp2.ProcessResult(kvp.Key);
//			}
//		}
//
//		availableEnergy = sr.currentEnergy;
//		
//		availableEnergy = Mathf.Min(availableEnergy + currentRechargeRate * CombatManager.TURNS_PER_INTERVAL, maxEnergy);
//	
//		currentRechargeRate = Mathf.Min(currentRechargeRate + BASE_RECHARGE_INCREASE, BASE_MAX_RECHARGE);
//	}
//	*/
//
//

	public int CurrentEnergyUsed(){

		//int energy = player.CurrentEnergy;

		int energyUsed = 0;

		foreach(CombatAction action in combatActions){

			ShipComponent comp = player.CurrentShip.GetRoom(action.roomID).CurrentComponent;

			energyUsed += comp.currentEnergy;
		}
		
		return energyUsed;	
	}	

	
}

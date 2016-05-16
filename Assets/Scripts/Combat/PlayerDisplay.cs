using UnityEngine;
using System.Collections;

public class PlayerDisplay : MonoBehaviour {
	
	[SerializeField]private TextMesh playerNameText;
	
	public TextMesh availableEnergyText;
	public TextMesh nextEnergyText;

	private ShipDisplay shipDisplay;
	[SerializeField] private Transform shipMount;

	[SerializeField]private TextMesh nextDodgeChance;

	[SerializeField]private TextMesh dodgeChance;

	public ShipDisplay ShipDisplay { get { return shipDisplay; } }

	private Player player;

	void Awake(){
		
		playerNameText.text = "<not connected>";
		availableEnergyText.text = "";
		nextEnergyText.text = "";
		dodgeChance.text = "Dodge: 0%";
	}
	
	public void SetPlayer(Player p){

		player = p;

		playerNameText.text = player.Name;

		SetCurrentShip(player.CurrentShip);

		UpdateEnergy();
	}

	public void UpdateDodgeChange(float chance){

		float chancePercent = chance * 100.0f;

		if(nextDodgeChance != null){
			nextDodgeChance.text = string.Format("Next Dodge: {0:0.00} %", chancePercent);
		}
	}

	public void UpdateEnergy(int energyUsedThisTurn = 0){

		int availableEnergy = player.CurrentEnergy - energyUsedThisTurn;

		availableEnergyText.text = availableEnergy.ToString();

		nextEnergyText.text = (player.NextEnergy - energyUsedThisTurn) + " / " + player.MaxEnergy;
	}

	public void SetCurrentShip(Ship ship){

		createNewShip();

		shipDisplay.SetShip(ship);

		Transform shipTransform = shipDisplay.transform;

		shipTransform.parent = shipMount;
		shipTransform.localPosition = Vector3.zero;
		shipTransform.localEulerAngles = Vector3.zero;
		shipTransform.localScale = Vector3.one;
	}

	protected virtual void createNewShip(){

		clearCurrentShip();

		shipDisplay = ShipDisplay.CreateNewShip();

		Transform shipTransform = shipDisplay.transform;

		shipTransform.parent = shipMount;
		shipTransform.localPosition = Vector3.zero;
		shipTransform.localEulerAngles = Vector3.zero;
		shipTransform.localScale = Vector3.one;
	}

	private void clearCurrentShip(){

		if(shipDisplay != null){

			Destroy(shipDisplay.gameObject);	
		}	

		shipDisplay = null;
	}

	void Update(){


		float currentDodge = shipDisplay.ship.CurrentDodgeChance * 100.0f;

		dodgeChance.text = string.Format("Dodge: {0:0.00} %", currentDodge);

	}

}

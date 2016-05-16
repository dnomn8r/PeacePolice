using System.Collections;
using System.Collections.Generic;
using System;

public delegate void FireEventDelegate(ShipRoom room, int projectileSpeed, bool isHit);
public delegate void AimEventDelegate(ShipRoom room);
public delegate void ChargeEventDelegate();
public delegate void ShieldEventDelegate(int damage);
public delegate void ActivationEventDelegate();

public class ShipComponent{
	
	private ShipRoom room;

	private ShipComponentData componentData;

	public ShipComponentData ComponentData{

		get {return componentData;}
	}

	public int maxSystem;
	public int maxArmour;

	public int currentSystem;
	public int currentArmour;
	public int currentEnergy;
		
	public event FireEventDelegate OnFireEvent;
	public event AimEventDelegate OnAimEvent;
	public event ShieldEventDelegate OnShieldEvent;

	public event ActivationEventDelegate OnActivationEvent;
	public event ChargeEventDelegate OnChargeEvent;

	private List<ComponentAbility> componentAbilities = new List<ComponentAbility>();

	public ShipComponent(ShipRoom room, ShipComponentData data){

		this.room = room;

		componentData = data;

		maxSystem = data.system;
		maxArmour = data.armour;
		currentSystem = data.system;
		currentArmour = data.armour;
		currentEnergy = data.energy;

		foreach(AbilityData ability in data.abilities){

			Console.WriteLine("parsing ability: " + ability.type + " with details: " + ability.details);

			ComponentAbility abilityBase = Activator.CreateInstance(Type.GetType(ability.type)) as ComponentAbility;

			abilityBase.AbilityData = ability;
			abilityBase.Initialize(this);

			componentAbilities.Add(abilityBase);
		}
	}


	public ShipRoom Room{
		
		get{return room;}	
	}

	public ComponentAbility GetAbility(string abilityName){

		foreach(ComponentAbility ability in componentAbilities){

			if(ability.AbilityData.type == abilityName) return ability;
		}

		Console.WriteLine("could not find component ability with name: " + abilityName);
		return null;
	}

	public void AddArmour(int amount){
		
		currentArmour += amount;
	}
	
	public List<ComponentAbility> Abilities{
		
		get{return componentAbilities;}	
	}
	
	public void ActivatePassives(){
	
		foreach(ComponentAbility ability in componentAbilities){
			
			ability.ActivatePassives();
		}	
	}
	
	public bool IsSelectable(){
		
		if(IsDestroyed()) return false;
		
		foreach(ComponentAbility ability in componentAbilities){

			//Debug.Log ("checking ability: " + ability.CurrentComponent.componentData.title);

			if(ability.GetTargetInfo().type != TargetType.NONE) return true;
		}
		
		return false;
	}


	
	public bool IsDestroyed(){
	
		//Debug.Log("current system is: " + currentSystem + " armour: " + currentArmour);

		return currentSystem <= 0 && currentArmour <= 0;
	}

	public void OnShieldDamage(int damage){

		if(OnShieldEvent != null){

			OnShieldEvent(damage);
		}
	}

	public void OnActivate(){

		if(OnActivationEvent != null){

			OnActivationEvent();
		}
	}

	public void OnTakeAim(ShipRoom room){

		if(OnAimEvent != null){

			OnAimEvent(room);
		}
	}

	public void OnCharge(){

		if(OnChargeEvent != null){

			OnChargeEvent();
		}
	}

	public void OnFire(ShipRoom room, int projectileSpeed, bool isHit){

		if(OnFireEvent != null){

			OnFireEvent(room, projectileSpeed, isHit);
		}
	}

	 public void OnTakeDamage(int damage){

		// first deal damage to armour

		if(currentArmour > 0){

			currentArmour -= damage;
			
			// if damage is more than remaining armour, deal it to system
			if(currentArmour < 0){
				
				currentSystem += currentArmour;
			}
		}else{
			
			currentSystem -= damage;
		}
	
		currentArmour = Math.Min(Math.Max(0, currentArmour), maxArmour);
		currentSystem = Math.Min(Math.Max(0, currentSystem), maxSystem);

		//if(currentSystem == 0){

			//Destroy(normalVersion);

			//destroyedVersion.SetActive(true);
		//}
	}
	
}

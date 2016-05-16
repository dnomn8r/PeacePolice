using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PeaceServerClientCommon;

public class ShipComponentController : MonoBehaviour {

	protected GameObject normalVersion;
	protected GameObject destroyedVersion;

	protected ShipComponent component;

	protected bool isDestroyed = false;

	public ShipComponent CurrentComponent{

		get{return component;}
	}
	
	public virtual void InitializeComponent(ShipComponent c){

		component = c;

		component.OnAimEvent += OnAim;
		component.OnFireEvent += OnFire;
		component.OnShieldEvent += OnShieldDamage;
		component.OnChargeEvent += OnCharge;
		component.OnActivationEvent += OnActivate;

		normalVersion = transform.Find("normal").gameObject;
		destroyedVersion = transform.Find("destroyed").gameObject;
		
		destroyedVersion.SetActive(false);
	}

	public virtual void OnShieldDamage(int damage){


	}

	public virtual void OnAim(ShipRoom room){


	}

	public virtual void OnFire(ShipRoom room, int projectileSpeed, bool isHit){


	}

	public virtual void OnCharge(){


	}

	public virtual void OnActivate(){


	}

//	public virtual void Activate(List<AbilityTarget> targets, AbilityData data){
//	
//	
//	}

	public void RefreshState(){

		if(component.IsDestroyed() && !isDestroyed){

			isDestroyed = true;

			Destroy(normalVersion);
			
			destroyedVersion.SetActive(true);

			enabled = false;
		}

	}
	
}

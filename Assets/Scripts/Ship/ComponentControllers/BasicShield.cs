using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BasicShield : ShipComponentController{

	private Shield myShield;
	
	public GameObject shieldVisual;


	public override void InitializeComponent(ShipComponent component){

		base.InitializeComponent(component);

		for(int i = 0; i < component.Abilities.Count; ++i){

			Shield shield = component.Abilities[i] as Shield;

			if(shield != null){

				SetShield(shield);
				break;
			}
		}
	}

	public override void OnShieldDamage(int damage){

		GameObject damageText = GameObject.Instantiate(Resources.Load("GUI/DamageIndicator")) as GameObject;
		damageText.transform.parent = transform;
		damageText.transform.localPosition = Vector3.zero;

		DamageIndicator damageIndicator = damageText.GetComponent<DamageIndicator>();
		damageIndicator.DisplayDamage(damage, true);

	}

	public void SetShield(Shield shield){
		
		myShield = shield;

		//Transform oldParent = shieldVisual.transform.parent;
		//shieldVisual.transform.parent = null;

		shieldVisual.transform.localScale = new Vector3(shield.CurrentRange*2.0f, shield.CurrentRange*2.0f, 1);
		//shieldVisual.transform.parent = oldParent;

	}

	void Update(){

		if(myShield != null){

			if(myShield.CurrentStrength > 0){

				shieldVisual.SetActive(true);
			}else{

				shieldVisual.SetActive(false);
			}

		}
	}

}

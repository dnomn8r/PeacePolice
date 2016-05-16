using UnityEngine;
using System.Collections;

public class InfoWindow : MonoBehaviour{

	public TextProcessor title;
	public TextProcessor desc;
	public TextProcessor system;
	public TextProcessor armour;
	public TextProcessor cost;

	//maybe also a pretty picture one day

	public void OnDisplay(ShipComponent component){

		this.gameObject.SetActive(true);

		title.OnSetText(component.ComponentData.title);
		desc.OnSetText(component.ComponentData.desc);
		system.OnSetText(component.currentSystem + "/" + component.ComponentData.system);
		armour.OnSetText(component.currentArmour + "/" + component.ComponentData.armour);
		cost.OnSetText(component.currentEnergy + "/" + component.ComponentData.energy);
	}

	public void OnHide(){

		this.gameObject.SetActive(false);
	}

}

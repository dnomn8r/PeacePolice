using UnityEngine;
using System.Collections;
using System;

public class ComponentDisplay : MonoBehaviour {
	
	private ShipComponentData componentData;
	
	public TextProcessor titleText;
	public TextProcessor sizeText;
	public TextProcessor structureText;
	public TextProcessor armourText;
	public TextProcessor energyCostText;
	
	public Transform prefabMount;
	
	public TextProcessor abilityText;

	private ShipComponent myComponent;

	public void SetComponentData(ShipComponentData data, ShipComponent component = null){
	
		componentData = data;
		myComponent = component;
		
		titleText.OnSetText(data.title);
		
		sizeText.OnSetText(data.size.ToString());

		armourText.OnSetText("A: " + data.armour);
		structureText.OnSetText("S: " + data.system);

		enabled = myComponent != null;

		energyCostText.OnSetText("Cost: " + data.energy);
		
		GameObject componentModel = GameObject.Instantiate(Resources.Load("Ship/Components/Prefabs/" + data.prefab)) as GameObject;
		
		componentModel.transform.parent = prefabMount;
		componentModel.transform.localPosition = Vector3.zero;
		componentModel.transform.localScale = Vector3.one;
		componentModel.transform.localEulerAngles = Vector3.zero;
		
		string abilityString = "";
		
		foreach(AbilityData ability in data.abilities){
		
			ComponentAbility abilityBase = Activator.CreateInstance(Type.GetType(ability.type)) as ComponentAbility;

			//Debug.Log("abiliyy with name being added: " + ability.type);

			abilityBase.AbilityData = ability;
			//abilityBase.ParseDefinition(ability.details);

			abilityString += abilityBase;
			/*
			ComponentAbility abilityScript = componentModel.AddComponent(ability.type) as ComponentAbility;
			
			abilityScript.ParseDefinition(ability.details);
			
			abilityString += abilityScript;
			*/
		}

		//Debug.Log("ability text: " + abilityString);

		abilityText.OnSetText(abilityString);
		
	}

	void Update(){

		if(myComponent != null){

			armourText.OnSetText("A: " + myComponent.currentArmour + "/" + componentData.armour);

			structureText.OnSetText("S: " + myComponent.currentSystem + "/" + componentData.system);
		}
	}
	
	void OnGUIButtonClicked(GUIButton b){
	
		SendMessageUpwards("OnComponentSelected", componentData);		
	}
	
}

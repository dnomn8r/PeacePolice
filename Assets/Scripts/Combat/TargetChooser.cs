using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PeaceServerClientCommon;



public class TargetChooser : MonoBehaviour {
	
	private ShipComponent selectedComponent;
	
	public Transform targettingTransform;
	
	private Camera selectionCamera;
	
	public TextMesh targetDisplay;
	
	private OurPlayerController owningPlayer;
	
	private ShipRoomController currentTarget = null;
	
	private List<ShipRoomController> potentialTargets = new List<ShipRoomController>();
	
	void Awake(){
	
		selectionCamera = GetComponentInChildren<Camera>();
	}
	
	public void Initialize(OurPlayerController p, ShipComponent component){
	
		owningPlayer = p;
		
		selectedComponent = component;
		
		StartCoroutine(handleComponentAbilities());
	}
	
	private IEnumerator handleComponentAbilities(){
	
		targetDisplay.text = "";

		CombatAction action = new CombatAction(owningPlayer.Player.Name, selectedComponent.Room.ID);

		foreach(ComponentAbility ability in selectedComponent.Abilities){
			
			TargetInfo targetInfo = ability.GetTargetInfo();

			//Debug.Log("target entry type: " + ability.AbilityData.type + " target entry details: " + ability.AbilityData.details);
	
			// if we can only target ourselves, we're done, just add ourselves to the list, and move on to the next ability
			
			if(targetInfo.type == TargetType.SELF){

				//Debug.Log("new entry type: " + ability.AbilityData.type);

				action.abilityEntryList.Add(new AbilityEntry(ability.AbilityData.type));

			}else{
			
				if(targetInfo.numTargets < 1){
				
					Debug.LogError(selectedComponent.ComponentData.title + " target type of 'enemy' or 'friend' needs to have at least 1 target!");
					continue;
				}
				
				if(targetInfo.type == TargetType.ENEMY){
				
					List<PlayerController> enemyPlayerControllers = UnityCombatManager.Instance.GetEnemyPlayerControllers(owningPlayer.Player.Name);
					
					potentialTargets.Clear();

					foreach(PlayerController p in enemyPlayerControllers){
						
						if(p.Display != null){

							foreach(ShipRoomController r in p.Display.ShipDisplay.RoomControllers){
								
								potentialTargets.Add(r);
							}	
						}
					}
					
					yield return StartCoroutine(gatherTargets(action, targetInfo, potentialTargets, ability));
					
				}else if(targetInfo.type == TargetType.FRIENDLY){
					
					potentialTargets.Clear();
						
					if(owningPlayer.Display != null){

						foreach(ShipRoomController r in owningPlayer.Display.ShipDisplay.RoomControllers){
							
							potentialTargets.Add(r);
						}	
					}
						
					yield return StartCoroutine(gatherTargets(action, targetInfo, potentialTargets, ability));
				}
				
			}
		}	
		
		Cleanup();
		
		owningPlayer.AddCombatAction(action);

		Destroy(this.gameObject);
	}

	private IEnumerator gatherTargets(CombatAction action, TargetInfo info, List<ShipRoomController> potentialTargets, 
										ComponentAbility ability){
		
		foreach(ShipRoomController r in potentialTargets){
					
			if(info.type == TargetType.ENEMY){
			
				r.SetHighlight(HighlightType.ENEMY);
			
			}else if(info.type == TargetType.FRIENDLY){
			
				r.SetHighlight(HighlightType.FRIENDLY);
			}
				
			r.GetComponent<GUIObject>().extraListeners.Add(this.gameObject);
		}
		
		
		//List<ShipRoom> selectedTargets = new List<ShipRoom>();
		AbilityEntry newEntry = new AbilityEntry(ability.AbilityData.type);

		//Debug.Log("new entry: " + ability.AbilityData.details);
		//Debug.Log("new entry type: " + ability.AbilityData.type + " which is: " + newEntry.abilityName);

		for(int i=0;i<info.numTargets;++i){
		
			bool plural = (info.numTargets - i) > 1;
			
			if(info.type == TargetType.ENEMY){
			
				targetDisplay.GetComponent<Renderer>().material.color = Color.red;
				targetDisplay.text = "Choose " + (info.numTargets - i) + " enemy room" + (plural ? "s" : "");
			
			}else if(info.type == TargetType.FRIENDLY){
				
				targetDisplay.GetComponent<Renderer>().material.color = Color.green;
				targetDisplay.text = "Choose " + (info.numTargets - i) + " friendly room" + (plural ? "s" : "");
			}
			
			// wait until we set a current target by clicking on it
			while(currentTarget == null){
				
				yield return new WaitForEndOfFrame();	
			}

			newEntry.targets.Add(new AbilityTarget(currentTarget.room.Ship.OwnerID, currentTarget.room.ID));

			currentTarget = null;
		}
		
		resetPotentialTargets();
		
		action.abilityEntryList.Add(newEntry);

	}

	private void resetPotentialTargets(){

		foreach(ShipRoomController r in potentialTargets){
			r.SetHighlight(HighlightType.NONE);
			r.GetComponent<GUIObject>().extraListeners.Remove(this.gameObject);
		}
	}
	
	public void Cleanup(){
		
		resetPotentialTargets();
		
		owningPlayer.DeselectCurrentComponent();
	}
	
	void OnGUIButtonClicked(GUIButton b){
	
		currentTarget = b.GetComponent<ShipRoomController>();
	}
	
	void Update(){
	
		if(Input.GetKeyDown(KeyCode.Escape)){
			
			Cleanup();
			Destroy(this.gameObject);
		
		}else{
			
			Vector3 pos = selectionCamera.ScreenToWorldPoint(Input.mousePosition);
			
			targettingTransform.position = pos;		
		}
	}
}




using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class AbstractEditor : MonoBehaviour {
	
	public Camera editorCamera;
	
	public TextMesh shipRoomText;
	
	public static string HULL_DIRECTORY = "HullDesigns/";
	public static string SHIP_DIRECTORY = "ShipDesigns/";

	public static string SHIP_EXTENSION = ".ship";
	
	public Transform shipMount;

	protected ShipDisplay currentShipController = null;
	
	private string currentShipName = "";
	
	public ShipDisplay CurrentShipController{
		
		get{return currentShipController;}	
	}
	
	public string CurrentShipName{
		
		get{return currentShipName;}
	}
	
	protected virtual void Awake(){
	
		//currentShip = createNewShip);
	}
	
	
	public void OnNewShip(){
		
		createNewShip();	
		
	}
	
	public void SetCurrentShip(Ship ship){
		
		createNewShip();

		currentShipController.SetShip(ship);
	
		Transform shipTransform = currentShipController.transform;
		
		shipTransform.parent = shipMount;
		shipTransform.localPosition = Vector3.zero;
		shipTransform.localEulerAngles = Vector3.zero;
		shipTransform.localScale = Vector3.one;

		shipRoomText.text = currentShipController.ship.ShipName;	
	}
	
	protected virtual void createNewShip(){
		
		clearCurrentShip();
		
		currentShipController = ShipDisplay.CreateNewShip();
		
		Transform shipTransform = currentShipController.transform;
		
		shipTransform.parent = shipMount;
		shipTransform.localPosition = Vector3.zero;
		shipTransform.localEulerAngles = Vector3.zero;
		shipTransform.localScale = Vector3.one;
		
		shipRoomText.text = "<new ship>";
	}
	
	private void clearCurrentShip(){
	
		currentShipName = "";
		
		if(currentShipController != null){
			
			Destroy(currentShipController.gameObject);	
		}	
	}
	
}




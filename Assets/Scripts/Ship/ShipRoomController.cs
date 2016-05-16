using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipRoomController : MonoBehaviour {
	
	public ShipRoom room;

	private ShipDisplay shipController;

	private List<ShipDoor> doors;
	
	private Transform roomTransform;
	private Renderer myRenderer;
	private GUIButton myButton;
	private Collider myCollider;
	
	private bool isPlacing = false;
	private bool isPlaced = false;
	
	private GameObject currentHighlight = null;
	
	public Dictionary<int, ShipDoor> doorDictionary = new Dictionary<int, ShipDoor>();
	
	private GameObject currentComponentModel = null;
	
	private GameObject currentLocationDisplay = null;
	
	//private ShipComponent component = null;
	private ShipComponentController componentController;

	private bool isSelected = false;

	public ShipComponentController ComponentController{

		get{return componentController;}
	}

	public ShipDisplay ShipController{

		get{return shipController;}
		set{shipController = value;}
	}

	public void SetRoom(ShipRoom r){

		room = r;

		//Debug.Log("room set: " + room.ID);

		room.OnDamageEvent += OnTakeDamage;
		room.OnMissEvent += OnDodge;
	}

	void Awake(){
		
		roomTransform = transform.Find("room");
		
		myRenderer = roomTransform.GetComponent<Renderer>();
		myButton = GetComponent<GUIButton>();
		myCollider = roomTransform.GetComponent<Collider>();
		
		if(myButton != null){
			
			myButton.enabled = false;
		}
		
		ShipDoor[] doorArray = GetComponentsInChildren<ShipDoor>();
		
		doors = new List<ShipDoor>(doorArray);
		
		foreach(ShipDoor door in doors){
			
			doorDictionary.Add(door.id, door);	
		}
		
		if(transform.Find("locationDisplay") != null){
			
			currentLocationDisplay = transform.Find("locationDisplay").gameObject;
		}
		
		DisplayLocation();
	}

	public void SetLocation(ShipLocation loc){
		
		room.Location = loc;
		
		DisplayLocation();
	}

	public void OnPlaceInShip(){

		isPlacing = true;	
		isPlaced = true;

		myButton.enabled = true;
	}

	private GameObject createHighlight(Object highlightObject){
		
		GameObject newHighlight = GameObject.Instantiate(highlightObject) as GameObject;
		
		newHighlight.transform.parent = roomTransform;
		newHighlight.transform.localPosition = new Vector3(0,0,-20);
		newHighlight.transform.localEulerAngles = Vector3.zero;
		newHighlight.transform.localScale = Vector3.one;
		
		return newHighlight;
	}

	
	public void RemoveDoor(ShipDoor door){
		
		doors.Remove(door);
		
		Destroy(door.gameObject);
	}

	public Collider Collider{
		
		get{return myCollider;}	
	}
	
	public bool IsSelected(){
		
		return isSelected;	
	}
	
	public void Select(){
		
		isSelected = true;
		SetHighlight(HighlightType.SELECTED);
	}
	
	public void Deselect(){
		
		isSelected = false;
		SetHighlight(HighlightType.NONE);
	}

	public List<ShipDoor> Doors{
		
		get{
			return doors;
		}
	}

	public ShipDoor GetDoor(int id){
		
		if(doorDictionary.ContainsKey(id)){
			
			return doorDictionary[id];
		}else{
			
			Debug.LogError("can't find door with id: " + id + " this should not happen");
			
			return null;	
		}
	}

	public void SetHighlight(HighlightType type){
		
		if(currentHighlight != null){
			
			Destroy(currentHighlight);	
		}
		
		switch(type){
			
		case HighlightType.SELECTED:
			
			currentHighlight = createHighlight(Resources.Load("RoomHighlights/selected"));
			
			break;
			
		case HighlightType.ENEMY:
			
			currentHighlight = createHighlight(Resources.Load("RoomHighlights/enemy"));
			
			break;
			
		case HighlightType.FRIENDLY:
			
			currentHighlight = createHighlight(Resources.Load("RoomHighlights/friend"));
			
			break;
			
		case HighlightType.ACTIVATED:
			
			currentHighlight = createHighlight(Resources.Load("RoomHighlights/activated"));
			
			break;
			
		}
	}
	
	public void DisplayLocation(){

		if(currentLocationDisplay != null){
			
			Destroy(currentLocationDisplay);		
		}

		if(room != null && room.Location == ShipLocation.AFT){

			currentLocationDisplay = GameObject.Instantiate(Resources.Load("RoomLocationDisplay")) as GameObject;
			currentLocationDisplay.transform.parent = transform;
			currentLocationDisplay.transform.localPosition = Vector3.zero;
			currentLocationDisplay.transform.localScale = Vector3.one;
			currentLocationDisplay.name = "locationDisplay";
			
			currentLocationDisplay.transform.eulerAngles = Vector3.zero;
			
			currentLocationDisplay.GetComponent<TextMesh>().text = "E";
			
			currentLocationDisplay.SetActive(componentController == null);	
		}

	}

	public void OnToggleCollider(bool toggle){
		
		myCollider.enabled = toggle;
	}
	
	void OnTriggerEnter(Collider c){
		
		SendMessageUpwards("OnRoomHitCollider", c, SendMessageOptions.DontRequireReceiver);
	}
	
	void OnTriggerExit(Collider c){
		
		SendMessageUpwards("OnRoomLeaveCollider", c, SendMessageOptions.DontRequireReceiver);
	}
	
	public void OnHighlightRoom(bool toggle, Color color){
		
		if(toggle){
			
			myRenderer.material.color = color;
		}else{
			
			myRenderer.material.color = Color.white;
		}
	}

	void OnGUIButtonClicked(GUIButton b){
		
		if(b != myButton) return;

		//Debug.Log ("button clicked: " + b.name + " is placing: " + isPlacing

		if(isPlaced){

			SendMessageUpwards("OnPlacedRoomSelected", this, SendMessageOptions.DontRequireReceiver);
		
		}else if(isPlacing){

			isPlaced = true;
		}
	}

	/*
	void OnGUIButtonPressed(GUIButton b){
		
		if(b != myButton) return;
	}*/

	public void RemoveComponent(){

		if(currentComponentModel != null){
		
			Destroy(currentComponentModel);
		}

		room.CurrentComponent = null;

		if(currentLocationDisplay != null){
			
			currentLocationDisplay.SetActive(true);	
		}
	}

	public void SetComponent(ShipComponent component){

		RemoveComponent();
		
		room.CurrentComponent = component;
		
		currentComponentModel = GameObject.Instantiate(Resources.Load("Ship/Components/Prefabs/" + component.ComponentData.prefab)) as GameObject;
		
		currentComponentModel.transform.parent = transform;
		currentComponentModel.transform.localPosition = new Vector3(0,0,-5);
		currentComponentModel.transform.localScale = Vector3.one;
		currentComponentModel.transform.localEulerAngles = Vector3.zero;

		componentController = currentComponentModel.GetComponent<ShipComponentController>();
	
		// if we have no component controller, just add the very basic default one
		if(componentController == null){

			componentController = currentComponentModel.AddComponent<ShipComponentController>();
		}

		componentController.InitializeComponent(component);

		if(currentLocationDisplay != null){
			
			currentLocationDisplay.SetActive(false);	
		}		
	}

	public void OnDodge(){

		GameObject damageText = GameObject.Instantiate(Resources.Load("GUI/MissIndicator")) as GameObject;
		damageText.transform.parent = transform;
		damageText.transform.localPosition = Vector3.zero;

		MissIndicator missIndicator = damageText.GetComponent<MissIndicator>();
		missIndicator.DisplayMiss();

		if(componentController != null){

			componentController.RefreshState();
		}
	}

	public void OnTakeDamage(int damage){

		GameObject damageText = GameObject.Instantiate(Resources.Load("GUI/DamageIndicator")) as GameObject;
		damageText.transform.parent = transform;
		damageText.transform.localPosition = Vector3.zero;
		
		DamageIndicator damageIndicator = damageText.GetComponent<DamageIndicator>();

		damageIndicator.DisplayDamage(damage);

		if(componentController != null){

			componentController.RefreshState();
		}
	}

	
	void OnDrawGizmos(){
		
		Gizmos.color = Color.red;

		if(room != null){

			foreach(Connection c in room.connections){

				//Gizmos.DrawLine(room.position.ToVector3(), c.position.ToVector3());

				Vector3 pos = new Vector3((float)room.position.X, (float)room.position.Y, 
				                          (float)room.position.Z) + shipController.transform.position;

				Vector3 diff = new Vector3((float)c.position.X, (float)c.position.Y, (float)c.position.Z);

				diff = Quaternion.Euler(new Vector3(0,0,room.rotation)) * diff;

				Vector3 connectionPos = pos + diff;

				Gizmos.DrawLine(pos, connectionPos);
			}
		}
	}

}

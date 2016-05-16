using UnityEngine;
using System.Collections;
using System;

public class RoomComponentDisplay : MonoBehaviour {
	
	public TextProcessor structureText;

	public Transform componentDisplayMount;

	private ShipRoom myRoom;

	public void SetRoom(ShipRoom room){
	
		myRoom = room;

		if(myRoom != null){

			structureText.OnSetText("S: " + myRoom.currentStructure + "/" + ShipRoom.MAX_STRUCTURE);
		}

		if(room.CurrentComponent != null){
		
			GameObject componentPrefab = GameObject.Instantiate(Resources.Load("GUI/ComponentDisplay")) as GameObject;
		
			componentPrefab.transform.parent = componentDisplayMount;
			componentPrefab.transform.localPosition = Vector3.zero;
			componentPrefab.transform.localScale = Vector3.one;
			componentPrefab.transform.localEulerAngles = Vector3.zero;

			componentPrefab.GetComponent<ComponentDisplay>().SetComponentData(room.CurrentComponent.ComponentData, room.CurrentComponent);
		}
	}

	void Update(){

		if(myRoom != null){

			structureText.OnSetText("S: " + myRoom.currentStructure + "/" + ShipRoom.MAX_STRUCTURE);
		}
	}

}

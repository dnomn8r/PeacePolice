using UnityEngine;
using System.Collections;

public class RoomDisplay : MonoBehaviour {
	
	private ShipRoomController currentRoomController;
	
	private Transform myTransform;
	
	void Awake(){
		
		myTransform = transform;
	}
	
	public void ClearDisplay(){
		
		if(currentRoomController != null){
			
			Destroy(currentRoomController.gameObject);	
		}
	}

	public void OnSetRoomData(ShipRoomData data){

		ClearDisplay();

		GameObject roomObject = Resources.Load("ShipRooms/" + data.name) as GameObject;

		GameObject newPiece = GameObject.Instantiate(roomObject) as GameObject;

		newPiece.transform.parent = myTransform;
		newPiece.transform.localPosition = new Vector3(0, 0, 100);
		newPiece.transform.localEulerAngles = Vector3.zero;
		newPiece.transform.localScale = roomObject.transform.localScale;
		newPiece.name = data.name;

		ShipRoom newRoom = new ShipRoom(null, data, 0);

		currentRoomController = newPiece.GetComponent<ShipRoomController>();
		currentRoomController.room = newRoom;
		
		currentRoomController.OnToggleCollider(false);
	}
	
	public void OnGUIButtonPressed(GUIButton b){
		
		if(currentRoomController == null) return;
		
		SendMessageUpwards("OnShipRoomSelected", currentRoomController);			
	}
	
}

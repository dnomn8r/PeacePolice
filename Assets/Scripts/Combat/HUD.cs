using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	[SerializeField]private Camera[] playerCameras;

	[SerializeField]private Transform selectedRoomDisplayMount;
	private GameObject currentlySelectedRoomDisplay;
	private ShipRoomController currentlySelectedRoom = null;

	[SerializeField]private Transform activeRoomDisplayMount;
	private GameObject activeRoomDisplay;
	private ShipRoomController currentlyActiveRoom = null;

	private OurPlayerController ourController;

	void Update () {
	
		if(UnityCombatScheduler.Instance != null && 
			UnityCombatScheduler.Instance.CombatScheduler != null &&
			UnityCombatScheduler.Instance.CombatScheduler.HasEvents()){

			if(currentlySelectedRoomDisplay != null){

				Destroy(currentlySelectedRoomDisplay);
			}
			currentlySelectedRoom = null;

			if(activeRoomDisplay != null){
				Destroy(activeRoomDisplay);
			}

			currentlyActiveRoom = null;

		}else{

			if(ourController == null){
				ourController = FindObjectOfType(typeof(OurPlayerController)) as OurPlayerController;
			}

			if(ourController != null){

				if(ourController.selectedRoomController != currentlyActiveRoom){

					if(activeRoomDisplay != null){
						Destroy(activeRoomDisplay);
					}

					if(ourController.selectedRoomController != null){
					
						currentlyActiveRoom = ourController.selectedRoomController;

						activeRoomDisplay = GameObject.Instantiate(Resources.Load("GUI/RoomDisplay")) as GameObject;

						activeRoomDisplay.transform.parent = activeRoomDisplayMount;
						activeRoomDisplay.transform.localPosition = Vector3.zero;
						activeRoomDisplay.transform.localEulerAngles = Vector3.zero;
						activeRoomDisplay.transform.localScale = Vector3.one;

						RoomComponentDisplay display = activeRoomDisplay.GetComponent<RoomComponentDisplay>();

						display.SetRoom(currentlyActiveRoom.room);
					} else{

						currentlyActiveRoom = null;
					}
				}

			}

			bool noHits = true;

			foreach(Camera currentCamera in playerCameras){
			
				Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

				RaycastHit hit;

				if(Physics.Raycast(ray.origin, ray.direction, out hit, currentCamera.farClipPlane, currentCamera.cullingMask)){ // if the raycast hits something, check if it's a gui object
			
					ShipRoomController room = hit.transform.GetComponent<ShipRoomController>();

					if(room != null){

						noHits = false;

						if(currentlySelectedRoom != room){

							if(currentlySelectedRoomDisplay != null){

								Destroy(currentlySelectedRoomDisplay);
							}
				
							currentlySelectedRoom = room;

							currentlySelectedRoomDisplay = GameObject.Instantiate(Resources.Load("GUI/RoomDisplay")) as GameObject;

							currentlySelectedRoomDisplay.transform.parent = selectedRoomDisplayMount;
							currentlySelectedRoomDisplay.transform.localPosition = Vector3.zero;
							currentlySelectedRoomDisplay.transform.localEulerAngles = Vector3.zero;
							currentlySelectedRoomDisplay.transform.localScale = Vector3.one;

							RoomComponentDisplay display = currentlySelectedRoomDisplay.GetComponent<RoomComponentDisplay>();

							display.SetRoom(currentlySelectedRoom.room);
						}
					}
				} 
			}

			if(noHits){

				if(currentlySelectedRoomDisplay != null){

					Destroy(currentlySelectedRoomDisplay);
				}

				currentlySelectedRoom = null;
			}
		}
	}
}

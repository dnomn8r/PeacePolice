using UnityEngine;
using System.Collections;

public class CameraChooser : MonoBehaviour {
	
	public CameraController player1Camera;
	public CameraController player2Camera;
	

	void Update(){
		
		player1Camera.enabled = Input.mousePosition.x <= Screen.width/2.0f;
		
		player2Camera.enabled = Input.mousePosition.x > Screen.width/2.0f;
	}
	
}

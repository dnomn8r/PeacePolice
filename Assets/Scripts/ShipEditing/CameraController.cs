using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	private Camera myCamera;
	private Transform myTransform;
	
	private float zoomSpeed = 100.0f;
	
	private float moveSpeed = 100.0f;
	
	public int maxZoom = 500;
	public int minZoom = 50;
	
	void Awake(){
		
		myCamera = GetComponent<Camera>();
		myTransform = transform;
		
		if(myCamera == null){
			
			Debug.LogError("camera controller has no camera, removing it");
			
			Destroy(this);
		}
	}
	
	void Update(){
		
		if(PopupManager.Instance.HasPopups()) return;
		
		if(Input.GetKey(KeyCode.A)){
			
			myCamera.orthographicSize += zoomSpeed * Time.deltaTime;
			
			if(myCamera.orthographicSize > maxZoom){
			
				myCamera.orthographicSize = maxZoom;
			}
		}
		
		if(Input.GetKey(KeyCode.Z)){
			
			myCamera.orthographicSize -= zoomSpeed * Time.deltaTime;
			
			if(myCamera.orthographicSize < minZoom){
			
				myCamera.orthographicSize = minZoom;
			}
		}
		
		if(Input.GetKey(KeyCode.LeftArrow)){
			
			myTransform.position += new Vector3(-moveSpeed * Time.deltaTime, 0, 0);	
		}
		
		if(Input.GetKey(KeyCode.RightArrow)){
			
			myTransform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);	
		}
		
		if(Input.GetKey(KeyCode.UpArrow)){
			
			myTransform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);	
		}
		
		if(Input.GetKey(KeyCode.DownArrow)){
			
			myTransform.position += new Vector3(0, -moveSpeed * Time.deltaTime, 0);	
		}
		
	}
}

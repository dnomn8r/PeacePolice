using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GUIObject : MonoBehaviour{
	
	protected Transform myTransform;

	public List<GameObject> extraListeners = new List<GameObject>();

	protected virtual void Awake(){
		
		myTransform = transform;
	}
	
	// Use this for initialization
	protected virtual void Start(){
		
		if(GUIController.Instance == null){
			
			Debug.LogError("Guicontroller was not found, this should never happen!");	
		}
	}
	
	public abstract void OnTouched();
	
	public abstract void OnReleased();
	
	public abstract void OnClicked();
}
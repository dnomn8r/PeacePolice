using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour {

	private static PopupManager instance;
	
	public static PopupManager Instance{
		
		get{return instance;}	
	}
	
	public GameObject popupSystemPrefab;
	
	private List<GameObject> currentPopups = new List<GameObject>();
	
	private Transform myTransform;

	public int PopupCount
	{
		get { return currentPopups.Count; }
	}
	
	/*
	private int tutorialPopupCount = 0;
	
	public bool HasTutorialPopups
	{
		get { return tutorialPopupCount > 0; }
	}
	*/
	
	void Awake(){
		
		instance = this;
		
		myTransform = transform;

		//if(!Application.isEditor){
		
			StartCoroutine(CheckForBackButton ());
		//}
	}
	
	public bool HasPopups(){
	
		return currentPopups.Count > 0;
		
	}
	
	// get rid of all existing popups when switching levels
	void OnLevelWasLoaded(){

		for(int i = currentPopups.Count - 1;i >= 0;i--){

			Popup p = currentPopups[i].GetComponentInChildren<Popup>();

			if(p != null && !p.dontDestroyOnLoad){

				Destroy(currentPopups[i]);
				currentPopups.RemoveAt(i);
			}
		}

		recalculatePopups();

	}
	
	public void OnAddPopup(GameObject newPopup, bool disableFader){
	
		recalculatePopups();

		GameObject newPopupSystem = GameObject.Instantiate(popupSystemPrefab) as GameObject;
		newPopupSystem.transform.parent = myTransform;
		newPopupSystem.transform.localPosition = new Vector3(currentPopups.Count*25000.0f, 10000, 0); //make sure camera don't interfere with each other
		newPopupSystem.transform.localEulerAngles = new Vector3(0, 0, 0);
		//newPopupSystem.name = "Popup - " + currentPopups.Count;
		
		Camera newPopupSystemCamera = newPopupSystem.GetComponent<Camera>();
		
		//newPopupSystemCamera.depth += currentPopups.Count * 0.1f;
		
		newPopup.transform.parent = newPopupSystem.transform;
		// put it right in the middle of it's viewing area, since it's the only thing going to be renderered by this 
		// camera
		newPopup.transform.localPosition = new Vector3(0, 0, newPopupSystemCamera.farClipPlane / 2.0f);
		newPopup.transform.localEulerAngles = Vector3.zero;
	
		
		if(disableFader){
		
			Destroy(newPopupSystem.transform.Find("fader").gameObject);
		}
		
		currentPopups.Add(newPopupSystem);

		recalculatePopups();

		newPopup.SendMessage("OnPopupMounted", SendMessageOptions.DontRequireReceiver);
	}
	
	
	public void OnRemovePopup(GameObject popup){
	
		if(popup.transform.parent != null){
			
			currentPopups.Remove(popup.transform.parent.gameObject);
		
			recalculatePopups();

			StartCoroutine(delayedDestroyPopup(popup));
		}	
	}

	private void recalculatePopups(){

		for(int i=0;i<currentPopups.Count;++i){

			currentPopups[i].name = "Popup - " + i;
			currentPopups[i].GetComponent<Camera>().depth = 80.0f + (i * 0.1f);

		}

	}

	public void RemoveAll(){
	
		foreach(GameObject obj in currentPopups){
			
			Destroy(obj);
		}
		
		currentPopups.Clear();
	}
	
	private IEnumerator delayedDestroyPopup(GameObject popup){
		
		yield return new WaitForEndOfFrame();
		if(popup != null)
		{
			Destroy(popup.transform.parent.gameObject);  // destroy the camera system as well when destroying a popup
		}
	}
	
	public Camera GetActivePopupCamera()
	{
		if(currentPopups.Count == 0) return(null);
		
		return(currentPopups[currentPopups.Count - 1].GetComponent<Camera>());
	}
	
	void OnDestroy(){
		
		instance = null;	
	}
	
	//void Update()
	IEnumerator CheckForBackButton(){
		
		while(true){
			
			bool isTryingToClose = Input.GetKeyUp(KeyCode.Escape);
			
			if(isTryingToClose && GUIController.Instance.enabled)
			{
				if(currentPopups.Count > 0)
				{
					Popup currentPopup = currentPopups[currentPopups.Count - 1].GetComponentInChildren<Popup>();

					if(currentPopup != null)
					{
						if(currentPopup.cancelButton != null || currentPopup.noButton != null && !currentPopup.cantUseBackButton)
						{
							currentPopup.RemovePopup();
						}
					}
					else
					{
						Debug.LogError("Popup is not really a popup!");
					}
				}
			}

			yield return null;
		}
	}
}

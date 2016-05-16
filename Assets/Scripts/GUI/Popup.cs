using UnityEngine;
using System.Collections;

public delegate void PopupCallback();

public class Popup : MonoBehaviour {
	
	public GUIButton yesButton;
	public GUIButton noButton;
	public GUIButton cancelButton;
	
	public bool disableFader = false;
	
	protected PopupCallback yesCallback;
	protected PopupCallback noCallback;
	
	public bool dontDestroyOnLoad = false;

	public bool cantUseBackButton = false;

	private bool isDestroying = false;
	
	protected virtual void Awake(){
		
		transform.position = new Vector3(10000,10000,10000);

		GUIButton[] buttons = GetComponentsInChildren<GUIButton>();

		if(yesButton == null){
			foreach(GUIButton button in buttons){
				if(button.name == "btn_yes"){
					yesButton = button;
					break;
				}
			}
		}

		if(noButton == null){
			foreach(GUIButton button in buttons){
				if(button.name == "btn_no"){
					noButton = button;
					break;
				}
			}
		}

		if(cancelButton == null){
			foreach(GUIButton button in buttons){
				if(button.name == "btn_cancel"){
					cancelButton = button;
					break;
				}
			}
		}
	
	}
	
	protected virtual void Start(){
		
		StartCoroutine(delayedAdd());
	}
	
	// wait until end of frame, to give a chance to any popups being deleted to be cleared from
	// the popup manager.
	private IEnumerator delayedAdd(){
		
		yield return new WaitForEndOfFrame();
		
		PopupManager.Instance.OnAddPopup(gameObject, disableFader);
	}
	
	public void SetCallbackFunctions(PopupCallback yesFunction, PopupCallback noFunction){
		
		yesCallback = yesFunction;
		noCallback = noFunction;
		
	}
	
	/*
	void OnDestroy()
	{
		if(PopupManager.Instance!= null)
		{
			
			PopupManager.Instance.OnRemovePopup(gameObject);
		}
	}
	*/
	
	public virtual void RemovePopup(){
		
		if(!isDestroying){
		
			isDestroying = true;
			PopupManager.Instance.OnRemovePopup(this.gameObject);
		}
	}
	
	public virtual void OnGUIButtonClicked(GUIButton b){
	
		if(b == yesButton || b.name == "btn_yes"){

			if(yesCallback != null){
				
				yesCallback();	
			}
			
			RemovePopup();
			//Destroy(this.gameObject);
			
		}else if(b == noButton || b.name == "btn_no"){
			if(noCallback != null){
				
				noCallback();	
			}
			
			RemovePopup();
			//Destroy(this.gameObject);
		
		}else if(b == cancelButton || b.name == "btn_cancel"){
			
			RemovePopup();
			//Destroy(this.gameObject);
		}
	}
}

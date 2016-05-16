using UnityEngine;
using System.Collections;

public class GUIButton : GUIObject
{	
	private bool isPressed = false;
	
	public GameObject pressObj;
	public GameObject upObj;
	public GameObject greyObj;
	
	public bool disabled = false;

	protected override void Start()
	{
		base.Start();
	
		//This section begins the check for objects with the proper name format within the child hierarchy
		//ie; pressXXXX, PressXXXX, press_XXXX, PrEsSXXX would all become the "press" object without manual assigning
		//This check assumes that there are children on this script's object, and that the name of the child starts with the designated word
		
		if(pressObj == null)
		{
			foreach(Transform child in myTransform)
			{
				if(child.name.ToLower().StartsWith("press")) //press_XXXX would be assigned here
				{
					pressObj = child.gameObject;
					break;
				}
			}
		}
		
		if(upObj == null)
		{
			foreach(Transform child in myTransform)
			{
				if(child.name.ToLower().StartsWith("up"))	//up_XXXX would be assigned here
				{
					upObj = child.gameObject;
					break;
				}
			}
		}
		
		if(greyObj == null)
		{
			foreach(Transform child in myTransform)
			{
				if(child.name.ToLower().StartsWith("grey") || child.name.ToLower().StartsWith("gray"))	//grey_XXXX would be assigned here
				{
					greyObj = child.gameObject;
					break;
				}
			}
		}
		
		updateButtonStates();
	}
	
	void OnEnable()
	{
		if(disabled)
		{
			DisableButton();
		}
		else
		{
			EnableButton();
		}	
	}
	
	
	public void DisableButton()
	{
		disabled = true;	
		
		isPressed = false;
		
		updateButtonStates();
	}
	
	public void EnableButton()
	{
		disabled = false;
		
		isPressed = false;
		
		updateButtonStates();
	}

	public override void OnTouched()
	{
		if(disabled || isPressed) return;
		
		//Debug.Log("tocuhed");
		
		isPressed = true;
		
		updateButtonStates();
		
		if(extraListeners != null)
		{
			foreach (GameObject listener in extraListeners)
			{
				listener.SendMessage("OnGUIButtonPressed", this, SendMessageOptions.DontRequireReceiver);
			}
		}
		
		SendMessageUpwards("OnGUIButtonPressed", this, SendMessageOptions.DontRequireReceiver);
	}
	
	public override void OnReleased()
	{
		if(disabled || !isPressed) return;
		
		//Debug.Log("released");
		
		isPressed = false;
		
		updateButtonStates();
			
		OnButtonUp();
	}
	
	public override void OnClicked()
	{
		if(disabled) return;
		
		isPressed = false;
		
		//Debug.Log("clicked");
		
		updateButtonStates();
		
		if(extraListeners != null)
		{
			foreach (GameObject listener in extraListeners)
			{
				listener.SendMessage("OnGUIButtonClicked", this, SendMessageOptions.DontRequireReceiver);
			}
		}
		
		SendMessageUpwards("OnGUIButtonClicked", this, SendMessageOptions.DontRequireReceiver);
		
		//OnButtonUp();
	}
		
	private void OnButtonUp()
	{
		if(disabled) return;

		if(extraListeners != null)
		{
			foreach (GameObject listener in extraListeners)
			{
				listener.SendMessage("OnGUIButtonUp", this, SendMessageOptions.DontRequireReceiver);
			}
		}
		
		SendMessageUpwards("OnGUIButtonUp", this, SendMessageOptions.DontRequireReceiver);
	}

	/// <summary>
	/// This graphically changes the way the buttons look when interacted with
	/// </summary>
	private void updateButtonStates()
	{
		if(greyObj != null)
		{
			greyObj.SetActive (disabled);
		}
		
		if(disabled) //If the button is disabled, then the grey state should be the only one visible, so we disable the other states
		{
			if(upObj != null){
			
				upObj.SetActive (!disabled);
			}
			
			if(pressObj != null){
			
				pressObj.SetActive (!disabled);
			}
		}
		else //If the button is enabled, then we alternate the pressed and neutral state of the button
		{
			if(upObj != null)
			{
				upObj.SetActive (!isPressed);
			}
			if(pressObj != null)
			{
				pressObj.SetActive (isPressed);
			}
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Linq;

public class InputField : MonoBehaviour {

	public TextMesh textMesh;
	protected Renderer textRenderer;
	
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
	TouchScreenKeyboard keyboard = null;
#endif	
	public string placeholderString;

	public bool usePlaceholderAsDefault = true;

	protected string actualString = "";

	public TouchScreenKeyboardType KeyboardType = TouchScreenKeyboardType.Default;

	public bool passwordField = false;
	
	public bool emailField = false;
	
	public bool clearTextOnClick = true;
	
	public bool clearTextIfSameAsStartup = true;
	
	public bool isGrabbingInput = false;
	
	public bool alphaNumericOnly = false;
	
	public bool alphaNumericSpaceOnly = false;
	
	public int maxLength = 30;
	
	public bool numericOnly = false;
	
	public string invalidChars = "";
	
#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
	private bool selected = false;
	private bool toBeSelected = false;
#endif
	
	public InputField otherField = null;
	
	public InputField nextField;
	//private GUIButton myBtn;
	
	public bool hideInputFieldInKeyboard = true;
	
	private static InputField current;
	
	private string originalText;
	
	private Vector2 textBoundsSize;
	//private bool hasBounds = false;
	public Vector2 sizeOffset;
	
	
	public string Value{
		
		get { return actualString; }
		set { setText( value ); }
	}
	
	private void Awake(){
	//private void Start(){
		
		if(textMesh == null){
			
			textMesh = GetComponent<TextMesh>();
			
			if(textMesh == null){
		
				Debug.LogError("InputField "+this.name+" was not assigned a textMesh");
				return;
			}
		}
		
		textRenderer = textMesh.gameObject.GetComponent<Renderer>();
		
		if (!string.IsNullOrEmpty (placeholderString)) {

			textMesh.text = placeholderString;
		}

		this.originalText = this.textMesh.text;

		if(usePlaceholderAsDefault){

			actualString = originalText;

		}

		//myBtn = this.gameObject.GetComponent<GUIButton>();
		
		TryGetBounds();
		
		
	}
	
	private string hideText(string input){
		return new string(input.Select(c => '*').ToArray());
	}
	
	public void appendText(char c) {
		this.Value = this.Value + c;
	}
	
	public void setText(string t){
	
		if (t.Length > maxLength || t == this.actualString) return;
		
		this.actualString = filterString(t);
		
		string visibleString = passwordField ? hideText(actualString) : actualString;
		
		// Display placeholder text if field is empty
		if (string.IsNullOrEmpty(actualString) && !string.IsNullOrEmpty(this.placeholderString)) {
			visibleString = this.placeholderString;
		}
		
		TrySetText(visibleString);
		
		//if visible string runs off the end
		//	chop off the front until it fits
		while(IsTooBig())	
		{
			visibleString = visibleString.Substring (1);
			TrySetText(visibleString);
		}
		
		SendMessageUpwards("OnInputFieldChanged", this, SendMessageOptions.DontRequireReceiver);
	}
	
	private void TrySetText(string text){
	
		this.textMesh.text = text;
	}
	
	private string filterString(string input) {
		
		string result = "";
		
		System.Func<char, bool> predicate = c => true; 
		
		if (numericOnly) 					{ predicate = c => char.IsDigit(c); }
		else if (alphaNumericOnly) 			{ predicate = c => char.IsLetterOrDigit(c); }
		else if (alphaNumericSpaceOnly) 	{ predicate = c => char.IsLetterOrDigit(c) || c == ' '; }
		
		result = new string(input.Where(predicate).ToArray());
		
		foreach(char ch in invalidChars)
		{
			result = result.Replace(ch.ToString(), "");
		}
		
		if (result.Length > maxLength)
		{
			result = result.Substring(0, maxLength);	
		}

		return result;
	}	
	

	void OnGUIButtonClicked(GUIButton b){
		
		//SoundManager.Instance.playSoundEffect(Resources.Load("SFX/menu_confirmclick") as AudioClip);
		
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		if(b.name == name && keyboard == null && !isGrabbingInput)
		{
			OnSelected(this);
		}
#else
		if(b.name == name)
		{
			OnSelected(this);
		}
#endif
	}
	
	private IEnumerator Selected()
	{
		yield return new WaitForEndOfFrame();
		
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		if(clearTextOnClick || (this.clearTextIfSameAsStartup && this.textMesh.text == this.originalText))
		{
			TrySetText("");
			actualString = "";
		}
		
		SendMessageUpwards("OnInputFieldStarted", this, SendMessageOptions.DontRequireReceiver);
		StartCoroutine(grabInput());
#else
		selected = true;
		current = this;
		
		if(clearTextOnClick || (this.clearTextIfSameAsStartup && this.textMesh.text == this.originalText))
		{
			TrySetText("");
			actualString = "";
		}
		
		SendMessageUpwards("OnInputFieldStarted", this, SendMessageOptions.DontRequireReceiver);
		
		if(otherField)
		{
			otherField.selected = false;
		}
#endif
		
		/*
		if(myBtn != null)
		{
			myBtn.OnSetPressState();
		}
		*/
	}
	
	public void OnSelected(InputField field){
		
		//Debug.Log(name + " Selected");
		
		SendMessageUpwards("OnInputFieldSelected", field, SendMessageOptions.DontRequireReceiver);
		
		StartCoroutine(Selected());
	}
	
	
#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
	void LateUpdate(){
		
		if(toBeSelected){
			
			toBeSelected = false;
			selected = true;
		}
	}
	
	void Update(){
		
		if(!selected || current != this){
		
			return;
		}
		
		if(Input.GetKeyDown(KeyCode.Tab) && otherField){
			
			//Debug.Log("Tab pressed on " + name);
			selected = false;
			
			otherField.toBeSelected = true;
			current = otherField;

			return;
		}

		string TEMPORARY_INPUT_STRING = "";

		for(int i=0;i<System.Enum.GetValues(typeof(KeyCode)).Length;++i){

			if(Input.GetKeyDown((KeyCode)i)){

				if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) continue;


				if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){

					TEMPORARY_INPUT_STRING += ((char)i).ToString().ToUpper();
				}else{
					TEMPORARY_INPUT_STRING += (char)i;
				}
			}
		}

		// for non-mobile platforms(editor, desktop, etc...)

		foreach(char c in TEMPORARY_INPUT_STRING){
		//foreach(char c in Input.inputString){ // FIXME : temporarily removed since unity 5.2 broke it and made it return blank all the time
			
			if (c == '\b') {
				
				//if(actualString.Length != 0){
					
				//	actualString = actualString.Substring(0, actualString.Length -1);	
				//}
				
				//setText(actualString);
				
			    if (this.Value.Length != 0) {
					this.Value = this.Value.Substring(0, this.Value.Length - 1);
				}
				
			} else if (c == '\n' || c == '\r') {
					 
				selected = false;
				Debug.Log("Unselecting " + name + ". InputString: |" + Input.inputString + "|");
				
				if(otherField) {otherField.selected = false;}
				
				SendMessageUpwards("OnInputFieldComplete", this, SendMessageOptions.DontRequireReceiver);
				
				/*
				if(myBtn != null)
				{
					myBtn.OnSetUpState();
				}
				*/
				
				if(nextField != null)
				{
					nextField.OnSelected(nextField);
				}
				
				//if visible string runs off the end
				//	chop off the end until it fits
				string visibleString = passwordField ? hideText(actualString) : actualString;
				TrySetText(visibleString);
				
				
				while(IsTooBig())	
				{
					visibleString = visibleString.Substring(0, visibleString.Length - 2);
					TrySetText(visibleString);
				}
			
			}
			else
			{
				appendText(c);
			}
    	}
	}
#endif

#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
	private IEnumerator grabInput(){
			
#if UNITY_IPHONE
		TouchScreenKeyboard.hideInput = !passwordField;
#endif
		keyboard = TouchScreenKeyboard.Open(this.Value, KeyboardType, false, false, passwordField, false, placeholderString);
		
		GUIController.Instance.Deactivate();
		
		isGrabbingInput = true;
		
		float checkForInvisibleStartTime = Time.realtimeSinceStartup+1.0f;
		
		Debug.Log("Keyboard started");
		
		//while(!keyboard.done && keyboard.active){
		do {
			
			// Filter input and reflect that in keyboard input field
			this.Value = keyboard.text;
			
			bool wasVisible = TouchScreenKeyboard.visible;
			//yield return new WaitForSeconds(0.1f);
			yield return null;
			
			if(wasVisible && !TouchScreenKeyboard.visible){
				Debug.Log("Resetting keyboard time out");
				checkForInvisibleStartTime = Time.realtimeSinceStartup+1.0f;
			}
		}while(!keyboard.done && (Time.realtimeSinceStartup < checkForInvisibleStartTime || TouchScreenKeyboard.visible));
		
		Debug.Log("Keyboard finished");
		Debug.Log("keyboard.done = "+keyboard.done);
		Debug.Log("TouchScreenKeyboard.visible = "+TouchScreenKeyboard.visible);
		
		GUIController.Instance.Activate();
		
		isGrabbingInput = false;

		SendMessageUpwards("OnInputFieldChanged", this, SendMessageOptions.DontRequireReceiver);
		
		if(keyboard.done){
		
			SendMessageUpwards("OnInputFieldComplete", this, SendMessageOptions.DontRequireReceiver);
		}else{
			
			SendMessageUpwards("OnInputFieldCancelled", this, SendMessageOptions.DontRequireReceiver);
		}
		
		keyboard = null;	
		if(nextField != null)
		{
			nextField.OnSelected(nextField);
		}
		if(myBtn != null)
		{
			myBtn.OnSetUpState();
		}
		
	}
#endif

	
	private void TryGetBounds(){
		
		BoxCollider textBoundsCollider = gameObject.GetComponent<BoxCollider>();
		
		//Debug.Log("textBoundsCollider = "+textBoundsCollider);
		
		if(textBoundsCollider != null){
			
			textBoundsSize.x = textBoundsCollider.bounds.size.x;
			textBoundsSize.y = textBoundsCollider.bounds.size.y;
			
			//sometimes the bounds come back as 0,0 mysteriously... in those cases grab the local size... not perfect but workable I guess...
			if(textBoundsSize.x < 0.001 || textBoundsSize.y < 0.001){
				
				textBoundsSize.x = textBoundsCollider.size.x;
				textBoundsSize.y = textBoundsCollider.size.y;
			}
			
		}
	}
	
	private bool IsTooBig(){
		
		return ((textRenderer.bounds.size.x > textBoundsSize.x + sizeOffset.x || textRenderer.bounds.size.y > textBoundsSize.y + sizeOffset.y) && textMesh.characterSize > 0);	
	}
}

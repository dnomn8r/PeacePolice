using UnityEngine;
using System.Collections;
using System.Text;

public class TextProcessor : FontChooser{
	
	public bool WordWrap;

	public bool autoTranslate;
	
	public bool DestroyColliderAfterInitilization = true;
	
	protected Renderer textRenderer;
	
	private Vector2 textBoundsSize;
	
	private bool hasBounds = false;
	
	public Vector2 sizeOffset;
	
	private string originalText;
	
	private static StringBuilder strBuilder;
	
	private string unchangedText;
	
	protected override void Awake()
	{
		if(null == strBuilder)
		{
			InitStringBuilder();
		}
		
		base.Awake();
		Initialize();
	}
	
	public string OriginalText{
		
		get{return originalText;}	
	}
	
	public string CurrentText{
	
		get{
			if(textMesh != null){
				
				return textMesh.text;
			}else{
				
				return "";
			}
		}
	}
	
	/*public void OnEnable()
	{
		Initialize();
	}
	 */
	private void Initialize(){
		
		textRenderer = textMesh.GetComponent<Renderer>();
		if(textRenderer == null){
			
			Debug.LogError("FontResizer "+this.name+" doesn't have a Renderer component.");
		}
		
		//if(!autoTranslate)
		//	Debug.Log("AUTOTRANSLATE IS NOT ON!!! CHANGE THE SETTING!!!");
		
		TryGetBounds();
		//Wrap();
		
		originalText = textMesh.text;
	}
	
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
			
			hasBounds = true;
			
			if(DestroyColliderAfterInitilization){
				
				Destroy(textBoundsCollider);
			}
		}
		
		if(autoTranslate){
			
			this.OnSetText(textMesh.text,true);
		}
	}
	
	public virtual void OnSetText(string text, bool shouldTranslate=false){
		
		if(shouldTranslate){
		
			OnSetTextNoLocalizing(LanguageManager.Instance.GetText(text));
		
		}else{
			
			OnSetTextNoLocalizing(text);
		}
	}
	
	public void OnSetText(int text){
		
		OnSetTextNoLocalizing(text.ToString());
	}
	
	public void OnSetTextNoLocalizing(string translatedText){
		
		unchangedText = translatedText;
		
		if(textMesh != null){
			
			textMesh.text = translatedText;
			textMesh.characterSize = originalCharacterSize;
			
			Wrap();
			
			if(/*!WordWrap && */LanguageManager.Instance != null && LanguageManager.Instance.GetCurrentLanguage() == Language.French.ToString())
			{
				char stupidFrenchSpace = (char)160;

				/*
				if(textMesh.text.Contains(stupidFrenchSpace.ToString()))
				{
					Debug.LogError("FRENCHSPACE - " + textMesh.text);
				}
				*/
				
				textMesh.text = textMesh.text.Replace(stupidFrenchSpace, ' ');
			}
			
			//do I want to do this (here or at all)? halp
			//probably not
			//System.GC.Collect();
			
			//Debug.LogError("SB size: " + strBuilder.Length + "/" + strBuilder.Capacity);
		}
		/*
		else
		{
			Debug.Log("textMesh was null");
		}
		*/
	}
		
	private void Wrap(){
		
		string[] wordList;
		//StringBuilder strBuilder = new StringBuilder(textMesh.text.Length);
		
		if(hasBounds && this.WordWrap){
			
			//wordList = textMesh.text.Split(' ');
			wordList = unchangedText.Split(' ');
			
			if(LanguageManager.Instance != null && LanguageManager.Instance.GetCurrentLanguage() == Language.French.ToString())
			{
				char stupidFrenchSpace = (char)160;
				
				for(int i = 0;i < wordList.Length;i++)
				{
					/*
					if(wordList[i].Contains(stupidFrenchSpace.ToString()))
					{
						Debug.LogError("FRENCHSPACE - " + wordList[i]);
					}
					*/
					
					wordList[i] = wordList[i].Replace(stupidFrenchSpace, ' ');
				}
			}
		
		}else{
			wordList = new string[0];
		}
		
		// if we want our text to be bound by a box
		if(hasBounds){
			
			while(IsTooBig()){
				
				if(this.WordWrap){
					
					//textMesh.text = "";
					
					if(wordList.Length <= 0){
						
						return;
					}
					
					//textMesh.text = wordList[0];
					
					//because stringbuilder doesn't have a clear() for some reason
					strBuilder.Length = 0;
					
					strBuilder.Append(wordList[0]);
					
					for(int i = 1; i < wordList.Length; i++){
						
						//string lastString = textMesh.text;
						string lastString = strBuilder.ToString();
						
						//textMesh.text = lastString + " " + wordList[i];
						strBuilder.Append(" ");
						strBuilder.Append(wordList[i]);
						
						textMesh.text = strBuilder.ToString();
						
						if(textRenderer.bounds.size.x > textBoundsSize.x){
							
							//textMesh.text = lastString + "\n" + wordList[i];
							
							strBuilder.Length = 0;
							strBuilder.Append(lastString);
							
							strBuilder.Append("\n");
							strBuilder.Append(wordList[i]);
							
							textMesh.text = strBuilder.ToString();
						}
					}
				}
				
				//if word wrapping above didn't solve our sizing issues shrink the font size
				if(IsTooBig()){
					
					//Debug.Log("We are too big, decreasing size");
					textMesh.characterSize -= 0.1f;
				}
			}
		}
	}
	
	private bool IsTooBig(){
		
		return ((textRenderer.bounds.size.x > textBoundsSize.x + sizeOffset.x || textRenderer.bounds.size.y > textBoundsSize.y + sizeOffset.y) && textMesh.characterSize > 0);	
	}
	
	public bool IsNullOrEmpty(){
		
		return string.IsNullOrEmpty(textMesh.text);
	}
	
	public float getTextHeight(){
		
		return textRenderer.bounds.size.y;
	}
	
	private void InitStringBuilder()
	{
		//strBuilder.Length = 0;
		//strBuilder.Capacity = 100;
		
		strBuilder = new StringBuilder(100);
	}
	
}

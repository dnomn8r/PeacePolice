using UnityEngine;
using System.Collections;

public class FontChooser : MonoBehaviour{
	
	protected TextMesh textMesh;
	
	protected float originalCharacterSize;
	
	protected Transform myTransform;
	
	protected virtual void Awake(){
		
		myTransform = transform;
		
		textMesh = gameObject.GetComponent<TextMesh>();
		
		if(textMesh == null){
			
			Debug.LogError("FontChooser "+this.name+" doesn't have a TextMesh component.");
		}

		originalCharacterSize = textMesh.characterSize;
		
	}
	
	protected virtual void Start(){
		
		Vector3 pos = myTransform.position;
		
		pos.x = Mathf.RoundToInt(pos.x) + 0.5f;
		pos.y = Mathf.RoundToInt(pos.y) + 0.5f;
		pos.z = Mathf.RoundToInt(pos.z) + 0.5f;
		
		myTransform.position = pos;
	}
}

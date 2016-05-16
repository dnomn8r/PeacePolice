using UnityEngine;
using System.Collections;

public class HullDisplay : MonoBehaviour {
	
	private Texture2D hullTexture;
	
	private Renderer myRenderer;
	
	public TextMesh nameText;
	
	private void Awake(){
		
		myRenderer = GetComponent<Renderer>();	
	}
	
	public Texture2D CurrentTexture{
		
		get{return hullTexture;}	
	}
	
	public void OnSetHull(Texture2D hull){
			
		if(myRenderer == null){
		
			Debug.LogError("Hull Display has no renderer");
			
			return;
		}
		
		if(hull != null){
			
			hullTexture = hull;
			
			myRenderer.material.mainTexture = hull;	
			
			float maxWidth = 200.0f;
			float maxHeight = 100.0f;
			
			float scaleDownFactor = 1.0f;
			
			if(hull.height > maxHeight){
			
				scaleDownFactor = hull.height / maxHeight;
			}
			
			if(hull.width > maxWidth){
			
				float newScaleDownFactor = hull.width / maxWidth;
				
				if(newScaleDownFactor > scaleDownFactor){
					scaleDownFactor = newScaleDownFactor;	
				}
			}
			
			myRenderer.transform.localScale = new Vector3(hull.width / scaleDownFactor, hull.height / scaleDownFactor, 1);
			
			nameText.text = hull.name + " " + hull.width + "x" + hull.height;
		}else{
			
			Debug.LogError("no valid texture");	
		}
	}
	
}

using UnityEngine;
using System.Collections;

public class HullBrowser : MonoBehaviour {
	
	private int currentPage = 0;
	
	private int maxPages = 0;
	
	private Object[] shipHulls;
	
	public TextMesh pageText;
	
	public HullDisplay[] hullDisplays;
	
	void Start(){
		
		shipHulls = Resources.LoadAll("ShipHulls");
		
		Debug.Log("number of ship hulls found: " + shipHulls.Length);
		
		if(hullDisplays.Length > 0 && shipHulls.Length > 0){
		
			maxPages = Mathf.CeilToInt(shipHulls.Length / (float)hullDisplays.Length);

		}else{
			
			maxPages = 0;	
		}
		
		
		refreshDisplay();
	}
	
	private void refreshDisplay(){
	
		if(maxPages == 0){
			
			pageText.text = "0/0";	
		
			return;
		}
		
		pageText.text = (currentPage + 1) + "/" + (maxPages + 1);
		
		for(int i=0;i<hullDisplays.Length;++i){
			
			int targetIndex = (currentPage*hullDisplays.Length + i);
			
			if(targetIndex < shipHulls.Length){
			
				Texture2D currentHull = (Texture2D)(shipHulls[targetIndex]);
		
				hullDisplays[i].OnSetHull(currentHull);
					
				SendMessageUpwards("OnHullSelected", hullDisplays[i]);
			}
		}
		
	}
	
	private void setPage(int newPage){
		
		if(newPage >= maxPages){
		
			newPage = 0;
			
		}else if(newPage < 0){
			
			newPage = maxPages-1;
		}
		
		currentPage = newPage;
		
		refreshDisplay();
	}
	
	
	public void OnGUIButtonPressed(GUIButton b){
		
		if(b.name == "nextBackgroundButton"){
		
			setPage(currentPage+1);
			
		}else if(b.name == "prevBackgroundButton"){
			
			setPage(currentPage-1);
		}
	}
	
	
}

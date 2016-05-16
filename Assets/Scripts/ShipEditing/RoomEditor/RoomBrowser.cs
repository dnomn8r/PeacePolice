using UnityEngine;
using System.Collections;

public class RoomBrowser : MonoBehaviour {
	
	private int currentPage = 0;
	
	private int maxPages = 0;
	
	public RoomDisplay[] roomDisplays;
	
	public TextMesh statusText;
	
	void Start(){
	
		if(roomDisplays.Length > 0 && Definitions.Instance.Rooms.Count > 0){
		
			maxPages = Mathf.CeilToInt(Definitions.Instance.Rooms.Count / (float)roomDisplays.Length);
			
		}else{
			
			maxPages = 0;	
		}
		
		refreshDisplay();
	}
	
	private void refreshDisplay(){
	
		if(maxPages == 0){
			
			statusText.text = "0/0";	
		
			return;
		}
		
		statusText.text = (currentPage + 1) + "/" + maxPages;
		
		for(int i=0;i<roomDisplays.Length;++i){
			
			int targetIndex = (currentPage*roomDisplays.Length + i);
			
			if(targetIndex < Definitions.Instance.Rooms.Count){

				roomDisplays[i].OnSetRoomData(Definitions.Instance.Rooms[targetIndex]);
			
			}else{
			
				roomDisplays[i].ClearDisplay();
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
		
		if(b.name == "nextButton"){
		
			setPage(currentPage+1);
			
		}else if(b.name == "previousButton"){
			
			setPage(currentPage-1);
		}
	}
}

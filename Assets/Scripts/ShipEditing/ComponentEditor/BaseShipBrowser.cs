using UnityEngine;
using System.Collections;
using System.IO;

public class BaseShipBrowser : MonoBehaviour {
	
	private int currentPage = 0;
	
	private int maxPages = 0;
	
	private Object[] shipHulls;
	
	public TextMesh pageText;
	
	public TextMesh currentBaseShipName;

	private FileInfo[] files;
	
	void Start(){
		
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		string pathToFile = Application.persistentDataPath + "/" + ComponentEditor.HULL_DIRECTORY;		
#else
		string pathToFile = ComponentEditor.HULL_DIRECTORY;
#endif
		
		//Debug.Log("path to file: " + pathToFile);
		
		DirectoryInfo d = new DirectoryInfo(pathToFile);
		files = d.GetFiles("*.hull");
		
		/*
		foreach(FileInfo file in files){
			
		 	Debug.Log("file: " + file.Name);
		}*/
		
		if(files.Length > 0){
		
			maxPages = (files.Length-1);
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
		
		for(int i=0;i<1;++i){
			
			int targetIndex = (currentPage + i);
			
			if(targetIndex < files.Length){
			
				currentBaseShipName.text = files[targetIndex].Name.Replace(files[targetIndex].Extension, "");
			}
		}
		
	}
	
	private void setPage(int newPage){
		
		if(newPage > maxPages){
		
			newPage = 0;
			
		}else if(newPage < 0){
			
			newPage = maxPages;
		}
		
		currentPage = newPage;
		
		refreshDisplay();
	}
	
	
	public void OnGUIButtonPressed(GUIButton b){
		
		if(b.name == "nextBaseShipButton"){
		
			setPage(currentPage+1);
			
		}else if(b.name == "prevBaseShipButton"){
			
			setPage(currentPage-1);
		
		}else if(b.name == "loadBaseShipButton"){
		
			SendMessageUpwards("OnBaseShipSelected", currentBaseShipName.text + ".hull");
		
		}
		
		//Debug.Log("button pressed: " + b.name);	
		
	}
	
	
}

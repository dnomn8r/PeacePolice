using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;

public class ShipLoadPopup : Popup{

	private LoadResult result;

	public GUIButton[] shipList;
	private TextMesh[] shipTitles;

	public GUIButton prevBtn;
	public GUIButton nextBtn;

	//private string currentShipFilename = "";

	private string[] shipNames;
	private int currentPage = 0;

	private string path;
	private string ext;

	private int numPages;
	
	public void SetLoadResult(LoadResult r){

		this.result = r;
	}

	public void LoadFiles(string dir, string extension){

		path = dir;
		ext = extension;

		shipNames = Directory.GetFiles(dir, "*" + extension);

		numPages = Mathf.CeilToInt((float)shipNames.Length / (float)shipList.Length);

		PopulateList();
	}

	protected override void Awake(){

		base.Awake();
		
		shipTitles = new TextMesh[shipList.Length];
		
		for(int i = 0;i < shipList.Length;i++){

			shipTitles[i] = shipList[i].GetComponentInChildren<TextMesh>();
		}
	}

	private void PopulateList(){

		int offset = currentPage * shipList.Length;

		for(int i = 0;i < shipList.Length;i++){

			int index = offset + i;

			if(index >= shipNames.Length){

				shipList[i].gameObject.SetActive(false);
			}else{
			
				shipList[i].gameObject.SetActive(true);
				shipTitles[i].text = shipNames[index].Substring(path.Length, shipNames[index].Length - path.Length - ext.Length);
			}
		}

		if(currentPage == 0){

			prevBtn.gameObject.SetActive(false);
		}else{
			prevBtn.gameObject.SetActive(true);
		}

		if(numPages > 1 && currentPage < numPages - 1){

			nextBtn.gameObject.SetActive(true);
		
		}else{
			nextBtn.gameObject.SetActive(false);
		}
	}
	
	public override void OnGUIButtonClicked(GUIButton b){

		base.OnGUIButtonClicked(b);

		if(b.name == prevBtn.name){

			currentPage = Mathf.Max(currentPage - 1, 0);
			PopulateList();
		
		}else if(b.name == nextBtn.name){

			currentPage = Mathf.Min(currentPage + 1, numPages - 1);
			PopulateList();
		
		}else if(b.name.StartsWith("btn_ship")){

			int index = (currentPage * shipList.Length) + int.Parse(b.name.Substring(8));

			string shipString = SaveAndLoad.Load(RoomEditor.SHIP_DIRECTORY, shipNames[index].Substring(path.Length));

			result.ship = Newtonsoft.Json.JsonConvert.DeserializeObject<ShipData>(shipString);
		
			yesCallback();
			
			RemovePopup();
		}
	}
}
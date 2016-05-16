using UnityEngine;
using System.Collections;
using PeaceServerClientCommon;

public class EndTurnButton : MonoBehaviour {

	[SerializeField]private TextMesh myText;

	public TextMesh Text{

		get{ return myText; }
	}

	void OnGUIButtonClicked(GUIButton b){

		if(!UnityCombatManager.Instance.OpponentHasLeft && !UnityCombatManager.Instance.HasWon &&
			!UnityCombatManager.Instance.HasLost) {
			
			UnityCombatManager.Instance.EndLocalPlayerTurn();

			gameObject.SetActive(false);

		}else{

			UnityCombatManager.Instance.Cleanup ();

			enabled = false;

			Application.LoadLevel ("Load");
		}
	}
		

	void Update(){

		if(UnityCombatManager.Instance.OpponentHasLeft || UnityCombatManager.Instance.HasWon ||
		   UnityCombatManager.Instance.HasLost){

			myText.text = "To load menu";
		
		} else if(UnityCombatScheduler.Instance.CombatScheduler != null && 
			UnityCombatScheduler.Instance.CombatScheduler.HasEvents()){

			gameObject.SetActive(false);
		}
	}

}

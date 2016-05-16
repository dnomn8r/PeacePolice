using UnityEngine;
using System.Collections;

using PeaceServerClientCommon;

public class StatusText : MonoBehaviour {

	private TextMesh myText;

	public TextMesh Text{

		get{ return myText; }

	}

	void Awake(){

		myText = GetComponent<TextMesh>();

		myText.text = "Game started...";
	}

	void Update(){

		if(UnityCombatManager.Instance.OpponentHasLeft){

			myText.text = "You win because your opponent left.";
		
		} else if(UnityCombatManager.Instance.WaitingForOtherPlayers){

			myText.text = "Waiting for opponent to send their actions";

		} else if(UnityCombatManager.Instance.HasWon){

			myText.text = "You win!";
		
		} else if(UnityCombatManager.Instance.HasLost){
			
			myText.text = "You lose!";

		}else{

			myText.text = "";

			if(UnityCombatScheduler.Instance != null){

				//Debug.Log("first not null");

				if(UnityCombatScheduler.Instance.CombatScheduler != null){

					//Debug.Log("seocnd not null");

					if(!UnityCombatScheduler.Instance.CombatScheduler.HasEvents()){

						//Debug.Log("no event!");
						myText.text = string.Format("{0:0.00}", UnityCombatScheduler.Instance.TimeLeftInTurn);
					}
				}

			}
			//myText.text = "";
		}
	}
}

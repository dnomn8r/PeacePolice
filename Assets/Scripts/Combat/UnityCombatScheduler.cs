using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UnityCombatScheduler : MonoBehaviour {

	private EndTurnButton endTurnButton;

	private float timeElapsedSinceLastSlice = 0.0f;

	private CombatScheduler combatScheduler;

	public CombatScheduler CombatScheduler{

		get{ return combatScheduler; }

		set{

			combatScheduler = value;
		}
	}
		
	private bool turnEnded = false;

	private float timeLeftInTurn = 0;

	public float TimeLeftInTurn{
		get{ return timeLeftInTurn; }
	}

	private float lastRealTime = 0.0f;

	private static UnityCombatScheduler instance;

	public static UnityCombatScheduler Instance{
		
		get{
				
			return instance;
		}
	}

	void OnDestroy(){

		instance = null;
	}

	void Awake(){

		instance = this;

		endTurnButton = (GameObject.FindObjectOfType(typeof(EndTurnButton)) as EndTurnButton);

		combatScheduler = UnityCombatManager.Instance.Game.Scheduler;

		timeLeftInTurn = PeaceServerClientCommon.PeaceConstants.SECONDS_PER_TURN;
		lastRealTime = Time.realtimeSinceStartup;
	}

	public void ExecuteTurn(List<ComponentActionResult> results){

		combatScheduler.CurrentSlice = 0;

		//GUIController.Instance.Deactivate();
		if(results.Count > 0){

			combatScheduler.AddActionResults(results);
		}

		Debug.Log("events? " + combatScheduler.HasEvents());
		//enabled = true;

		Debug.Log("end of execute turn");

		turnEnded = true;
	}

	private void startNewTurn(){


		timeElapsedSinceLastSlice = 0;

		GUIController.Instance.Activate();

		endTurnButton.gameObject.SetActive(true);

		UnityCombatManager.Instance.UpdateGameStatus();


		timeLeftInTurn = PeaceServerClientCommon.PeaceConstants.SECONDS_PER_TURN;
		lastRealTime = Time.realtimeSinceStartup;
	}

	void Update(){

		if(turnEnded){ // have to do it with a boolean because of unity threading

			if(combatScheduler == null || !combatScheduler.HasEvents()){
				
				OurPlayerController localController = FindObjectOfType(typeof(OurPlayerController)) as OurPlayerController;

				localController.CombatActions.Clear();

				localController.Refresh();
				startNewTurn();
			}

			turnEnded = false;
		}

		//Debug.Log("time left in turn: " + timeLeftInTurn);

		timeLeftInTurn -= (Time.realtimeSinceStartup - lastRealTime);
		lastRealTime = Time.realtimeSinceStartup;

		if(combatScheduler == null || !combatScheduler.HasEvents()) return;

		if(GUIController.Instance.enabled){

			GUIController.Instance.Deactivate();
		}

		timeElapsedSinceLastSlice += Time.deltaTime;
		
		while(timeElapsedSinceLastSlice > CombatScheduler.TIME_SLICE_LENGTH){

			//Debug.Log("about to execute slice");

			if(combatScheduler.ExecuteNextSlice()){

				//GUIController.Instance.Activate();

				//endTurnButton.gameObject.SetActive(true);
			
				//UnityCombatManager.Instance.UpdateGameStatus();
				//enabled = false;
				startNewTurn();
		

				break;

			}

			timeElapsedSinceLastSlice -= CombatScheduler.TIME_SLICE_LENGTH;
		}


	}

}


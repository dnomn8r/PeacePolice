using UnityEngine;
using System.Collections;

public class MissIndicator : ShipIndicator {

	public TextMesh missText;

	public void DisplayMiss(){

		textTransform = missText.transform;

		missText.text = "Miss";
	
		DelayedDestroy delayedDestroy = gameObject.AddComponent<DelayedDestroy>();
		delayedDestroy.delay = displayTime;

		enabled = true;
	}

		
}

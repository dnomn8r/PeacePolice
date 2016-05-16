using UnityEngine;
using System.Collections;

public class ShipIndicator : MonoBehaviour {

	public float displayTime = 2.0f;
	public float floatSpeed = 10.0f;

	protected Transform textTransform;


	void Update(){

		if(textTransform != null){
	
			textTransform.localPosition = Vector3.Lerp(textTransform.localPosition, textTransform.localPosition + new Vector3(0, floatSpeed, 0), 5.0f * Time.deltaTime);
		}
	}	
}

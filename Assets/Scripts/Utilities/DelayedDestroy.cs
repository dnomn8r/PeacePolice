using UnityEngine;
using System.Collections;

public class DelayedDestroy : MonoBehaviour {
	
	public float delay = 1.0f;
	
	void Start(){
		
		StartCoroutine(delayedDestroy(delay));
	}
	
	private IEnumerator delayedDestroy(float delay){
	
		yield return new WaitForSeconds(delay);
	
		Destroy(this.gameObject);
	}
}

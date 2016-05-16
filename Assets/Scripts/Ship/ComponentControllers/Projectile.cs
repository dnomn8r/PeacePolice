using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	private Transform myTransform;
	private int speed;
	private Vector3 targetPos;

	private bool isHit;

	public GameObject collisionEffect;

	private bool hasReachedGoal = false;

	private Vector3 dir;

	void Awake(){
		
		myTransform = transform;	
		
		enabled = false;
	}

	public void OnSetTarget(Vector3 pos, int speed, bool isHit){

		this.targetPos = pos;
		this.speed = speed;
		this.isHit = isHit;


		dir = (targetPos - transform.position).normalized;
		enabled = true;

		//Debug.Log ("distnce to target from porjectile: " + Vector3.Distance(pos, myTransform.position));
	}
	
	void Update(){

		Vector3 diff = targetPos - myTransform.position;
		
		if(!hasReachedGoal && diff.magnitude <= speed * Time.deltaTime){
		
			hasReachedGoal = true;
			//Debug.Log ("boom: " + Time.time);

			if(isHit){
				GameObject explosion = GameObject.Instantiate(collisionEffect) as GameObject;
				explosion.transform.position = targetPos;
		
				Destroy(this.gameObject);	
			}else{
		
				StartCoroutine(delayedDestroy());
			}
		}else{
			
			myTransform.position += dir.normalized * speed * Time.deltaTime;	

			myTransform.rotation = Quaternion.LookRotation(dir, myTransform.up);
		}
	}

	private IEnumerator delayedDestroy(){

		yield return new WaitForSeconds(10.0f);

		Destroy(this.gameObject);
	}

}

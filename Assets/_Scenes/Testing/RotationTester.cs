using UnityEngine;
using System.Collections;

public class RotationTester : MonoBehaviour {

	public Transform target;

	public Transform cannonPivot;
	
	// Update is called once per frame
	void Update () {

		Quaternion targetQuat = Quaternion.LookRotation(new Vector3((float)target.position.x, (float)target.position.y, (float)target.position.z)
			- cannonPivot.position, cannonPivot.up);

		cannonPivot.rotation = Quaternion.Slerp(cannonPivot.rotation, targetQuat, 5.0f * Time.deltaTime);
	}
}

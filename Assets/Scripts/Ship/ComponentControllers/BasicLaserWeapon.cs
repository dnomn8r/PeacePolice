using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using PeaceServerClientCommon;

public class BasicLaserWeapon : ShipComponentController{

	public GameObject laserProjectile;
	public Transform spawnpoint;
	public Transform cannonPivot;
	private Quaternion targetQuat;


	public override void InitializeComponent(ShipComponent component){

		base.InitializeComponent(component);


		ResetToIdleTarget();
	}

	private void ResetToIdleTarget(){

		targetQuat = Quaternion.LookRotation(normalVersion.transform.up);
	}

	public override void OnAim(ShipRoom target){

		ShipRoomController targetRoomController = UnityCombatManager.Instance.GetRoomController(target.OwnerID, target.ID);

		targetQuat = Quaternion.LookRotation(targetRoomController.transform.position - cannonPivot.position, cannonPivot.up);
	}

	public override void OnFire(ShipRoom target, int projectileSpeed, bool isHit){

		GameObject newProjectile = GameObject.Instantiate(laserProjectile) as GameObject;
		newProjectile.transform.position = spawnpoint.position;

		Projectile projectile = newProjectile.GetComponent<Projectile>();

		//Debug.Log("tagrte is: " + target.ID + " with owner: " + target.OwnerID);
		//Debug.Log("position: " + target.position);
		//Debug.Log("data: " + myData.projectileSpeed);

		ShipRoomController targetRoomController = UnityCombatManager.Instance.GetRoomController(target.OwnerID, target.ID);

		//Vector3 position = new Vector3((float)PeaceConstants.DISTANCE_BETWEEN_SHIPS, 0, 0) + new Vector3((float)target.position.X, (float)target.position.Y, (float)target.position.Z);

		//Debug.Log("adjusted position: " + targetRoomController.transform.position);

		projectile.OnSetTarget(targetRoomController.transform.position, projectileSpeed, isHit);
	}
		
	
	private void Update(){

		cannonPivot.rotation = Quaternion.Slerp(cannonPivot.rotation, targetQuat, 5.0f * Time.deltaTime);
	}
}

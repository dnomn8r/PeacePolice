using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BasicBeamWeapon : ShipComponentController{

	public LineRenderer beam;
	public Transform cannonPivot;
	public Transform beamSpawn;

	public ParticleSystem chargeParticles;

	private Quaternion targetQuat;

	public float beamDuration = 1.0f;

	public override void InitializeComponent(ShipComponent component){

		base.InitializeComponent(component);

		ResetToIdleTarget();

		chargeParticles.enableEmission = false;
	}

	private void ResetToIdleTarget(){

		targetQuat = Quaternion.LookRotation(normalVersion.transform.up);
	}

	public override void OnAim(ShipRoom target){

		ShipRoomController targetRoomController = UnityCombatManager.Instance.GetRoomController(target.OwnerID, target.ID);

		targetQuat = Quaternion.LookRotation(targetRoomController.transform.position - cannonPivot.position, cannonPivot.up);
	}

	public override void OnFire(ShipRoom target, int projectileSpeed, bool isHit){

		chargeParticles.enableEmission = false;

		ShipRoomController targetRoomController = UnityCombatManager.Instance.GetRoomController(target.OwnerID, target.ID);
	
		beam.enabled = true;

		beam.SetPosition(0, beamSpawn.position);

		if(isHit){
		
			beam.SetPosition(1, targetRoomController.transform.position);

		}else{

			Vector3 dir = (targetRoomController.transform.position - beamSpawn.position).normalized;
			beam.SetPosition(1, beamSpawn.position + (dir * 5000.0f)); // miss! 
		}

		StartCoroutine (delayedBeamDisable());
	}

	public override void OnCharge(){

		chargeParticles.enableEmission = true;

	}
		
	private IEnumerator delayedBeamDisable(){

		yield return new WaitForSeconds(beamDuration);

		beam.enabled = false;
	}
	
	private void Update(){

		cannonPivot.rotation = Quaternion.Slerp(cannonPivot.rotation, targetQuat, 5.0f * Time.deltaTime);
	}
}

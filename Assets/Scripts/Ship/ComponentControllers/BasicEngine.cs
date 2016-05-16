using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BasicEngine : ShipComponentController{

	public ParticleSystem[] exhaustParticles;

	public float engineDuration = 5.0f;

	public override void InitializeComponent(ShipComponent component){

		base.InitializeComponent(component);

		foreach(ParticleSystem exhaust in exhaustParticles){
			exhaust.enableEmission = false;
		}
	}

	public override void OnActivate(){

		StartCoroutine(activateEngines());
	}

	private IEnumerator activateEngines(){

		foreach(ParticleSystem exhaust in exhaustParticles){
			exhaust.enableEmission = true;
		}

		yield return new WaitForSeconds(engineDuration);

		foreach(ParticleSystem exhaust in exhaustParticles){
			exhaust.enableEmission = false;
		}
	}
}

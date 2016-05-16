using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPlayerController : PlayerController{
	
	void Start(){

		UnityCombatManager.Instance.AddPlayerController(this);

		player = UnityCombatManager.Instance.GetFirstEnemy();

		display.SetPlayer(player);

	}
	
}

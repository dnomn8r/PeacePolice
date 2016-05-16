using UnityEngine;
using System.Collections;

public class DamageIndicator : ShipIndicator {

	public TextMesh damageText;

	public void DisplayDamage(int damage, bool isShield = false){

		textTransform = damageText.transform;

		damageText.text = Mathf.Abs(damage).ToString();
	
		Color colour = damage > 0 ? (isShield ? Color.blue : Color.red) : Color.green;

		damageText.GetComponent<Renderer>().material.color = colour;

		DelayedDestroy delayedDestroy = gameObject.AddComponent<DelayedDestroy>();
		delayedDestroy.delay = displayTime;

		enabled = true;
	}

		
}

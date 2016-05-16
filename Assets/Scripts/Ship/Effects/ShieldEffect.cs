//using UnityEngine;
//using System.Collections;
//
//public class ShieldEffect : MonoBehaviour {
//
//	private Shield controllingShield;
//
//	private GameObject shieldEffectVisual;
//
//	public bool IsShieldActive(){
//
//		return controllingShield.CurrentStrength > 0;
//	}
//
//	public void SetShield(Shield s){
//
//		controllingShield = s;
//
//		shieldEffectVisual = GameObject.Instantiate(Resources.Load("Effects/Shield")) as GameObject;
//
//		shieldEffectVisual.transform.parent = transform;
//		shieldEffectVisual.transform.localPosition = new Vector3(0,0,-20);
//
//	}
//
//	public void OnTakeDamage(int damage){
//
//		controllingShield.OnTakeDamage(damage);
//	}
//
//	void OnDisable(){
//
//		if(shieldEffectVisual != null){
//
//			Destroy(shieldEffectVisual);
//		}
//	}
//}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	[SerializeField]protected PlayerDisplay display;

	public PlayerDisplay Display{
		get{ return display; }
	}

	protected Player player;

	public Player Player{
		get{return player;}
	}

	
}

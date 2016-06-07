using System.Collections;
using System.Collections.Generic;

public class ShipComponentStatus{

	public string statusType;

	public float value;

	public int turnsLeft;

	public ShipComponentStatus(string type, float val, int turns){

		statusType = type;
		value = val;
		turnsLeft = turns;
	}
}


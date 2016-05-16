using UnityEngine;
using System.Collections;

public class PathDetails{
	
	public Vector3 currentDirection = Vector3.zero;
	public int stepsRemaining = 0;
	//public float carryOverMovement = 0.0f;
	
}


public class Unit : MonoBehaviour {

	public float freeTurnAngle = 20;

	public float noTurnAngle = 40; // if angle higher than this, no turning at all!

	public float moveSpeed = 10.0f;
	
	public float turnSpeed = 2.0f;
	
	private PathMeshMaker meshMaker = null;
	
	private CatmullRomSpline orders = new CatmullRomSpline();
	
	private Transform myTransform;
	
	private Transform unitTransform;
	
	private float savedMovement = 0.0f;

	public bool CanMove
	{
		get { return Input.GetKey(KeyCode.Space); }
	}
	
	// ignore the first order, that's our starting position
	private int currentOrder = 1;
	
	private PathDetails currentPathDetails = null;
	
	void Awake(){
		
		myTransform = transform;
		
		unitTransform = myTransform.Find("model");
		
		meshMaker = GetComponentInChildren<PathMeshMaker>();
		
		orders = new CatmullRomSpline();
	}

	void Update()
	{
		if(CanMove)
		{
			AdvanceStep();
		}
	}

	public void AdvanceStep(){
		
		if(currentOrder >= orders.GetNumPoints()) return;
		
		
		// move towards the next order, given our current position
		
		float moveSpeedLeft = moveSpeed + savedMovement;
		
		Vector3 currentGoal = orders.GetIndexPoint(currentOrder);
		
		float distanceToGoal = Vector3.Distance(unitTransform.position, currentGoal);
		
		Vector3 direction = (currentGoal - unitTransform.position).normalized;
		
		float angleToGoal = Vector3.Angle(direction, unitTransform.forward);
		
		float adjustedMoveSpeed = calculateMoveSpeed(moveSpeedLeft, angleToGoal);
		
		
		unitTransform.forward = Vector3.RotateTowards(unitTransform.forward, direction, turnSpeed * Mathf.Deg2Rad, 0.0f); 
	
		if(distanceToGoal <= (adjustedMoveSpeed + 0.05f)  /*|| distanceToGoal < 0.01f*/){
			
			unitTransform.position = currentGoal;
			
			savedMovement = moveSpeed - distanceToGoal;
			
			currentOrder++;
		
		}else{
			
			unitTransform.position += direction * adjustedMoveSpeed;
			
			savedMovement = 0;
			
		}
	}
	
	private float calculateMoveSpeed(float baseSpeed, float angleToGoal){
		
		// have to move slower while we are turning

		if(angleToGoal > noTurnAngle){

			return 0.0f;

		}else if(angleToGoal > freeTurnAngle && freeTurnAngle < noTurnAngle){

			float turnSlowdownRatio =  Mathf.Min(angleToGoal / (noTurnAngle - freeTurnAngle), 1.0f); // can't slow down lower than stop!

			return baseSpeed * Mathf.Pow((1.0f - turnSlowdownRatio), 2); // exponential slowdown if moving while turning

		}
		
		return baseSpeed;
	}

	public void Highlight(){
		
		meshMaker.Highlight();	
	}
	
	public void Unhighlight(){
		
		currentPathDetails = null;
		
		meshMaker.Unhighlight();
	
		//enabled = false;
	}

	public void StartPath(){
		
		orders.Clear();
		
		orders.AddPoint(unitTransform.position);
		
		meshMaker.StartDrawing(orders);	
		
		//enabled = true;
		
		currentPathDetails = new PathDetails();
		
		//currentPathDetails.stepsRemaining = GameManager.STEPS_PER_PHASE;
		currentPathDetails.stepsRemaining = 5000;
		currentPathDetails.currentDirection = unitTransform.forward;
		//currentPathDetails.carryOverMovement = 0;
	
	}
	
	public bool ProcessPoint(Vector3 point){
		
		if(orders == null || orders.GetNumPoints() == 0) return false;
		
		Vector3 adjustedPoint = new Vector3(point.x, unitTransform.position.y, point.z);
		
		Vector3 difference = adjustedPoint - orders.GetLastPoint();
		
		int numPointsToAdd = (int)(difference.magnitude / 250);//GameManager.DISTANCE_BETWEEN_POINTS);
	
		for(int i=0;i<numPointsToAdd;++i){
		
			Vector3 currentPoint = orders.GetLastPoint() + (difference.normalized*250);//GameManager.DISTANCE_BETWEEN_POINTS);
			
			if(processPotentialOrder(currentPoint)){
			
				orders.AddPoint(currentPoint);
			}else{
				
				break;	
			}
			
		}
		
		if(numPointsToAdd > 0){
			
			if(meshMaker != null){
		
				meshMaker.FillMesh(orders);
			}
			
			return true;
		}else{
		
			return false;
		}
	}
	
	private bool processPotentialOrder(Vector3 point){
		
		if(orders.GetNumPoints() == 0){
			
			Debug.LogError("should always have at least the starting order when getting here");
			
			return false;	
		}
		
		
		Vector3 currentPosition = orders.GetLastPoint();
		
		int stepsUsed = 0;
		
		float adjustedMoveSpeed = moveSpeed;// + currentPathDetails.carryOverMovement;
		
		Vector3 dir = currentPathDetails.currentDirection;

		//
		Vector3 previousPosition = orders.GetSecondLastPoint();

		float angle = Vector3.Angle( (currentPosition - previousPosition), (point - currentPosition));

		//Debug.Log("Angle is " + angle);

		//
		
		while(currentPathDetails.stepsRemaining > stepsUsed){
			
			stepsUsed++;
			
			float distanceToGoal = Vector3.Distance(point, currentPosition);
			
			Vector3 direction = (point - currentPosition).normalized;
			
			float angleToGoal = Vector3.Angle(direction, dir);

			adjustedMoveSpeed = calculateMoveSpeed(adjustedMoveSpeed, angleToGoal);

			dir = Vector3.RotateTowards(dir, direction, turnSpeed * Mathf.Deg2Rad, 0.0f); 

			if(distanceToGoal <= (adjustedMoveSpeed + 0.05f))
			{
				if(angle < noTurnAngle){
					//currentPathDetails.carryOverMovement = moveSpeed - distanceToGoal;

					currentPathDetails.stepsRemaining -= stepsUsed;

					currentPathDetails.currentDirection = dir;
					return true;
				}
				else //force the angle and draw a point anyway
				{
					//should rotate the vector between current point and point so that the angle is within the minimum
				}

				//return true;

			}else{
			
				currentPosition += direction * adjustedMoveSpeed;
				
				adjustedMoveSpeed = moveSpeed;
				
			}	
		}
		
		return false;
	}

	void OnGUI(){
		
		GUI.color = Color.magenta;
		GUI.Label(new Rect(10,10,1000,500), printUnitInfo());	
	}
	
	private string printUnitInfo(){
		
		string unitString = "";
		
		unitString = "position: " + unitTransform.position +
			"\nmovement: " + moveSpeed + 
			"\nfree turn angle: " + freeTurnAngle + 
			"\nno turn angle: " + noTurnAngle + 
			"\nturn speed: " + turnSpeed;
		
		if(currentPathDetails != null){
		
			unitString += "\n\nsteps remaining: " + currentPathDetails.stepsRemaining;// + "\ncarry over movement: " + currentPathDetails.carryOverMovement;
		}

		drawDebugData(orders);
		
		return unitString;
	}
		
	void drawDebugData(CatmullRomSpline spline){
		
		if(spline != null){
			
			for(int i = 0;i < spline.GetNumPoints();++i){
				
				float t = i / (spline.GetNumPoints() - 1.0f);
				
				Vector3 pointA = spline.GetPoint(t);
				Debug.DrawRay( pointA, new Vector3(0, 50.0f * i, 0), Color.yellow);
			}
		}
	}
	
}

using UnityEngine;
using System.Collections;

public class UnitSelector : MonoBehaviour {
	
	private Unit selectedUnit = null;
	
	public LayerMask unitLayer;
	public LayerMask mapLayer;
	
	private bool hasBegunPath = false;
	
	private Unit highlightedUnit = null;
	

	void Update(){
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane, unitLayer)){
			
			highlightedUnit = hit.transform.GetComponent<Unit>();
		}else{
			
			if(highlightedUnit != null && highlightedUnit != selectedUnit){
			
				highlightedUnit.Unhighlight();
			}
			
			highlightedUnit = null;	
		}
		
		if(selectedUnit != null){
			
			if(Input.GetMouseButton(0)){
				
				Debug.DrawRay(ray.origin, ray.direction * Camera.main.farClipPlane, Color.red, 10f);
				
				
				if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane)){//, (mapLayer | unitLayer) )){
						
					//Debug.Log(hit.point);
					
					// don't draw path on self, at least initially, then allow it once you've left it once
					if(hit.transform.gameObject != selectedUnit.gameObject || hasBegunPath){
						
						hasBegunPath = selectedUnit.ProcessPoint(hit.point);
						
					}
				}
			
			}else if(Input.GetMouseButtonUp(0)){
				
				selectedUnit.Unhighlight();
				
				selectedUnit = null;
				
			}
			
		}else{
			
			if(Input.GetMouseButtonDown(0)){
			
				if(Physics.Raycast(ray, out hit, Camera.main.farClipPlane, unitLayer)){
			
					selectedUnit = hit.transform.GetComponent<Unit>();
					
					if(selectedUnit != null){
						
						selectedUnit.StartPath();
						
						hasBegunPath = false;
					}else{
					
						Debug.LogError("we're clicking on an object with the unit layer, but not a unit?  this correct?");
					}
				}	
			}	
		}
		
		// highlight path of unit we are mousing over
		
		if(highlightedUnit != null && highlightedUnit != selectedUnit){
			
			highlightedUnit.Highlight();
		}
		
	}
	
}

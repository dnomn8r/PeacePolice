using UnityEngine;
using System.Collections.Generic;

public class CatmullRomSpline{
	
	private List<Vector3> points = new List<Vector3>();
	public float maxDistance = 2.0f;
	
	public CatmullRomSpline Clone(){
		
		CatmullRomSpline clone = new CatmullRomSpline();
		for( int i = 0; i < points.Count; ++i )
		clone.AddPoint( points[i] );
		return clone;
	}
	
	
	public void RemoveAfterIndex(int index){
	
		if(index < points.Count){
	
			points.RemoveRange(index, points.Count - index);
		}
	}
	
	public void PrependSpline(CatmullRomSpline s){
		
		for(int i = s.GetNumPoints()-1;i>=0;--i){
			
			InsertPoint( s.points[i] );
		}
	}
	
	public void PrependPoints(List<Vector3> p){
		
		for(int i = p.Count - 1;i>=0;--i){
			
			InsertPoint( p[i] );
		}
	}
	
	public void Clear(){
		
		points.Clear();
	}
	
	public int GetNumPoints(){
		
		return points.Count;
	}
	
	public void InsertPoint(Vector3 p){
		
		points.Insert(0, p);
	}
	
	public void AddPoint( Vector3 p )
	{
		points.Add(p);
	}
	
	public int GetNumSegments(){
		
		return points.Count - 1;
	}
	
	public void RemoveLastPoint(){
		
		if(points.Count > 0){
			
			points.RemoveAt(points.Count - 1);
		}
	}
	
	public void RemoveFirstPoint(){
		
		if(points.Count > 0){
			points.RemoveAt(0);
		}
	}
	
	public void SetLastPoint(Vector3 p){
		
		points[points.Count - 1] = p;
	}
	
	public Vector3 GetLastPoint(){
		
		return points[points.Count - 1];
	}
	
	public Vector3 GetFirstPoint(){
		
		if(points.Count > 0){
			
			return points[0]; 
		}else{
			
			return(Vector3.zero);
		}
	}
	
	public Vector3 GetIndexPoint(int i){
		
		return points[i];
	}

	public Vector3 GetSecondLastPoint()
	{
		if(points.Count > 1)
		{
			return points[points.Count-2]; 
		}
		else
		{
			return(Vector3.zero);
		}
	}
	
	public Vector3 GetPoint(float t){
		
		int nP = points.Count - 1;
		
		float t_times_nP = t*nP;
	
		int curveIndex = (int)(t_times_nP);
		float curveOffset = (t_times_nP) - curveIndex;

		return GetPoint(curveIndex, curveOffset);
	}
	
	public Vector3 GetPoint(int ci, float t){
		
		if(points.Count == 0){
		
			return new Vector3( 0, 0, 0 );
		}
		
		int[] ndx = new int[4];
		
		for(int i = 0;i<4;++i){
			
			int val = ci + i - 1;

			if(val < 0){
				val = 0;
			}
			
			if(val >= points.Count){
			
				val = points.Count - 1;
			}
			
			ndx[i] = val;
		}
							
		Vector3 P0 = points[ndx[0]];
		Vector3 P1 = points[ndx[1]];
		Vector3 P2 = points[ndx[2]];
		Vector3 P3 = points[ndx[3]];

		float t2 = t * t;
		float t3 = t2 * t;
			
		return 0.5f * ( ( 2.0f * P1 ) + ( -P0 + P2 ) * t + ( 2.0f * P0 - 5 * P1 + 4 * P2 - P3 ) * t2 + ( -P0 + 3*P1 - 3*P2 + P3 ) * t3 ); 
	}
}
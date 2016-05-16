using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class PathMeshMaker : MonoBehaviour{
	
	public float lineWidth = 25.0f; // 0.5
	public Vector3 offset = new Vector3( 0.0f, 0.01f, 0.0f );
	public int numPoints = 100;
	
	private float uvScale = 0.15f; // 0.2
	
	public Material drawingMaterial;
	public Material regularMaterial;
	public Material highlightMaterial;
	
	public Transform xMarksTheSpot;
	
	public bool isRecording = false;
	public bool isActive = true;
	
	//public FollowPath followPath;
	public float fadeScale = 2.0f;
	
	private Vector3[] vertArray;
	private int[] faceArray;
	private Vector2[] uvArray;
	private Vector3[] normalArray;
	private Color[] colorArray;
	private Mesh myMesh;
	
	public bool isSuccess = false;
	
	private Renderer myRenderer;
	
	void Awake(){
		
		if(numPoints < 2){
			
			numPoints = 2;
		}
			
		vertArray = new Vector3[numPoints * 2];
		uvArray = new Vector2[numPoints * 2];
		normalArray = new Vector3[numPoints * 2];
		colorArray = new Color[numPoints * 2];
		faceArray = new int[(numPoints * 2 - 2) * 3];
	
		InitMesh();
		
		myMesh = new Mesh();
		myMesh.Clear();
		myMesh.vertices = vertArray;
		myMesh.uv = uvArray;
		myMesh.uv2 = uvArray;
		myMesh.colors = colorArray;
		myMesh.normals = normalArray;
		myMesh.triangles = faceArray;
		
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		
		meshFilter.mesh = myMesh;
		transform.position = new Vector3(0, 0, 0);
		transform.eulerAngles = new Vector3(0, 0, 0);
		
		myRenderer = GetComponent<Renderer>();
		/*
		if(xMarksTheSpot != null){
			
			Color c = xMarksTheSpot.renderer.material.GetColor( "_TintColor" );
			c.a = 0;
			xMarksTheSpot.renderer.material.SetColor( "_TintColor", c );
		}*/
		
		Unhighlight();
	}
	
	public void StartDrawing(CatmullRomSpline spline){
		
		myRenderer.sharedMaterial = drawingMaterial;
		
		FillMesh(spline);
	}
	
	public void Highlight(){
		
		myRenderer.sharedMaterial = highlightMaterial;
	}
	
	public void Unhighlight(){
		
		myRenderer.sharedMaterial = regularMaterial;	
	}
	
	void InitMesh(){
		
		for(int i=0;i<numPoints;++i){
			
			uvArray[i*2] = new Vector2(0.5f, 0);
			uvArray[i*2+1] = new Vector2(0.5f, 1);
			
			normalArray[i*2] = new Vector3(0, 1, 0);
			normalArray[i*2+1] = new Vector3(0, 1, 0);
			
			vertArray[i*2] = new Vector3(0, 0, 0);
			vertArray[i*2+1] = new Vector3(0, 0, 0);
			
			colorArray[i*2] = new Color(1, 1, 1, 1);
			colorArray[i*2+1] = new Color(1, 1, 1, 1);
			
			if(i < (numPoints - 1)){
				
				faceArray[(i*6)+0] = (i*2)+1;
				faceArray[(i*6)+1] = (i*2)+2;
				faceArray[(i*6)+2] = (i*2)+0;
				
				faceArray[(i*6)+3] = (i*2)+1;
				faceArray[(i*6)+4] = (i*2)+3;
				faceArray[(i*6)+5] = (i*2)+2;
			}
		}
	}
	
	public void FillMesh(CatmullRomSpline spline){
		
		float ux = 0.0f;
		float ux2 = 0.0f;
		
		for(int i = 0;i<numPoints - 1;++i){
			
			ux = ux2;
			float t = i / (numPoints - 1.0f);
			float t2 = (i + 1) / (numPoints - 1.0f);
			
			Vector3 pointA = spline.GetPoint(t) + offset;
			Vector3 pointB = spline.GetPoint(t2) + offset;
			
			ux2 = ux + Vector3.Distance(pointA, pointB) * uvScale;
			
			Vector3 nVec = (pointB - pointA);
			float swap = nVec.z;
			nVec.z = -nVec.x;
			nVec.x = swap;
			nVec.y = 0.0f;
			nVec.Normalize();
		
			uvArray[i*2] = new Vector2(ux, 0);
			uvArray[i*2+1] = new Vector2(ux, 1);
			
			uvArray[i*2+2] = new Vector3(ux2, 0);
			uvArray[i*2+3] = new Vector3(ux2, 1);
					
			vertArray[i*2] = pointA + nVec * lineWidth;
			vertArray[i*2+1] = pointA - nVec * lineWidth;
			
			vertArray[i*2+2] = pointB + nVec * lineWidth;
			vertArray[i*2+3] = pointB - nVec * lineWidth;
		}
		
		/*
		if( !Input.GetButton( "Fire1" ) )
		{
			float lastUV = uvArray[ (numPoints-1) * 2 ].x;
			float targetUV = Mathf.Floor( lastUV ) - 0.1f;
			
			int i = numPoints - 1;
			while( uvArray[ i * 2 ].x > targetUV && i > 0 )
			{
				colorArray[i*2].a = 0;
				colorArray[i*2+1].a = 0;
				--i;
			}
		}
		*/
		
		myMesh.vertices = vertArray;
		myMesh.colors = colorArray;
		myMesh.uv = uvArray;
		myMesh.RecalculateBounds();		
	}
	
	
	/*
	void LateUpdate(){
		
		if( recorder == null ) return;
				
		Color c;
		
		if(wasDrawing && !isRecording){
			
			if(isSuccess){
				meshRenderer.material = successMaterial;	
			}else{		
				meshRenderer.material = doneMaterial;		
			}
			
			StartCoroutine("slowFade");
			
		}else if(isRecording){
			StopCoroutine("slowFade");
			
			meshRenderer.material = drawingMaterial;
		}
		
		
		wasDrawing = isRecording;
		
		myRenderer.enabled = true;
		
		if( xMarksTheSpot != null ) xMarksTheSpot.renderer.enabled = true;
		
		if( xMarksTheSpot != null )
		{
			//Debug.DrawRay(xMarksTheSpot.transform.position, Vector3.up * 300, Color.black);
			Vector3 p = recorder.GetSpline().GetPoint( 1.0f ) + offset;
			xMarksTheSpot.transform.position = p;
	
			c = xMarksTheSpot.renderer.material.GetColor( "_TintColor" );
			//if( !isRecording )
			{
				
				c.a -= fadeScale * Time.deltaTime;
				
			}
			
			p = recorder.GetSpline().GetPoint( 0.9999f );
			p.y = xMarksTheSpot.transform.position.y;
			xMarksTheSpot.transform.LookAt( p );
			xMarksTheSpot.renderer.material.SetColor( "_TintColor", c );
		}
	}
	*/
}
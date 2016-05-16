Shader "CPX_Custom/Mobile/VertexLit/VertexLit Transparent No Color" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

Category {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	SubShader {
	
		Pass {
			ColorMaterial AmbientAndDiffuse
			Lighting Off
			
	        SetTexture [_MainTex] {
	            Combine texture * primary, texture * primary
	        }
       
		}
	} 
}
}
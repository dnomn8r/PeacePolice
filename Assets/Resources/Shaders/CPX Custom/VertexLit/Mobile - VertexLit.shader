Shader "CPX_Custom/Mobile/VertexLit/VertexLit" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

Category {

	SubShader {
		Material {
			Diffuse [_Color]
			Ambient [_Color]
		}
		Pass {
			ColorMaterial AmbientAndDiffuse
			Lighting Off
        SetTexture [_MainTex] {
            constantColor [_Color]
            Combine texture * constant DOUBLE
        }  
		}
	} 
}
}
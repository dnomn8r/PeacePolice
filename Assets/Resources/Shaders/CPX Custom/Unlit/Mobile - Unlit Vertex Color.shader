// - no lighting
// - no lightmap support

Shader "CPX_Custom/Mobile/Unlit/Unlit Vertex Color" {
Properties {
	_Color ("Main Color", Color) = (1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	Material {
		Diffuse [_Color]
		Ambient [_Color]
	}
	Pass {
		ColorMaterial AmbientAndDiffuse
		Lighting Off
    SetTexture [_MainTex] {
        Combine texture * primary, texture * primary
    }
    SetTexture [_MainTex] {
        constantColor [_Color]
        Combine previous * constant, previous * constant
    }  
	}
}
}

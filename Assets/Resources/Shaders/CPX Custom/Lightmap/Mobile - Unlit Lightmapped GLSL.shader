Shader "CPX_Custom/Mobile/Lightmap/Unlit Lightmapped GLSL" {
   Properties {
      _Color ("Main Color", Color) = (1,1,1,1)
	  _MainTex ("Base (RGB)", 2D) = "white" {}
 	  _LightMap ("Lightmap (RGB)", 2D) = "black" {}
   }
   SubShader {
	  Tags { "RenderType"="Opaque" }
      LOD 100
      Pass {  
		 Lighting Off
         GLSLPROGRAM
         uniform mediump sampler2D _MainTex, _LightMap; 
         uniform highp vec4 _MainTex_ST, _LightMap_ST;
         varying mediump vec2 uv0;
         varying mediump vec2 uv1;
         #ifdef VERTEX
         void main(){
            uv0 = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            uv1 = gl_MultiTexCoord1.xy * _LightMap_ST.xy + _LightMap_ST.zw;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
         	vec4 color = texture2D(_MainTex, uv0);
         	vec4 lightMap = texture2D(_LightMap, uv1);
            gl_FragColor = vec4((color.rgb * 0.5)*(lightMap.rgb+1.0),color.a);         
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "Legacy Shaders/Lightmapped/Diffuse"
}
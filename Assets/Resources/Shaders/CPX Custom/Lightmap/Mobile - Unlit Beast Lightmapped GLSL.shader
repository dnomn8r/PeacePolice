Shader "CPX_Custom/Mobile/Lightmap/Unlit Beast Lightmapped GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
   }
   SubShader {
	  Tags { "RenderType"="Opaque" }
      LOD 100
      Pass {  
         //Tags { "LightMode" = "VertexLM" }
		 Lighting Off
         GLSLPROGRAM
         #include "UnityCG.glslinc"
         uniform sampler2D _MainTex, unity_Lightmap; 
         uniform mediump mat4 unity_LightmapMatrix; 
         uniform highp vec4 _MainTex_ST, unity_LightmapST;
         varying highp vec2 uv1,uv2;
         varying highp vec2 uvColor;
         
         #ifdef VERTEX
         void main(){
            uv1 = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            uv2 = gl_MultiTexCoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
         	vec4 color = texture2D(_MainTex, uv1);
         	vec4 lightMap = texture2D(unity_Lightmap, uv2);
            gl_FragColor = vec4(color.rgb*(2.0*lightMap.rgb),color.a);      
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Lightmap/Unlit Beast Lightmapped CG"
}
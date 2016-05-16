Shader "CPX_Custom/Mobile/Particle/Alphablend Normal" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {} 
      _TintColor ("Tint Color", Color) = (1,1,1,1)
   }
   SubShader {
      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      Pass {    
	     Blend SrcAlpha OneMinusSrcAlpha 
	     AlphaTest Off
		 ColorMask RGB 
		 Cull Off ZWrite Off Fog { Color (0,0,0,0) }
         GLSLPROGRAM
         uniform lowp sampler2D _MainTex; 
         uniform highp vec4 _TintColor, _MainTex_ST;
         varying highp vec2 uv; 
         varying lowp vec4 vertexColor;
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw; 
            vertexColor = gl_Color;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
            gl_FragColor = vertexColor * _TintColor * texture2D(_MainTex, uv);            
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "Mobile/Particles/Alpha Blended"
}
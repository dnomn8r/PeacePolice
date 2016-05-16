Shader "CPX_Custom/Mobile/Transparent/Unlit Transparent GLSL No Color" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
   }
   SubShader {
      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      Pass {    
         ZWrite Off
	     Blend SrcAlpha OneMinusSrcAlpha 
	     AlphaTest Off
         GLSLPROGRAM
         uniform lowp sampler2D _MainTex; 
         uniform highp vec4 _MainTex_ST;
         varying lowp vec2 uv;
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
           
            gl_FragColor = texture2D(_MainTex, uv);      
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Transparent/Unlit Transparent"
}
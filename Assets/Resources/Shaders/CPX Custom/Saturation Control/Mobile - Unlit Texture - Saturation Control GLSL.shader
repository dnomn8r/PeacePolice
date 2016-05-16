Shader "CPX_Custom/Mobile/Saturation Control/Unlit Texture - Saturation Control GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Saturation ("Saturation", Range(0,1)) = 0.0 
   }
   SubShader {
      Pass {    
         GLSLPROGRAM
         #include "../../Headers/PhotoshopMath.glsl"
         uniform lowp sampler2D _MainTex; 
         uniform highp vec4 _MainTex_ST;
         varying lowp vec2 uv;
         uniform lowp float _Saturation;
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
            gl_FragColor = Desaturate(texture2D(_MainTex, uv).rgb,_Saturation);          
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Saturation Control/Unlit Texture - Saturation Control CG"
}
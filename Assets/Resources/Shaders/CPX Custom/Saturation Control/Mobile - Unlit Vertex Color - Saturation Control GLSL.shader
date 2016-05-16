Shader "CPX_Custom/Mobile/Saturation Control/Unlit Vertex Color - Saturation Control GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Color ("Main Color", Color) = (1,1,1,1)
      _Saturation ("Saturation", Range(0,1)) = 0.0 
   }
   SubShader {
      Pass {    
         GLSLPROGRAM
         #include "../../Headers/PhotoshopMath.glsl"
         uniform lowp sampler2D _MainTex; 
         uniform lowp vec4 _Color;
         uniform highp vec4 _MainTex_ST;
         uniform lowp float _Saturation;
         varying lowp vec2 uv;
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
            vec4 color = texture2D(_MainTex, uv);
            color += color;
            gl_FragColor = Desaturate((color * _Color).rgb,_Saturation);              
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Saturation Control/Unlit Vertex Color - Saturation Control CG"
}
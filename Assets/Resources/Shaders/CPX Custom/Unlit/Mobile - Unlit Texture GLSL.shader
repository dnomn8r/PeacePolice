Shader "CPX_Custom/Mobile/Unlit/Unlit Texture GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
   }
   SubShader {
      Pass {    
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
   Fallback "Unlit/Texture"
}
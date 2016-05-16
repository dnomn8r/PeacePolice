Shader "CPX_Custom/Mobile/Saturation Control/Unlit Transparent - Saturation Control GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Color ("Main Color", Color) = (1,1,1,1)
      _Saturation ("Saturation", Range(0,1)) = 0.0 
   }
   SubShader {
      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      Pass {    
         ZWrite Off
	     Blend SrcAlpha OneMinusSrcAlpha 
	     AlphaTest Off
         GLSLPROGRAM
         #include "../../../Headers/PhotoshopMath.glsl"
         uniform lowp sampler2D _MainTex; 
         uniform highp vec4 _MainTex_ST;
         uniform lowp vec4 _Color;
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
            vec4 color = texture2D(_MainTex, uv);
            gl_FragColor = vec4(Desaturate(color.rgb,_Saturation).rgb*_Color.rgb,color.a*_Color.a);            
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Saturation Control/Unlit Transparent - Saturation Control CG"
}
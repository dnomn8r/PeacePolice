Shader "CPX_Custom/Outline - Pixel GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Color("Main Color", Color) = (1,1,1,1)
      _OutlineColor("Outline Color", Color) = (1,1,1,1)
      _Tolerance("Tolerance", Range(0,1)) = 0.5
   }
   SubShader {
      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      Pass {    
         ZWrite Off
	     Blend SrcAlpha OneMinusSrcAlpha 
	     AlphaTest Off
         GLSLPROGRAM
         uniform lowp sampler2D _MainTex; 
         uniform lowp vec4 _Color, _MainTex_ST;
         uniform lowp vec4 _OutlineColor;
         uniform lowp float _Tolerance;
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
            color.rgb *= _Color.rgb;
            if(color.a < _Tolerance)
            {
            	color.rgb = _OutlineColor.rgb;
            	color.rgb *= _OutlineColor.a;
            	color.a += _OutlineColor.a * 0.5;
            }
            gl_FragColor = color;            
         }
         #endif
         ENDGLSL
      }
   }
   // Fallback "Unlit/Texture"
}
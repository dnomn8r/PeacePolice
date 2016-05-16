Shader "CPX_Custom/Mobile/Transparent/Cutout/Cutout GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Color("Main Color", Color) = (1,1,1,1)
      _Cutout("Tolerance", Range(0,1)) = 0.5
   }
   SubShader {
      Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
      Pass {    
         ZWrite On
         GLSLPROGRAM
         uniform lowp sampler2D _MainTex; 
         uniform lowp vec4 _Color, _MainTex_ST;
         uniform lowp float _Cutout;
         varying highp vec2 uv;
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
            vec4 color = texture2D(_MainTex, uv);
            if(color.a - _Cutout < 0.0){discard;}
            vec4 finalColor = vec4(color.rgb*_Color.rgb,color.a*_Color.a);
            gl_FragColor = finalColor;
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Transparent/Cutout/Cutout CG"
}
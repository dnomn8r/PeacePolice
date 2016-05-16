Shader "CPX_Custom/Mobile/Text/Text (Outline) GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Color("Main Color", Color) = (1,1,1,1)
      _OutlineColor("Outline Color", Color) = (1,1,1,1)
   }
   SubShader {
      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      Pass {   
	     Blend SrcAlpha OneMinusSrcAlpha 
	     AlphaTest Off
	     Lighting Off Cull Off ZWrite Off Fog { Mode Off }
         GLSLPROGRAM
         uniform lowp sampler2D _MainTex; 
         uniform lowp vec4 _Color, _OutlineColor;
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
            vec4 color = texture2D(_MainTex, uv);
            color.rgb = color.rgb != vec3(1.0,1.0,1.0) ? mix(_OutlineColor.rgb,_Color.rgb,color.rgb*color.rgb) * color.a : _Color.rgb* color.a;
            gl_FragColor =  color;
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Text/Text (Outline) CG"
}
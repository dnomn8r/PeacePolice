Shader "CPX_Custom/Mobile/VertexLit/VertexLit Transparent GLSL No Color" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
   }
   SubShader {
      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      ZWrite Off
	  Blend SrcAlpha OneMinusSrcAlpha 
      Pass {    
         GLSLPROGRAM
         uniform lowp sampler2D _MainTex; 
         uniform highp vec4 _MainTex_ST;
         varying lowp vec2 uv;
         varying lowp vec4 vertexColor;
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
            vertexColor = gl_Color;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
            vec4 color = texture2D(_MainTex, uv);
            gl_FragColor = vec4(color.rgb,color.a*vertexColor.a);              
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/VertexLit/VertexLit Transparent No Color"
}
Shader "CPX_Custom/Mobile/VertexLit/VertexLit Transparent Super Bright GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Color ("Main Color", Color) = (1,1,1,1)
   }
   SubShader {
      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      ZWrite Off
	  Blend SrcAlpha OneMinusSrcAlpha 
      Pass {    
         GLSLPROGRAM
         uniform lowp sampler2D _MainTex; 
         uniform lowp vec4 _Color;
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
            gl_FragColor = vec4((color.rgb + color.rgb)*_Color.rgb,color.a*vertexColor.a*_Color.a);              
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/VertexLit/VertexLit Transparent Super Bright"
}
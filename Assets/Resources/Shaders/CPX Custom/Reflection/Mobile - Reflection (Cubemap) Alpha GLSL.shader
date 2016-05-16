Shader "CPX_Custom/Mobile/Reflection/Reflection (Cubemap) Alpha GLSL" {
   Properties {
   	  _MainTex ("Base (RGB)", 2D) = "white" {}
      _Cubemap ("Reflection Map", Cube) = "" {}
      _ReflectStrength ("Reflect Strength", Range(0,1.0)) = 0.5
   }
   SubShader {
      Pass {    
         GLSLPROGRAM
         #include "UnityCG.glslinc"
         uniform lowp sampler2D _MainTex;
         uniform lowp samplerCube _Cubemap;
         uniform highp vec4 _MainTex_ST; 
         varying highp vec3 normalDirection, viewDirection;
         uniform lowp float _ReflectStrength;
         varying highp vec2 uv;
         #ifdef VERTEX
         void main(){
            normalDirection = gl_Normal;
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
            viewDirection = (gl_Vertex - (_World2Object*vec4(_WorldSpaceCameraPos,1.0))).xyz;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
         	vec4 color = texture2D(_MainTex,uv);
         	vec3 reflectedDirection = reflect(viewDirection, normalize(normalDirection));
         	gl_FragColor = color + (textureCube(_Cubemap, reflectedDirection) * _ReflectStrength * color.a);         
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Reflection/Reflection (Cubemap) Alpha CG"
}
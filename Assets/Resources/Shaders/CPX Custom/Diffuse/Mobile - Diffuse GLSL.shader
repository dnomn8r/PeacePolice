Shader "CPX_Custom/Mobile/Diffuse/Diffuse GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
   }
   SubShader {
      Pass {    
         GLSLPROGRAM
         #include "UnityCG.glslinc"
         #include "../../Headers/Globals.glsl"
         uniform lowp sampler2D _MainTex; 
         uniform highp vec4 _MainTex_ST;
         varying highp vec2 uv;
         varying highp vec4 normalDirection,lightDirection;
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            normalDirection = _Object2World * vec4(gl_Normal,0.0);
			lightDirection = (_World2Object * LIGHT_DIRECTION);
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
            lowp vec4 textureColor = texture2D(_MainTex, uv);
            lowp vec4 newNormal = normalize(normalDirection);
         	lowp float diffuseColor = dot(newNormal,normalize(lightDirection));
         	gl_FragColor = vec4(textureColor.rgb*diffuseColor,1.0);         
         }
         #endif
         ENDGLSL
      }
   }
   //Fallback "CPX_Custom/Mobile/Color Selection/Unlit Texture - 1 Color Selection CG"
}
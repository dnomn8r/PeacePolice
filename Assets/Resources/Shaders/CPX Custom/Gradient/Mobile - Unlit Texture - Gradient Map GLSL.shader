Shader "CPX_Custom/Mobile/Gradient/Gradient Map GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _GradientTex ("Gradient", 2D) = "white" {}
   }
   SubShader {
      Pass {   
      	 Tags {"LightMode" = "ForwardBase"}  
         GLSLPROGRAM
         #include "UnityCG.glslinc"
         uniform lowp sampler2D _MainTex;
         uniform lowp sampler2D _GradientTex; 
         uniform highp vec4 _MainTex_ST, _GradientTex_ST;
         varying highp vec3 normalDirection, lightDirection;
         varying lowp vec2 uv;
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            normalDirection = gl_Normal;
            lightDirection = (_World2Object * _WorldSpaceLightPos0).xyz;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
            lowp vec4 textureColor = texture2D(_MainTex, uv);
            highp vec3 newNormal = normalize(normalDirection);
			lowp float diffuseNormal = dot(newNormal,normalize(lightDirection).xyz);
			lowp vec4 gradient = texture2D(_GradientTex,vec2(diffuseNormal,0.0)) + gl_LightModel.ambient;
         	gl_FragColor = vec4((textureColor.rgb)*(gradient.rgb),1.0);         
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Gradient/Gradient Map CG"
}
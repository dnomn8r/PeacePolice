Shader "CPX_Custom/Mobile/Rim/Fixed Rim GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _RimColor ("Rim Color", Color) = (0.97,0.88,1,0.75)
      _RimPower ("Rim Power", Float) = 10
   }
   SubShader {
      Pass {   
         GLSLPROGRAM
         #include "UnityCG.glslinc"
         uniform lowp sampler2D _MainTex;
         uniform highp vec4 _MainTex_ST;
         uniform lowp float _RimPower;
         uniform highp vec4 _RimColor;
         varying highp vec3 normalDirection, viewDirection;
         varying highp vec2 uv;
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            normalDirection = gl_Normal;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			float normalLength = length(gl_Normal);
            viewDirection = ObjSpaceViewDir(gl_Vertex);
         }
         #endif
         #ifdef FRAGMENT
         void main(){
            lowp vec4 textureColor = texture2D(_MainTex, uv);
            highp vec3 newNormal = normalize(normalDirection);
			lowp float rimMask = pow(1.0 - (dot(newNormal,normalize(viewDirection).xyz)),16.0);
			lowp vec3 rim = (rimMask * _RimPower) * _RimColor.rgb * _RimColor.a;
         	gl_FragColor = vec4(textureColor.rgb+rim,1.0);         
         }
         #endif
         ENDGLSL
      }
   }
   //Fallback "CPX_Custom/Mobile/Rim/Fixed Rim CG"
}
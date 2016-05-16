Shader "CPX_Custom/Mobile/Specular/Unlit - Specular Map (Fixed) GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _SpecularColor ("Specular Color", Color) = (0.97,0.88,1,0.75)
      _SpecularPower ("Specular Power", Range(0,10.0)) = 2.5
   }
   SubShader {
      Pass {    
         GLSLPROGRAM
         #include "UnityCG.glslinc"
         #include "../../Headers/Globals.glsl"
         uniform lowp sampler2D _MainTex; 
         uniform lowp vec4 _SpecularColor;
         uniform highp vec4 _MainTex_ST;
         uniform lowp float _SpecularPower;
         varying highp vec2 uv;
         varying lowp vec4 specular;
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
            lowp vec3 normal = normalize(gl_Normal).xyz;
			lowp vec3 view = normalize(ObjSpaceViewDir(gl_Vertex)).xyz;
			specular = pow(saturate((dot(view,normal))),10.0/_SpecularPower) * _SpecularColor;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
            lowp vec4 textureColor = texture2D(_MainTex, uv);
         	gl_FragColor = vec4(textureColor.rgb+(textureColor.a*specular.rgb*specular.a),1.0);         
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Specular/Unlit - Specular Map (Fixed) CG"
}
Shader "CPX_Custom/Mobile/Color Selection/Rim/Unlit Texture - 2 Color Selection Rim GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Lookup ("Lookup Texture", 2D) = "white" {}
      _Color1 ("Color 1", Color) = (1.0,1.0,1.0,1.0)
      _Color2 ("Color 2", Color) = (1.0,1.0,1.0,1.0)
      _RimColor ("Rim Color", Color) = (0.97,0.88,1,0.75)
      _RimDirection ("Rim Direction Vector (XYZ)", Vector) = (1, 0, 0, 0)
      _RimPower ("Rim Power", Float) = 2.5
   }
   SubShader {
      Pass {    
         GLSLPROGRAM
         uniform lowp sampler2D _MainTex, _Lookup; 
         uniform lowp vec4 _Color1, _Color2, _RimColor, _RimDirection;
         uniform highp vec4 _MainTex_ST;
         uniform lowp float _RimPower;
         varying lowp vec2 uv;
         varying highp vec3 normalDirection, viewDirection;
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            normalDirection = gl_Normal.xyz;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
            lowp vec4 textureColor = texture2D(_MainTex, uv);
         	highp float lookup = texture2D(_Lookup, uv).a;
         	lowp vec3 finalColor = vec3(0,0,1);
         	lowp vec3 rim = pow(max(0.0, dot(normalize(vec3(_RimDirection)), normalize(normalDirection))), _RimPower) * _RimColor.rgb;
         	if(lookup <= 0.125) { finalColor = textureColor.rgb;}
         	else if(lookup <= 0.375){ finalColor = (_Color1.rgb)*textureColor.rgb;}
         	else { finalColor = (_Color2.rgb)*textureColor.rgb;}
         	gl_FragColor = vec4(finalColor+rim,1.0);         
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Color Selection/Unlit Texture - 2 Color Selection CG"
}
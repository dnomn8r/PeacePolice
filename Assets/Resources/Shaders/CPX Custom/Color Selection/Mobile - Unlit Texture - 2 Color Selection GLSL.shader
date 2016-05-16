Shader "CPX_Custom/Mobile/Color Selection/Unlit Texture - 2 Color Selection GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Lookup ("Lookup Texture", 2D) = "white" {}
      _Color1 ("Color 1", Color) = (1.0,1.0,1.0,1.0)
      _Color2 ("Color 2", Color) = (1.0,1.0,1.0,1.0)
   }
   SubShader {
      Pass {    
         GLSLPROGRAM
         uniform lowp sampler2D _MainTex, _Lookup; 
         uniform lowp vec4 _Color1, _Color2;
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
            lowp vec4 textureColor = texture2D(_MainTex, uv);
         	highp float lookup = texture2D(_Lookup, uv).a;
         	lowp vec3 finalColor = vec3(0,0,1);
         	if(lookup <= 0.125) { finalColor = textureColor.rgb;}
         	else if(lookup <= 0.375){ finalColor = (_Color1.rgb)*textureColor.rgb;}
         	else { finalColor = (_Color2.rgb)*textureColor.rgb;}
         	gl_FragColor = vec4(finalColor,1.0);         
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Color Selection/Unlit Texture - 2 Color Selection CG"
}
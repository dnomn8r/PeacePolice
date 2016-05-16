Shader "CPX_Custom/Mobile/Lightmap/Unlit Beast Lightmapped wSpecular GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _SpecularColor ("Specular Color", Color) = (0.97,0.88,1,0.75)
      _SpecularPower ("Specular Power", Range(0,10.0)) = 2.5
   }
   SubShader {
	  Tags { "RenderType"="Opaque" }
      LOD 100
      Pass {  
         Tags { "LightMode" = "ForwardBase" }
		 Lighting Off
         GLSLPROGRAM
         #include "UnityCG.glslinc"
         #include "../../Headers/Globals.glsl"
         uniform sampler2D _MainTex, unity_Lightmap; 
         uniform mediump mat4 unity_LightmapMatrix; 
         uniform highp vec4 _MainTex_ST, unity_LightmapST;
         uniform lowp vec4 _SpecularColor;
         uniform lowp float _SpecularPower;
         varying highp vec2 uv1,uv2;
         varying lowp vec4 specular;
         #ifdef VERTEX
         void main(){
            uv1 = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            uv2 = gl_MultiTexCoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
            lowp vec3 normal = normalize(gl_Normal).xyz;
            lowp vec3 lightDirection = normalize(LIGHT_DIRECTION).xyz;
            lowp float diffuseNormal = dot(normal,lightDirection);
            lowp vec3 reflect = normalize(2.0 * diffuseNormal * normal - lightDirection);
            lowp vec3 view = normalize(ObjSpaceViewDir(gl_Vertex)).xyz;
            specular = pow(saturate((dot(view,reflect))),10.0/_SpecularPower) * _SpecularColor;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
         	vec4 color = texture2D(_MainTex, uv1);
         	vec4 lightMap = texture2D(unity_Lightmap, uv2);
            gl_FragColor = vec4((color.rgb*(2.0*lightMap.rgb))+(specular.rgb*specular.a),color.a);      
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Lightmap/Unlit Beast Lightmapped wSpecular CG"
}
Shader "CPX_Custom/Mobile/Gradient/Gradient Map + Rim GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _GradientTex ("Gradient", 2D) = "white" {}
      _RimColor ("Rim Color", Color) = (0.97,0.88,1,0.75)
      _RimPower ("Rim Power", Float) = 0
      _LightPosition ("LightPosition (XYZ)", Vector) = (0, 1, 0, 0)
      _SceneAmbient ("Scene Ambient", Color) = (0.0,0.0,0.0,0.75)
      _LightIntensity ("Light Intensity",Float) = 1000.0
   }
   SubShader {
      Pass {   
         GLSLPROGRAM
         #include "UnityCG.glslinc"
         uniform lowp sampler2D _MainTex;
         uniform lowp sampler2D _GradientTex; 
         uniform highp vec4 _MainTex_ST, _GradientTex_ST;
         uniform lowp float _RimPower,_LightIntensity;
         uniform highp vec4 _LightPosition, _SceneAmbient, _RimColor;
         varying lowp float attenuation;
         varying highp vec3 lightVector;
         varying highp vec3 normalDirection, viewDirection;
         varying highp vec2 uv;
         float clampLight(float x) 
		 { 
			 return max(_SceneAmbient.r*2.0, min(_SceneAmbient.r*6.0, x)); 
		 }
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            normalDirection = gl_Normal;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
            lightVector = (_World2Object * _LightPosition).xyz - gl_Vertex.xyz;
			float lightFalloff = length(lightVector)/_LightIntensity;
			float normalLength = length(gl_Normal);
            attenuation = clampLight(pow(normalLength/lightFalloff,2.0));
            viewDirection = ObjSpaceViewDir(gl_Vertex);
         }
         #endif
         #ifdef FRAGMENT
         void main(){
            lowp vec4 textureColor = texture2D(_MainTex, uv);
            highp vec3 newNormal = normalize(normalDirection);
			highp float diffuseLight = (dot(newNormal,normalize(lightVector)) * 0.5 + 0.5);
			lowp vec4 gradient = texture2D(_GradientTex,vec2((diffuseLight),0.0)) + _SceneAmbient;
			lowp float normalBias = dot(vec3(0.0,1.0,0.0),newNormal) * 0.5 + 0.5;
			lowp float rimMask = pow(1.0 - (max(dot(newNormal,normalize(viewDirection).xyz),1.0-diffuseLight)),4.0);
			lowp vec3 rim = (normalBias * rimMask * _RimPower) * _RimColor.rgb * _RimColor.a;
         	gl_FragColor = vec4((textureColor.rgb * gradient.rgb * attenuation)+rim,1.0);         
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Gradient/Gradient Map + Rim CG"
}
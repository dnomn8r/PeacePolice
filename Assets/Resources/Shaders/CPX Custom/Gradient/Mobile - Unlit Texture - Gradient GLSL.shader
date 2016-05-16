Shader "CPX_Custom/Mobile/Gradient/Gradient GLSL" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _GradientStartColor ("Gradient Start Color", Color) = (0.0,0.0,0.0,1.0)
      _GradientEndColor ("Gradient End Color", Color) = (1.0,1.0,1.0,1.0)
      _StepNumber ("Step Number", Float) = 3.0
   }
   SubShader {
      Pass {   
      	 Tags {"LightMode" = "ForwardBase"}  
         GLSLPROGRAM
         #include "UnityCG.glslinc"
         uniform lowp sampler2D _MainTex; 
         uniform lowp vec4 _GradientStartColor, _GradientEndColor;
         uniform highp vec4 _MainTex_ST;
         uniform lowp float _StepNumber;
         varying highp vec3 normalDirection;
         varying lowp vec2 uv;
         #ifdef VERTEX
         void main(){
            uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            normalDirection = gl_Normal;
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
         #endif
         #ifdef FRAGMENT
         void main(){
            lowp vec4 textureColor = texture2D(_MainTex, uv);
            highp vec3 newNormal = normalize(normalDirection);
			lowp float stepSize = 1.0 / clamp(_StepNumber-1.0,1.0,256.0);
			lowp vec4 diffuseBase = _GradientEndColor == _GradientStartColor ? _GradientStartColor * stepSize : _GradientEndColor;
			lowp float diffuseNormal = dot(newNormal,normalize(_WorldSpaceLightPos0).xyz);
			diffuseNormal = clamp(floor((diffuseNormal / stepSize) + 0.5) * stepSize,0.0,1.0);
			lowp vec3 diffuseLight = mix(diffuseBase.rgb,_GradientStartColor.rgb,diffuseNormal);
         	gl_FragColor = vec4(textureColor.rgb*diffuseLight.rgb,1.0);         
         }
         #endif
         ENDGLSL
      }
   }
   Fallback "CPX_Custom/Mobile/Gradient/Gradient CG"
}
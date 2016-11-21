// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "CPX_Custom/Mobile/Gradient/Gradient Map + Rim CG" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _GradientTex ("Gradient", 2D) = "white" {}
      _RimColor ("Rim Color", Color) = (0.97,0.88,1,0.75)
      _RimPower ("Rim Power", Float) = 2.5
      _LightPosition ("LightPosition (XYZ)", Vector) = (0, 1, 0, 0)
      _SceneAmbient ("Scene Ambient", Color) = (0.0,0.0,0.0,0.75)
      _LightIntensity ("Light Intensity",Float) = 1000.0
   }
   SubShader {
      Pass {   
         CGPROGRAM
         #include "UnityCG.cginc"
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex;
         uniform sampler2D _GradientTex; 
         uniform float4 _MainTex_ST, _GradientTex_ST;
         uniform fixed _RimPower,_LightIntensity;
         uniform fixed4 _LightPosition,_SceneAmbient, _RimColor;
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	float2 texcoord	: 	TEXCOORD0;
         	float3 normal	: NORMAL0;
         };
         struct vertexOutput{
         	float4 pos		:	POSITION;
         	float2 UV		:	TEXCOORD0;
         	float3 normalDirection : TEXCOORD1;
         	float3 viewDirection : TEXCOORD2;
         	float3 lightDistance : TEXCOORD3; 
         	float attenuation	: TEXCOORD4;
         };
         struct pixelOutput{
         	float4 color	: COLOR0;
         }; 
         float clampLight(float x) 
		 { 
			 return max(_SceneAmbient.r*2.0, min(_SceneAmbient.r*6.0, x)); 
		 }
         vertexOutput vertexPass(vertexInput input){
         	vertexOutput output;
            output.UV = input.texcoord;
            output.normalDirection = input.normal;
            output.pos = mul(UNITY_MATRIX_MVP,input.vertex);
            output.lightDistance = mul(unity_WorldToObject,_LightPosition).xyz - input.vertex.xyz;
			float lightFalloff = length(output.lightDistance)/_LightIntensity;
			float normalLength = length(input.normal);
            output.attenuation = clampLight(pow(normalLength/lightFalloff,2.0) * 0.5 + 0.5);
            output.viewDirection = ObjSpaceViewDir(input.vertex);
            return output;
         }
         pixelOutput pixelPass(vertexOutput input){
         	pixelOutput output;
            fixed4 textureColor = tex2D(_MainTex, input.UV);
            float3 newNormal = normalize(input.normalDirection);
			float diffuseNormal = dot(newNormal,normalize(input.lightDistance)) * 0.5 + 0.5;
			fixed4 gradient = tex2D(_GradientTex,fixed2(diffuseNormal,0.0)) + _SceneAmbient;
			fixed normalBias = dot(fixed3(0.0,1.0,0.0),newNormal) * 0.5 + 0.5;
			fixed rimMask = pow(1.0 - (max(dot(newNormal,normalize(input.viewDirection).xyz),1.0-diffuseNormal)),4.0);
			fixed3 rim = (normalBias * rimMask * _RimPower) * _RimColor.rgb * _RimColor.a;
         	output.color = float4((textureColor.rgb * gradient.rgb * input.attenuation)+rim,1.0);  
         	return output;       
         }
         ENDCG
      }
   }
}
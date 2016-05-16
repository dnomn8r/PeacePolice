Shader "CPX_Custom/Mobile/Gradient/Gradient CG" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _GradientStartColor ("Gradient Start Color", Color) = (0.0,0.0,0.0,1.0)
      _GradientEndColor ("Gradient End Color", Color) = (1.0,1.0,1.0,1.0)
      _StepNumber ("Step Number", Float) = 3.0
   }
   SubShader {
      Pass {   
      	 Tags {"LightMode" = "ForwardBase"}  
         CGPROGRAM
         #include "UnityCG.cginc"
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex;
         uniform fixed4 _GradientStartColor, _GradientEndColor;
         uniform float4 _MainTex_ST;
         uniform fixed _StepNumber;
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	float2 texcoord	: 	TEXCOORD0;
         	float3 normal	: NORMAL0;
         };
         struct vertexOutput{
         	float4 pos		:	POSITION;
         	float2 UV		:	TEXCOORD0;
         	float3 normalDirection : TEXCOORD1;
         };
         struct pixelOutput{
         	float4 color	: COLOR0;
         };
         vertexOutput vertexPass(vertexInput input){
         	vertexOutput output;
            output.UV = input.texcoord;
            output.normalDirection = input.normal;
            output.pos = mul(UNITY_MATRIX_MVP,input.vertex);
            return output;
         }
         pixelOutput pixelPass(vertexOutput input){
         	pixelOutput output;
            fixed4 textureColor = tex2D(_MainTex, input.UV);
            float3 newNormal = normalize(input.normalDirection);
			fixed stepSize = 1.0 / clamp(_StepNumber-1.0,1.0,256.0);
			fixed4 diffuseBase = _GradientEndColor == _GradientStartColor ? _GradientStartColor * stepSize : _GradientEndColor;
			fixed diffuseNormal = dot(newNormal,normalize(_WorldSpaceLightPos0).xyz);
			diffuseNormal = clamp(floor((diffuseNormal / stepSize) + 0.5) * stepSize,0.0,1.0);
			fixed3 diffuseLight = lerp(diffuseBase.rgb,_GradientStartColor.rgb,diffuseNormal);
         	output.color = float4(textureColor.rgb*diffuseLight.rgb,1.0);  
         	return output;       
         }
         ENDCG
      }
   }
}
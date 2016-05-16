Shader "CPX_Custom/Mobile/Gradient/Gradient Map CG" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _GradientTex ("Gradient", 2D) = "white" {}
   }
   SubShader {
      Pass {   
      	 Tags {"LightMode" = "ForwardBase"}  
         CGPROGRAM
         #include "UnityCG.cginc"
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex;
         uniform sampler2D _GradientTex; 
         uniform float4 _MainTex_ST, _GradientTex_ST;
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	float2 texcoord	: 	TEXCOORD0;
         	float3 normal	: NORMAL0;
         };
         struct vertexOutput{
         	float4 pos		:	POSITION;
         	float2 UV		:	TEXCOORD0;
         	float3 normalDirection : TEXCOORD1;
         	float3 lightDirection : TEXCOORD2;
         };
         struct pixelOutput{
         	float4 color	: COLOR0;
         };
         vertexOutput vertexPass(vertexInput input){
         	vertexOutput output;
            output.UV = input.texcoord;
            output.normalDirection = input.normal;
            output.lightDirection = mul(_World2Object,_WorldSpaceLightPos0).xyz;
            output.pos = mul(UNITY_MATRIX_MVP,input.vertex);
            return output;
         }
         pixelOutput pixelPass(vertexOutput input){
         	pixelOutput output;
            fixed4 textureColor = tex2D(_MainTex, input.UV);
            float3 newNormal = normalize(input.normalDirection);
			fixed diffuseNormal = dot(newNormal,normalize(input.lightDirection).xyz);
			fixed4 gradient = tex2D(_GradientTex,fixed2(diffuseNormal,0.0)) + UNITY_LIGHTMODEL_AMBIENT;
         	output.color = float4((textureColor.rgb * gradient.rgb),1.0);  
         	return output;       
         }
         ENDCG
      }
   }
}
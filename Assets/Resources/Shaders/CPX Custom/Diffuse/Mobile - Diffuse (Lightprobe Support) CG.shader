Shader "CPX_Custom/Mobile/Diffuse/Diffuse (Lightprobe Support) CG" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
   }
   SubShader {
      Pass {   
      	 Tags {"LightMode" = "ForwardBase"}  
         CGPROGRAM
         #include "UnityCG.cginc"
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex;
         uniform float4 _MainTex_ST;
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	float2 texcoord	: 	TEXCOORD0;
         	float4 normal	: NORMAL0;
         };
         struct vertexOutput{
         	float4 pos		:	POSITION;
         	float2 UV		:	TEXCOORD0;
         	float3 worldNormal : TEXCOORD1;
         };
         struct pixelOutput{
         	float4 color	: COLOR0;
         };
         vertexOutput vertexPass(vertexInput input){
         	vertexOutput output;
            output.UV = input.texcoord;
            output.worldNormal = mul((float3x3)_Object2World,input.normal.xyz);
            output.pos = mul(UNITY_MATRIX_MVP,input.vertex);
            return output;
         }
         pixelOutput pixelPass(vertexOutput input){
         	pixelOutput output;
            fixed4 textureColor = tex2D(_MainTex, input.UV);
         	output.color = float4(textureColor.rgb * ShadeSH9(float4(input.worldNormal,1.0)),1.0);  
         	return output;       
         }
         ENDCG
      }
   }
}
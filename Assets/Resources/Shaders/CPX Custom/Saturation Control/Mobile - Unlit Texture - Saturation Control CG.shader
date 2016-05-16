Shader "CPX_Custom/Mobile/Saturation Control/Unlit Texture - Saturation Control CG" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Saturation ("Saturation", Range(0,1)) = 0.0 
   }
   SubShader {
      Pass {  
		 CGPROGRAM
		 #include "../../Headers/PhotoshopMath.hlsl"
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex;
         uniform fixed4 _MainTex_ST;  
         uniform fixed _Saturation;	
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	fixed2 texcoord	: 	TEXCOORD0;
         };
         struct vertexOutput{
         	float4 pos		:	POSITION;
         	fixed2 UV		:	TEXCOORD0;
         };
         struct pixelOutput{
         	float4 color	: COLOR0;
         };
         vertexOutput vertexPass(vertexInput input){
            vertexOutput output;
            output.UV = input.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
            output.pos = mul(UNITY_MATRIX_MVP,input.vertex);
            return output;
         }
         pixelOutput pixelPass(vertexOutput input){
         	pixelOutput output;
         	output.color = Desaturate(tex2D(_MainTex, input.UV).rgb,_Saturation);
         	return output;          
         }
         ENDCG
      }
   }
}
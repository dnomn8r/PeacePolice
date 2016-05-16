// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "CPX_Custom/Mobile/Lightmap/Unlit Lightmapped CG" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
   }
   SubShader {
	  Tags { "RenderType"="Opaque" }
      LOD 100
      Pass {  
         CGPROGRAM
         #include "UnityCG.cginc"
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex;
         // uniform sampler2D unity_Lightmap; 
         uniform float4 _MainTex_ST;
         // uniform float4 unity_LightmapST;
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	float2 texcoord	: 	TEXCOORD0;
         	float2 texcoord1:	TEXCOORD1;
         };
         struct vertexOutput{
         	float4 pos		:	POSITION;
         	float4 UV		:	TEXCOORD0;
         };
         struct pixelOutput{
         	float4 color	: COLOR0;
         };
         vertexOutput vertexPass(vertexInput input){
            vertexOutput output;
            output.UV.xy = input.texcoord;
            output.UV.zw = input.texcoord1 * unity_LightmapST.xy + unity_LightmapST.zw;
            output.pos = mul(UNITY_MATRIX_MVP,input.vertex);
            return output;
         }
         pixelOutput pixelPass(vertexOutput input){
         	pixelOutput output;
         	float4 color = tex2D(_MainTex, TRANSFORM_TEX(input.UV.xy,_MainTex));
         	float4 lightMap = float4(DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, input.UV.zw)),1.0);
         	output.color = float4(color.rgb*lightMap.rgb,color.a);
         	return output;          
         }
         ENDCG
      }
   }
   // Fallback "Unlit/Texture"
}
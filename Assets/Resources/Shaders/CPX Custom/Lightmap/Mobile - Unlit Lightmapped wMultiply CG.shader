Shader "CPX_Custom/Mobile/Lightmap/Unlit Lightmapped wMultiply CG" {
   Properties {
	  _MainTex ("Base (RGB)", 2D) = "white" {}
 	  _LightMap ("Lightmap (RGB)", 2D) = "black" {}
 	  _LightMapModifier ("Lightmap Modifier", Float) = 1.0
   }
   SubShader {
	  Tags { "RenderType"="Opaque" }
      LOD 100
      Pass {  
		 CGPROGRAM
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex;
         uniform sampler2D _LightMap; 
         uniform float4 _MainTex_ST;
         uniform float4 _LightMap_ST;
         uniform float _LightMapModifier;
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	float2 texcoord	: 	TEXCOORD0;
         	float2 texcoord1:	TEXCOORD1;
         };
         struct vertexOutput{
         	float4 pos		:	POSITION;
         	float2 UV1		:	TEXCOORD0;
         	float2 UV2		:	TEXCOORD1;
         };
         struct pixelOutput{
         	float4 color	: COLOR0;
         };
         vertexOutput vertexPass(vertexInput input){
            vertexOutput output;
            output.UV1 = input.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
            output.UV2 = input.texcoord1 * _LightMap_ST.xy + _LightMap_ST.zw;
            output.pos = mul(UNITY_MATRIX_MVP,input.vertex);
            return output;
         }
         pixelOutput pixelPass(vertexOutput input){
         	pixelOutput output;
         	float4 color = tex2D(_MainTex, input.UV1);
         	float4 lightMap = tex2D(_LightMap, input.UV2);
         	output.color = float4(color.rgb*(lightMap.rgb*_LightMapModifier),color.a);
         	return output;          
         }
         ENDCG
      }
   }
}
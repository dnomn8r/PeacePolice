// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "CPX_Custom/Mobile/Lightmap/Unlit Beast Lightmapped wSpecular CG" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _SpecularColor ("Specular Color", Color) = (0.97,0.88,1,0.75)
      _SpecularPower ("Specular Power", Range(0,10.0)) = 2.5
   }
   SubShader {
	  Tags { "RenderType"="Opaque" }
      LOD 100
      Pass {  
         CGPROGRAM
         #include "UnityCG.cginc"
         #include "../../Headers/Globals.hlsl"
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex;
         // uniform sampler2D unity_Lightmap; 
         uniform float4 _MainTex_ST;
         // uniform float4 unity_LightmapST;
         uniform fixed _SpecularPower;
         uniform fixed4 _SpecularColor;
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	float2 texcoord	: 	TEXCOORD0;
         	float2 texcoord1: 	TEXCOORD1;
         	float3 normal	:	NORMAL0;
         };
         struct vertexOutput{
         	float4 pos		:	POSITION;
         	float2 UV1		:	TEXCOORD0;
         	float2 UV2		:	TEXCOORD1;
         	fixed4 specular :	TEXCOORD2;
         };
         struct pixelOutput{
         	float4 color	: COLOR0;
         };
         vertexOutput vertexPass(vertexInput input){
            vertexOutput output;
            output.UV1 = input.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            output.UV2 = float2(input.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw);
            output.pos = mul(UNITY_MATRIX_MVP,input.vertex);
            fixed3 normal = normalize(input.normal);
			fixed3 lightDirection = normalize(LIGHT_DIRECTION).xyz;
			fixed3 diffuseNormal = dot(normal,lightDirection);
			fixed3 reflect = normalize(2.0 * diffuseNormal * normal - lightDirection);
			fixed3 view = normalize(ObjSpaceViewDir(input.vertex)).xyz;
			output.specular = pow(saturate((dot(view,reflect))),10.0/_SpecularPower) * _SpecularColor;
            return output;
         }
         pixelOutput pixelPass(vertexOutput input){
         	pixelOutput output;
         	float4 color = tex2D(_MainTex, input.UV1);
         	float4 lightMap = float4(DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, input.UV2)),1.0);
         	output.color = float4((color.rgb*lightMap.rgb)+(input.specular.rgb*input.specular.a),1.0);
         	return output;          
         }
         ENDCG
      }
   }
   //Fallback "CPX_Custom/Mobile/Lightmap/Unlit (Lightmap Support)"
}
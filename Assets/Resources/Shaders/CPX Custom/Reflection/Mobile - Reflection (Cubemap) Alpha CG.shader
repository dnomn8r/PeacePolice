// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "CPX_Custom/Mobile/Reflection/Reflection (Cubemap) Alpha CG" {
   Properties {
   	  _MainTex ("Base (RGB)", 2D) = "white" {}
      _Cubemap ("Reflection Map", Cube) = "" {}
      _ReflectStrength ("Reflect Strength", Range(0,1.0)) = 0.5
   }
   SubShader {
      Pass {   
      	 Tags {"LightMode" = "ForwardBase"}  
         CGPROGRAM
         #include "UnityCG.cginc"
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform samplerCUBE _Cubemap; 
         uniform sampler2D _MainTex; 
         uniform float4 _MainTex_ST;
         uniform fixed _ReflectStrength;
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	float2 texcoord	: 	TEXCOORD0;
         	float3 normal	: 	NORMAL0;
         };
         struct vertexOutput{
         	float4 pos		:	POSITION;
         	float2 UV		:	TEXCOORD0;
         	float3 normalDirection : TEXCOORD1;
         	float3 viewDirection : TEXCOORD2;
         };
         struct pixelOutput{
         	float4 color	: COLOR0;
         };
         vertexOutput vertexPass(vertexInput input){
         	vertexOutput output;
         	output.UV = input.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
            output.normalDirection = input.normal.xyz;
            output.pos = mul(UNITY_MATRIX_MVP,input.vertex);
            //output.viewDirection = ObjSpaceViewDir(input.vertex).xyz;
            output.viewDirection = (input.vertex - (mul(unity_WorldToObject,float4(_WorldSpaceCameraPos,1.0)))).xyz;
            return output;
         }
         pixelOutput pixelPass(vertexOutput input){
         	pixelOutput output;
         	fixed4 color = tex2D(_MainTex,input.UV);
            fixed3 reflectedDirection = reflect(input.viewDirection, normalize(input.normalDirection));
         	output.color = color + (texCUBE(_Cubemap, reflectedDirection) * _ReflectStrength * color.a);  
         	return output;       
         }
         ENDCG
      }
   }
}
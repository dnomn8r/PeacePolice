Shader "CPX_Custom/Mobile/Shadows/Shadow Caster CG" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		//Tags { "RenderType"="Geometry" }
		LOD 200
		Pass {  
			 CGPROGRAM
			 #pragma multi_compile_fwdbase
	         #pragma vertex vertexPass
			 #pragma fragment pixelPass
	         uniform sampler2D _MainTex;
	         uniform fixed4 _MainTex_ST;  
	         struct vertexInput{
	         	float4 vertex	:	POSITION;
	         	float2 texcoord	: 	TEXCOORD0;
	         };
	         struct vertexOutput{
	         	float4 pos		:	POSITION;
	         	float2 UV		:	TEXCOORD0;
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
	         	float4 textureColor = tex2D(_MainTex, input.UV);
	         	output.color = textureColor;
	         	return output;         
	         }
	         ENDCG
	    }
		// Pass to render object as a shadow caster
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			CGPROGRAM
			#pragma vertex vertexPass
			#pragma fragment pixelPass
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			uniform float4 _MainTex_ST;
			sampler2D _MainTex;

			struct vertexInput{
	         	float4 vertex	:	POSITION;
	        };
			struct vertexOutput{
				V2F_SHADOW_CASTER;
	        };
	        vertexOutput vertexPass(vertexInput v){
	            vertexOutput o;
	            TRANSFER_SHADOW_CASTER(o)
	            return o;
	        }
	        fixed4 pixelPass(vertexOutput i) : COLOR{
	         	SHADOW_CASTER_FRAGMENT(i) 
	        }
			ENDCG
	
		}
	} 
	//FallBack "Diffuse"
}

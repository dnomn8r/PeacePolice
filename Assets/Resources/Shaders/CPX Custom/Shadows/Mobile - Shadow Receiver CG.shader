Shader "CPX_Custom/Mobile/Shadows/Shadow Receiver CG" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
SubShader {
		Tags { "RenderType"="Geometry" }
		LOD 200
		Pass {  
			 Tags{"LightMode"="ForwardBase"}
			 CGPROGRAM
			 #include "UnityCG.cginc"
			 #include "AutoLight.cginc"
			 #include "Lighting.cginc"
			 #pragma multi_compile_fwdbase
			 #pragma fragmentoption ARB_precision_hint_fastest
	         #pragma vertex vertexPass
			 #pragma fragment pixelPass
	         uniform sampler2D _MainTex;
	         uniform fixed4 _MainTex_ST;  
	         struct vertexInput{
	         	float4 vertex	:	POSITION;
	         	fixed2 texcoord	: 	TEXCOORD0;
	         };
	         struct vertexOutput{
	         	float4 pos		:	POSITION;
	         	fixed2 UV		:	TEXCOORD0;
	         	LIGHTING_COORDS(1,2)
	         };
	         struct pixelOutput{
	         	float4 color	: COLOR0;
	         };
	         vertexOutput vertexPass(vertexInput v){
	            vertexOutput o;
	            o.UV = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
	            o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
	            TRANSFER_VERTEX_TO_FRAGMENT(o);
	            return o;
	         }
	         pixelOutput pixelPass(vertexOutput i){
	         	pixelOutput output;
	         	float4 textureColor = tex2D(_MainTex, i.UV);
	         	float  shadow = LIGHT_ATTENUATION(i);
	         	output.color = textureColor * shadow;
	         	return output;         
	         }
	         ENDCG
	    }
	    // Pass to render object as a shadow caster
		Pass {
			Name "ShadowCollector"
			Tags { "LightMode" = "ShadowCollector" }
	
			CGPROGRAM
			#pragma vertex vertexPass
			#pragma fragment pixelPass
			#pragma multi_compile_shadowcollector
			#pragma fragmentoption ARB_precision_hint_fastest
			#define SHADOW_COLLECTOR_PASS
			#include "UnityCG.cginc"
			
			uniform float4 _MainTex_ST;
			sampler2D _MainTex;

			struct vertexInput{
	         	float4 vertex	:	POSITION;
	         	float2 texcoord	: 	TEXCOORD0;
	        };
			struct vertexOutput{
				V2F_SHADOW_COLLECTOR;
	         	//float2 UV		:	TEXCOORD5;
	        };
	        vertexOutput vertexPass(vertexInput v){
	            vertexOutput o;
	            TRANSFER_SHADOW_COLLECTOR(o)
	            //o.UV = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
	            return o;
	        }
	        fixed4 pixelPass(vertexOutput i) : COLOR{
	         	//fixed4 textureColor = tex2D(_MainTex, i.UV);
	         	SHADOW_COLLECTOR_FRAGMENT(i)      
	        }
			ENDCG
	
		}
	}
}
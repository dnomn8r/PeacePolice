Shader "CPX_Custom/Mobile/Text/Text CG" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Color("Main Color", Color) = (1,1,1,1)
   }
   SubShader {
      Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      Pass {   
	     Blend SrcAlpha OneMinusSrcAlpha 
	     AlphaTest Off
	     Lighting Off Cull Off ZWrite Off Fog { Mode Off }
         CGPROGRAM
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex;
         uniform fixed4 _Color; 
         uniform float4 _MainTex_ST;  
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	fixed2 texcoord	: 	TEXCOORD0;
         };
         struct vertexOutput{
         	float4 pos		:	POSITION;
         	half2 UV		:	TEXCOORD0;
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
         	fixed4 color = tex2D(_MainTex, input.UV);
         	output.color = color * _Color;
         	return output;          
         }
         ENDCG
      }
   }
   //Fallback "CPX_Custom/Mobile/Text/Text"
}
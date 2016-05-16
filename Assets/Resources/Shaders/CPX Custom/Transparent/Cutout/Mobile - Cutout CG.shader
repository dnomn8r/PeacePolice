Shader "CPX_Custom/Mobile/Transparent/Cutout/Cutout CG" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Color("Main Color", Color) = (1,1,1,1)
      _Cutout("Tolerance", Range(0,1)) = 0.5
   }
   SubShader {
      Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
      Pass {   
	     ZWrite On
         CGPROGRAM
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex;
         uniform fixed4 _Color; 
         uniform float4 _MainTex_ST;  
         uniform float _Cutout;
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	fixed2 texcoord	: 	TEXCOORD0;
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
         	fixed4 color = tex2D(_MainTex, input.UV);
         	if(color.a - _Cutout < 0.0){discard;}
         	fixed4 finalColor = fixed4(color.rgb*_Color.rgb,color.a*_Color.a);
         	output.color = finalColor;
         	return output;      
         }
         ENDCG
      }
   }
   //Fallback "CPX_Custom/Mobile/Text/Text"
}
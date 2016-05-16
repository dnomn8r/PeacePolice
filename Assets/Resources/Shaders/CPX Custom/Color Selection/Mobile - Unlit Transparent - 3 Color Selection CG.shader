Shader "CPX_Custom/Mobile/Color Selection/Unlit Transparent - 3 Color Selection CG" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _Lookup ("Lookup Texture", 2D) = "white" {}
      _Color1 ("Color 1", Color) = (1.0,1.0,1.0,1.0)
      _Color2 ("Color 2", Color) = (1.0,1.0,1.0,1.0)
      _Color3 ("Color 3", Color) = (1.0,1.0,1.0,1.0)
   }
   SubShader {
   	  Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
      Pass {  
	     Zwrite Off
	     Blend SrcAlpha OneMinusSrcAlpha 
	     AlphaTest Off
		 CGPROGRAM
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex, _Lookup;
         uniform fixed4 _MainTex_ST, _Color1, _Color2, _Color3;  
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
         	float4 textureColor = tex2D(_MainTex, input.UV);
         	float lookup = tex2D(_Lookup, input.UV).a;
         	fixed3 finalColor = fixed3(0,0,1);
         	if(lookup <= 0.125) { finalColor = textureColor.rgb;}
         	else if(lookup <= 0.375){ finalColor = (_Color1.rgb)*textureColor.rgb;}
         	else if(lookup <= 0.625){ finalColor = (_Color2.rgb)*textureColor.rgb;}
         	else { finalColor = (_Color3.rgb)*textureColor.rgb;}
         	output.color = fixed4(finalColor,textureColor.a);
         	return output;         
         }
         ENDCG
      }
   }
}
Shader "CPX_Custom/Mobile/Specular/Unlit - Specular Map (Fixed) CG" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _SpecularColor ("Specular Color", Color) = (0.97,0.88,1,0.75)
      _SpecularPower ("Specular Power", Range(0,10.0)) = 2.5
   }
   SubShader {
      Pass {   
      	 Tags {"LightMode" = "ForwardBase"}  
         CGPROGRAM
         #include "UnityCG.cginc"
         #include "../../Headers/Globals.hlsl"
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex;
         uniform float4 _MainTex_ST;
         uniform fixed _SpecularPower;
         uniform fixed4 _SpecularColor;
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	float2 texcoord	: 	TEXCOORD0;
         	float3 normal	: NORMAL0;
         };
         struct vertexOutput{
         	float4 pos		:	POSITION;
         	float2 UV		:	TEXCOORD0;
         	fixed4 specular : TEXCOORD1;
         };
         struct pixelOutput{
         	float4 color	: COLOR0;
         };
         vertexOutput vertexPass(vertexInput input){
         	vertexOutput output;
            output.UV = input.texcoord;
            output.pos = mul(UNITY_MATRIX_MVP,input.vertex);
            fixed3 normal = normalize(input.normal);
			fixed3 view = normalize(ObjSpaceViewDir(input.vertex)).xyz;
			output.specular = pow(saturate((dot(view,normal))),10.0/_SpecularPower) * _SpecularColor;
            return output;
         }
         pixelOutput pixelPass(vertexOutput input){
         	pixelOutput output;
            fixed4 textureColor = tex2D(_MainTex, input.UV);
         	output.color = float4(textureColor.rgb + (textureColor.a*input.specular.rgb*input.specular.a),1.0);  
         	return output;       
         }
         ENDCG
      }
   }
}
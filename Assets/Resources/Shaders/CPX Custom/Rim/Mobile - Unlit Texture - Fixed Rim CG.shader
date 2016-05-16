Shader "CPX_Custom/Mobile/Rim/Fixed Rim CG" {
   Properties {
      _MainTex ("Base (RGB)", 2D) = "white" {}
      _GradientTex ("Gradient", 2D) = "white" {}
      _RimColor ("Rim Color", Color) = (0.97,0.88,1,0.75)
      _RimPower ("Rim Power", Float) = 2.5
      _LightPosition ("LightPosition (XYZ)", Vector) = (0, 1, 0, 0)
      _SceneAmbient ("Scene Ambient", Color) = (0.0,0.0,0.0,0.75)
      _LightIntensity ("Light Intensity",Float) = 1000.0
   }
   SubShader {
      Pass {   
         CGPROGRAM
         #include "UnityCG.cginc"
         #pragma vertex vertexPass
		 #pragma fragment pixelPass
         uniform sampler2D _MainTex;
         uniform float4 _MainTex_ST;
         uniform fixed _RimPower;
         uniform fixed4 _RimColor;
         struct vertexInput{
         	float4 vertex	:	POSITION;
         	float2 texcoord	: 	TEXCOORD0;
         	float3 normal	: NORMAL0;
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
            output.UV = input.texcoord;
            output.normalDirection = input.normal;
            output.pos = mul(UNITY_MATRIX_MVP,input.vertex);
			float normalLength = length(input.normal);
            output.viewDirection = ObjSpaceViewDir(input.vertex);
            return output;
         }
         pixelOutput pixelPass(vertexOutput input){
         	pixelOutput output;
            fixed4 textureColor = tex2D(_MainTex, input.UV);
            float3 newNormal = normalize(input.normalDirection);
			fixed rimMask = pow(1.0 - (dot(newNormal,normalize(input.viewDirection).xyz)),16.0);
			fixed3 rim = (rimMask * _RimPower) * _RimColor.rgb * _RimColor.a;
         	output.color = float4(textureColor.rgb+rim,1.0);  
         	return output;       
         }
         ENDCG
      }
   }
}
Shader "Custom/DitherImageEffect"
{
		Properties
		{
			_MainTex("Texture", 2D) = "white" {}
			_NoiseTex("Noise", 2D) = "white" {}
			_DitherDistance("Dither Distance", float) = 0.1
			_DitherAmount("Dither Amount", Range(0,1)) = 0
			_DitherColorThreshold("Dither Colour Threshold", Range(0,1)) = 0
			_NoiseMultiply("Noise Texture UV multiply", float) = 10
			_CameraPositionX("Camera Position X", float) = 0
			_CameraPositionY("Camera Position Y", float) = 0
			_CameraSizeX("Camera Size X", float) = 0
			_CameraSizeY("Camera Size Y", float) = 0
		}
		SubShader
		{ 
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					float2 noiseUV : TEXCOORD1;
				};


				sampler2D _MainTex;
				sampler2D _NoiseTex;
				float _DitherDistance;
				float _DitherAmount;
				float _DitherColorThreshold;
				float _NoiseMultiply;
				float _CameraPositionX;
				float _CameraPositionY;
				float _CameraSizeX;
				float _CameraSizeY;

				v2f vert(appdata v) 
				{
					v2f o = (v2f)0;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = v.uv;

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float2 worldCoord;
					worldCoord.x = lerp(_CameraPositionX - _CameraSizeX , _CameraPositionX + _CameraSizeX , i.uv.x);
					worldCoord.y = lerp(_CameraPositionY - _CameraSizeY , _CameraPositionY + _CameraSizeY , i.uv.y);

					float2 noiseUV = worldCoord / _NoiseMultiply;

					fixed4 n = tex2D(_NoiseTex, noiseUV);

					fixed2 offset = (n.rg-0.5) * step(n.b, _DitherAmount);
					
					fixed4 col = tex2D(_MainTex, i.uv + (offset*_DitherDistance));
					fixed4 origCol = tex2D(_MainTex, i.uv);

					fixed3 diff = origCol.rgb - col.rgb;
					float dist = length(diff);

					col = lerp(origCol, col, step (dist, _DitherColorThreshold));
					return col;
				}
				ENDCG
			}
		}
}

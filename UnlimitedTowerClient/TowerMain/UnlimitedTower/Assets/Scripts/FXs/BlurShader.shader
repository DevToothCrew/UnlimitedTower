Shader "Hidden/BlurShader"
{
	Properties
	{
		_BlurRange("Blur Range", range(0.0, 3.0)) = 1.0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		GrabPass {
		}
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
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = float2(v.uv.x, 1.0 - v.uv.y);
				return o;
			}

			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _BlurRange;

			float4 frag (v2f i) : SV_Target
			{
				const float gaussianFilter[9] = {0.05, 0.09, 0.12, 0.15, 0.18, 0.15, 0.12, 0.09, 0.05};
				const float gaussianDist[9] = { -4.0, -3.0, -2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0 };

				float4 col = float4(0.0, 0.0, 0.0, 0.0);

				for (int j = 0; j < 9; j++)
				{
					col += gaussianFilter[j] * tex2D(_GrabTexture, i.uv + _GrabTexture_TexelSize.xy * float2(gaussianDist[j] * _BlurRange, 0.0));
				}
				col.a = 1.0f;
				
				return col;
			}
			ENDCG
		}
		GrabPass {
		}
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
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = float2(v.uv.x, 1.0 - v.uv.y);
				return o;
			}

			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			float _BlurRange;

			float4 frag (v2f i) : SV_Target
			{
				const float gaussianFilter[9] = {0.05, 0.09, 0.12, 0.15, 0.18, 0.15, 0.12, 0.09, 0.05};
				const float gaussianDist[9] = { -4.0, -3.0, -2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0 };

				float4 col = float4(0.0, 0.0, 0.0, 0.0);

				for (int j = 0; j < 9; j++)
				{
					col += gaussianFilter[j] * tex2D(_GrabTexture, i.uv + _GrabTexture_TexelSize.xy * float2(0.0, gaussianDist[j] * _BlurRange));
				}
				col.a = 1.0f;
				return col;
			}
			ENDCG
		}
	}
}

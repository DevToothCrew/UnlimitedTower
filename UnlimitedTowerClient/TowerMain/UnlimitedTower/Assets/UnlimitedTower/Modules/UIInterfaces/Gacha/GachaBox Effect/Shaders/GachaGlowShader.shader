Shader "Hidden/GachaGlowShader"
{
	Properties
	{
		_NoiseTex("Noise", 2D) = "white" {}
		_Alpha("Alpha", range(0.0, 1.0)) = 0.0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

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
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _NoiseTex;
			float _Alpha;


			float getDegreeRatio(float2 co)
			{
				return ((atan2(co.x, co.y) + 3.14159265359) / 3.14159265359) * 0.5;
			}

			float getCircle(float2 co, float rad)
			{
				return smoothstep(0., rad, length(co));
			}

			float4 frag (v2f i) : SV_Target
			{
				const float pi = 3.14159265359;
				const float pi2 = pi * 2.0;

				float2 co = (i.uv - 0.5);
				float3 seedA = tex2D(_NoiseTex, float2(getDegreeRatio(co) + _Time.y * 0.01, 0.25)).rgb;
				float3 seedB = tex2D(_NoiseTex, float2(getDegreeRatio(co) - _Time.y * 0.01, 0.50)).rgb;

				float brigA = 0.2 + seedA.r * 0.6 + 0.1 * sin(seedA.g * pi2 + _Time.y * 0.01);
				float brigB = 0.2 + seedB.r * 0.6 + 0.1 * sin(seedB.b * pi2 + _Time.y * 0.01);

				float brig = max(brigA * brigB, 0.0);
				
				float circle = 1.0 - getCircle(co, brig);
				return float4(1.0, 1.0, 0.0, circle * _Alpha);
			}
			ENDCG
		}
	}
}

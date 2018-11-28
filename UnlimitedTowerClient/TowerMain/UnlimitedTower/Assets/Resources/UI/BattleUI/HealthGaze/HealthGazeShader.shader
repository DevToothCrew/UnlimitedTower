Shader "Hidden/HealthGazeShader"
{
	Properties
	{
		_ValidHealthColor("Valid Health Color", Color) = (0.2, 1.0, 0.3, 1.0)
		_TempHealthColor("Temp Health Color", Color) = (1.0, 0.8, 0.3, 1.0)
		_MaxHealthGrid("Max Health Grid", range(2.0, 20.0)) = 10

		_CurrValue("Current Value", range(0.0, 1.0)) = 0.75
		_NextValue("Next Value", range(0.0, 1.0)) = 0.5

		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Back ZWrite Off ZTest Always
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
			
			float4 _ValidHealthColor;
			float4 _TempHealthColor;
			float _MaxHealthGrid;

			float _CurrValue;
			float _NextValue;

			sampler2D _MainTex;

			float4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				if (col.r > 0.5)
				{
					float gap =  _MaxHealthGrid * 0.02;

					col.rgb = lerp(col.rgb, _TempHealthColor.rgb, smoothstep(_CurrValue + 0.01, _CurrValue - 0.01, i.uv.x));

					float grid = smoothstep(0.5, 0.5 - gap, abs(frac(i.uv.x * _MaxHealthGrid) - 0.5));
					col.rgb = lerp(col.rgb, _ValidHealthColor.rgb * grid, smoothstep(_NextValue + 0.01, _NextValue - 0.01, i.uv.x));
				}
				return col;
			}
			ENDCG
		}
	}
}

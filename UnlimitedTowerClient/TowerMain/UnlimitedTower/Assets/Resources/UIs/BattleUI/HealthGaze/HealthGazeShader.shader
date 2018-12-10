Shader "Hidden/HealthGazeShader"
{
	Properties
	{
		_FCCol("Full Charged Color", Color) = (0.2, 1.0, 0.3, 1.0)
		_EPCol("Empty Color", Color) = (1.0, 0.2, 0.1, 1.0)
		_EGCol("Edge Color", COlor) = (0.3, 0.3 , 0.3, 1.0)

		_CurrVal("Current Value", range(0.0, 1.0)) = 0.75

		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Back ZWrite Off ZTest On
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
			float4 _FCCol;
			float4 _EPCol;
			float4 _EGCol;

			float4 _TempHealthColor;

			float _CurrVal;
			sampler2D _MainTex;

			float4 GetIngageColor(float hr) {
				return lerp(_EPCol, _FCCol, hr);
			}

			float4 GetHighlightedValidColor(float hr, float2 uv)
			{
				float4 shCol = GetIngageColor(hr);
				float4 hlCol = float4(1.0, 1.0, 1.0, 1.0);
				float4 btCol = lerp(shCol, float4(1.0, 1.0, 1.0, 1.0), 0.5);
				hlCol = lerp(hlCol, btCol, smoothstep(0.0, 1.0, uv.x * uv.x));
				return lerp(shCol, hlCol, smoothstep(0.0, 0.8, uv.y));
			}

			float GetEdgeAlpha(float val, float2 uv)
			{
				uv.x += (1.0 - val);
				return tex2D(_MainTex, uv).r;
			}

			inline float GetRatio(float val, float2 uv)
			{
				float egalp = GetEdgeAlpha(_CurrVal, uv);
				return smoothstep(val + 0.002, val - 0.002, uv.x) * egalp;
			}

			float4 frag (v2f i) : SV_Target
			{
				float4 mskch = tex2D(_MainTex, i.uv);


				if (mskch.r > 0.5)
				{
					float4 validColor = GetHighlightedValidColor(_CurrVal, i.uv);

					mskch.rgb = lerp(_EGCol.rgb, validColor.rgb, GetRatio(_CurrVal, i.uv));
				}
				else
				{
					mskch.rgb = _EGCol;
				}


				return mskch;
			}
			ENDCG
		}
	}
}

Shader "Hidden/PopupShader"
{
	Properties
	{
		_ColorEdge("Edge Color", Color) = (1.0, 0.0, 1.0, 1.0)
		_ColorTitle("Title Color", Color) = (0.1, 0.1, 0.1, 1.0)
		_ColorBackground("Background Color", Color) = (0.2, 0.2, 1.0, 1.0)
		_MainTex ("Texture", 2D) = "white" {}
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
			
			sampler2D _MainTex;
			float4 _ColorEdge;
			float4 _ColorTitle;
			float4 _ColorBackground;

			float rand(float2 co)
			{
				return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
			}

			float2 getGridVector(float2 seed, float time)
			{
				//orin dir
				float3 vec = float3(
					rand(seed + 0.01) - 0.5,
					rand(seed + 0.02) - 0.5,
					0.5) * 2.0;

				//rot mat
				float3x3 mat = float3x3(
					cos(time), -sin(time), 0.0,
					sin(time), cos(time), 0.0,
					0.0, 0.0, 1.0);

				//mul
				return normalize(mul(mat, vec).xy);
			}

			float2 getIdx(float2 uv)
			{
				return float2(
					floor(uv.x),
					floor(uv.y));
			}

			float sstep(float a, float b, float t)
			{
				t = t * t * (3.0 - 2.0 * t);
				return lerp(a, b, t);
			}

			float getPerlin(float2 uv, float time)
			{
				float2 co = frac(uv);

				float2 idx_LT = getIdx(uv + float2(0.0, 0.0));
				float2 idx_RT = getIdx(uv + float2(1.0, 0.0));
				float2 idx_LB = getIdx(uv + float2(0.0, 1.0));
				float2 idx_RB = getIdx(uv + float2(1.0, 1.0));

				float2 dir_LT = getGridVector(idx_LT, time);
				float2 dir_RT = getGridVector(idx_RT, time);
				float2 dir_LB = getGridVector(idx_LB, time);
				float2 dir_RB = getGridVector(idx_RB, time);

				float dot_LT = dot(dir_LT, float2(co.x, co.y));
				float dot_RT = dot(dir_RT, float2(-1.0 + co.x, co.y));
				float dot_LB = dot(dir_LB, float2(co.x, -1.0 + co.y));
				float dot_RB = dot(dir_RB, float2(-1.0 + co.x, -1.0 + co.y));

				float tv = sstep(dot_LT, dot_RT, co.x);
				float bv = sstep(dot_LB, dot_RB, co.x);
				float retv = sstep(tv, bv, co.y);

				return retv * 0.5 + 0.5;//dot(co.xy, retv);//
			}

			float4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);

				float4 ret = col.b * _ColorTitle;
				ret = ret + col.g * _ColorBackground;
				ret = ret + col.r * _ColorEdge;

				float perlinAlpha = getPerlin(i.uv * 4.0, _Time.y * 4.0);
				ret.a = lerp(col.a, perlinAlpha * perlinAlpha, col.r);
				
				return ret;
			}
			ENDCG
		}
	}
}

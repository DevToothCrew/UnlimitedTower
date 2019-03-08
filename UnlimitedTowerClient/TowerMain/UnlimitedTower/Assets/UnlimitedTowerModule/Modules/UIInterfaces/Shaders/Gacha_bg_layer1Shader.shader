Shader "Hidden/GachaFxShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
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

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _MainTex;

	float random(in float2 co) {
		return frac(sin(dot(co.xy, float2(12.9898, 78.233)))* 43758.5453123);
	}

	float noise(in float2 co) {
		float2 i = floor(co);
		float2 f = frac(co);

		// Four corners in 2D of a tile
		float a = random(i);
		float b = random(i + float2(1.0, 0.0));
		float c = random(i + float2(0.0, 1.0));
		float d = random(i + float2(1.0, 1.0));

		float2 u = f * f * (3.0 - 2.0 * f);

		return lerp(a, b, u.x) +
			(c - a)* u.y * (1.0 - u.x) +
			(d - b) * u.x * u.y;
	}

#define NUM_OCTAVES 5

	float fbm(in float2 co) {
		float v = 0.0;
		float a = 0.5;
		float2 shift = float2(100.0, 100.0);
		// Rotate to reduce axial bias
		float2x2 rot = float2x2(cos(0.5), sin(0.5),
			-sin(0.5), cos(0.50));
		for (int i = 0; i < NUM_OCTAVES; ++i) {
			v += a * noise(co);
			co = mul(rot, co) * 2.0 + shift;
			a *= 0.5;
		}
		return v;
	}

	float4 frag(v2f i) : SV_Target
	{

		float2 st = i.uv * 3.0;
		//st += st * abs(sin(_Time.y*0.1)*3.0);
		float3 color = float3(0.0, 0.0, 0.0);

		float2 q = float2(0.0, 0.0);
		q.x = fbm(st + 0.00 * _Time.y);
		q.y = fbm(st + float2(1.0, 1.0));

		float2 r = float2(0.0, 0.0);
		r.x = fbm(st + 1.0*q + float2(1.7,9.2) + 0.15 * _Time.y);
		r.y = fbm(st + 1.0*q + float2(8.3,2.8) + 0.126 * _Time.y);

		float f = fbm(st + r);

		color = lerp(float3(0.101961,0.619608,0.666667),
			float3(0.666667,0.666667,0.498039),
			clamp((f*f)*4.0,0.0,1.0));

		color = lerp(color,
			float3(0,0,0.164706),
			clamp(length(q),0.0,1.0));

		color = lerp(color,
			float3(0.666667,1,1),
			clamp(length(r.x),0.0,1.0));
		color = length(color.rgb) / length(float3(1.0, 1.0, 1.0));
		float4 result = float4((f*f*f + .6*f*f + .5*f)*color, 1. * tex2D(_MainTex, i.uv).a);

		return result;// tex2D(_MainTex, i.uv + (result.xy) * min(0.2, 0.1 / length(i.uv - 0.5)));
	}
		ENDCG
	}
	}
}

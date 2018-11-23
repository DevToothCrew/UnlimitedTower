﻿Shader "Hidden/NumberUIShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Alpha("Alpha", range(0.0, 1.0)) = 1.0
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
			
			sampler2D _MainTex;
			fixed _Alpha;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col.a *= _Alpha;
				return col;
			}
			ENDCG
		}
	}
}

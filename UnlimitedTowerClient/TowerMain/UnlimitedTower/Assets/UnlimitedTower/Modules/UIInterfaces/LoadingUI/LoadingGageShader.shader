Shader "Hidden/LoadingGageShader"
{
	Properties
	{
		uAlpha("Alpha", range(0.0, 1.0)) = 1.0
		uProgress ("Progress", range(0.0, 1.0)) = 0.0
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
			float uProgress;
			float uAlpha;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed2 co = (i.uv - 0.5);
				float rad = (atan2(-co.x, -co.y) + 3.141592) / (3.141592 * 2.0);
				rad = smoothstep(uProgress + 0.001, uProgress - 0.001, rad);
				col.a *= rad * uAlpha;
				return col;
			}
			ENDCG
		}
	}
}

/*MIT License

Copyright(c) 2018 Luke Kabat

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files(the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions :

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/



Shader "Outlined/UltimateOutline"
{
	Properties
	{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("Texture", 2D) = "white" {}

		_FirstOutlineColor("Outline color", Color) = (1,0,0,1)
		_FirstOutlineWidth("Outlines width", Range(0.0, 2.0)) = 0.018

		_SecondOutlineColor("Outline color", Color) = (1,0,0,1)
		_SecondOutlineWidth("Outlines width", Range(0.0, 2.0)) = 0.01

		_Angle("Switch shader on angle", Range(0.0, 180.0)) = 56
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata{
		float4 vertex : POSITION;
		float4 normal : NORMAL;
	};

	struct v2f{
		float4 pos : POSITION;
	};

	uniform float4 _FirstOutlineColor;
	uniform float _FirstOutlineWidth;

	uniform float4 _SecondOutlineColor;
	uniform float _SecondOutlineWidth;

	uniform sampler2D _MainTex;
	uniform float4 _Color;
	uniform float _Angle;

	ENDCG

	SubShader{
		Tags{ "Queue" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" }

		//First outline
		Pass{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Back
			CGPROGRAM
			

			#pragma vertex vert
			#pragma fragment frag

			v2f vert(appdata v){
				appdata original = v;

				float3 scaleDir = normalize(v.vertex.xyz - float4(0,0,0,1));
				//This shader consists of 2 ways of generating outline that are dynamically switched based on demiliter angle
				//If vertex normal is pointed away from object origin then custom outline generation is used (based on scaling along the origin-vertex vector)
				//Otherwise the old-school normal vector scaling is used
				//This way prevents weird artifacts from being created when using either of the methods
				if ( degrees(acos(dot(scaleDir.xyz, v.normal.xyz))) > _Angle) {
					v.vertex.xyz += normalize(v.normal.xyz) * _FirstOutlineWidth;
				}else {
					v.vertex.xyz += scaleDir *_FirstOutlineWidth;
				}

				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR{
				return _FirstOutlineColor;
			}

			ENDCG
		}

		//Second outline
		Pass{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Back
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

				v2f vert(appdata v) {
					appdata original = v;

					float3 scaleDir = normalize(v.vertex.xyz - float4(0,0,0,1));
					//This shader consists of 2 ways of generating outline that are dynamically switched based on demiliter angle
					//If vertex normal is pointed away from object origin then custom outline generation is used (based on scaling along the origin-vertex vector)
					//Otherwise the old-school normal vector scaling is used
					//This way prevents weird artifacts from being created when using either of the methods
					if (degrees(acos(dot(scaleDir.xyz, v.normal.xyz))) > _Angle) {
						v.vertex.xyz += normalize(v.normal.xyz) * _SecondOutlineWidth;
					}
					else {
						v.vertex.xyz += scaleDir *_SecondOutlineWidth;
					}

					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					return o;
				}

			half4 frag(v2f i) : COLOR{
				return _SecondOutlineColor;
			}

			ENDCG
		}


		//Surface shader
		Tags{ "Queue" = "Geometry" }
		CGPROGRAM
		//#pragma surface surf Lambert
		//#pragma surface surf Lambert
		#pragma surface surf NoLighting noambient noforwardadd
		struct Input {
			float2 uv_MainTex;
		};

		/*void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}*/


		float4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
			float4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
			
		}

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
	ENDCG
	}
		
	Fallback "Diffuse"
}

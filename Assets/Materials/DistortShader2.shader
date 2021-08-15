Shader "Custom/Distort2"
{
	Properties {
		_BumpAmt  ("Distortion", range (0,128)) = 10
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}

	Category {

		Tags { "Queue"="Transparent" "RenderType"="Opaque" }


		SubShader {
			GrabPass {
				Name "BASE"
				Tags { "LightMode" = "Always" }
			}
			Pass {
				Name "BASE"
				Tags { "LightMode" = "Always" }
				
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord: TEXCOORD0;
					float4 vertexColor : COLOR;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float4 uvgrab : TEXCOORD0;
					float2 uvbump : TEXCOORD1;
					float2 uvmain : TEXCOORD2;
					float4 vertexColor : COLOR;

				};

				sampler2D _GrabTexture;
				float4 _GrabTexture_TexelSize;
				float _BumpAmt;
				float4 _BumpMap_ST;


				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.vertexColor = v.vertexColor;

					float scale = 1.0;
					#if UNITY_UV_STARTS_AT_TOP
					if (_GrabTexture_TexelSize.y < 0)
						scale = -1.0;
					#endif
					o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
					o.uvgrab.zw = o.vertex.zw;
					o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );

					return o;
				}

				sampler2D _BumpMap;

				half4 frag (v2f i) : SV_Target
				{
					half2 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )).rg; 
					float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy * i.vertexColor.a;
					i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
					
					half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));

					return col;
				}
				ENDCG
			}
		}

	}
}
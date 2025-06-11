Shader "Custom/Distort" {
	Properties{
		_BumpAmt("Distortion", range(0,4096)) = 10
		_BumpMap("Normalmap", 2D) = "bump" {}
		_Speed("Speed", range(0.1,4)) = 2
	}

	SubShader{
		Tags { "RenderType" = "Opaque" "Glowable" = "True" "Queue" = "Transparent+101" "IgnoreProjector" = "True" }

		// Used to grab the current framebuffer.
		GrabPass{
				Name "BASE"
				Tags{ "LightMode" = "Always" }
			}

		Pass{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord: TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float4 uvgrab : TEXCOORD0;
				float2 uvbump : TEXCOORD1;
				fixed4 color : COLOR;
			};

			float _BumpAmt;
			float _Speed;
			float4 _BumpMap_ST;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale = 1.0;
				#endif
				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.zw;
				o.uvbump = TRANSFORM_TEX(ComputeScreenPos(v.vertex), _BumpMap);
				o.uvbump.x += _Time.x * _Speed;
				o.uvbump.y += _Time.x * _Speed;
				o.color = v.color;
				return o;
			}

			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
			sampler2D _BumpMap;

			half4 frag(v2f i) : SV_Target
			{
				// calculate perturbed coordinates
				half2 bump = UnpackNormal(tex2D(_BumpMap, i.uvbump)).rg; // we could optimize this by just reading the x & y without reconstructing the Z
				float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy* i.color.r;
				i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;

				return tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
			}
			ENDCG
		}
	}

	// ------------------------------------------------------------------
	// Fallback for older cards and Unity non-Pro

	Fallback "Diffuse"

}
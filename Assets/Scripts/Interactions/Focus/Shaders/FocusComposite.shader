Shader "Hidden/FocusComposite"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

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
				float4 vertex : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			float2 _MainTex_TexelSize;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv0 = v.uv;
				o.uv1 = v.uv;

				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
					o.uv1.y = 1 - o.uv1.y;
				#endif

				return o;
			}

			sampler2D _MainTex;
			sampler2D _BlurredFrameTex;

			float4 frag(v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv0);
				float4 blur = tex2D(_BlurredFrameTex, i.uv1);
				float x = abs(i.uv1.x - 0.5);
				float y = abs(i.uv1.y - 0.5);
				float blend = x * x + y * y;
				blend = min(1, blend * blend * 35);
				return col * (1-blend) + blur * blend;
			}
			ENDCG
		}
	}
}
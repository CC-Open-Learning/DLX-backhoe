Shader "CoreStandard" {
    Properties{
        _Color("Color", Color) = (0.588,0.588,0.588,1)
        _MainTex("Texture", 2D) = "white" {}
        
        _MetallicSmoothnessOcclusionMap("Metallic(R)/Smoothness(G)/Occlusion(B)", 2D) = "white" {}

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0

        _BumpScale("Normal Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _RimColor ("Rim Color", Color) = (0.89,0.93,1.0,0.0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 4.0
        _RimStrength ("Rim Level", Range(0.0,1.0)) = 1.0
    }
    SubShader{
        Tags{ "RenderType" = "Opaque" "Glowable" = "True"}
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #include "UnityCG.cginc"

        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 viewDir;
        };

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _MetallicSmoothnessOcclusionMap;
        float4 _Color;
        float _Glossiness;
        float _Metallic;
        float _OcclusionStrength;
        float _BumpScale;

        float4 _RimColor;
        float _RimPower;
        float _RimStrength;

        void surf(Input IN, inout SurfaceOutputStandard o) {
            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb * _Color;
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap)) * _BumpScale;
            fixed3 m = tex2D (_MetallicSmoothnessOcclusionMap, IN.uv_MainTex).rgb;
            o.Metallic = m.r; // _Metallic;
            o.Smoothness = (1-m.g) * _Glossiness; // _Glossiness;
            o.Occlusion = m.b * _OcclusionStrength;

            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = _RimStrength*(_RimColor.rgb * pow (rim, _RimPower)) * o.Occlusion;
        }
        ENDCG
    }
    Fallback "Diffuse"
}
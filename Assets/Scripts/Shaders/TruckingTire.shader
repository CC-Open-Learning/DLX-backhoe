Shader "Trucking/Wheel" {
    Properties{
        _Color("Color", Color) = (0.588,0.588,0.588,1)
        _MainTex("Texture", 2D) = "white" {}
        
        _MetallicSmoothnessOcclusionMap("Metallic(R)/Smoothness(G)/Occlusion(B)", 2D) = "white" {}

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0

        _BumpScale("Normal Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _BadMainTex("Bad Texture", 2D) = "white" {}
        _BadMRO("Bad MRO", 2D) = "white" {}
        _BadBumpMap("Bad Normal Map", 2D) = "white" {}

        _BlendMask("Blend Mask", 2D) = "white" {}

        _RimDamage("Wheel Rim Damage", Range(0.0, 1.0)) = 0  
        _TreadHeightOffset("Extra Tread Height", Range(0.0, 0.2)) = 0
        _TreadWear("Tread Wear", Range(0.0, 1.0)) = 0
        _TreadWearDisplacement("Tread Wear Max Vertex Displacement", Range(0.0, 0.1)) = 0.03
        _UnevenWear("Tread Unevenness", Range(0.0, 1.0)) = 0
        _UnevenWearStart("Uneven Wear Offset", Range(-5,5)) = 0
        _WeatherCracks("Weather Cracks", Range(0.0, 1.0)) = 0
        
        _RimColor ("Rim Color", Color) = (0.89,0.93,1.0,0.0)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 4.0
        _RimStrength ("Rim Level", Range(0.0,1.0)) = 1.0
    }
    SubShader{
        Tags{ "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
            
            // we will use this to pass custom data to the surface function
            fixed4 color : COLOR;
        };

        struct Input {
            float2 uv_MainTex;
            float2 uv_BadMainTex;
            float2 uv_BlendMask;
            float2 uv_BumpMap;
            float3 viewDir;
            float4 color : COLOR;      
        };

        float _TreadWear;
        float _TreadHeightOffset;
        float _UnevenWear;
        float _UnevenWearStart;
        float _WeatherCracks;
        float _RimDamage;
        float _TreadWearDisplacement;

        void vert (inout appdata v)
        {
            float isTread = (1-v.color.r);
            float isNormalAffected = (1-v.color.g);

            float unevenWear = abs((v.vertex.x - _UnevenWearStart) / (_UnevenWearStart * 2)) * _UnevenWear;
            float normalWear = _TreadWear;

            float unevenWearDisp = isTread * unevenWear * _TreadWearDisplacement;
            float normalWearDisp = isTread * (normalWear * _TreadWearDisplacement -  _TreadHeightOffset);

            v.vertex.yz -= (normalWearDisp + unevenWearDisp) * v.vertex.yz;
            v.normal = lerp(v.normal, float3(0, v.vertex.y, v.vertex.z), saturate(isNormalAffected * (normalWear + unevenWear)));
            v.color.rgb = v.vertex.xyz;
        }

        sampler2D _BadMainTex;
        sampler2D _BadBumpMap;
        sampler2D _BadMRO;

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _MetallicSmoothnessOcclusionMap;

        sampler2D _BlendMask;

        float4 _Color;
        float _Glossiness;
        float _Metallic;
        float _OcclusionStrength;
        float _BumpScale;

        float4 _RimColor;
        float _RimPower;
        float _RimStrength;

        void surf(Input IN, inout SurfaceOutputStandard o) {
            fixed3 blendMask = tex2D (_BlendMask, IN.uv_BlendMask).rgb;

            float3 localPosition = IN.color.rgb;

            fixed weatherCracks = blendMask.g * _WeatherCracks;
            fixed rimDamage = blendMask.b * _RimDamage;
            fixed treadWear = blendMask.r * (_TreadWear + _UnevenWear * abs((localPosition - _UnevenWearStart) / (_UnevenWearStart * 2)));
            fixed totalBlend = weatherCracks + rimDamage + treadWear;

            o.Albedo = ((1-totalBlend) * tex2D (_MainTex, IN.uv_MainTex).rgb + totalBlend * tex2D (_BadMainTex, IN.uv_BadMainTex).rgb) * _Color;
            o.Normal = ((1-totalBlend) * UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap)) + totalBlend * UnpackNormal (tex2D (_BadBumpMap, IN.uv_BadMainTex))) * _BumpScale;
            fixed3 m = (1-totalBlend) * tex2D (_MetallicSmoothnessOcclusionMap, IN.uv_MainTex).rgb + totalBlend * tex2D (_BadMRO, IN.uv_BadMainTex).rgb;
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
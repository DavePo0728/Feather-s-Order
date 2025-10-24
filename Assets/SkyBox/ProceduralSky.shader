Shader "Skybox/ProceduralSky"
{
    Properties
    {
        _TopColor ("Sky Top Color", Color) = (0.05,0.2,0.6,1)
        _HorizonColor ("Horizon Color", Color) = (0.7,0.5,0.3,1)
        _BottomColor ("Ground Color", Color) = (0.9,0.9,0.8,1)
        _HorizonHeight ("Horizon Height (-1..1)", Range(-1,1)) = 0.0
        _HorizonSharpness ("Horizon Sharpness", Range(0.1,10)) = 3.0
        _Exposure ("Exposure (brightness)", Range(0,4)) = 1.0
        _Tint ("Overall Tint", Color) = (1,1,1,1)

        _MainTex("Overlay Texture", 2D) = "white" {}
        _OverlayBlend("Overlay Blend", Range(0,1)) = 0

        // ⭐ 星星屬性
        _StarTex("Star Texture", 2D) = "black" {}
        _StarIntensity("Star Intensity", Range(0,5)) = 1.0
        _StarAlpha("Star Alpha", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" }
        Cull Front
        ZWrite Off
        ZTest LEqual
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appv
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _OverlayBlend;

            fixed4 _TopColor;
            fixed4 _HorizonColor;
            fixed4 _BottomColor;
            float _HorizonHeight;
            float _HorizonSharpness;
            float _Exposure;
            fixed4 _Tint;

            // ⭐ 星星
            sampler2D _StarTex;
            float4 _StarTex_ST;
            float _StarIntensity;
            float _StarAlpha;

            v2f vert(appv v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                return o;
            }

            fixed4 BlendGradient(float y)
            {
                float topFactor = saturate((y - _HorizonHeight) * _HorizonSharpness * 0.5 + 0.5);
                float bottomEdge = _HorizonHeight - 0.25;
                float bottomFactor = saturate((y - bottomEdge) * _HorizonSharpness * 0.5 + 0.5);

                fixed3 topBlend = lerp(_HorizonColor.rgb, _TopColor.rgb, topFactor);
                fixed3 finalCol = lerp(_BottomColor.rgb, topBlend, bottomFactor);

                return fixed4(finalCol, 1.0);
            }

           fixed4 frag(v2f i) : SV_Target
{
    float y = i.worldNormal.y;
    fixed4 gradCol = BlendGradient(y);

    // ---- 基本 Overlay ----
    float2 uv;
    uv.x = atan2(i.worldNormal.x, i.worldNormal.z) / (2 * UNITY_PI) + 0.5;
    uv.y = i.worldNormal.y * 0.5 + 0.5;

    fixed4 texCol = tex2D(_MainTex, uv);
    fixed4 skyCol = lerp(gradCol, texCol, _OverlayBlend);

    // ---- 星星 (加 UV 控制) ----
    float2 starUV = TRANSFORM_TEX(uv, _StarTex); // 可用 Tiling/Offset
    fixed4 starCol = tex2D(_StarTex, starUV);
    starCol.rgb *= _StarIntensity;
    starCol.a *= _StarAlpha;

    // Additive 混合
    skyCol.rgb = lerp(skyCol.rgb, skyCol.rgb + starCol.rgb, starCol.a);

    // ---- 曝光 & Tint ----
    skyCol.rgb *= _Exposure * _Tint.rgb;

    return skyCol;
}

            ENDCG
        }
    }

    FallBack Off
}

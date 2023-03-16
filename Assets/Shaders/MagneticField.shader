Shader "Custom/ElectromagneticFieldURP"
{
    Properties
    {
        [MainTexture] _MainTex("Texture", 2D) = "white" {}
        [MainColor] _Color("Color", Color) = (1, 1, 1, 1)
        _SecondaryColor("Secondary Color", Color) = (0, 1, 0, 1)
        _ColorBlendFalloff("Color Blend Falloff", Range(0.1, 2)) = 1
        _Falloff("Falloff", Range(0, 2)) = 1
        _Intensity("Intensity", Range(0, 10)) = 1
        _Speed("Speed", Range(-5, 5)) = 1
        _NoiseScale("Noise Scale", Range(0, 10)) = 1
        _NumWaves("Number of Waves", Range(1, 10)) = 4
        _WaveSharpness("Wave Sharpness", Range(0, 10)) = 2
        _FresnelPower("Fresnel Power", Range(0, 10)) = 3
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 normalWS : NORMAL;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _Color;
        float4 _SecondaryColor;
        float _ColorBlendFalloff;
        float _Falloff;
        float _Intensity;
        float _Speed;
        float _NoiseScale;
        float _NumWaves;
        float _WaveSharpness;
        float _FresnelPower;

        Varyings vert(Attributes v)
        {
            Varyings o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_TRANSFER_INSTANCE_ID(v, o);
            o.uv = v.uv;
            o.vertex = TransformObjectToHClip(v.positionOS);
            o.normalWS = TransformObjectToWorldNormal(v.normalOS);
            return o;
        }

        half4 frag(Varyings IN) : SV_Target
        {
            float2 center = float2(0.5, 0.5);
            float distanceFromCenter = distance(IN.uv, center);
            float alpha = smoothstep(0.5, 0.4, distanceFromCenter);

            // Adding noise for randomness
            float noise = _Intensity * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv * _NoiseScale).r;

            // Adding sine wave for wavy effect
            float wave = sin(distanceFromCenter * _Falloff * _NumWaves * 2 * 3.14159 + _Time.y * _Speed);
            wave = pow(abs(wave), _WaveSharpness);
            float fieldStrength = wave * noise;

            // Fresnel effect for blending
            float3 viewDirection = normalize(_WorldSpaceCameraPos - IN.vertex.xyz) + 0.0001;
            float fresnel = pow(1.0 - abs(dot(normalize(IN.normalWS), viewDirection)), _FresnelPower);
            fieldStrength *= fresnel;

            float colorLerp = smoothstep(0.5 - (_ColorBlendFalloff / 2), 0.5 + (_ColorBlendFalloff / 2), distanceFromCenter);
            half4 col = lerp(_Color, _SecondaryColor, colorLerp) * fieldStrength;

            col.a *= alpha;
            return col;
        }

        ENDHLSL

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent" }

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

            #pragma multi_compile _ _SHADOW_SOFTNESS
            #pragma multi_compile _ _SHADOWS_CLIP

            ENDHLSL
        }
    }
}

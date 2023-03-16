Shader "Custom/MagneticField" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _FieldLines ("Field Line Intensity", Range(0, 5)) = 1
        _LineSpacing ("Line Spacing", Range(0.1, 5)) = 1
        _LineWidth ("Line Width", Range(0.001, 0.1)) = 0.01
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Speed ("Animation Speed", Range(-5, 5)) = 1
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        float _FieldLines;
        float _LineSpacing;
        float _LineWidth;
        float4 _Color;
        float _Speed;

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        float2 curlNoise(float2 uv) {
            const float e = 0.01;
            float2 d0 = float2(e, 0);
            float2 d1 = float2(0, e);
            float2 texelSize;
            _MainTex.GetDimensions(texelSize.x, texelSize.y);
            texelSize = 1.0 / texelSize;
            float2 noise0 = texelSize * (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d0).r - SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - d0).r);
            float2 noise1 = texelSize * (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + d1).r - SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - d1).r);
            return float2(noise1.x - noise0.y, noise0.x + noise1.y);
        }

        float4 MagneticFieldFunction(float2 uv) {
            float2 timeOffset = _Time.y * _Speed * float2(1, -1);
            float2 center = float2(0.5, 0.5);
            float2 offset = uv - center;
            float2 fieldDirection = normalize(curlNoise((uv + timeOffset) * _FieldLines) * _LineSpacing);
            float fieldLine = dot(offset, fieldDirection);
            float lineVal = smoothstep(0.5 - _LineWidth, 0.5 + _LineWidth, 0.5 + sin(fieldLine) * 0.5);
            return float4(lineVal, lineVal, lineVal, lineVal);
        }

        struct Attributes {
            float3 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        Varyings vert(Attributes v) {
            Varyings o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_TRANSFER_INSTANCE_ID(v, o);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            o.uv = v.uv;
            float3 positionWS = TransformObjectToWorld(v.positionOS);
            o.vertex = TransformObjectToHClip(positionWS);
            return o;
        }

        half4 frag(Varyings IN) : SV_Target {
            half4 col = _Color * MagneticFieldFunction(IN.uv).r;
            return col;
        }

        ENDHLSL

        Pass {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent" }

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

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

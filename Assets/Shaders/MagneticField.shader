Shader "Custom/MagneticField" {
    Properties {
        _FieldLines ("Field Line Intensity", Range(0, 5)) = 1
        _LineSpacing ("Line Spacing", Range(0.1, 5)) = 1
        _LineWidth ("Line Width", Range(0.001, 0.1)) = 0.01
        _Color ("Color", Color) = (1, 1, 1, 1)
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

        float4 MagneticFieldFunction(float2 uv) {
            float2 center = float2(0.5, 0.5);
            float2 offset = uv - center;
            float angle = atan2(offset.y, offset.x);
            float radius = length(offset);

            float fieldLine = sin(_FieldLines * angle - radius * _LineSpacing);
            float lineVal = smoothstep(0.5 - _LineWidth, 0.5 + _LineWidth, 0.5 + fieldLine * 0.5);

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
            #pragma target 3.0
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}

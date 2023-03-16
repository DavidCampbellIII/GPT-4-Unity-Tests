Shader "Custom/FadeEllipseURP"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
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
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _Color;

        Varyings vert(Attributes v)
        {
            Varyings o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_TRANSFER_INSTANCE_ID(v, o);
            o.uv = v.uv;
            o.vertex = TransformObjectToHClip(v.positionOS);
            return o;
        }

        half4 frag(Varyings IN) : SV_Target
        {
            float2 center = float2(0.5, 0.5);
            float distanceFromCenter = distance(IN.uv, center);
            float alpha = smoothstep(0.5, 0.4, distanceFromCenter);
            half4 col = _Color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
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

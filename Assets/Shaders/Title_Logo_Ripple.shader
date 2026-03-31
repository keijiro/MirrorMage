Shader "Custom/LogoRipple"
{
    Properties
    {
        [MainTexture] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Speed("Speed", Float) = 4.0
        _Frequency("Frequency", Float) = 15.0
        _Amplitude("Amplitude", Float) = 0.015
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _Color;
            float _Speed;
            float _Frequency;
            float _Amplitude;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color * _Color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Upward moving ripple effect
                // Using sin based on Y to offset X
                float ripple = sin(input.uv.y * _Frequency - _Time.y * _Speed) * _Amplitude;
                float2 uv = input.uv + float2(ripple, 0);
                
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * input.color;
                
                // Premultiply alpha for Sprite rendering
                color.rgb *= color.a;
                return color;
            }
            ENDHLSL
        }
    }
}

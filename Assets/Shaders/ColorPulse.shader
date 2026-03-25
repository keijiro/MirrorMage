Shader "Custom/ColorPulse"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _TargetColor ("Target Color", Color) = (1, 0, 1, 1)
        _Tolerance ("Tolerance", Range(0, 2)) = 0.5
        _Softness ("Softness", Range(0.01, 1)) = 0.1
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.5
        _PulseSpeed ("Pulse Speed", Float) = 2.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TargetColor;
            float _Tolerance;
            float _Softness;
            float _PulseAmount;
            float _PulseSpeed;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // Calculate distance to target color
                float dist = distance(c.rgb, _TargetColor.rgb);
                
                // Calculate weight based on tolerance and softness
                // Smoothstep provides a nice transition
                float weight = 1.0 - smoothstep(_Tolerance - _Softness, _Tolerance, dist);
                
                // Pulse logic
                float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount * weight;
                
                c.rgb *= pulse;
                c.rgb *= c.a; // Premultiplied alpha
                return c;
            }
            ENDCG
        }
    }
}

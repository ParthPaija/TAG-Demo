Shader "Unlit/PealDistortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainDistortion ("MainDistortion", 2D) = "white" {}
        _Pattern ("Pattern", 2D) = "white" {}
        _PatternColor("Pattern Color",Color) = (1,1,1,1)

        _ScrollSpeed("Scroll Speed",Float) = 1

        _PatternPower ("Pattern Power",Range(0,5)) = 1
        _MainLinePower ("Main Line Power Power",Range(0,5)) = 1

        _MainDistortionStrength ("Main Distortion Strength",Float) = 1
        _MainDistortionScale ("Main Distortion Scale",Float) = 1
        _MainDistortionSpeed ("Main Distortion Speed",Float) = 1

        _Pow("Pow",Float) = 1

        _Seed("Seed",Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _PatternDistortionNoise;
            sampler2D _Pattern;
            sampler2D _MainDistortion;
            sampler2D _GlowTexture;
            float4 _PatternDistortionNoise_ST;
            float _PatternNoiseStrength;
            float _PatternPower;
            float4 _PatternColor;
            float _MainDistortionStrength;
            float _MainDistortionScale;
            float _MainDistortionSpeed;
            fixed _Seed;
            fixed _MainLinePower;
            fixed _ScrollSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uvf = i.uv;
                uvf.x += _Time.y * _MainDistortionSpeed + _Seed;
                uvf *= _MainDistortionScale;

                fixed4 n1 = tex2D(_MainDistortion, uvf);
                float n2 = (n1.r + n1.g + n1.b)/3;

                i.uv += n2 * _MainDistortionStrength;
                i.uv.y -= _MainDistortionStrength/2;
                i.uv.x -= _MainDistortionStrength/2;

                fixed2 uv  = i.uv;
                uv.x += _Time.y * _ScrollSpeed + _Seed;

                fixed4 col = tex2D(_MainTex, uv) * i.color;

                fixed4 upperN = tex2D(_Pattern, i.uv) * _PatternColor;
                fixed pbw = (upperN.r + upperN.g + upperN.b)/3;

                upperN.a = pow(upperN.a, _PatternPower);

                float a = col.a;

                col = lerp(col,upperN,upperN.a);

                col.a = pow(a, _MainLinePower);
                return col;
            }
            ENDCG
        }
    }
}
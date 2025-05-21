Shader "TAG/Sprite Bubble Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _Glow ("Glow", 2D) = "white" {}
        _GlowSpeed("Glow Speed",Float)=1
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowPower("Glow Power",Float)=1
        _Strength("Strength",Range(0,1))=1
        _Scale("Noise Scale",Float)=1
        _Speed("Speed",Float)=1
        _TimeOffset("Time Offset",Float)=1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100
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
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Noise;
            float4 _Noise_ST;
            sampler2D _Glow;
            float4 _Glow_ST;
            float2 uv;
            fixed _Speed;
            fixed _Strength;
            fixed _Scale;
            fixed _GlowSpeed;
            fixed4 _GlowColor;
            fixed alpha;
            fixed _GlowPower;
            fixed _TimeOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 nos = tex2D(_Noise, ((((i.uv*_Noise_ST.xy)+_Noise_ST.zw)*_Scale)+(_Speed * _Time.y)));
                fixed noise = (nos.r + nos.g + nos.b) /3;
                uv = i.uv;
                uv.xy += noise * _Strength;
                uv.xy -= _Strength/2;
                fixed4 col = tex2D(_MainTex, uv) * i.color;
                fixed4 glowT = tex2D(_Glow, (( uv.xy * _Glow_ST.xy)+_Glow_ST.zw));
                alpha = col.a;
                col += (pow(glowT.a,_GlowPower) * clamp((sin((_Time.y + _TimeOffset) * _GlowSpeed)),0,1)) * _GlowColor;
                col.a = alpha;
                return col; 
            }
            ENDCG
        }
    }
}

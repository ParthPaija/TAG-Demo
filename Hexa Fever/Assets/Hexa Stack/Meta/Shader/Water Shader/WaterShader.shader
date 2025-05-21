Shader "Unlit/WaterShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _CausticsTex("Caustics Texture", 2D) = "white" {}
        _FillTexture ("Fill Texture", 2D) = "white" {}
        _Speed("Speed",Range(0,10)) = 1
        _Strength("Strength",Range(0,10))=1
        _Color("Caustic Color",Color)=(1,1,1,1)
        _NoiseSize("Noise Size",float) = 1
        _CausticSize("Cuastic Size",float) = 1
        _SSpeed("Scroll Speed",Range(0,5)) = 1
        _Offset("Offset",Range(0,5)) = 1
        _Fill("Fill",Range(-0.1,1)) = 1
            _CausticBrightness("Caustic Brightness",Range(0,5)) = 1
            [MaterialToggle] _DirectionLR("Left Rigth",float) = 1
            [MaterialToggle] _DirectionUD("Up Down",float) = 1


    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD2;
            };

            struct v2f
            {
                float2 uv : TEXCOORD2;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            sampler2D _CausticsTex;
            float4 _CausticsTex_ST;
            float4 _NoiseTex_ST;
            sampler2D _FillTexture;
            uniform float _Speed;
            uniform float _Strength;
            uniform float4 _Color;
            uniform float _CausticSize;
            uniform float _NoiseSize;
            uniform float _SSpeed;
            uniform float _Offset;
            uniform float _DirectionLR;
            uniform float _DirectionUD;
            uniform float _DLR;
            uniform float _DUD;
            uniform float _Fill;
            uniform float _CausticBrightness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.vertex = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {


                float2 tempuv = i.uv;
                float2 tempuv2 = tempuv;

                tempuv.x *= _CausticsTex_ST.x;
                tempuv.y *= _CausticsTex_ST.y;

                tempuv2.x *= _NoiseTex_ST.x;
                tempuv2.y *= _NoiseTex_ST.y;

                tempuv2.x += _Time.x * _Speed;
                tempuv2.xy *= _NoiseSize;
                fixed4 noiseCol = tex2D(_NoiseTex, tempuv2);
                float noise = (noiseCol.r + noiseCol.g + noiseCol.b) / 3;

                tempuv.x += (noise * _Strength);
                tempuv.y += (noise * _Strength);
                tempuv.xy *= _CausticSize;
                if (_DirectionLR == 0)
                    _DLR = -1;
                else
                    _DLR = 1;

                if (_DirectionUD == 0)
                    _DUD = -1;
                else
                    _DUD = 1;

                tempuv.x += ((_SSpeed * _Time.y) + _Offset) * _DLR;
                tempuv.y += ((_SSpeed * _Time.y) + _Offset) * _DUD;

                float2 tuv = i.uv;
                tuv.x += (noise * _Strength);
                tuv.y += (noise * _Strength);

                fixed4 caustics = tex2D(_CausticsTex, tempuv);
                float caustCol = (caustics.r + caustics.g + caustics.b) / 3;
                //fixed4 caustics2 = tex2D(_CausticsTex, tuv);
                //float caustCol2 = (caustics2.r + caustics2.g + caustics2.b) / 3;
                //caustCol2 = caustCol2 / 3;
                fixed4 col = tex2D(_MainTex, tuv) * i.color;;
                //caustCol += caustCol2;
                caustCol *= _CausticBrightness;
                col += (caustCol * _Color);

                fixed4 fillTex = tex2D(_FillTexture, i.uv);
                float fill = (fillTex.r + fillTex.g + fillTex.b) / 3;
                if (_Fill < fill)
                    col.a = 0;
                return col;
            }
            ENDCG
        }
    }
}

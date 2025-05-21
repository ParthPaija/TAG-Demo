Shader "Unlit/DeformWater"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Displacer("Displacer",2D) = "white" {}
        _Flow("Flow",2D) = "white" {}
        _Displacement("Displacement",2D) = "white" {}
        _FlowMask("FlowMask",2D) = "white" {}
        _FillTexture("Fill Texture",2D) = "white" {}
        _Strength("Strength", Range(0,2)) = 1
        _Speed("Speed", Range(0,2)) = 1
        _Contrast("Contrast",Range(0,1)) = 1
        _NoiseStrenght("Noise Strenght",Range(0,1)) = 1
        _NoiseScale("Noise Scale",Range(0,10)) = 1
        _NoiseSpeed("Noise Speed",Range(0,10)) = 1
        _DirX("Dir X",Range(-1,1)) = 1
        _DirY("Dir Y",Range(-1,1))=1
            _FlowColor("Flow Color",Color)=(1,1,1,1)
            _FlowBrightness("Flow Brightness",Range(0,5)) = 1
            _Fill("Fill",Range(-0.1,1)) = 1
        _RotationSpeed("Rotation", float) = 2.0
        [MaterialToggle] _UseImageDepth("Use Image Depth",float) = 1
        [MaterialToggle] _InvertDisp("Invert Displace Map",float) = 1
        [MaterialToggle] _UseCustomDir("Use Custom Dir",float) =1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            //Textures
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Displacer;
            float4 _Displacer_ST;
            sampler2D _Flow;
            float4 _Flow_ST;
            sampler2D _Displacement;
            float4 _Displacement_ST;
            sampler2D _FlowMask;
            float4 _FlowMask_ST;
            sampler2D _FillTexture;

            //Other
            uniform float2 tempuv;
            uniform float _Strength;
            uniform float _Speed;
            uniform float dip;
            uniform float _UseImageDepth;
            uniform float _UseCustomDir;
            uniform float _InvertDisp;
            uniform float _Contrast;
            uniform float _DirX;
            uniform float _DirY;
            uniform float _RotationSpeed;
            uniform float _NoiseStrenght;
            uniform float _NoiseScale;
            uniform float _NoiseSpeed;
            uniform float2 DispUV;
            uniform float2 DisplUV;
            uniform float4 _FlowColor;
            uniform float _FlowBrightness;
            uniform float _Fill;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                DispUV = i.uv;

                float sinX = sin(_RotationSpeed);
                float cosX = cos(_RotationSpeed);
                float sinY = sin(_RotationSpeed);
                float2x2 rotationMatrix = float2x2(cosX, -sinX, sinY, cosX);
                DispUV = mul(DispUV, rotationMatrix);

                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 disp = tex2D(_Displacer, i.uv);
                tempuv = DispUV;

                if(_UseImageDepth)
                dip = (col.r + col.g + col.b) / 3;
                else
                dip = (disp.r + disp.g + disp.b) / 3;

                dip *= (_Contrast * 5);

                if (_InvertDisp)
                    dip = 1 - dip;

                tempuv.x += (dip * _Strength);

                //Move
                if (_UseCustomDir) {
                    tempuv.x += (_Speed * _Time.y) * _DirX;
                    tempuv.y += (_Speed * _Time.y) * _DirY;
                }
                else
                {
                    tempuv.y += (_Speed * _Time.y);
                }

                DisplUV = DispUV;
                DisplUV.y += _NoiseSpeed * _Time.y;

                DisplUV.x *= (_Displacement_ST.x * _NoiseScale);
                DisplUV.y *= (_Displacement_ST.y * _NoiseScale);

                fixed4 displ = tex2D(_Displacement, DisplUV);
                float nos = (displ.r + displ.g + displ.b) / 3;

                tempuv.y += (nos * _NoiseStrenght);
                tempuv.x += (nos * _NoiseStrenght);

                tempuv.x *= _Flow_ST.x;
                tempuv.y *= _Flow_ST.y;

                fixed4 flow = tex2D(_Flow, tempuv);
                fixed4 flowm = tex2D(_FlowMask, i.uv);
                float flowMask = (flowm.r + flowm.g + flowm.b) / 3;
                flow *= _FlowColor;
                flow *= _FlowBrightness;
                flow *= flowMask;
                flow.a = col.a;
                col += flow;

                fixed4 fillTex = tex2D(_FillTexture, i.uv);
                float fill = (fillTex.r + fillTex.g + fillTex.b) / 3;
                if (_Fill < fill)
                    col.a = 0;
                else
                    col.a = flow.a;

                    col*=i.color;

                return col;
            }
            ENDCG
        }
    }
}

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//////////////////////////////////////////////
/// 2DxFX - 2D SPRITE FX - by VETASOFT 2017 //
/// http://vetasoft.store/2dxfx/            //
//////////////////////////////////////////////

Shader "2DxFX/Standard/CircleFade"
{ 
Properties
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_Color ("_Color", Color) = (1,1,1,1)
_Offset ("Offset", Range(-1,1.1)) = 0.5
_Sharpness ("Sharp", Range(0,0.2)) = 0.15

_InOut ("InOut", Range(0,1)) = 0.5
_Alpha ("Alpha", Range (0,1)) = 1.0

// required for UI.Mask
_StencilComp ("Stencil Comparison", Float) = 8
_Stencil ("Stencil ID", Float) = 0
_StencilOp ("Stencil Operation", Float) = 0
_StencilWriteMask ("Stencil Write Mask", Float) = 255
_StencilReadMask ("Stencil Read Mask", Float) = 255
_ColorMask ("Color Mask", Float) = 15

}

SubShader
{
Tags {"Queue"="Transparent" "IgnoreProjector"="true" "RenderType"="Transparent"}
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off
// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp] 
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float4 _Color;
float _Offset;
float _Sharpness;
float _InOut;
float _Alpha;


v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}

float4 frag(v2f i) : COLOR
{
    float2 uv = i.texcoord.xy;
    float4 tex = tex2D(_MainTex, uv) * i.color;

    float alpha = tex.a;
    float2 center = float2(0.5, 0.5);
    float2 radi = i.texcoord.xy - center;
    radi.x *= _ScreenParams.x / _ScreenParams.y;
    
    float dist = 1.0 - smoothstep(_Offset, _Offset + _Sharpness, length(radi));
    float c = 0;

    if (_InOut == 0)
    {
        c = dist;
    }
    else
    {
        c = 1 - dist;
    }

    tex.a = alpha * c - _Alpha;
    return tex;
}

ENDCG
}
}
Fallback "Sprites/Default"

}

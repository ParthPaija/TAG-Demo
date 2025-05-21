Shader "Custom/UnlitUVAlpha"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Edge1("Edge 1", Range(0,1)) = 1
        _Edge2("Edge 2", Range(0,1)) = 1
        _Smoothness("Smoothness", Float) = 1
    }
    SubShader
    {
        Tags{ "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Edge1;
            float _Edge2;
            float _Smoothness;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Sample the texture
                half4 texColor = tex2D(_MainTex, i.uv);
                
                // Multiply the alpha by uv.y
                texColor.a *= clamp(0,1, pow(smoothstep(_Edge1,_Edge2, i.uv.y),_Smoothness));

                return texColor;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Texture"
}

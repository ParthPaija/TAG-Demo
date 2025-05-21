Shader "UI/New Dissolve"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}

//        _StencilComp ("Stencil Comparison", Float) = 8
//        _Stencil ("Stencil ID", Float) = 0
//        _StencilOp ("Stencil Operation", Float) = 0
//        _StencilWriteMask ("Stencil Write Mask", Float) = 255
//        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        _NoiseTex ("Texture", 2D) = "white" {}
		[HDR]_EdgeColour1 ("Edge colour 1", Color) = (1.0, 1.0, 1.0, 1.0)
		[HDR]_EdgeColour2 ("Edge colour 2", Color) = (1.0, 1.0, 1.0, 1.0)
		_Level ("Dissolution level", Range (0.0, 1.0)) = 0.1
		_Edges ("Edge width", Range (0.0, 1.0)) = 0.1
        _NoiseSize("Noise Size",Float) = 1

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        
        _DissolveY("Current Y of the dissolve effect", Float) = 0
        _DissolveSize("Size of the effect", Float) = 2
        _StartingY("Starting point of the effect", Float) = -1 //the number is supposedly in meters. Is compared to the Y coordinate in world space I believe.
        [MaterialToggle] _Horizontal("Horizontal",float) = 1
        [MaterialToggle] _Vertical("Vertical",float) = 1
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

//        Stencil
//        {
//            Ref [_Stencil]
//            Comp [_StencilComp]
//            Pass [_StencilOp]
//            ReadMask [_StencilReadMask]
//            WriteMask [_StencilWriteMask]
//        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR0;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR0;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
			float4 _EdgeColour1;
			float4 _EdgeColour2;
			float _Level;
			float _Edges;
	        float _DissolveY;
            float _StartingY;
            float _DissolveSize;
            uniform float _NoiseSize;
			uniform float _Horizontal;
		    uniform float _Vertical;
				
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color ;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                //float cutout = tex2D(_NoiseTex, IN.texcoord).r;
                float transition = _DissolveY; //Cutoff value where world position is taken into account.
                
                float2 noiseUV = IN.worldPosition;
                noiseUV *= _NoiseSize;

                if(_Horizontal)
                    transition = _DissolveY +  (IN.worldPosition.x );
                else if(_Vertical)
                    transition = _DissolveY +  (IN.worldPosition.y );
                 
                
                float cutout = _StartingY + (transition + (tex2D(_NoiseTex, noiseUV)) * _DissolveSize);
                
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
                if (cutout <= _Level) {
				    color.a = 0;
			    }
                else {
                    if(cutout < color.a && cutout < _Level + _Edges){ 
                        color = lerp(_EdgeColour1, _EdgeColour2, (cutout - _Level) / _Edges);
                    }
                }
                #ifdef UNITY_UI_ALPHACLIP
                clip(_StartingY + (transition + (tex2D(_NoiseTex, noiseUV)) * _DissolveSize)); //Clip = cutoff if above 0.
                
                //clip (color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}
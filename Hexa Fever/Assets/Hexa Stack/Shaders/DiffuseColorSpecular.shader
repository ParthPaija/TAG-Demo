Shader "Mobile/Color Specular" {
    Properties {
        _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
        _Color("Color", Color) = (1,1,1,1)
        _CustomLightDir1 ("Custom Light Direction 1", Vector) = (0.2, 1, 0.3, 0)
        _CustomLightDir2 ("Custom Light Direction 2", Vector) = (0.2, 1, 0.3, 0)
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 250
  
        // First Pass - Main Light
        CGPROGRAM
        #pragma surface surf MobileBlinnPhong exclude_path:prepass noforwardadd halfasview interpolateview

        inline fixed4 LightingMobileBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
        {
            fixed diff = max (0, dot (s.Normal, lightDir));
            fixed nh = max (0, dot (s.Normal, halfDir));
            fixed spec = pow (nh, s.Specular*128) * s.Gloss;
  
            fixed4 c;
            c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
            UNITY_OPAQUE_ALPHA(c.a);
            return c;
        }

        half _Shininess;
        fixed4 _Color;
        struct Input {
            float3 pos;
        };
        
        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = _Color.rgb;
            o.Gloss = _Color.a;
            o.Alpha = _Color.a;
            o.Specular = _Shininess;
        }
        ENDCG

        // Second Pass - Custom Light Direction
        Pass {
            Blend One One // Additive blending
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            half _Shininess;
            fixed4 _Color;
            fixed4 _CustomLightDir1;
            fixed4 _CustomLightDir2;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 normalDir = normalize(i.normal);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

                float3 lightDir1 = normalize(_CustomLightDir1.xyz);
                float3 lightDir2 = normalize(_CustomLightDir2.xyz);

                float3 halfDir1 = normalize(lightDir1 + viewDir);
                float3 halfDir2 = normalize(lightDir2 + viewDir);

                float diff1 = max(0, dot(normalDir, lightDir1));
                float diff2 = max(0, dot(normalDir, lightDir2));

                float nh1 = max(0, dot(normalDir, halfDir1));
                float nh2 = max(0, dot(normalDir, halfDir2));

                float spec1 = pow(nh1, _Shininess * 128);
                float spec2 = pow(nh2, _Shininess * 128);

                fixed4 c;
                c.rgb = (_Color.rgb * (diff1 + diff2) + (spec1 + spec2) * _Color.a);
                c.a = 1;
                return c;
            }
            ENDCG
        }

        // Third Pass - Custom Light Direction
        // Pass {
        //     Blend One One Additive blending
            
        //     CGPROGRAM
        //     #pragma vertex vert
        //     #pragma fragment frag
        //     #include "UnityCG.cginc"

        //     struct appdata {
        //         float4 vertex : POSITION;
        //         float3 normal : NORMAL;
        //     };

        //     struct v2f {
        //         float4 pos : SV_POSITION;
        //         float3 normal : TEXCOORD0;
        //         float3 worldPos : TEXCOORD1;
        //     };

        //     half _Shininess;
        //     fixed4 _Color;
        //     fixed4 _CustomLightDir2;

        //     v2f vert (appdata v) {
        //         v2f o;
        //         o.pos = UnityObjectToClipPos(v.vertex);
        //         o.normal = UnityObjectToWorldNormal(v.normal);
        //         o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        //         return o;
        //     }

        //     fixed4 frag (v2f i) : SV_Target {
        //         float3 normalDir = normalize(i.normal);
        //         float3 lightDir = normalize(_CustomLightDir2.xyz);
        //         float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
        //         float3 halfDir = normalize(lightDir + viewDir);

        //         float diff = max(0, dot(normalDir, lightDir));
        //         float nh = max(0, dot(normalDir, halfDir));
        //         float spec = pow(nh, _Shininess * 128);

        //         fixed4 c;
        //         c.rgb = (_Color.rgb * diff + spec * _Color.a);
        //         c.a = 1;
        //         return c;
        //     }
        //     ENDCG
        // }
    }
    FallBack "Mobile/VertexLit"
}
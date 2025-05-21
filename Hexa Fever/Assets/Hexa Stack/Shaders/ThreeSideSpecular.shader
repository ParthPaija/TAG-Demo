Shader "Custom/ThreeSideSpecularWithColor"
{
    Properties {
    _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
    _Color("Color", Color) = (1,1,1,1)
    _CustomLightDir1 ("Custom Light Direction 1", Vector) = (0.2, 1, 0.3, 0)
    _CustomLightDir2 ("Custom Light Direction 2", Vector) = (0.2, 1, 0.3, 0)
    //_CustomLightDir3 ("Custom Light Direction 3", Vector) = (0.2, 1, 0.3, 0)  // New property
    _GradientDirection ("Gradient Direction", Vector) = (0, 0, 1, 0)
    _GradientStrength ("Gradient Strength", Range(0, 1)) = 0.5
    _GradientOffset ("Gradient Offset", Range(-5, 5)) = 0
}

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 250
  
    // Main Pass
    Pass {
        Tags { "LightMode"="ForwardBase" }
        
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_instancing
        #pragma multi_compile_fwdbase
        #pragma multi_compile_shadowcaster
        
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc"

        struct appdata {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f {
            float4 pos : SV_POSITION;
            float3 normal : TEXCOORD0;
            float3 localPos : TEXCOORD1;
            SHADOW_COORDS(2)
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        half _Shininess;
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
        UNITY_INSTANCING_BUFFER_END(Props)
        fixed4 _CustomLightDir1;
        fixed4 _CustomLightDir2;
        fixed4 _GradientDirection;
        float _GradientStrength;
        float _GradientOffset;

        v2f vert (appdata v) {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_TRANSFER_INSTANCE_ID(v, o);
            o.pos = UnityObjectToClipPos(v.vertex);
            o.normal = UnityObjectToWorldNormal(v.normal);
            o.localPos = v.vertex.xyz;
            TRANSFER_SHADOW(o)
            return o;
        }

        fixed4 frag (v2f i) : SV_Target {
            UNITY_SETUP_INSTANCE_ID(i);
            float3 normalDir = normalize(i.normal);
            float3 viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, float4(i.localPos, 1)).xyz);

            float3 gradientDirObjectSpace = mul(unity_WorldToObject, float4(normalize(_GradientDirection.xyz), 0)).xyz;
            float gradientFactor = dot(i.localPos, normalize(gradientDirObjectSpace)) + _GradientOffset;
            gradientFactor = saturate(gradientFactor);
            
            float3 mainLightDir = normalize(_WorldSpaceLightPos0.xyz);
            float3 lightDir1 = normalize(_CustomLightDir1.xyz);
            float3 lightDir2 = normalize(_CustomLightDir2.xyz);
            float3 lightDir3 = normalize(_WorldSpaceLightPos0.xyz);  // New light direction

            float3 halfDir1 = normalize(lightDir1 + viewDir);
            float3 halfDir2 = normalize(lightDir2 + viewDir);
            float3 halfDir3 = normalize(lightDir3 + viewDir);  // New half direction

            float diff1 = max(0, dot(normalDir, lightDir1));
            float diff2 = max(0, dot(normalDir, lightDir2));
            float diff3 = max(0, dot(normalDir, lightDir3));  // New diffuse

            float nh1 = max(0, dot(normalDir, halfDir1));
            float nh2 = max(0, dot(normalDir, halfDir2));
            float nh3 = max(0, dot(normalDir, halfDir3));  // New normal-half dot

            float spec1 = pow(nh1, _Shininess * 128);
            float spec2 = pow(nh2, _Shininess * 128);
            float spec3 = pow(nh3, _Shininess * 128);  // New specular

            fixed4 instancedColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            fixed4 c;
            
            // Add ambient lighting term
            float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * instancedColor.rgb;
            
            // Combine ambient with existing lighting
            c.rgb = ambient + (instancedColor.rgb * _LightColor0.rgb * (diff1 + diff2 + diff3) + 
                    _LightColor0.rgb * (spec1 + spec2 + spec3) * instancedColor.a);
            
            c.rgb = lerp(c.rgb * 0.2, c.rgb, gradientFactor * _GradientStrength + (1 - _GradientStrength));
            c.a = 1;

            fixed shadow = SHADOW_ATTENUATION(i);
            c.rgb *= shadow;
            return c;
        }
        ENDCG
    }

    // Shadow caster pass
    UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
}
FallBack "Diffuse"
}
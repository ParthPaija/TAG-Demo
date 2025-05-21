Shader "Custom/UnlitWithShadowsSurface"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags{ "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 200

        CGPROGRAM
        // ---------------------------------------------------------
        // Tell Unity we want:
        //   - a Surface Shader named `surf`
        //   - a custom lighting function named `UnlitWithShadowLight`
        //   - to include real-time shadows (addshadow)
        //   - no additional ambient or other lighting (noambient)
        // ---------------------------------------------------------
        #pragma surface surf UnlitWithShadowLight addshadow noambient

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        half4 _Color;

        // The surface function: defines the final 'surface properties'
        // before lighting. We'll just store our texture * color in Albedo.
        void surf (Input IN, inout SurfaceOutput o)
        {
            half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb; 
            o.Alpha  = c.a;
        }

        // ---------------------------------------------------------
        // Custom lighting function: "Unlit" + Shadows
        // We only multiply Albedo by the shadow attenuation
        // to darken the object in shadowed areas.
        // ---------------------------------------------------------
        half4 LightingUnlitWithShadowLight (SurfaceOutput s, half3 lightDir, half atten)
        {
            // If atten=1 => fully lit (no shadow), color is unchanged.
            // If atten<1 => partial/fully in shadow, color is darkened.
            // We ignore actual light color & direction (like an unlit shader would).
            return half4(s.Albedo * atten, s.Alpha);
        }

        ENDCG
    }
    // (Optional) fallback if something is unsupported
    FallBack "Diffuse"
}
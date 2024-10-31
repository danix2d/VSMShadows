Shader "Custom/VSM_Lit_No_Sadow"
{
    Properties
    {
        _Color ("Color", Color) = (1,0,0,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _CullMode ("Cull Mode", Int) = 2
        _UseShadows ("Use Shadows", Float) = 1
    }
    SubShader
    {
        Tags { 
            "RenderPipeline" = "UniversalPipeline" 
            "IgnoreProjector" = "True" 
            "Queue" = "Geometry" 
            "RenderType" = "Opaque"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        // Apply culling mode based on property value
        CULL [_CullMode]

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma enable_cbuffer
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0; // Added UV for texture coordinates
            }; 

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2; // Added UV to pass to fragment shader
            };

            v2f vert (appdata v)
            {
                v2f o;
                // Transform position to homogeneous clip space
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                // Transform position to world space
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // Transform normal to world space
                o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
                // Pass UV coordinates to the fragment shader
                o.uv = v.uv;
                return o;
            }

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                sampler2D _MainTex; // Texture sampler
                float _UseShadows;  // Shadow toggle
            CBUFFER_END

            float3 Lambert(float3 lightColor, float3 lightDir, float3 normal)
            {
                float NdotL = saturate(dot(normal, lightDir));
                return lightColor * NdotL;
            }

            float4 frag (v2f i) : SV_Target
            {
                // Sample texture
                float4 texColor = tex2D(_MainTex, i.uv);
                float4 color = texColor * _Color; // Combine texture color with base color

                // Ambient lighting
                float3 ambientColor = _GlossyEnvironmentColor.xyz;

                // Main light
                float3 lightPos = _MainLightPosition.xyz;
                float3 lightCol = Lambert(_MainLightColor.rgb * unity_LightData.z, lightPos, i.normal);

                // Additional lights
                uint lightsCount = GetAdditionalLightsCount();
                for (uint j = 0; j < lightsCount; j++)
                {
                    Light light = GetAdditionalLight(j, i.worldPos);
                    lightCol += Lambert(light.color.rgb * light.distanceAttenuation, light.direction, i.normal);
                }

                // Combine ambient and main light contributions
                float3 finalLightCol = ambientColor + (lightCol);

                // Apply the lighting color to the texture color
                color.rgb *= finalLightCol;

                return color;
            }

            ENDHLSL
        }
    }
}

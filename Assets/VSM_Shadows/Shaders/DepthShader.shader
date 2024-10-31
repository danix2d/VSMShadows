Shader "CustomShadows/Depth"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata i)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(i.vertex.xyz);
                o.uv = i.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                
                // Calculate depth as 1 - z to fit normalized depth
                float depth = i.vertex.z / i.vertex.w;

                #if defined(UNITY_REVERSED_Z)
                depth = 1 - depth;
                #else
                depth = depth;
                #endif
                
                // Output depth value as a color where RGB channels represent depth
                return float4(depth, pow(depth, 2), 0, 1);
            }
            ENDHLSL
        }
    }
}

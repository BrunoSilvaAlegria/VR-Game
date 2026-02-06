Shader "Custom/Outline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,1,1,1)
        _OutlineThickness ("Outline Thickness", Range(0.0, 1.0)) = 0.3
        _OutlinePower ("Outline Power", Range(1, 8)) = 3
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "OutlinePass"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
                float3 viewDirWS  : TEXCOORD1;
            };

            float4 _OutlineColor;
            float _OutlineThickness;
            float _OutlinePower;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformWorldToHClip(worldPos);

                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = normalize(_WorldSpaceCameraPos - worldPos);

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float ndotv = dot(normalize(IN.normalWS), normalize(IN.viewDirWS));

                // Edge detection
                float edge = 1.0 - saturate(ndotv);
                edge = pow(edge, _OutlinePower);

                // Thickness control
                edge = smoothstep(_OutlineThickness, 1.0, edge);

                return _OutlineColor * edge;
            }
            ENDHLSL
        }
    }
}

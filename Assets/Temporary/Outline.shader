Shader "Custom/Watercolor_Shader"
{
    Properties
    {
        //Outline
        _OutlineAmount("Outline Amount", Range(0, 1)) = 0.25
        _OutlineColor("Outline Color", Color) = (0.5,0.5,0.5,1)
    }

    SubShader
    {
        Tags { 
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque" 
            "Queue" = "Geometry"
        }
        
        Pass
        {
            Name "Outline"

            Cull Front

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                       
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD3;
            };  

            CBUFFER_START(UnityPerMaterial)
                float _OutlineAmount;
                float4 _OutlineColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);

                worldPos += float4(normalize(normalWS), 0) * _OutlineAmount;
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.positionWS = worldPos;
 
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 outlineColor = _OutlineColor;

                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
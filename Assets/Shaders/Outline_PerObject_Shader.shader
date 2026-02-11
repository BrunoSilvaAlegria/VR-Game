Shader "Custom/Outline_PerObject_Shader"
{
    Properties
    {
        _DiffuseColor("Color", Color) = (1,1,1,1)
        
        //Outline
        _OutlineAmount("Outline Amount", Range(0, 1)) = 0.25
        _OutlineColor("Outline Color", Color) = (0.5,0.5,0.5,1)

        //Textures
        //_WallTex("Wall Details", 2D) = "white" {}
        //_FloorTex("Floor Details", 2D) = "white" {}
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
            Name "WateryShadows"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            Blend One Zero //Opaque

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS            

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;
                float3 positionWS : TEXCOORD3;
            };

            //TEXTURE2D(_WallTex);
            //SAMPLER(sampler_WallTex);
            //TEXTURE2D(_FloorTex);
            //SAMPLER(sampler_FloorTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _DiffuseColor; 
                //float4 _WallTex_ST;
                //float4 _FloorTex_ST;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs norms = GetVertexNormalInputs(IN.normalOS);

                OUT.positionHCS = pos.positionCS;
                OUT.positionWS = pos.positionWS;
                OUT.normalWS = normalize(norms.normalWS);
                OUT.uv = IN.uv;
                //OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
            
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {             
                //Light light = GetMainLight(IN.shadowCoord); //Get the properties of the main light (directional) for the shadow coordinates
                //float lighting = saturate(dot(IN.normalWS, normalize(light.direction)));

                //half4 wall = SAMPLE_TEXTURE2D(_WallTex, sampler_WallTex, TRANSFORM_TEX(IN.uv, _WallTex_ST)); 
                //half4 floor = SAMPLE_TEXTURE2D(_FloorTex, sampler_FloorTex, TRANSFORM_TEX(IN.uv, _FloorTex_ST));
                
                return _DiffuseColor;
            }
            ENDHLSL
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
                float3 normalWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;
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
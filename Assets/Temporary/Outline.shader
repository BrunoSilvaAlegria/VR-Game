Shader "Custom/ReactiveOutline"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
        
        [Header(Outline Settings)]
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width", Range(0, 0.2)) = 0.05
        _IntensityPower("Light Sensitivity", Range(0.1, 5)) = 1.0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" }

        // --- PASS 1: THE OBJECT ---
        Pass
        {
            Name "BaseObject"
            Tags { "LightMode" = "UniversalForward" }
            
            ZWrite On
            ZTest LEqual
            Blend Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };
            
            float4 _BaseColor;

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                return _BaseColor;
            }
            ENDHLSL
        }

        // --- PASS 2: THE REACTIVE OUTLINE ---
        Pass
        {
            Name "Outline"
            // CHANGE 1: Use UniversalForward to get proper lighting/shadow data
            Tags { "LightMode" = "UniversalForward" }
            
            Cull Front
            ZWrite Off // CHANGE 2: ZWrite Off is usually safer for fading transparent objects to avoid "invisible occlusion"
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // CHANGE 3: Add Pragmas for Shadows and multiple lights
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings { 
                float4 positionCS : SV_POSITION; 
                float3 normalWS : TEXCOORD0; 
                float3 positionWS : TEXCOORD1; 
            };

            float4 _OutlineColor;
            float _OutlineWidth;
            float _IntensityPower;

            Varyings vert(Attributes IN) {
                Varyings OUT;
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                
                // Extrude
                positionWS += normalWS * _OutlineWidth;
                
                OUT.positionCS = TransformWorldToHClip(positionWS);
                OUT.positionWS = positionWS;
                OUT.normalWS = normalWS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                float3 normal = normalize(IN.normalWS);
                float lightSum = 0;

                // 1. CALCULATE MAIN LIGHT (Sun) + SHADOWS
                float4 shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                
                float nDotL_Main = saturate(dot(normal, mainLight.direction));
                // Multiply by shadowAttenuation (0 = in shadow, 1 = lit)
                lightSum += nDotL_Main * mainLight.distanceAttenuation * mainLight.shadowAttenuation;

                // 2. CALCULATE ADDITIONAL LIGHTS (Point/Spot)
                uint lightCount = GetAdditionalLightsCount();
                for (uint i = 0u; i < lightCount; ++i) {
                    // GetAdditionalLight automatically handles shadows if keywords are enabled
                    Light light = GetAdditionalLight(i, IN.positionWS, half4(1,1,1,1)); // Ensure shadow mask is passed if needed
                    
                    float nDotL = saturate(dot(normal, light.direction));
                    lightSum += nDotL * light.distanceAttenuation * light.shadowAttenuation;
                }

                // Apply Sensitivity Curve
                float alpha = saturate(pow(lightSum, _IntensityPower));
                
                // Final Alpha calculation
                return float4(_OutlineColor.rgb, alpha * _OutlineColor.a);
            }
            ENDHLSL
        }
    }
}
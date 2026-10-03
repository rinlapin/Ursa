Shader "Fog/SbFadeToSkyboxFog"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
    }
        
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        
        Pass
        {
            Name "Skybox Fog Postprocess Pass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            #pragma shader_feature _ UNITY_REVERSED_Z
            #pragma shader_feature _ _SKYBOXFOG_LINEAR _SKYBOXFOG_EXP _SKYBOXFOG_EXP2
            #pragma shader_feature _ _SKYBOXFOG_VERTICAL_LINEAR _SKYBOXFOG_VERTICAL_EXP _SKYBOXFOG_VERTICAL_EXP2
            #pragma shader_feature _ _SKYBOXFOG_WORLD_DISTANCE
            #pragma shader_feature _ _SKYBOXFOG_CLIP_FADE
            #pragma shader_feature _ _SKYBOXFOG_FOG_ON_SKYBOX
            #pragma shader_feature _ _SKYBOXFOG_BLUR
            #pragma shader_feature _ _SKYBOXFOG_VERTICAL
            #pragma shader_feature _ _SKYBOXFOG_VERTICAL_RELATIVE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            #include "SbFadeToSkyboxFogFunc.cginc"

            TEXTURE2D(_CopiedTex);
            SAMPLER(sampler_CopiedTex);

            real4 frag (Varyings input) : SV_Target
            {
                static float Epsilon = 0.1f / _ZBufferParams.x;

                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Sample the color from the input texture
                half4 color = SAMPLE_TEXTURE2D(_CopiedTex, sampler_CopiedTex, input.texcoord.xy);
                
#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2) || defined(_SKYBOXFOG_LINEAR) || defined(_SKYBOXFOG_EXP) || defined(_SKYBOXFOG_EXP2)
                half4 depthColor = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, input.texcoord.xy);

                #if UNITY_REVERSED_Z
                    real depth = SampleSceneDepth(input.texcoord);
                #else
                    // z -> OpenGL NDC
                    real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(input.texcoord));
                #endif

                float linearDepth = Linear01Depth(depth, _ZBufferParams);

                float eyeDepth = LinearDepthToEyeDepth(1.f - linearDepth);
                #if defined(_SKYBOXFOG_WORLD_DISTANCE) || defined(_SKYBOXFOG_VERTICAL)
                    float3 worldPos = ComputeWorldSpacePosition(input.texcoord.xy, depth, UNITY_MATRIX_I_VP);
                #endif
                #if defined(_SKYBOXFOG_WORLD_DISTANCE)
                    float fogDist = length((worldPos - _WorldSpaceCameraPos).xyz);
                #else
                    float fogDist = eyeDepth;
                #endif
              
                half fogFactor = ComputeSkyboxFogFactor(fogDist);
                half fogIntensity = ComputeSkyboxFogIntensity(fogFactor);

                half density =  1.f - fogIntensity;
                int skyboxMask = step(eyeDepth, 1.f / (_ZBufferParams.w + Epsilon));
                #if !defined(_SKYBOXFOG_FOG_ON_SKYBOX)
                    density *= skyboxMask;
                #endif
                
                #if defined(_SKYBOXFOG_WORLD_DISTANCE) || defined(_SKYBOXFOG_VERTICAL)
                half heightDensity = ComputeHeightDensity(worldPos);
                #else
                half heightDensity = 1.h;
                #endif
                #if defined(_SKYBOXFOG_VERTICAL)
                    half colorFogFadeRate = ComputeSkyboxFadeRate(density * heightDensity, _SkyboxBlendParams.z);
                    half heightFadeRate = ComputeSkyboxFadeRate(heightDensity, _SkyboxBlendParams.z);
                #else
                    half colorFogFadeRate = ComputeSkyboxFadeRate(density, _SkyboxBlendParams.z);
                    half heightFadeRate = 1.f;
                #endif
                

                
                half4 skyboxOrigCol = tex2D(_SkyboxOrigTex, input.texcoord.xy);
                half4 skyboxCol = tex2D(_SkyboxTex, input.texcoord.xy);
                
                color = color * skyboxMask + skyboxOrigCol * (1 - skyboxMask);

                half clipFogDensity = (1.f - ComputeSkyboxFogIntensity(ComputeSkyboxFogFactor(_SkyboxClipPlane)));
                half clipFadeRate = ComputeSkyboxFadeRate(clipFogDensity, _SkyboxBlendParams.z);
                skyboxCol = lerp(skyboxCol, ComputeFogOutColor(skyboxCol, clipFogDensity), 1.f - clipFadeRate);
                
                half4 colorFogOutColor = ComputeFogOutColor(color, density * heightDensity);
                #if defined(_SKYBOXFOG_FOG_ON_SKYBOX)
                    half4 skyboxHeightCol = lerp(skyboxOrigCol, skyboxCol, heightFadeRate);
                    half4 baseSkyboxCol = skyboxCol;

                    skyboxCol = lerp(skyboxHeightCol, skyboxCol, heightDensity);

                    half4 outColor = lerp(colorFogOutColor, baseSkyboxCol * skyboxMask + skyboxCol * (1 - skyboxMask), colorFogFadeRate);
                #else
                    half4 outColor = lerp(colorFogOutColor, skyboxCol, colorFogFadeRate);
                #endif

                #if defined(_SKYBOXFOG_CLIP_FADE)
                
                    #if defined(_SKYBOXFOG_FOG_ON_SKYBOX)
                        return lerp(outColor, skyboxCol, 1.f - ComputeSkyboxBlurFadeFactor(fogDist));
                    #else
                        return lerp(outColor, skyboxOrigCol, 1.f - ComputeSkyboxBlurFadeFactor(fogDist));
                    #endif

                #elif defined(_SKYBOXFOG_FOG_ON_SKYBOX)
                    return outColor * skyboxMask + skyboxCol * (1 - skyboxMask);

                #else
                    return outColor;
                #endif
#else
                return color;
#endif
            } 
            ENDHLSL
        }
        
        Pass
        {
            Name "Skybox Blur DownSample Pass1"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
            
            #include "SbFadeToSkyboxFogFunc.cginc"

            half4 _SkyboxBlurTex_0_TexelSize;

            real4 frag (Varyings input) : SV_Target
            {
                return BlurDownSampling(_SkyboxBlurTex_0, _SkyboxBlurTex_0_TexelSize, input.texcoord);
            } 
            ENDHLSL
        }
        
        Pass
        {
            Name "Skybox Blur DownSample Pass2"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
            
            #include "SbFadeToSkyboxFogFunc.cginc"

            half4 _SkyboxBlurTex_1_TexelSize;

            real4 frag (Varyings input) : SV_Target
            {
                return BlurDownSampling(_SkyboxBlurTex_1, _SkyboxBlurTex_1_TexelSize, input.texcoord);
            } 
            ENDHLSL
        }
        
        Pass
        {
            Name "Skybox Blur DownSample Pass3"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
            
            #include "SbFadeToSkyboxFogFunc.cginc"

            half4 _SkyboxBlurTex_2_TexelSize;

            real4 frag (Varyings input) : SV_Target
            {
                return BlurDownSampling(_SkyboxBlurTex_2, _SkyboxBlurTex_2_TexelSize, input.texcoord);
            } 
            ENDHLSL
        }
        
        Pass
        {
            Name "Skybox Blur DownSample Pass4"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
            
            #include "SbFadeToSkyboxFogFunc.cginc"

            half4 _SkyboxBlurTex_3_TexelSize;

            real4 frag (Varyings input) : SV_Target
            {
                return BlurDownSampling(_SkyboxBlurTex_3, _SkyboxBlurTex_3_TexelSize, input.texcoord);
            } 
            ENDHLSL
        }
        
        Pass
        {
            Name "Skybox Blur UpSample Pass1"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
            
            #include "SbFadeToSkyboxFogFunc.cginc"

            half4 _SkyboxBlurTex_4_TexelSize;

            real4 frag (Varyings input) : SV_Target
            {
                return BlurUpSampling(_SkyboxBlurTex_4, _SkyboxBlurTex_4_TexelSize, input.texcoord);
            } 
            ENDHLSL
        }
        
        Pass
        {
            Name "Skybox Blur UpSample Pass2"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
            
            #include "SbFadeToSkyboxFogFunc.cginc"

            half4 _SkyboxBlurTex_5_TexelSize;

            real4 frag (Varyings input) : SV_Target
            {
                return BlurUpSampling(_SkyboxBlurTex_5, _SkyboxBlurTex_5_TexelSize, input.texcoord);
            } 
            ENDHLSL
        }
        
        Pass
        {
            Name "Skybox Blur UpSample Pass3"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
            
            #include "SbFadeToSkyboxFogFunc.cginc"

            half4 _SkyboxBlurTex_6_TexelSize;

            real4 frag (Varyings input) : SV_Target
            {
                return BlurUpSampling(_SkyboxBlurTex_6, _SkyboxBlurTex_6_TexelSize, input.texcoord);
            } 
            ENDHLSL
        }
        
        Pass
        {
            Name "Skybox Blur UpSample Pass4"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
            
            #include "SbFadeToSkyboxFogFunc.cginc"

            half4 _SkyboxBlurTex_7_TexelSize;

            real4 frag (Varyings input) : SV_Target
            {
                return BlurUpSampling(_SkyboxBlurTex_7, _SkyboxBlurTex_7_TexelSize, input.texcoord);
            } 
            ENDHLSL
        }
    }
}

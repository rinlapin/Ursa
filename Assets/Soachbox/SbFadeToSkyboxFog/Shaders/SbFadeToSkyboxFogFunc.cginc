

#pragma shader_feature _ UNITY_REVERSED_Z
#pragma shader_feature _ _SKYBOXFOG_LINEAR _SKYBOXFOG_EXP _SKYBOXFOG_EXP2
#pragma shader_feature _ _SKYBOXFOG_VERTICAL_LINEAR _SKYBOXFOG_VERTICAL_EXP _SKYBOXFOG_VERTICAL_EXP2
#pragma shader_feature _ _SKYBOXFOG_WORLD_DISTANCE
#pragma shader_feature _ _SKYBOXFOG_CLIP_FADE
#pragma shader_feature _ _SKYBOXFOG_VERTICAL
#pragma shader_feature _ _SKYBOXFOG_VERTICAL_RELATIVE

sampler2D _SkyboxOrigTex;
sampler2D _SkyboxTex;
sampler2D _SkyboxBlurTex_0;
sampler2D _SkyboxBlurTex_1;
sampler2D _SkyboxBlurTex_2;
sampler2D _SkyboxBlurTex_3;

sampler2D _SkyboxBlurTex_4;
sampler2D _SkyboxBlurTex_5;
sampler2D _SkyboxBlurTex_6;
sampler2D _SkyboxBlurTex_7;

float _SkyboxClipPlane;

float4 _SkyboxFogColor;
float4 _SkyboxFogParams;

float _SkyboxBlurLevel;
float4 _SkyboxClipFadeParams;

float4 _SkyboxVerticalFogParams;

int _IsEnableSkyboxFog;

// x: _SkyboxBlendFactor
// y: _ColorFogEndPoint
// z: _FadeToSkyboxStartPoint
float4 _SkyboxBlendParams;

float4 ComputeSkyboxScreenPos(in float4 positionCS)
{
    float4 o = positionCS * 0.5f;
    o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;
    o.zw = positionCS.zw;
    return o;
}

half ComputeSkyboxFadeRate(real density, real threshold)
{
    return saturate((density - threshold) / (1.f - threshold)) * _SkyboxBlendParams.x;
}

real ComputeSkyboxFogFactor(in float z)
{
#if defined(_SKYBOXFOG_LINEAR)
    // factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
    real fogFactor = saturate(z * _SkyboxFogParams.z + _SkyboxFogParams.w);
#elif defined(FOG_LINEAR)
    // factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
    real fogFactor = saturate(z * unity_FogParams.z + unity_FogParams.w);
#elif defined(_SKYBOXFOG_EXP) || defined(_SKYBOXFOG_EXP2)
    // factor = exp(-(density*z)^2)
    real fogFactor = real(_SkyboxFogParams.x * z);
#else
    real fogFactor = real(unity_FogParams.x * z);
#endif
    return fogFactor;
}

real ComputeSkyboxFogIntensity(in real fogFactor)
{
    real fogIntensity = 0.0;
#if defined(FOG_EXP) || defined(_SKYBOXFOG_EXP)
            // factor = exp(-density*z)
            fogIntensity = saturate(exp2(-fogFactor));
#elif defined(FOG_EXP2) || defined(_SKYBOXFOG_EXP2)
            // factor = exp(-(density*z)^2)
            fogIntensity = saturate(exp2(-fogFactor * fogFactor));
#elif defined(FOG_LINEAR) || defined(_SKYBOXFOG_LINEAR)
            fogIntensity = fogFactor;
#endif
    return fogIntensity;
}

real ComputeSkyboxBlurFadeFactor(in real z)
{
#if defined(_SKYBOXFOG_CLIP_FADE)
    // linear fog
    // factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
    return saturate(z * _SkyboxClipFadeParams.z + _SkyboxClipFadeParams.w);
#else
    return 0.f;
#endif
}

half4 GetFogColor()
{
#if defined(_SKYBOXFOG_LINEAR) || defined(_SKYBOXFOG_EXP) || defined(_SKYBOXFOG_EXP2)
    return _SkyboxFogColor;
#else
    return unity_FogColor;
#endif
}

half4 ComputeFogOutColor(in half4 color, in half density)
{
#if defined(_SKYBOXFOG_LINEAR) || defined(_SKYBOXFOG_EXP) || defined(_SKYBOXFOG_EXP2)
    half4 fogCol = lerp(color, float4(_SkyboxFogColor.xyz, 1.f), _SkyboxFogColor.a);
    half colorFogRate = saturate(density / _SkyboxBlendParams.y);
    return lerp(color, fogCol, colorFogRate);
#else
    return color;
#endif
}

real ComputeHeightDensity(in float3 worldPos)
{
#if defined(_SKYBOXFOG_VERTICAL)
    #if defined(_SKYBOXFOG_VERTICAL_RELATIVE)
        half yDist = abs(worldPos.y - (_WorldSpaceCameraPos.y + _SkyboxVerticalFogParams.x));
    #else
        half yDist = abs(worldPos.y - _SkyboxVerticalFogParams.x);
    #endif

    #if defined(_SKYBOXFOG_VERTICAL_LINEAR)
        real heightFactor = saturate(yDist / _SkyboxVerticalFogParams.y);
        return 1.f - heightFactor;
    #elif defined(_SKYBOXFOG_VERTICAL_EXP)
        real heightFactor = saturate(_SkyboxVerticalFogParams.z * yDist);
        return (saturate(exp2(-heightFactor)) - 0.5f) * 2.f;
    #elif defined(_SKYBOXFOG_VERTICAL_EXP2)
        real heightFactor = saturate(_SkyboxVerticalFogParams.z * yDist);
        return (saturate(exp2(-heightFactor * heightFactor)) - 0.5f) * 2.f;
    #endif
#endif
    return 1.f;
}

half4 BlurDownSampling(in sampler2D tex, in float4 texelSize, in float2 uv)
{
    float4 offset = texelSize.xyxy * float4(-1.f, -1.f, 1.f, 1.f);
    offset *= _SkyboxBlurLevel;
    half4 outColor = tex2D(tex, uv) * 4;
    outColor += tex2D(tex, uv + offset.xy);
    outColor += tex2D(tex, uv + offset.xw);
    outColor += tex2D(tex, uv + offset.zy);
    outColor += tex2D(tex, uv + offset.zw);
    return outColor / 8.h;
}

half4 BlurUpSampling(in sampler2D tex, in float4 texelSize, in float2 uv)
{
    float4 offset = texelSize.xyxy * float4(-1.f, -1.f, 1.f, 1.f);
    offset *= _SkyboxBlurLevel;
    half4 outColor = tex2D(tex, uv + float2(offset.x, 0.f));
    outColor += tex2D(tex, uv + float2(offset.z, 0.f));
    outColor += tex2D(tex, uv + float2(0.f, offset.y));
    outColor += tex2D(tex, uv + float2(0.f, offset.w));
    outColor += tex2D(tex, uv + offset.xy * 0.5f) * 2.f;
    outColor += tex2D(tex, uv + offset.xw * 0.5f) * 2.f;
    outColor += tex2D(tex, uv + offset.zy * 0.5f) * 2.f;
    outColor += tex2D(tex, uv + offset.zw * 0.5f) * 2.f;
    return outColor / 12.h;
}


void SkyboxFog(in real3 inPosition, out real4 color, out real density)
{
#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2) || defined(_SKYBOXFOG_LINEAR) || defined(_SKYBOXFOG_EXP) || defined(_SKYBOXFOG_EXP2)
    real4 positionHCS = TransformObjectToHClip(inPosition);
    real4 screenPos = ComputeSkyboxScreenPos(positionHCS);
    
    half2 texCoord = screenPos.xy / screenPos.w;
    half4 texCol = tex2D(_SkyboxTex, texCoord);
    
    real3 worldPos = TransformObjectToWorld(inPosition) - _WorldSpaceCameraPos;
#if defined(_SKYBOXFOG_WORLD_DISTANCE)
        real fogDist = length(worldPos.xz);
#else
        real fogDist = UNITY_Z_0_FAR_FROM_CLIPSPACE(TransformObjectToHClip(inPosition).z);
#endif
    
    real heightDensity = ComputeHeightDensity(worldPos + _WorldSpaceCameraPos);
    
    density = (1.h - ComputeSkyboxFogIntensity(ComputeSkyboxFogFactor(fogDist))) * heightDensity;
        
#if defined(_SKYBOXFOG_LINEAR) || defined(_SKYBOXFOG_EXP) || defined(_SKYBOXFOG_EXP2)
        half4 fogCol = _SkyboxFogColor;
        real colorFogRate = _SkyboxBlendParams.y;
        real colorFogFadeRate = ComputeSkyboxFadeRate(density, _SkyboxBlendParams.z);
    
        color = lerp(fogCol * fogCol.a, texCol, colorFogFadeRate);
#else
        half4 fogCol = unity_FogColor;
        real colorFogFadeRate = ComputeSkyboxFadeRate(density, _SkyboxBlendParams.z);
    
        color = lerp(fogCol * fogCol.a, texCol, colorFogFadeRate);
#endif
    
    half4 colorFogOutColor = ComputeFogOutColor(color, density);
    color = lerp(colorFogOutColor, texCol, colorFogFadeRate);
#else
    density = 0.f;
    color = unity_FogColor;
#endif
#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2) || defined(_SKYBOXFOG_LINEAR) || defined(_SKYBOXFOG_EXP) || defined(_SKYBOXFOG_EXP2)
#if defined(_SKYBOXFOG_CLIP_FADE)
    half4 skyboxClipCol = tex2D(_SkyboxOrigTex, texCoord);
    color = lerp(color, skyboxClipCol, 1.h - ComputeSkyboxBlurFadeFactor(fogDist));
#endif
#endif
}

void SkyboxFog_float(in float3 inPosition, out float4 color, out float density)
{
    SkyboxFog(inPosition, color, density);
}

void SkyboxFog_half(in half3 inPosition, out half4 color, out half density)
{
    SkyboxFog(inPosition, color, density);
}

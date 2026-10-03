Shader "AuroraBorealis_Pack/AuroraBorealis"
{
    Properties
    {
        _Falloff ("Falloff", 2D) = "white" {}
        _Ramp ("Ramp", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}

        _NoiseEvolutionSpeed ("Noise Evolution Speed", Float) = 1
        [MaterialToggle]
        _InvertEvolutionDirection ("Invert Evolution Direction", Float) = 1

        [MaterialToggle]
        _InvertNoiseTexture ("Invert Noise Texture", Float) = 0

        _MovementSpeed ("Movement Speed", Float) = 1
        _Sharpness ("Sharpness", Range(0, 1)) = 0.1

        _Color ("Color", 2D) = "white" {}
        _Color_Multiplier ("Color Multiplier", Float) = 1

        _Dark_Multiplier ("Dark Multiplier", Float) = 1
        _Dark_Tint ("Dark Tint", Color) = (1,1,1,1)

        _Middle_Multiplier ("Middle Multiplier", Float) = 1
        _Middle_Tint ("Middle Tint", Color) = (1,1,1,1)

        _Highlight_Multiplier ("Highlight Multiplier", Float) = 1
        _Highlight_Tint ("Highlight Tint", Color) = (1,1,1,1)

        [HideInInspector]
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Name "Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            Blend One One
            Cull Off
            ZWrite Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #pragma target 3.5

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // ---------------------------------------------------------
            // Textures
            // ---------------------------------------------------------

            TEXTURE2D(_Noise);
            SAMPLER(sampler_Noise);

            TEXTURE2D(_Ramp);
            SAMPLER(sampler_Ramp);

            TEXTURE2D(_Falloff);
            SAMPLER(sampler_Falloff);

            TEXTURE2D(_Color);
            SAMPLER(sampler_Color);

            // ---------------------------------------------------------
            // Material properties
            // ---------------------------------------------------------

            CBUFFER_START(UnityPerMaterial)

                float4 _Noise_ST;
                float4 _Ramp_ST;
                float4 _Falloff_ST;
                float4 _Color_ST;

                float _NoiseEvolutionSpeed;
                float _InvertEvolutionDirection;
                float _InvertNoiseTexture;

                float _MovementSpeed;
                float _Sharpness;

                float _Color_Multiplier;

                float _Dark_Multiplier;
                float4 _Dark_Tint;

                float _Middle_Multiplier;
                float4 _Middle_Tint;

                float _Highlight_Multiplier;
                float4 _Highlight_Tint;

                float _Cutoff;

            CBUFFER_END

            // ---------------------------------------------------------
            // Vertex
            // ---------------------------------------------------------

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;

                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;

                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;

                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;

                float3 positionWS : TEXCOORD2;
                float3 normalWS : TEXCOORD3;

                float4 color : COLOR;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(input.positionOS.xyz);

                VertexNormalInputs normalInputs =
                    GetVertexNormalInputs(input.normalOS);

                output.positionCS = positionInputs.positionCS;
                output.positionWS = positionInputs.positionWS;
                output.normalWS = normalInputs.normalWS;

                output.uv0 = input.uv0;
                output.uv1 = input.uv1;

                output.color = input.color;

                return output;
            }

            // ---------------------------------------------------------
            // Helpers
            // ---------------------------------------------------------

            float2 TransformUV(
                float2 uv,
                float4 st
            )
            {
                return uv * st.xy + st.zw;
            }

            // ---------------------------------------------------------
            // Fragment
            // ---------------------------------------------------------

            half4 Frag(
                Varyings input,
                FRONT_FACE_TYPE facing : FRONT_FACE_SEMANTIC
            ) : SV_Target
            {
                // -----------------------------------------------------
                // Double-sided normal
                // -----------------------------------------------------

                float3 normalDirection = normalize(input.normalWS);

                // In URP this handles front/back faces.
                float faceSign = IS_FRONT_VFACE(facing, 1.0, -1.0);

                normalDirection *= faceSign;

                float3 viewDirection =
                    normalize(
                        GetCameraPositionWS() -
                        input.positionWS
                    );

                // -----------------------------------------------------
                // Animated noise
                // -----------------------------------------------------

                float2 uv1 = input.uv1;

                float noiseY = uv1.g;

                float timeX = _Time.y;
                float timeY = _Time.x;

                float evolutionDirection =
                    lerp(
                        _NoiseEvolutionSpeed,
                        -_NoiseEvolutionSpeed,
                        _InvertEvolutionDirection
                    );

                float noiseEvolution =
                    (noiseY * 0.05) +
                    ((timeY / 1000.0) * evolutionDirection);

                float2 noiseUV;

                noiseUV.x =
                    uv1.r +
                    (timeX * _MovementSpeed * 0.1);

                noiseUV.y =
                    noiseEvolution;

                noiseUV =
                    TransformUV(
                        noiseUV,
                        _Noise_ST
                    );

                float noise =
                    SAMPLE_TEXTURE2D(
                        _Noise,
                        sampler_Noise,
                        noiseUV
                    ).r;

                // Optional noise inversion
                noise =
                    lerp(
                        noise,
                        1.0 - noise,
                        saturate(_InvertNoiseTexture)
                    );

                // -----------------------------------------------------
                // Aurora shape
                // -----------------------------------------------------

                float threshold = uv1.g;

                float sharpness =
                    (_Sharpness * 20.0) + 5.0;

                float auroraShape =
                    (
                        1.0 -
                        ((noise - threshold) * sharpness)
                    )
                    *
                    (
                        (noise - (threshold - 0.005))
                        * sharpness
                    )
                    *
                    3.0;

                // Cut transparent portions
                clip(
                    saturate(auroraShape * 100.0)
                    - 0.5
                );

                // -----------------------------------------------------
                // Ramp
                // -----------------------------------------------------

                float2 rampUV =
                    float2(
                        0.5,
                        1.0 - input.color.r
                    );

                rampUV =
                    TransformUV(
                        rampUV,
                        _Ramp_ST
                    );

                float ramp =
                    SAMPLE_TEXTURE2D(
                        _Ramp,
                        sampler_Ramp,
                        rampUV
                    ).r;

                float auroraIntensity =
                    (auroraShape * ramp) / 3.0;

                // -----------------------------------------------------
                // Color texture
                // -----------------------------------------------------

                float colorPosition =
                    input.uv1.r;

                float colorGradient =
                    1.0 - input.color.r;

                // Highlight
                float4 highlightColor =
                    SAMPLE_TEXTURE2D(
                        _Color,
                        sampler_Color,
                        TransformUV(
                            float2(0.5, 0.05),
                            _Color_ST
                        )
                    );

                // Middle
                float4 middleColor =
                    SAMPLE_TEXTURE2D(
                        _Color,
                        sampler_Color,
                        TransformUV(
                            float2(0.5, 0.5),
                            _Color_ST
                        )
                    );

                // Dark
                float4 darkColor =
                    SAMPLE_TEXTURE2D(
                        _Color,
                        sampler_Color,
                        TransformUV(
                            float2(0.5, 0.95),
                            _Color_ST
                        )
                    );

                // -----------------------------------------------------
                // Original color weighting
                // -----------------------------------------------------

                float node1984 =
                    colorGradient;

                float node3430 =
                    (node1984 * -3.333333) + 1.0;

                float highlightWeight =
                    saturate(node3430);

                float middleWeight =
                    saturate(
                        (1.0 - node3430) *
                        (1.0 - node1984)
                    );

                float darkWeight =
                    pow(
                        saturate(node1984),
                        0.5
                    );

                float3 colorResult =
                    (
                        highlightColor.rgb *
                        highlightWeight *
                        _Highlight_Multiplier *
                        _Highlight_Tint.rgb
                    )
                    +
                    (
                        middleColor.rgb *
                        middleWeight *
                        _Middle_Multiplier *
                        _Middle_Tint.rgb
                    )
                    +
                    (
                        darkColor.rgb *
                        darkWeight *
                        _Dark_Multiplier *
                        _Dark_Tint.rgb
                    );

                // -----------------------------------------------------
                // Sharpness multiplier
                // -----------------------------------------------------

                float sharpnessMultiplier =
                    _Sharpness * 0.5 + 0.5;

                float sharpnessExtra =
                    saturate(
                        _Sharpness * 2.0 - 1.0
                    ) + 1.0;

                sharpnessMultiplier *=
                    sharpnessExtra;

                // -----------------------------------------------------
                // Edge/facing falloff
                // -----------------------------------------------------

                float viewDot =
                    saturate(
                        dot(
                            normalDirection,
                            viewDirection
                        )
                    );

                float facingFalloff =
                    1.0 -
                    pow(
                        1.0 - viewDot,
                        1.0
                    );

                // -----------------------------------------------------
                // Falloff texture
                // -----------------------------------------------------

                float2 falloffUV =
                    float2(
                        input.uv1.g,
                        input.uv1.r
                    );

                falloffUV =
                    TransformUV(
                        falloffUV,
                        _Falloff_ST
                    );

                float falloff =
                    SAMPLE_TEXTURE2D(
                        _Falloff,
                        sampler_Falloff,
                        falloffUV
                    ).r;

                // -----------------------------------------------------
                // Vertical fade
                // -----------------------------------------------------

                float verticalFade =
                    (1.0 - input.uv0.r) *
                    input.uv0.r *
                    4.0;

                // -----------------------------------------------------
                // Final emissive color
                // -----------------------------------------------------

                float3 emissive =
                    saturate(
                        auroraIntensity *
                        saturate(
                            lerp(
                                colorPosition,
                                1.0 - colorPosition,
                                step(
                                    0.5,
                                    colorPosition
                                )
                            ) / 0.1
                        )
                    )
                    *
                    colorResult
                    *
                    (
                        _Color_Multiplier *
                        sharpnessMultiplier
                    )
                    *
                    facingFalloff
                    *
                    falloff
                    *
                    verticalFade;

                return half4(
                    emissive,
                    1.0
                );
            }

            ENDHLSL
        }
    }

    FallBack Off
}

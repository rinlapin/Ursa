using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

namespace Soachbox
{
    public class SbFadeToSkyboxFog : ScriptableRendererFeature
    {
        public enum EFogType : int
        {
            Builtin = 0,
            Linear,
            Exp,
            Exp2,
        }
        public enum EVerticalFogType : int
        {
            Linear = 0,
            Exp,
            Exp2,
        }

        public enum EVerticalFogPosition : int
        {
            Absolute = 0,
            Relative,
        }

        // Use this class to pass around settings from the feature to the pass
        [Serializable]
        public class FadeToSkyboxFogSettings
        {
            public bool m_IsEnableOnSceneView = false;

            public bool m_IsUseFogOnSkybox = false;

            public bool m_IsSetSkyboxDistance = false;

            public float m_SkyboxDistance = 1000f;

            [HideInInspector]
            public Material m_OverrideMaterial = null;

            public RenderPassEvent m_FogRenderTiming = RenderPassEvent.BeforeRenderingPostProcessing;

            public int m_FogRenderTimingOffset = 0;

            [Range(0f, 1f)]
            public float m_SkyboxBlendFactor = 1f;

            [Range(0f, 1f)]
            public float m_ColorFadeEndPoint = 1f;

            [Range(0f, 1f)]
            public float m_FadeToSkyboxStartPoint = 0.5f;

            public bool m_IsUseWorldDistance = false;

            public bool m_IsUseVerticalFog = false;

            public EVerticalFogPosition m_VerticalFogPosition = EVerticalFogPosition.Absolute;

            public EVerticalFogType m_VerticalFogType = EVerticalFogType.Linear;

            public float m_VerticalFogCenterY = 0f;

            [Min(0.1f)]
            public float m_VerticalFogHeight = 1000f;

            [Range(0, 1)]
            public float m_VerticalFogDensity = 0.003f;

            public float VerticalFogDensitySliderRate
            {
                get => m_VerticalFogDensity == 0f ? 0f : (Mathf.Log(m_VerticalFogDensity, 2f) + 10) / 10f;
                set => m_VerticalFogDensity = value == 0 ? 0 : Mathf.Pow(2f, 10 * value - 10f);
            }

            public bool m_IsUsePostProcess = false;

            public bool m_IsUseSkyboxBlur = true;

            [Min(1)]
            public int m_BlurProcessLevel = 3;

            [Range(1, 20)]
            public int m_BlurLevel = 1;

            [Range(0, 1)]
            public float m_SkyboxClipFadeStartPoint = 0.95f;

            public Color m_FogColor = Color.white;

            public EFogType m_FogType;

            [Min(0f)]
            public float m_FogStartDist = 100;

            [Min(0f)]
            public float m_FogEndDist = 500;

            [Range(0, 1)]
            public float m_FogDensity = 0.003f;

            public float FogDensitySliderRate
            {
                get => m_FogDensity == 0f ? 0f : (Mathf.Log(m_FogDensity, 2f) + 10) / 10f;
                set => m_FogDensity = value == 0 ? 0 : Mathf.Pow(2f, 10 * value - 10f);
            }

            public void Validate()
            {
                m_FogStartDist = Mathf.Max(m_FogStartDist, 0f);
                m_FogEndDist = Mathf.Max(m_FogEndDist, 0f);

                if (m_FogStartDist > m_FogEndDist + 0.1f)
                    m_FogEndDist = m_FogStartDist + 0.1f;

                m_FogDensity = Mathf.Clamp(m_FogDensity, 0f, 1f);

                m_BlurProcessLevel = Mathf.Clamp(m_BlurProcessLevel, 1, MaxDownsamplingLevel);
            }

            public bool IsRenderCameraType(CameraType cameraType)
            {
                if (cameraType == CameraType.Game)
                    return true;
                if (m_IsEnableOnSceneView && cameraType == CameraType.SceneView)
                    return true;
                return false;
            }
        }

        [SerializeField]
        private FadeToSkyboxFogSettings m_Settings   = new FadeToSkyboxFogSettings();

        public FadeToSkyboxFogSettings Settings => m_Settings;

        private FadeToSkyboxFogPass m_SkyboxFogPass;

        private SkyboxCapturePass m_CapturePass;
        private ScreenCopyPass m_CopyPass;
        private TextureBlurDownsamplingPass[] m_BlurDownsamplingPasses = new TextureBlurDownsamplingPass[MaxDownsamplingLevel];
        private TextureBlurUpsamplingPass[] m_BlurUpsamplingPasses = new TextureBlurUpsamplingPass[MaxDownsamplingLevel];

        private readonly int k_EnableFlag = Shader.PropertyToID("_IsEnableSkyboxFog");

        private GlobalKeyword m_KeywordLinearFog;
        private GlobalKeyword m_KeywordExpFog;
        private GlobalKeyword m_KeywordExp2Fog;
        private GlobalKeyword m_KeywordUseWorldDistance;
        private GlobalKeyword m_KeywordUseClipFade;
        private GlobalKeyword m_KeywordUseFogOnSkybox;
        private GlobalKeyword m_KeywordSkyboxBlur;
        private GlobalKeyword m_KeywordUseVerticalFog;
        private GlobalKeyword m_KeywordVerticalFogRelative;
        private GlobalKeyword m_KeywordVerticalLinearFog;
        private GlobalKeyword m_KeywordVerticalExpFog;
        private GlobalKeyword m_KeywordVerticalExp2Fog;

        private readonly int k_SkyboxBlendParams = Shader.PropertyToID("_SkyboxBlendParams");
        private readonly int k_SkyboxBlurLevel = Shader.PropertyToID("_SkyboxBlurLevel");
        private readonly int k_SkyboxClipFadeParams = Shader.PropertyToID("_SkyboxClipFadeParams");
        private readonly int k_SkyboxClipPlane = Shader.PropertyToID("_SkyboxClipPlane");
        private readonly int k_SkyboxFogColor = Shader.PropertyToID("_SkyboxFogColor");
        private readonly int k_SkyboxFogParams = Shader.PropertyToID("_SkyboxFogParams");
        private readonly int k_SkyboxVerticalFogParams = Shader.PropertyToID("_SkyboxVerticalFogParams");

        static private readonly string k_BaseSkyboxTextureName = "_SkyboxOrigTex";
        static private readonly string k_FinalSkyboxTextureName = "_SkyboxTex";
        static public string BaseSkyboxTextureName => k_BaseSkyboxTextureName;
        static public string FinalSkyboxTextureName => k_FinalSkyboxTextureName;

        static private readonly int k_MaxDownsamplingLevel = 4;
        static public int MaxDownsamplingLevel => k_MaxDownsamplingLevel;

        private PassTextureInfo m_PassTextureInfo;


        public void ApplySetting(in FadeToSkyboxFogSettings srcSetting)
        {
            m_Settings = JsonUtility.FromJson<SbFadeToSkyboxFog.FadeToSkyboxFogSettings>(JsonUtility.ToJson(srcSetting));
        }

        private void OnEnable()
        {
            m_KeywordLinearFog = GlobalKeyword.Create("_SKYBOXFOG_LINEAR");
            m_KeywordExpFog = GlobalKeyword.Create("_SKYBOXFOG_EXP");
            m_KeywordExp2Fog = GlobalKeyword.Create("_SKYBOXFOG_EXP2");
            m_KeywordUseWorldDistance = GlobalKeyword.Create("_SKYBOXFOG_WORLD_DISTANCE");
            m_KeywordUseClipFade = GlobalKeyword.Create("_SKYBOXFOG_CLIP_FADE");
            m_KeywordUseFogOnSkybox = GlobalKeyword.Create("_SKYBOXFOG_FOG_ON_SKYBOX");
            m_KeywordSkyboxBlur = GlobalKeyword.Create("_SKYBOXFOG_BLUR");
            m_KeywordUseVerticalFog = GlobalKeyword.Create("_SKYBOXFOG_VERTICAL");
            m_KeywordVerticalFogRelative = GlobalKeyword.Create("_SKYBOXFOG_VERTICAL_RELATIVE");
            m_KeywordVerticalLinearFog = GlobalKeyword.Create("_SKYBOXFOG_VERTICAL_LINEAR");
            m_KeywordVerticalExpFog = GlobalKeyword.Create("_SKYBOXFOG_VERTICAL_EXP");
            m_KeywordVerticalExp2Fog = GlobalKeyword.Create("_SKYBOXFOG_VERTICAL_EXP2");
        }

        private bool FindMaterial()
        {
            var skyboxFogShader = Shader.Find("Fog/SbFadeToSkyboxFog");
            if (skyboxFogShader != null)
            {
                m_Settings.m_OverrideMaterial = new Material(skyboxFogShader);
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public override void Create()
        {
            if (m_PassTextureInfo == null)
                m_PassTextureInfo = new PassTextureInfo();

            RemovePasses();

            m_SkyboxFogPass = new FadeToSkyboxFogPass(m_Settings);

            if (m_Settings.m_OverrideMaterial == null)
            {
                FindMaterial();
            }

            // Skybox Capture Pass
            m_CapturePass = new SkyboxCapturePass(m_Settings, m_PassTextureInfo);
            m_CapturePass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;

            // Blur Pass
            for (int i = 0; i < MaxDownsamplingLevel; ++i)
            {
                m_BlurDownsamplingPasses[i] = new TextureBlurDownsamplingPass(m_Settings, m_PassTextureInfo, i);
                m_BlurDownsamplingPasses[i].renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;

                m_BlurUpsamplingPasses[i] = new TextureBlurUpsamplingPass(m_Settings, m_PassTextureInfo, i);
                m_BlurUpsamplingPasses[i].renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            }

            // Apply Fog
            int renderTiming = (int)m_Settings.m_FogRenderTiming + m_Settings.m_FogRenderTimingOffset;

            m_CopyPass = new ScreenCopyPass(m_Settings, m_PassTextureInfo);
            m_CopyPass.renderPassEvent = (RenderPassEvent)renderTiming;

            m_SkyboxFogPass.renderPassEvent = (RenderPassEvent)renderTiming;

            Shader.SetGlobalInt(k_EnableFlag, isActive ? 1 : 0);
        }

        private void SetShaderParams(ref RenderingData renderingData)
        {
            Shader.DisableKeyword(m_KeywordLinearFog);
            Shader.DisableKeyword(m_KeywordExpFog);
            Shader.DisableKeyword(m_KeywordExp2Fog);
            Shader.DisableKeyword(m_KeywordUseWorldDistance);
            Shader.DisableKeyword(m_KeywordUseClipFade);
            Shader.DisableKeyword(m_KeywordUseFogOnSkybox);

            Shader.DisableKeyword(m_KeywordUseVerticalFog);
            Shader.DisableKeyword(m_KeywordVerticalFogRelative);
            Shader.DisableKeyword(m_KeywordVerticalLinearFog);
            Shader.DisableKeyword(m_KeywordVerticalExpFog);
            Shader.DisableKeyword(m_KeywordVerticalExp2Fog);

            m_Settings.Validate();

            if( m_Settings.m_IsUseFogOnSkybox)
                Shader.EnableKeyword(m_KeywordUseFogOnSkybox);

            switch (m_Settings.m_FogType)
            {
                case EFogType.Linear:
                    Shader.EnableKeyword(m_KeywordLinearFog);
                    break;
                case EFogType.Exp:
                    Shader.EnableKeyword(m_KeywordExpFog);
                    break;
                case EFogType.Exp2:
                    Shader.EnableKeyword(m_KeywordExp2Fog);
                    break;
                default: break;
            }


            switch (m_Settings.m_VerticalFogPosition)
            {
                case EVerticalFogPosition.Relative:
                    Shader.EnableKeyword(m_KeywordVerticalFogRelative);
                    break;
            }
            switch (m_Settings.m_VerticalFogType)
            {
                case EVerticalFogType.Linear:
                    Shader.EnableKeyword(m_KeywordVerticalLinearFog);
                    break;
                case EVerticalFogType.Exp:
                    Shader.EnableKeyword(m_KeywordVerticalExpFog);
                    break;
                case EVerticalFogType.Exp2:
                    Shader.EnableKeyword(m_KeywordVerticalExp2Fog);
                    break;
                default: break;
            }

            if (m_Settings.m_IsUseWorldDistance)
                Shader.EnableKeyword(m_KeywordUseWorldDistance);

            if (m_Settings.m_IsSetSkyboxDistance)
            {
                Shader.SetGlobalFloat(k_SkyboxClipPlane, m_Settings.m_SkyboxDistance);
            }
            else
            {
                Shader.SetGlobalFloat(k_SkyboxClipPlane, renderingData.cameraData.camera.farClipPlane);
            }

            if (m_Settings.m_IsUseVerticalFog)
            {
                Shader.EnableKeyword(m_KeywordUseVerticalFog);
                Shader.SetGlobalVector(k_SkyboxVerticalFogParams, new Vector4(m_Settings.m_VerticalFogCenterY, 
                                                                        m_Settings.m_VerticalFogHeight,
                                                                        Mathf.Pow(m_Settings.m_VerticalFogDensity, 2f) / Mathf.Sqrt(Mathf.Log(2)),
                                                                        0f));
            }

            if (m_Settings.m_FogType != EFogType.Builtin)
            {
                var skyboxFogParams = new Vector4(m_Settings.m_FogDensity / Mathf.Sqrt(Mathf.Log(2)),
                                    m_Settings.m_FogDensity / Mathf.Log(2),
                                    -1 / (m_Settings.m_FogEndDist - m_Settings.m_FogStartDist),
                                    m_Settings.m_FogEndDist / (m_Settings.m_FogEndDist - m_Settings.m_FogStartDist)
                            );
                Shader.SetGlobalVector(k_SkyboxFogParams, skyboxFogParams);
                Shader.SetGlobalVector(k_SkyboxFogColor, m_Settings.m_FogColor);
            }

            Shader.SetGlobalInt(k_EnableFlag, isActive ? 1 : 0);
            Shader.SetGlobalVector(k_SkyboxBlendParams, new Vector4(
                                                                m_Settings.m_SkyboxBlendFactor,
                                                                isActive ? m_Settings.m_ColorFadeEndPoint : 1f,
                                                                isActive ? m_Settings.m_FadeToSkyboxStartPoint : 1f,
                                                                0f));

            float m_BlurLevelRate = m_Settings.m_BlurProcessLevel / (float)MaxDownsamplingLevel;
            Shader.SetGlobalFloat(k_SkyboxBlurLevel, m_Settings.m_BlurLevel);

            if (m_Settings.m_IsUseSkyboxBlur)
                Shader.EnableKeyword(m_KeywordSkyboxBlur);
            else
                Shader.DisableKeyword(m_KeywordSkyboxBlur);

            if (m_Settings.m_SkyboxClipFadeStartPoint < 1f)
            {
                float cameraFarClip = renderingData.cameraData.camera.farClipPlane;
                float fadeStartClip = cameraFarClip * m_Settings.m_SkyboxClipFadeStartPoint;
                var blurFadeParams = new float4(0f, 0f,
                                    -1 / (cameraFarClip - fadeStartClip),
                                    cameraFarClip / (cameraFarClip - fadeStartClip)
                                    );
                Shader.SetGlobalVector(k_SkyboxClipFadeParams, blurFadeParams);
                Shader.EnableKeyword(m_KeywordUseClipFade);
            }
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (m_Settings.m_OverrideMaterial == null)
            {
                if (!FindMaterial())
                    Debug.LogWarning("FadeToSkyboxFog: material is not set");
                return;
            }

            SetShaderParams(ref renderingData);

            renderer.EnqueuePass(m_CapturePass);

            if (m_Settings.m_IsUseSkyboxBlur)
            {
                int blurLevelOffset = MaxDownsamplingLevel - m_Settings.m_BlurProcessLevel;
                for (int i = 0; i < m_Settings.m_BlurProcessLevel; ++i)
                    renderer.EnqueuePass(m_BlurDownsamplingPasses[i + blurLevelOffset]);
                for (int i = 0; i < m_Settings.m_BlurProcessLevel; ++i)
                    renderer.EnqueuePass(m_BlurUpsamplingPasses[i]);
            }

            renderer.EnqueuePass(m_CopyPass);

            if (m_Settings.m_IsUsePostProcess)
                renderer.EnqueuePass(m_SkyboxFogPass);
        }

        public class PassTextureInfo
        {
            public TextureHandle sourceTexture = TextureHandle.nullHandle;

            public void Dispose()
            {
            }
        }

        private void RemovePasses()
        {
            m_CapturePass?.Dispose();
            m_CapturePass = null;

            for (int i = 0; i < MaxDownsamplingLevel; ++i)
            {
                m_BlurDownsamplingPasses[i]?.Dispose();
                m_BlurDownsamplingPasses[i] = null;

                m_BlurUpsamplingPasses[i]?.Dispose();
                m_BlurUpsamplingPasses[i] = null;
            }

            m_CopyPass?.Dispose();
            m_CopyPass = null;
        }

        protected override void Dispose(bool disposing)
        {
            RemovePasses();

            m_PassTextureInfo?.Dispose();
        }
    }

    class SkyboxCapturePass : ScriptableRenderPass
    {
        readonly SbFadeToSkyboxFog.FadeToSkyboxFogSettings settings;
        readonly SbFadeToSkyboxFog.PassTextureInfo passTextureInfo;

        private RTHandle m_OutputHandle;
        private RTHandle m_BaseOutputHandle;

        private string m_OutputName = "_SkyboxTex";
        public string OutputName => m_OutputName;

        private int m_OutputNameId;
        private int m_BaseSkyboxTexNameId;

        public SkyboxCapturePass(SbFadeToSkyboxFog.FadeToSkyboxFogSettings settings, SbFadeToSkyboxFog.PassTextureInfo passInfo)
        {
            this.settings = settings;
            this.passTextureInfo = passInfo;

            int blurOffsetLevel = SbFadeToSkyboxFog.MaxDownsamplingLevel - settings.m_BlurProcessLevel;

            m_OutputName = settings.m_IsUseSkyboxBlur ? TextureBlurDownsamplingPass.k_OutputNameBase + $"_{blurOffsetLevel}" : SbFadeToSkyboxFog.FinalSkyboxTextureName;

            m_OutputNameId = Shader.PropertyToID(OutputName);
            m_BaseSkyboxTexNameId = Shader.PropertyToID(SbFadeToSkyboxFog.BaseSkyboxTextureName);
        }

        // This class stores the data needed by the RenderGraph pass.
        // It is passed as a parameter to the delegate function that executes the RenderGraph pass.
        private class PassData
        {
            public TextureHandle sourceTexture;
            public TextureHandle destTexture;
            public RendererListHandle objectsToDraw;
        }

        // This static method is passed as the RenderFunc delegate to the RenderGraph render pass.
        // It is used to execute draw commands.
        static void ExecutePass(PassData data, RasterGraphContext context)
        {
            context.cmd.DrawRendererList(data.objectsToDraw);
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string passName = "Skybox Capture Pass";

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            if (!settings.IsRenderCameraType(cameraData.cameraType))
                return;

            var desc = cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;

            RenderingUtils.ReAllocateHandleIfNeeded(ref m_OutputHandle, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: OutputName);

            TextureHandle source = resourceData.activeColorTexture;

            TextureHandle destination = renderGraph.ImportTexture(m_OutputHandle);

            if (!source.IsValid() || !destination.IsValid())
                return;

            Shader.SetGlobalTexture(m_OutputNameId, m_OutputHandle);
            Shader.SetGlobalTexture(m_BaseSkyboxTexNameId, m_OutputHandle);

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                passData.objectsToDraw = renderGraph.CreateSkyboxRendererList(in cameraData.camera);

                builder.UseRendererList(passData.objectsToDraw);
                builder.UseTexture(source);
                builder.SetRenderAttachment(destination, 0);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }

            passTextureInfo.sourceTexture = destination;
            resourceData.cameraColor = source;
        }

        public void Dispose()
        {
            m_OutputHandle?.Release();
        }
    }

    class ScreenCopyPass : ScriptableRenderPass
    {
        readonly SbFadeToSkyboxFog.FadeToSkyboxFogSettings settings;
        readonly SbFadeToSkyboxFog.PassTextureInfo passTextureInfo;

        private RTHandle m_OutputHandle;
        private const string k_OutputName = "_CopiedTex";
        static public string OutputName => k_OutputName;

        private int m_OutputNameId;

        public ScreenCopyPass(SbFadeToSkyboxFog.FadeToSkyboxFogSettings settings, SbFadeToSkyboxFog.PassTextureInfo passInfo)
        {
            this.settings = settings;
            this.passTextureInfo = passInfo;

            m_OutputNameId = Shader.PropertyToID(OutputName);
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            if (!settings.IsRenderCameraType(cameraData.cameraType))
                return;

            var desc = cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            RenderingUtils.ReAllocateHandleIfNeeded(ref m_OutputHandle, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: OutputName);

            TextureHandle source = resourceData.activeColorTexture;
            TextureHandle destination = renderGraph.ImportTexture(m_OutputHandle);

            if (!source.IsValid() || !destination.IsValid())
                return;

            Shader.SetGlobalTexture(m_OutputNameId, m_OutputHandle);

            renderGraph.AddCopyPass(source, destination);
        }

        public void Dispose()
        {
            m_OutputHandle?.Release();
        }
    }

    class TextureBlurDownsamplingPass : ScriptableRenderPass
    {
        readonly SbFadeToSkyboxFog.FadeToSkyboxFogSettings settings;
        readonly SbFadeToSkyboxFog.PassTextureInfo passTextureInfo;

        public const string k_OutputNameBase = "_SkyboxBlurTex";

        private RTHandle m_OutputHandle;
        private string m_OutputName = k_OutputNameBase;
        public string OutputName => m_OutputName;

        private int m_OutputNameId;

        private int m_DownLevel;

        private int m_PassIndex;
        private int m_PassIndexOffset;

        public TextureBlurDownsamplingPass(SbFadeToSkyboxFog.FadeToSkyboxFogSettings settings, SbFadeToSkyboxFog.PassTextureInfo passInfo, int downLevel)
        {
            Debug.Assert(downLevel < SbFadeToSkyboxFog.MaxDownsamplingLevel, $"Error TextureBlurDownsamplingPass: downsampling level is exceeded {SbFadeToSkyboxFog.MaxDownsamplingLevel}");

            this.settings = settings;
            this.passTextureInfo = passInfo;
            this.m_DownLevel = downLevel;

            m_OutputName = m_OutputName + $"_{m_DownLevel + 1}";
            m_OutputNameId = Shader.PropertyToID(OutputName);

            m_PassIndex = 1 + m_DownLevel;
            m_PassIndexOffset = SbFadeToSkyboxFog.MaxDownsamplingLevel - settings.m_BlurProcessLevel;
        }

        // This class stores the data needed by the RenderGraph pass.
        // It is passed as a parameter to the delegate function that executes the RenderGraph pass.
        private class PassData
        {
        }


        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string passName = "Skybox Blur DownSampling Pass";

            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

            if (!settings.IsRenderCameraType(cameraData.cameraType))
                return;

            TextureDesc srcTexDesc = passTextureInfo.sourceTexture.GetDescriptor(renderGraph);
            TextureHandle sourceTexture = renderGraph.CreateTexture(in srcTexDesc);

            sourceTexture = passTextureInfo.sourceTexture;

            var desc = cameraData.cameraTargetDescriptor;
            if (m_DownLevel == m_PassIndexOffset)
            {
                desc.height = (srcTexDesc.height >> (1 + m_PassIndexOffset));
                desc.width = (srcTexDesc.width >> (1 + m_PassIndexOffset));
            }
            else
            {
                desc.height = (srcTexDesc.height >> 1);
                desc.width = (srcTexDesc.width >> 1);
            }
            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;
            RenderingUtils.ReAllocateHandleIfNeeded(ref m_OutputHandle, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: OutputName);

            TextureHandle destinationTexture = renderGraph.ImportTexture(m_OutputHandle);

            var blitParams = new RenderGraphUtils.BlitMaterialParameters(sourceTexture, destinationTexture, settings.m_OverrideMaterial, m_PassIndex);

            using (var builder = renderGraph.AddBlitPass(blitParams, passName, true))
            {
                builder.AllowGlobalStateModification(true);
                builder.SetGlobalTextureAfterPass(destinationTexture, m_OutputNameId);
            }

            passTextureInfo.sourceTexture = destinationTexture;
        }

        public void Dispose()
        {
            m_OutputHandle?.Release();
        }
    }

    class TextureBlurUpsamplingPass : ScriptableRenderPass
    {
        readonly SbFadeToSkyboxFog.FadeToSkyboxFogSettings settings;
        readonly SbFadeToSkyboxFog.PassTextureInfo passTextureInfo;

        private RTHandle m_OutputHandle;

        private string m_OutputName = TextureBlurDownsamplingPass.k_OutputNameBase;
        public string OutputName => m_OutputName;

        private int m_OutputNameId;

        private int m_UpLevel;

        private int m_PassIndex;

        public TextureBlurUpsamplingPass(SbFadeToSkyboxFog.FadeToSkyboxFogSettings settings, SbFadeToSkyboxFog.PassTextureInfo passInfo, int upLevel)
        {
            this.settings = settings;
            this.passTextureInfo = passInfo;
            this.m_UpLevel = upLevel;

            m_PassIndex = 1 + m_UpLevel + SbFadeToSkyboxFog.MaxDownsamplingLevel;

            if (upLevel == settings.m_BlurProcessLevel - 1)
                m_OutputName = SbFadeToSkyboxFog.FinalSkyboxTextureName;
            else
                m_OutputName = m_OutputName + $"_{m_PassIndex}";

            m_OutputNameId = Shader.PropertyToID(OutputName);
        }

        // This class stores the data needed by the RenderGraph pass.
        // It is passed as a parameter to the delegate function that executes the RenderGraph pass.
        private class PassData
        {
        }


        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string passName = "Skybox Blur Pass";

            // カメラの情報を取得
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

            if (!settings.IsRenderCameraType(cameraData.cameraType))
                return;

            TextureDesc srcTexDesc = passTextureInfo.sourceTexture.GetDescriptor(renderGraph);
            TextureHandle sourceTexture = renderGraph.CreateTexture(in srcTexDesc);

            sourceTexture = passTextureInfo.sourceTexture;

            var desc = cameraData.cameraTargetDescriptor;
            desc.height = (srcTexDesc.height << 1);
            desc.width = (srcTexDesc.width << 1);
            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;
            RenderingUtils.ReAllocateHandleIfNeeded(ref m_OutputHandle, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: OutputName);

            TextureHandle destinationTexture = renderGraph.ImportTexture(m_OutputHandle);

            var blitParams = new RenderGraphUtils.BlitMaterialParameters(sourceTexture, destinationTexture, settings.m_OverrideMaterial, m_PassIndex);

            using (var builder = renderGraph.AddBlitPass(blitParams, passName: "newPass", returnBuilder: true))
            {
                builder.AllowGlobalStateModification(true);
                builder.SetGlobalTextureAfterPass(destinationTexture, m_OutputNameId);
            }

            passTextureInfo.sourceTexture = destinationTexture;
        }

        public void Dispose()
        {
            m_OutputHandle?.Release();
        }
    }

    class FadeToSkyboxFogPass : ScriptableRenderPass
    {
        readonly SbFadeToSkyboxFog.FadeToSkyboxFogSettings settings;


        public FadeToSkyboxFogPass(SbFadeToSkyboxFog.FadeToSkyboxFogSettings settings)
        {
            this.settings = settings;
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            const string passName = "FogToSkybox Fog Pass";

            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

            if (!settings.IsRenderCameraType(cameraData.cameraType))
                return;

            TextureDesc srcTexDesc = resourceData.activeColorTexture.GetDescriptor(renderGraph);
            TextureHandle sourceTexture = renderGraph.CreateTexture(in srcTexDesc);

            TextureDesc dstTexDesc = resourceData.activeColorTexture.GetDescriptor(renderGraph);
            TextureHandle destinationTexture = renderGraph.CreateTexture(in dstTexDesc);

            var blitParams = new RenderGraphUtils.BlitMaterialParameters(sourceTexture, destinationTexture, settings.m_OverrideMaterial, 0);

            renderGraph.AddBlitPass(blitParams, passName);

            resourceData.cameraColor = destinationTexture;
        }
    }

}
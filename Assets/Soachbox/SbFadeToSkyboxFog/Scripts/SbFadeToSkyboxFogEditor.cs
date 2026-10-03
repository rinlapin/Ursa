using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Soachbox
{
    [CustomEditor(typeof(SbFadeToSkyboxFog))]
    public class SbFadeToSkyboxFogEditor : ScriptableRendererFeatureEditor
    {
        private SbFadeToSkyboxFog m_TargetFog;
        private FadeToSkyboxFogSettingGUI m_GUIItem;

        SbFadeToSkyboxFogEditor()
        {
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        void OnUndoRedo()
        {
            m_GUIItem?.CopySettingsFrom(m_TargetFog.Settings);
        }

        private void OnEnable()
        {
            if (m_TargetFog == null)
            {
                m_TargetFog = target as SbFadeToSkyboxFog;
            }
            if (m_TargetFog != null && m_GUIItem == null)
            {
                m_GUIItem = new FadeToSkyboxFogSettingGUI(m_TargetFog, m_TargetFog.Settings);
            }

            m_GUIItem?.OnEnable();
        }

        private void OnDisable()
        {
            m_GUIItem?.OnEnable();
        }

        public override void OnInspectorGUI()
        {

            m_GUIItem?.OnInspectorGUI();
        }
    }

    public class FadeToSkyboxFogSettingGUI
    {
        private SbFadeToSkyboxFog m_Parent;
        private SbFadeToSkyboxFog.FadeToSkyboxFogSettings m_Settings;

        private bool m_IsChanged = false;
        public bool IsChanged => m_IsChanged;

        private ScriptableRenderer m_CameraRenderer = null;
        private bool m_IsParentCurrentRenderer = false;
        private bool m_IsRendererChanged = false;

        private readonly string[] k_BlurQualityNames =
        {
            "Full (8-pass)",
            "Half (6-pass)",
            "Quater (4-pass)",
            "One-Eighth (2-pass)",
        };

        private readonly string[] k_RenderTimingNames =
        {
            RenderPassEvent.AfterRenderingOpaques.ToString(),
            RenderPassEvent.BeforeRenderingSkybox.ToString(),
            RenderPassEvent.AfterRenderingSkybox.ToString(),
            RenderPassEvent.BeforeRenderingTransparents.ToString(),
            RenderPassEvent.AfterRenderingTransparents.ToString(),
            RenderPassEvent.BeforeRenderingPostProcessing.ToString(),
            RenderPassEvent.AfterRenderingPostProcessing.ToString(),
        };

        private readonly RenderPassEvent[] k_RenderTimingEnums;

        public FadeToSkyboxFogSettingGUI(SbFadeToSkyboxFog parent, SbFadeToSkyboxFog.FadeToSkyboxFogSettings settings)
        {
            m_Parent = parent;
            CopySettingsFrom(settings);

            k_RenderTimingEnums = new RenderPassEvent[k_RenderTimingNames.Length];

            var renderPassValues = (RenderPassEvent[])Enum.GetValues(typeof(RenderPassEvent));
            for(int i = 0; i < k_RenderTimingNames.Length; ++i)
            {
                var enumIndex = Mathf.Max(Array.FindIndex( renderPassValues, value => k_RenderTimingNames[i] == value.ToString() ), 0);

                k_RenderTimingEnums[i] = renderPassValues[enumIndex];
            }
        }

        public void CopySettingsFrom(SbFadeToSkyboxFog.FadeToSkyboxFogSettings settings)
        {
            m_Settings = JsonUtility.FromJson<SbFadeToSkyboxFog.FadeToSkyboxFogSettings>(JsonUtility.ToJson(settings));
        }

        [MenuItem("Tools/FadeToSkyboxFog/Select Main Camera", false, 0)]
        public static void SelectMainCamera()
        {
            Selection.activeObject = Camera.main;
        }

        [MenuItem("Tools/FadeToSkyboxFog/Open RenderPipeline Asset", false, 11)]
        public static void OepnRenderPipelineAsset()
        {
            var pipelineAsset = GetRenderPipelineAsset();
            if (pipelineAsset != null)
            {
                Selection.activeObject = pipelineAsset;
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Current Render Pipeline is not URP.\nPlease check Project Settings.", "OK");
            }
        }

        [MenuItem("Tools/FadeToSkyboxFog/Open MainCamera RendererData", true)]
        public static bool OepnCurrentMainCameraRendererDataValidator()
        {
            return GetCurrentMainCameraRendererData() != null;
        }

        [MenuItem("Tools/FadeToSkyboxFog/Open MainCamera RendererData", false, 12)]
        public static void OepnCurrentMainCameraRendererData()
        {
            var rendererData = GetCurrentMainCameraRendererData();
            if (rendererData != null)
            {
                Selection.activeObject = rendererData;
            }
        }

        [MenuItem("Tools/FadeToSkyboxFog/Open MainCamera SkyboxFog Settings", true)]
        public static bool OepnCurrentMainCameraSkyboxFogValidator()
        {
            return GetCurrentMainCameraSkyboxFog() != null;
        }

        [MenuItem("Tools/FadeToSkyboxFog/Open MainCamera SkyboxFog Settings", false, 13)]
        public static void OepnCurrentMainCameraSkyboxFog()
        {
            var skyboxFog = GetCurrentMainCameraSkyboxFog();
            if (skyboxFog != null)
            {
                Selection.activeObject = skyboxFog;
            }
        }

        private static UniversalRenderPipelineAsset GetRenderPipelineAsset()
        {
            if (QualitySettings.renderPipeline is UniversalRenderPipelineAsset pipelineAsset)
            {
                return pipelineAsset;
            }
            return null;
        }

        private static ScriptableRendererData GetCurrentMainCameraRendererData()
        {
            if (QualitySettings.renderPipeline is UniversalRenderPipelineAsset pipelineAsset)
            {
                var cameraData = Camera.main.GetUniversalAdditionalCameraData();
                if (cameraData != null && pipelineAsset.renderers.Length == pipelineAsset.rendererDataList.Length)
                {
                    for (int i = 0; i < pipelineAsset.renderers.Length; ++i)
                    {
                        if (pipelineAsset.renderers[i] == cameraData.scriptableRenderer)
                        {
                            return pipelineAsset.rendererDataList[i];
                        }
                    }
                }
            }
            return null;
        }

        private static int GetCurrentMainCameraRendererIndex()
        {
            if (QualitySettings.renderPipeline is UniversalRenderPipelineAsset pipelineAsset)
            {
                var cameraData = Camera.main.GetUniversalAdditionalCameraData();
                if (cameraData != null && pipelineAsset.renderers.Length == pipelineAsset.rendererDataList.Length)
                {
                    for (int i = 0; i < pipelineAsset.renderers.Length; ++i)
                    {
                        if (pipelineAsset.renderers[i] == cameraData.scriptableRenderer)
                            return i;
                    }
                }
            }
            return 0;
        }

        private static SbFadeToSkyboxFog GetCurrentMainCameraSkyboxFog()
        {
            var rendererData = GetCurrentMainCameraRendererData();
            if (rendererData == null)
                return null;

            foreach (var renderFeature in rendererData.rendererFeatures)
            {
                if (renderFeature is SbFadeToSkyboxFog skyboxFog)
                {
                    return skyboxFog;
                }
            }
            return null;
        }

        private void DrawDispSetting()
        {
            using (new EditorGUI.DisabledScope(!m_IsParentCurrentRenderer))
            {
                var prevIsEnableOnSceneView = m_Settings.m_IsEnableOnSceneView;
                m_Settings.m_IsEnableOnSceneView = EditorGUILayout.Toggle("Enable on Scene View", m_Settings.m_IsEnableOnSceneView);

                if (!prevIsEnableOnSceneView && m_Settings.m_IsEnableOnSceneView)
                {
                    m_IsRendererChanged = true;
                }
            }

            if (m_IsParentCurrentRenderer)
            {
                if (!m_Settings.m_IsEnableOnSceneView)
                    EditorGUILayout.HelpBox("This option changes the current RenderPipelineAsset's default renderer.", MessageType.Info);
                else
                    EditorGUILayout.HelpBox("In Scene View, the skybox may not display properly.", MessageType.Info);
            }
            else
            {
                m_Settings.m_IsEnableOnSceneView = false;
                EditorGUILayout.HelpBox("This renderer is not set to the current main camera's renderer.", MessageType.Info);
            }

            EditorGUILayout.Space(5);

            using (new EditorGUILayout.VerticalScope("box"))
            {
                m_Settings.m_IsUsePostProcess = EditorGUILayout.Toggle("Enable Screen Fog", m_Settings.m_IsUsePostProcess);

                if( m_Settings.m_IsUsePostProcess && GetRenderPipelineAsset()?.supportsCameraDepthTexture == false )
                {
                    EditorGUILayout.HelpBox("Depth texture must be enabled.\nSee current RenderPipelineAsset setting.", MessageType.Error);
                }

                using (new EditorGUI.DisabledScope(!m_Settings.m_IsUsePostProcess))
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        int timingIndex = Mathf.Max(System.Array.FindIndex(k_RenderTimingNames, name => m_Settings.m_FogRenderTiming.ToString() == name), 0);
                        timingIndex = EditorGUILayout.Popup("Render Timing", timingIndex, k_RenderTimingNames);
                        m_Settings.m_FogRenderTiming = k_RenderTimingEnums[timingIndex];

                        using (new EditorGUI.IndentLevelScope())
                        {
                            m_Settings.m_FogRenderTimingOffset = EditorGUILayout.IntField("Offset", m_Settings.m_FogRenderTimingOffset, GUILayout.ExpandWidth(false));
                        }
                    }

                    using (new EditorGUI.IndentLevelScope())
                    {
                        m_Settings.m_IsUseFogOnSkybox = EditorGUILayout.Toggle("Apply Fog on Skybox", m_Settings.m_IsUseFogOnSkybox);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            m_Settings.m_IsSetSkyboxDistance = EditorGUILayout.Toggle("Set Skybox Distance", m_Settings.m_IsSetSkyboxDistance);

                            using (new EditorGUI.DisabledScope(!m_Settings.m_IsSetSkyboxDistance))
                            using (new EditorGUI.IndentLevelScope())
                            {
                                m_Settings.m_SkyboxDistance = EditorGUILayout.FloatField("Distance", m_Settings.m_SkyboxDistance);
                            }
                        }
                    }
                }
            }

            EditorGUILayout.Space(5);

            m_Settings.m_IsUseWorldDistance = EditorGUILayout.Toggle("Use World Distance", m_Settings.m_IsUseWorldDistance);

            EditorGUILayout.Space(5);
        }

        private void DrawFogBaseSetting()
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                m_Settings.m_FogColor = EditorGUILayout.ColorField("Fog Color", m_Settings.m_FogColor);
                EditorGUILayout.HelpBox("Set alpha to zero to disable fog color.", MessageType.Info);

                EditorGUILayout.Space(5);

                using (new EditorGUILayout.VerticalScope("box"))
                {
                    m_Settings.m_FogType = (SbFadeToSkyboxFog.EFogType)EditorGUILayout.EnumPopup("Fog Type", m_Settings.m_FogType);

                    if (m_Settings.m_FogType == SbFadeToSkyboxFog.EFogType.Builtin)
                    {
                        if (RenderSettings.fog)
                            EditorGUILayout.HelpBox("Built-in Fog is used.", MessageType.Info);
                        else
                            EditorGUILayout.HelpBox("Built-in Fog is disabled. Check Lighing settings.", MessageType.Warning);

                    }
                    else if (m_Settings.m_FogType == SbFadeToSkyboxFog.EFogType.Linear)
                    {
                        if (RenderSettings.fog)
                            EditorGUILayout.HelpBox("Built-in Fog is enabled.", MessageType.Warning);

                        using (new EditorGUI.IndentLevelScope())
                        {
                            m_Settings.m_FogStartDist = EditorGUILayout.FloatField("Start", m_Settings.m_FogStartDist);
                            m_Settings.m_FogEndDist = EditorGUILayout.FloatField("End", m_Settings.m_FogEndDist);
                        }
                    }
                    else
                    {
                        if (RenderSettings.fog)
                            EditorGUILayout.HelpBox("Built-in Fog is enabled.", MessageType.Warning);

                        using (new EditorGUI.IndentLevelScope())
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField("Density", GUILayout.Width(EditorGUIUtility.labelWidth));
                                m_Settings.FogDensitySliderRate = GUILayout.HorizontalSlider(m_Settings.FogDensitySliderRate, 0f, 1f);
                                m_Settings.m_FogDensity = EditorGUILayout.FloatField(m_Settings.m_FogDensity, GUILayout.Width(60));
                            }
                        }
                    }
                }
            }

            EditorGUILayout.Space(5);
        }

        private void DrawBlendSetting()
        {
            m_Settings.m_SkyboxBlendFactor = EditorGUILayout.Slider("Skybox Blend Factor", m_Settings.m_SkyboxBlendFactor, 0f, 1f);
            m_Settings.m_ColorFadeEndPoint = EditorGUILayout.Slider("ColorFog Fade End Point", m_Settings.m_ColorFadeEndPoint, 0f, 1f);
            m_Settings.m_FadeToSkyboxStartPoint = EditorGUILayout.Slider("Fade To Skybox Start Point", m_Settings.m_FadeToSkyboxStartPoint, 0f, 1f);

            EditorGUILayout.Space(5);

            using (new EditorGUILayout.VerticalScope("box"))
            {
                m_Settings.m_IsUseSkyboxBlur = EditorGUILayout.Toggle("Blur On Skybox", m_Settings.m_IsUseSkyboxBlur);

                using (new EditorGUI.DisabledScope(!m_Settings.m_IsUseSkyboxBlur))
                using (new EditorGUI.IndentLevelScope())
                {
                    int blurLevel = EditorGUILayout.Popup("Blur Quality", SbFadeToSkyboxFog.MaxDownsamplingLevel - m_Settings.m_BlurProcessLevel, k_BlurQualityNames);
                    m_Settings.m_BlurProcessLevel = SbFadeToSkyboxFog.MaxDownsamplingLevel - blurLevel;

                    m_Settings.m_BlurLevel = EditorGUILayout.IntSlider("Blur Level", m_Settings.m_BlurLevel, 1, 20);
                }
            }

            EditorGUILayout.Space(5);
            m_Settings.m_SkyboxClipFadeStartPoint = EditorGUILayout.Slider("FarClip Fade Start Point", m_Settings.m_SkyboxClipFadeStartPoint, 0f, 1f);

            EditorGUILayout.Space(5);
        }

        private void DrawVerticalSetting()
        {
            using (new EditorGUILayout.VerticalScope("box"))
            {
                m_Settings.m_IsUseVerticalFog = EditorGUILayout.Toggle("Height Fog", m_Settings.m_IsUseVerticalFog);

                using (new EditorGUI.DisabledScope(!m_Settings.m_IsUseVerticalFog))
                using (new EditorGUI.IndentLevelScope())
                {
                    m_Settings.m_VerticalFogPosition = (SbFadeToSkyboxFog.EVerticalFogPosition)EditorGUILayout.EnumPopup("Position", m_Settings.m_VerticalFogPosition);
                    m_Settings.m_VerticalFogType = (SbFadeToSkyboxFog.EVerticalFogType)EditorGUILayout.EnumPopup("Type", m_Settings.m_VerticalFogType);
                    m_Settings.m_VerticalFogCenterY = EditorGUILayout.FloatField("Center Y", m_Settings.m_VerticalFogCenterY);

                    if (m_Settings.m_VerticalFogType == SbFadeToSkyboxFog.EVerticalFogType.Linear)
                    {
                        m_Settings.m_VerticalFogHeight = EditorGUILayout.FloatField("Thickness", m_Settings.m_VerticalFogHeight);
                    }
                    else
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Fade Intensity", GUILayout.Width(EditorGUIUtility.labelWidth));
                            m_Settings.VerticalFogDensitySliderRate = GUILayout.HorizontalSlider(m_Settings.VerticalFogDensitySliderRate, 0f, 1f);
                            m_Settings.m_VerticalFogDensity = EditorGUILayout.FloatField(m_Settings.m_VerticalFogDensity, GUILayout.Width(60));
                        }
                    }
                }
            }

            EditorGUILayout.Space(5);
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
        {

        }

        public void OnInspectorGUI()
        {
            var cameraData = Camera.main.GetUniversalAdditionalCameraData();
            if (cameraData != null && cameraData.scriptableRenderer != m_CameraRenderer)
            {
                m_CameraRenderer = cameraData.scriptableRenderer;

                if (m_IsParentCurrentRenderer)
                {
                    m_IsParentCurrentRenderer = GetCurrentMainCameraSkyboxFog() == m_Parent;

                    if (!m_IsParentCurrentRenderer)
                        m_IsRendererChanged = true;
                }
                else
                {
                    m_IsParentCurrentRenderer = GetCurrentMainCameraSkyboxFog() == m_Parent;
                }
            }

            using (var checker = new EditorGUI.ChangeCheckScope())
            {
                DrawDispSetting();
                DrawFogBaseSetting();

                DrawBlendSetting();
                DrawVerticalSetting();

                m_IsChanged = checker.changed;
                if (m_IsChanged)
                {
                    Undo.RecordObject(m_Parent, "Change SbFadeToSkyboxFog seting");
                }
                if(m_IsRendererChanged)
                {
                    var urpAsset = GetRenderPipelineAsset();
                    var pipelineType = typeof(UniversalRenderPipelineAsset);
                    FieldInfo fieldInfo = pipelineType.GetField("m_DefaultRendererIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    int setIndex = GetCurrentMainCameraRendererIndex();
                    fieldInfo.SetValue(urpAsset, setIndex);
                }
                if (m_IsChanged || m_IsRendererChanged)
                {
                    m_IsRendererChanged = false;
                    OnSettingChanged();
                }
            }
        }

        void OnSettingChanged()
        {
            m_Settings.Validate();
            m_Parent.ApplySetting(m_Settings);

            m_Parent.Create();

            if (m_Parent == GetCurrentMainCameraSkyboxFog())
            {
                EditorApplication.QueuePlayerLoopUpdate();
                SceneView.RepaintAll();
            }
        }
    }
}
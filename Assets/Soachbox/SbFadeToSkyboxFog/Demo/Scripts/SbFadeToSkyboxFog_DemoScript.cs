using Soachbox;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

namespace Soachbox
{
    public class SbFadeToSkyboxFog_DemoScript : MonoBehaviour
    {
        [SerializeField]
        public UniversalRenderPipelineAsset m_DemoRenderPipeline;

        [SerializeField]
        public float m_RotateSpeed;

        private Camera m_Camera;
        private int m_CurrentRendererIndex = 0;

        private SbFadeToSkyboxFog m_CurrentFog = null;

        void Start()
        {
            m_Camera = GetComponent<Camera>();

            if (QualitySettings.renderPipeline is UniversalRenderPipelineAsset pipelineAsset)
            {
                if (pipelineAsset == m_DemoRenderPipeline)
                {
                    m_CurrentFog = GetCurrentFogSetting();
                    m_CurrentFog?.SetActive(true);
                }

                if (m_CurrentFog == null)
                {
                    Debug.LogError("Sample fog data not found.<br>First go to \"Edit > Project Settings > Quality\" and set \"Render Pipeline Asset\" to \"FadeToSkyboxDemoURPAsset\"");
                }
                else
                {
                    Debug.Log($"Current Fog is {m_CurrentFog?.name}");
                }
            }
        }

        void Update()
        {
            // rotate camera
            var cameraRot = m_Camera.transform.rotation;
            cameraRot *= Quaternion.AngleAxis(m_RotateSpeed * Time.deltaTime, Vector3.up);

            m_Camera.transform.rotation = cameraRot;
        }

        private SbFadeToSkyboxFog GetCurrentFogSetting()
        {
            if (QualitySettings.renderPipeline is UniversalRenderPipelineAsset pipelineAsset)
            {
                // find current renderer's SbFadeToSkyboxFog
                var rendererData = pipelineAsset.rendererDataList[m_CurrentRendererIndex];
                foreach (var rendererFeature in rendererData.rendererFeatures)
                {
                    if (rendererFeature is SbFadeToSkyboxFog skyboxFog)
                        return skyboxFog;
                }
            }
            return null;
        }

        public void OnChangeFogButton()
        {
            if (QualitySettings.renderPipeline is UniversalRenderPipelineAsset pipelineAsset)
            {
                var cameraData = m_Camera.GetUniversalAdditionalCameraData();
                if (cameraData != null)
                {
                    m_CurrentRendererIndex = (m_CurrentRendererIndex + 1) % pipelineAsset.renderers.Length;
                    cameraData.SetRenderer(m_CurrentRendererIndex);

                    m_CurrentFog = GetCurrentFogSetting();
                    m_CurrentFog?.SetActive(true);
                    Debug.Log($"Fog changed to {m_CurrentFog?.name}");
                }
            }
        }

        public void OnToggleFogButton()
        {
            m_CurrentFog?.SetActive(!m_CurrentFog.isActive);
        }
    }
}
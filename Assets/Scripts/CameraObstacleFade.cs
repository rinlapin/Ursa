
using System.Collections.Generic;
using UnityEngine;

public class CameraObstacleFade : MonoBehaviour
{
    [Header("References")]
    public Camera targetCamera;
    public Transform player;
    public Transform treesRoot;

    [Header("Fade Settings")]
    [Range(0f, 1f)]
    public float fadedAlpha = 0.25f;

    public float fadeSpeed = 5f;

    private static readonly int BaseColor =
        Shader.PropertyToID("_BaseColor");

    private static readonly int OldColor =
        Shader.PropertyToID("_Color");

    private class RendererData
    {
        public Renderer renderer;
        public MaterialPropertyBlock block;
        public Color originalColor;
        public int colorID;
        public float alpha = 1f;
    }

    private readonly Dictionary<Transform, List<RendererData>>
        treeRenderers = new();

    private readonly HashSet<Transform> blockedTrees = new();

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null || player == null || treesRoot == null)
        {
            Debug.LogError(
                "CameraObstacleFade: Assign the Camera, Player, " +
                "and Trees Root in the Inspector."
            );
            enabled = false;
            return;
        }

        // Each direct child of TreesRoot represents one whole tree.
        foreach (Transform tree in treesRoot)
        {
            List<RendererData> renderers = new();

            foreach (Renderer r in
                     tree.GetComponentsInChildren<Renderer>(true))
            {
                if (r.sharedMaterial == null)
                    continue;

                int colorID = r.sharedMaterial.HasProperty(BaseColor)
                    ? BaseColor
                    : OldColor;

                if (!r.sharedMaterial.HasProperty(colorID))
                    continue;

                renderers.Add(new RendererData
                {
                    renderer = r,
                    block = new MaterialPropertyBlock(),
                    originalColor = r.sharedMaterial.GetColor(colorID),
                    colorID = colorID
                });
            }

            treeRenderers[tree] = renderers;
        }
    }

    void LateUpdate()
    {
        if (targetCamera == null || player == null)
            return;

        blockedTrees.Clear();

        Vector3 origin = targetCamera.transform.position;
        Vector3 direction = player.position - origin;
        float distance = direction.magnitude;

        if (distance > 0.01f)
        {
            RaycastHit[] hits = Physics.RaycastAll(
                origin,
                direction.normalized,
                distance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Collide
            );

            foreach (RaycastHit hit in hits)
            {
                // Find which whole tree owns this collider.
                Transform current = hit.collider.transform;

                while (current != null &&
                       current.parent != treesRoot)
                {
                    current = current.parent;
                }

                if (current != null &&
                    current.parent == treesRoot &&
                    treeRenderers.ContainsKey(current))
                {
                    blockedTrees.Add(current);
                }
            }
        }

        foreach (var pair in treeRenderers)
        {
            float targetAlpha = blockedTrees.Contains(pair.Key)
                ? fadedAlpha
                : 1f;

            foreach (RendererData data in pair.Value)
            {
                data.alpha = Mathf.MoveTowards(
                    data.alpha,
                    targetAlpha,
                    fadeSpeed * Time.deltaTime
                );

                Color color = data.originalColor;
                color.a *= data.alpha;

                data.renderer.GetPropertyBlock(data.block);
                data.block.SetColor(data.colorID, color);
                data.renderer.SetPropertyBlock(data.block);
            }
        }
    }
}

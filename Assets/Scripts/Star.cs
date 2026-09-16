using UnityEngine;

public class Star : MonoBehaviour
{
    public Vector3 StartingPosition => GetComponent<RectTransform>().anchoredPosition;
    
    public Camera targetCamera;

    private void LateUpdate()
    {
        if (!targetCamera) return;

        transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward, targetCamera.transform.rotation * Vector3.up);
    }
}

using UnityEngine;

public class Star : MonoBehaviour
{
    public Vector3 StartingPosition => GetComponent<RectTransform>().anchoredPosition;
    
    private Camera _targetCamera;

    private void Awake()
    {
        _targetCamera = ConstellationFramework.instance.PuzzleCamera;
    }

    private void LateUpdate()
    {
        if (!_targetCamera) return;

        transform.LookAt(transform.position + _targetCamera.transform.rotation * Vector3.forward, _targetCamera.transform.rotation * Vector3.up);
    }
}

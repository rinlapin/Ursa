/*
using System;
using UnityEngine;

public class BillboardStar : MonoBehaviour
{
    private Camera _targetCamera;

    private void Awake()
    {
        _targetCamera = ConstellationFramework.instance.PuzzleCamera;
    }

    private void LateUpdate()
    {
        if (_targetCamera == null)
            return;

        transform.LookAt(transform.position + _targetCamera.transform.rotation * Vector3.forward, _targetCamera.transform.rotation * Vector3.up);
    }
}
*/

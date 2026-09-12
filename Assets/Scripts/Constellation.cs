using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Constellation : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private List<RectTransform> _points;
    
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        _points = new List<RectTransform>();
        var stars = GetComponentsInChildren<Star>();
        _lineRenderer.positionCount = stars.Length;

        foreach (var star in stars)
        {
            _points.Add(star.GetComponent<RectTransform>());
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < _points.Count; i++)
        {
            _lineRenderer.SetPosition(i, _points[i].transform.position);
        }
    }
}

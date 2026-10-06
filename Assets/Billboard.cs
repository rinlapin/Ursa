using System;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _camera;
    private LayerMask _layerMask;
    
    private void OnEnable()
    {
        _camera = Camera.main;
    }

    private void Awake()
    {
        _layerMask = LayerMask.GetMask("Terrain");
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 2f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.red);
            Debug.Log(hit.transform.gameObject.name);
            transform.rotation = Quaternion.Euler(hit.normal);
        }
    }
}

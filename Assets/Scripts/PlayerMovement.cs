using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5f;
    private Vector3 _move = Vector3.zero;
    private Rigidbody _rigidbody;
    private Camera _camera;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = Camera.main;
    }

    private void Update()
    {
        ConvertInput();
        ProcessMovement();
    }

    private void ProcessMovement()
    {
        _rigidbody.linearVelocity = _move * playerSpeed;
    }

    public void ConvertInput()
    {
        var inputDirection = GameManager.Instance.walkInput;
        
        GetCameraInputOffset(out var cameraForward, out var cameraRight);

        cameraForward *= inputDirection.y;
        cameraRight *= inputDirection.x;

        _move = cameraForward + cameraRight;
    }
    
    private void GetCameraInputOffset(out Vector3 forward, out Vector3 right)
    {
        forward = _camera.transform.forward;
        right = _camera.transform.right;
        
        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();
    }
}

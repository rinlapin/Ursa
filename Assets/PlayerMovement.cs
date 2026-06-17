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
        ProcessMovement();
    }

    private void ProcessMovement()
    {
        _rigidbody.linearVelocity = _move * playerSpeed;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
        
        if (Player.state is not PlayerState.Moving and not PlayerState.Idle) return;
        var inputDirection = context.ReadValue<Vector2>();
        
        Player.state = inputDirection != Vector2.zero ? PlayerState.Moving : PlayerState.Idle;
        
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

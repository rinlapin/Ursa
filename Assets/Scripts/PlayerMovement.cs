using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float groundDrag, airDrag;
    [SerializeField] private bool isGrounded = false;
    
    private LayerMask _groundLayer;
    private Vector3 _move = Vector3.zero;
    private Rigidbody _rigidbody;
    private Camera _camera;
    private CapsuleCollider _collider;

    public static PlayerMovement Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _groundLayer = LayerMask.GetMask("Terrain");
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _camera = Camera.main;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, _collider.height * 0.5f + 0.2f, _groundLayer);
        _rigidbody.linearDamping = isGrounded ? groundDrag : airDrag;
        
        ConvertInput();
        ProcessMovement();
    }

    private void ProcessMovement()
    {
        _rigidbody.AddForce(_move.normalized * movementSpeed, ForceMode.Force);
    }

    public void ConvertInput()
    {
        var inputDirection = GameManager.Instance.walkInput;
        
        GetCameraInputOffset(out var cameraForward, out var cameraRight);

        cameraForward *= inputDirection.y;
        cameraRight *= inputDirection.x;

        _move = cameraForward + cameraRight;
    }

    public void Jump()
    {
        if (!isGrounded) return;
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ConstellationFramework : MonoBehaviour
{
    public static ConstellationFramework instance;
    
    [SerializeField] private float rotationSpeed = 0.2f;
    [SerializeField] private float damping = 3f;

    private bool _isDraggingX, _isDraggingY;
    private bool _isOpen;
    private bool _isCloseToGoal;
    private Vector3 _previousMousePosition;
    private Vector2 _rotationVelocity;
    private Vector2 _input;

    public Canvas canvas;
    
    public Vector2 difference;

    private Quaternion _endRotation;

    private Transform _constellation;

    private ConstellationHint _hint;

    public Transform Constellation
    {
        set
        {
            if (_constellation == value) return;
            _constellation = value;
            
            _isOpen = value;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(value);
            }

            if (value)
            {
                _endRotation = value.localRotation;
                _hint.Setup(value);
                SetRandomRotation(value);
            }
        }

    }
    
    private Camera _puzzleCamera;
    
    public Camera PuzzleCamera => _puzzleCamera;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if  (instance != this)
        {
            Destroy(gameObject);
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        _puzzleCamera = GetComponentInChildren<Camera>(true);
        _hint = GetComponentInChildren<ConstellationHint>(true);
    }

    public void OnClickX(InputAction.CallbackContext context)
    {
        if (Player.state != PlayerState.Interacting) return;
        if (PuzzleManager.state == PuzzleState.Hinting) return;
        
        if (context.started)
        {
            PuzzleManager.state = PuzzleState.Rotating;
            _previousMousePosition.x = Input.mousePosition.x;
            _isDraggingX = true;
        }

        if (context.canceled)
        {
            PuzzleManager.state = PuzzleState.Idle;
            _isDraggingX = false;
            _previousMousePosition.x = Input.mousePosition.x;
        }
    }

    public void OnClickY(InputAction.CallbackContext context)
    {
        if (Player.state != PlayerState.Interacting) return;
        if (PuzzleManager.state == PuzzleState.Hinting) return;
        
        if (context.started)
        {
            PuzzleManager.state = PuzzleState.Rotating;
            _previousMousePosition.y = Input.mousePosition.y;
            _isDraggingY = true;
        }
        
        if (context.canceled)
        {
            PuzzleManager.state = PuzzleState.Idle;
            _isDraggingY = false;
            _previousMousePosition.y = Input.mousePosition.y;
        }
    }

    public void OnHint(InputAction.CallbackContext context)
    {
        if (Player.state != PlayerState.Interacting) return;
        if (PuzzleManager.state != PuzzleState.Idle) return;

        if (context.started)
        {
            _hint.Activate();
        }
    }

    private void Update()
    {
        if (!_isOpen) return;
        if (Player.state != PlayerState.Interacting)
            return;

        Vector3 delta = Vector3.zero;
        
        delta.x = _isDraggingX ? Input.mousePosition.x - _previousMousePosition.x : 0;
        delta.y = _isDraggingY ? Input.mousePosition.y - _previousMousePosition.y : 0;

        _rotationVelocity = new Vector2(
            delta.x * rotationSpeed,
            delta.y * rotationSpeed
        );

        Quaternion yaw =
            Quaternion.AngleAxis(
                -delta.x * rotationSpeed,
                _puzzleCamera.transform.up);

        Quaternion pitch =
            Quaternion.AngleAxis(
                delta.y * rotationSpeed,
                _puzzleCamera.transform.right);

        _constellation.transform.rotation =
            pitch *
            yaw *
            _constellation.transform.rotation;

        _rotationVelocity = Vector2.Lerp(
            _rotationVelocity,
            Vector2.zero,
            damping * Time.deltaTime);

        _previousMousePosition = Input.mousePosition;
        
        difference = new(Mathf.Abs(_constellation.rotation.eulerAngles.x - _endRotation.eulerAngles.x),
            Mathf.Abs(_constellation.rotation.eulerAngles.y - _endRotation.eulerAngles.y));

        if (difference.x >= 360)
        {
            difference.x -= 360;
        }

        if (difference.y >= 360)
        {
            difference.y -= 360;
        }
        
        _isCloseToGoal =
            Mathf.Abs(_constellation.rotation.eulerAngles.x - _endRotation.eulerAngles.x) < 10 &&
            Mathf.Abs(_constellation.rotation.eulerAngles.y - _endRotation.eulerAngles.y) < 10;
    }

    private void SetRandomRotation(Transform target)
    {
        if (!target) return;
        
        target.Rotate(Random.Range(45, 180), Random.Range(45, 180), 0);
    }
}
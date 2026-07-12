using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ConstellationFramework : MonoBehaviour
{
    public static ConstellationFramework instance;
    
    [SerializeField] private float rotationSpeed = 1f;
    //[SerializeField] private float damping = 3f;
    
    //private bool _isOpen;
    private bool _isCloseToGoal;
    private Vector3 _previousMousePosition;
    private Vector2 _rotationVelocity;

    public float graceWindow = 2f;

    public Canvas canvas;
    
    public Vector2 difference;

    private Quaternion _endRotation;

    private Transform _constellation;

    private ConstellationHint _hint;
    
    private Vector3 _input;
    
    public Vector3 Input
    {
        get => _input;
        set
        {
            _input = value;
            PuzzleManager.state = _input == Vector3.zero ? PuzzleState.Idle : PuzzleState.Rotating;
        }
    }
    
    public Transform Constellation
    {
        private get => _constellation;
        
        set
        {
            if (_constellation == value) return;
            _constellation = value;
            
            //_isOpen = value;
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

    public void OnRotateX(InputAction.CallbackContext context)
    {
        if (PuzzleManager.state == PuzzleState.Hinting) return;
        Input = new Vector3 (context.ReadValue<float>(), Input.y, Input.z);
    }
    
    public void OnRotateY(InputAction.CallbackContext context)
    {
        if (PuzzleManager.state == PuzzleState.Hinting) return;
        Input = new Vector3 (Input.x, context.ReadValue<float>(), Input.z);
    }
    
    public void OnRotateZ(InputAction.CallbackContext context)
    {
        if (PuzzleManager.state == PuzzleState.Hinting) return;
        Input = new Vector3 (Input.x, Input.y, context.ReadValue<float>());
    }

    public void Update()
    {
        if (Input == Vector3.zero) return;
        if (!Constellation) return;
        
        Quaternion yaw =
            Quaternion.AngleAxis(
                -Input.x * rotationSpeed,
                _puzzleCamera.transform.up);

        Quaternion pitch =
            Quaternion.AngleAxis(
                Input.y * rotationSpeed,
                _puzzleCamera.transform.right);
        
        Quaternion roll =
            Quaternion.AngleAxis(
                -Input.z * rotationSpeed,
                _puzzleCamera.transform.forward);
        
        _constellation.transform.rotation =
            pitch *
            yaw *
            roll *
            _constellation.transform.rotation;
    }


    /*public void OnClickX(InputAction.CallbackContext context)
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
    }*/

    public void OnHint(InputAction.CallbackContext context)
    {
        if (Player.state != PlayerState.Interacting) return;
        if (PuzzleManager.state != PuzzleState.Idle) return;

        if (!context.started) return;

        _hint.Check();
    }

    /*private void Update()
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
    }*/

    private void SetRandomRotation(Transform target)
    {
        if (!target) return;
        
        target.Rotate(Random.Range(45, 180), Random.Range(45, 180), 0);
    }
}
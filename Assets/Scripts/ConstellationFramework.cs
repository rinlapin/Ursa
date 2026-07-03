using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConstellationFramework : MonoBehaviour
{
    public static ConstellationFramework instance;
    
    [SerializeField] private float rotationSpeed = 0.2f;
    [SerializeField] private float damping = 3f;

    private bool _isDragging;
    private bool _isOpen;
    private bool _isCloseToGoal;
    private Vector3 _previousMousePosition;
    private Vector2 _rotationVelocity;

    public Vector2 difference;

    private Quaternion _endRotation;

    private Transform _constellation;

    public Transform Constellation
    {
        set
        {
            if (_constellation == value) return;
            _constellation = value;
            _endRotation = value.localRotation;
            
            _isOpen = value;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(value);
            }
            
            SetRandomRotation(value);
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
    }

    private void Start()
    {
        _puzzleCamera = GetComponentInChildren<Camera>(true);
    }

    private void Update()
    {
        if (!_isOpen) return;
        if (Player.state != PlayerState.Interacting)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _previousMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }

        if (!_isDragging) return;

        Vector3 delta = Input.mousePosition - _previousMousePosition;

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
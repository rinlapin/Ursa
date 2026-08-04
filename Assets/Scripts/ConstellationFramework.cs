using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ConstellationFramework : MonoBehaviour
{
    public static ConstellationFramework instance;
    
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private GameObject pivot;
    [SerializeField] private GameObject starPrefab;
    
    private bool _isCloseToGoal;
    private Vector3 _previousMousePosition;
    private Vector2 _rotationVelocity;

    public float graceWindow = 2f;

    public Canvas canvas;

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
    
    public ConstellationData Data { get; private set; }
    
    /*public Transform Constellation
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
                _hint.Setup(value);
                SetRandomRotation(value);
            }
        }

    }*/
    
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
        if (!Data)
        {
            return;
        }
        
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
        
        pivot.transform.rotation =
            pitch *
            yaw *
            roll *
            pivot.transform.rotation;
    }

    public void OnHint(InputAction.CallbackContext context)
    {
        if (Player.state != PlayerState.Interacting) return;
        if (PuzzleManager.state != PuzzleState.Idle) return;

        if (!context.started) return;

        _hint.Check();
    }

    private void SetRandomRotation(Transform target)
    {
        if (!target) return;
        
        target.Rotate(Random.Range(45, 180), Random.Range(45, 180), 0);
    }

    public void Setup(ConstellationData activeData)
    {
        Data = activeData;
        
        foreach (Star star in pivot.GetComponentsInChildren<Star>())
        {
            Destroy(star.gameObject);
        }

        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(true);
        }

        foreach (Vector3 position in activeData.positions)
        {
            var newStar = Instantiate(starPrefab, pivot.transform);
            newStar.GetComponent<RectTransform>().anchoredPosition3D = position;
        }

        _hint.Setup(pivot.transform);
        SetRandomRotation(pivot.transform);
    }
}
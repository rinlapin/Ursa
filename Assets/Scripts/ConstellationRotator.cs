using UnityEngine;

public class ConstellationRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.2f;
    [SerializeField] private float damping = 3f;

    private bool dragging;
    private Vector3 previousMousePosition;
    private Vector2 rotationVelocity;
    
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Player.state != PlayerState.Interacting)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            dragging = true;
            previousMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

        if (!dragging) return;

        Vector3 delta = Input.mousePosition - previousMousePosition;

        rotationVelocity = new Vector2(
            delta.x * rotationSpeed,
            delta.y * rotationSpeed
        );

        Quaternion yaw =
            Quaternion.AngleAxis(
                -delta.x * rotationSpeed,
                mainCamera.transform.up);

        Quaternion pitch =
            Quaternion.AngleAxis(
                delta.y * rotationSpeed,
                mainCamera.transform.right);

        transform.rotation =
            pitch *
            yaw *
            transform.rotation;

        rotationVelocity = Vector2.Lerp(
            rotationVelocity,
            Vector2.zero,
            damping * Time.deltaTime);

        previousMousePosition = Input.mousePosition;
    }
}
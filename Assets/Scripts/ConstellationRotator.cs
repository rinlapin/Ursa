using UnityEngine;

public class ConstellationRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.2f;
    [SerializeField] private float damping = 3f;

    private bool isDragging;
    private Vector3 previousMousePosition;
    private Vector2 rotationVelocity;
    
    [SerializeField] private Camera puzzleCamera;

    private void Update()
    {
        if (Player.state != PlayerState.Interacting)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            previousMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (!isDragging) return;

        Vector3 delta = Input.mousePosition - previousMousePosition;

        rotationVelocity = new Vector2(
            delta.x * rotationSpeed,
            delta.y * rotationSpeed
        );

        Quaternion yaw =
            Quaternion.AngleAxis(
                -delta.x * rotationSpeed,
                puzzleCamera.transform.up);

        Quaternion pitch =
            Quaternion.AngleAxis(
                delta.y * rotationSpeed,
                puzzleCamera.transform.right);

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
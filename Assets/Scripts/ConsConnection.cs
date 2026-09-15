using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class Cons : MonoBehaviour
{
    public Transform starA;
    public Transform starB;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.positionCount = 2;
    }

    private void LateUpdate()
    {
        line.SetPosition(0, starA.position);
        line.SetPosition(1, starB.position);
  
    }
}

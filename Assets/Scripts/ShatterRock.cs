using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Break : MonoBehaviour
{
    public GameObject fractured;
    public float breakForce;
    public bool timeToBreak = false;

    private void Update()
    {
        if (timeToBreak == true)
        {
            BreakTheThing();
        }
    }

    public void BreakTheThing()
    {
        GameObject frac = Instantiate(fractured, transform.position, transform.rotation);

        foreach (Rigidbody rb in frac.GetComponentsInChildren<Rigidbody>())
        {
            Vector3 force = (rb.transform.position - transform.position).normalized * breakForce;
            rb.AddForce(force);
            rb.useGravity = true;
            rb.AddForce(Vector3.down * 10f, ForceMode.Acceleration);
        }

        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : MonoBehaviour
{
    public GameObject fractured;
    public float breakForce;

    private void OnCollisionEnter(Collision collision)
    {
        BreakTheThing();
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

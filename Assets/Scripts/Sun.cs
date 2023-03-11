using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void SetKinematicOff()
    {
        rb.isKinematic = false;
    }
    public void SetKinematicOn()
    {
        rb.isKinematic = true;
    }
}

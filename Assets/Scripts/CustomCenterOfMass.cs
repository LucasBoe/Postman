using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCenterOfMass : MonoBehaviour
{
    public Vector3 customCenterOfMass;
    private void Start()
    {
        Debug.Log(transform.rotation);
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
            rigidbody.centerOfMass = customCenterOfMass;
    }
}

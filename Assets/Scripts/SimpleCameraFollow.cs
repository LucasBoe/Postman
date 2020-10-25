using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;
    public bool smooth = true;
    public float smoothAmount = 0.2f;

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = target.position + target.forward * 3 + Vector3.up * 2.5f;

        // Smoothly move the camera towards that target position
        if (smooth)
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothAmount);
        else
            transform.position = targetPosition;

        transform.forward = (target.position + Vector3.up * 1.5f) - transform.position;
    }
}

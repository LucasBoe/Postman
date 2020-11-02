using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum newspaperThrowState
{
    NONE,
    AIM,
    THROW,
    FETCHNEW
}

public class SimpleNewspaperThrower : MonoBehaviour
{
    int newspapersLeft;
    public newspaperThrowState state;

    [SerializeField] GameObject newspaperPrefab;
    [SerializeField] float throwForceMultiplier = 100;

    private void Update()
    {
        switch (state)
        {
            case newspaperThrowState.NONE:
                if (Input.GetMouseButtonDown(1))
                {
                    state = newspaperThrowState.AIM; 
                }
                break;

            case newspaperThrowState.AIM:
                if (Input.GetMouseButtonUp(1))
                {
                    state = newspaperThrowState.NONE;
                } else if (Input.GetMouseButtonDown(0))
                {
                    Rigidbody rigidbody = Instantiate(newspaperPrefab, transform.position + (transform.right + transform.up) * 0.5f, Quaternion.identity).GetComponent<Rigidbody>();
                    rigidbody.AddForce((CalculateThrowTarget() - transform.position).normalized * throwForceMultiplier);
                }
                break;
        }
    }

    public Vector3 CalculateThrowTarget()
    {
        float xMouse = RemapMouse(Input.mousePosition.x, Screen.width);
        float yMouse = RemapMouse(Input.mousePosition.y, Screen.height);

        Vector3 targetBase = transform.position + transform.right * 8 * xMouse + transform.forward * 4;
        Vector3 targetOffset = transform.up * 2 +  transform.up * yMouse;

        return targetBase + targetOffset;
    }

    private float RemapMouse(float input, float max)
    {
        return ((Mathf.Clamp(input, 0, max) / max) - 0.5f) * 2;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, CalculateThrowTarget());
    }
}

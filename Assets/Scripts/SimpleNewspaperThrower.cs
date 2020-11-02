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
    public int newspapersLeft;
    public newspaperThrowState state;

    [SerializeField] GameObject newspaperPrefab;
    [SerializeField] float throwForceMultiplier = 100;
    [SerializeField] GameObject throwVisulization;

    private void Update()
    {
        switch (state)
        {
            case newspaperThrowState.NONE:
                if (Input.GetMouseButtonDown(1))
                {
                    state = newspaperThrowState.AIM;
                    throwVisulization.SetActive(true);
                }
                break;

            case newspaperThrowState.AIM:
                Vector3 throwDirection =(CalculateThrowTarget() - transform.position).normalized;

                throwVisulization.transform.LookAt(transform.position + Vector3.up + throwDirection);

                if (Input.GetMouseButtonUp(1))
                {
                    state = newspaperThrowState.NONE;
                    throwVisulization.SetActive(false);
                } else if (Input.GetMouseButtonDown(0))
                {
                    Throw(throwDirection);
                }
                break;
        }
    }

    public void Throw(Vector3 throwDirection)
    {
        if (newspapersLeft <= 0)
            return;

        Rigidbody rigidbody = Instantiate(newspaperPrefab, transform.position + Vector3.up * 0.8f + (throwDirection)* 0.66f, Quaternion.LookRotation(throwDirection)).GetComponent<Rigidbody>();
        rigidbody.AddForce(throwDirection * throwForceMultiplier);
        newspapersLeft -= 1;
    }

    public Vector3 CalculateThrowTarget()
    {
        float xMouse = RemapMouse(Input.mousePosition.x, Screen.width);
        float yMouse = RemapMouse(Input.mousePosition.y, Screen.height);

        Vector3 targetBase = (transform.position + transform.up) + transform.right * 8 * xMouse + transform.forward * 4;
        Vector3 targetOffset = transform.up +  transform.up * yMouse;

        return targetBase + targetOffset;
    }

    private float RemapMouse(float input, float max)
    {
        return ((Mathf.Clamp(input, 0, max) / max) - 0.5f) * 2;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine((transform.position), CalculateThrowTarget());
    }
}

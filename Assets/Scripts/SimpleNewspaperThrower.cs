using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

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
    [SerializeField] Rig handThrowRig;
    [SerializeField] Transform throwTarget;
    [SerializeField] GameObject newspaperInHand;

    [Header("Line Visualization")]
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float maxTime;
    [SerializeField] int sampleAmount;

    float handBlendAmount = 0;
    float throwForcePercent = 0;
    private void Update()
    {
        switch (state)
        {
            case newspaperThrowState.NONE:
                handBlendAmount = Mathf.Clamp(handBlendAmount - Time.deltaTime * 3, 0, 1);
                throwForcePercent = 0;
                if (Input.GetMouseButton(1))
                {
                    state = newspaperThrowState.AIM;
                    newspaperInHand.SetActive(true);
                }
                break;

            case newspaperThrowState.AIM:
                Vector3 throwDirection = GetThrowVelocity().normalized;

                throwTarget.position = transform.position + (throwDirection / 1.5f);
                throwTarget.rotation = Quaternion.LookRotation(Vector3.up * 0.5f, throwDirection);
                handBlendAmount = -0.001f;
                handThrowRig.weight = 0.66f;
                throwForcePercent = Mathf.Clamp01(throwForcePercent + Time.deltaTime);

                if (Input.GetMouseButtonUp(1))
                {
                    state = newspaperThrowState.NONE;
                    newspaperInHand.SetActive(false);
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(Throw());
                }
                break;
        }

        if (handBlendAmount >= 0)
            handThrowRig.weight = handBlendAmount;

        UpdateThrowVisuals();
    }

    IEnumerator Throw()
    {
        var throwDirection = GetThrowVelocity().normalized;
        state = newspaperThrowState.THROW;
        throwTarget.position = transform.position + (transform.up + throwDirection);

        for (float i = 0.25f; i < 0.66; i += 3 * Time.deltaTime)
        {
            handBlendAmount += 3 * Time.deltaTime;
            yield return null;
        }

        if (newspapersLeft > 0)
        {
            Rigidbody rigidbody = Instantiate(newspaperPrefab, GetPaperSpawnPosition(), Quaternion.LookRotation(throwDirection)).GetComponent<Rigidbody>();
            rigidbody.AddForce(GetThrowVelocity(), ForceMode.VelocityChange);
            newspapersLeft -= 1;
            newspaperInHand.SetActive(false);

            for (float i = 0; i < 1; i += 3 * Time.deltaTime)
            {
                handBlendAmount += 3 * Time.deltaTime;
                yield return null;
            }
        }
        state = newspaperThrowState.NONE;
    }

    private void UpdateThrowVisuals()
    {
        if (lineRenderer == null)
            return;

        if(state == newspaperThrowState.AIM)
        {
            lineRenderer.positionCount = sampleAmount;
            Vector3[] locations = new Vector3[sampleAmount];

            float singleStep = maxTime / sampleAmount;

            for (int i = 0; i < sampleAmount; i++)
            {
                locations[i] = CalculateThrowPointAt(singleStep * i, GetThrowVelocity(), GetPaperSpawnPosition());
            }
            lineRenderer.SetPositions(locations);

            Gradient gradient = lineRenderer.colorGradient;
            gradient.alphaKeys = new GradientAlphaKey[2] { new GradientAlphaKey(throwForcePercent, 0), new GradientAlphaKey(0, 1) };
            lineRenderer.colorGradient = gradient;
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    private Vector3 GetThrowVelocity()
    {
        return (CalculateThrowTarget() - transform.position).normalized * throwForceMultiplier * throwForcePercent;
    }

    private Vector3 GetPaperSpawnPosition()
    {
        return transform.position + Vector3.up * 0.8f + (GetThrowVelocity().normalized) * 0.66f;
    }

    public Vector3 CalculateThrowPointAt(float t, Vector3 startVelocity, Vector3 startPosition)
    {
        Vector3 output = 0.5f * Physics.gravity * t * t + startVelocity * t + startPosition;
        return output;
    }

    public Vector3 CalculateThrowTarget()
    {
        float xMouse = RemapMouse(Input.mousePosition.x, Screen.width);
        float yMouse = RemapMouse(Input.mousePosition.y, Screen.height);

        Vector3 targetBase = (transform.position + transform.up) + transform.right * 8 * xMouse + transform.forward * 4;
        Vector3 targetOffset = transform.up + transform.up * yMouse;

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

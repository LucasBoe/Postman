﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;

    public float positionSmoothAmount = 0.2f;
    public float rotationSmoothAmount = 0.2f;

    public float CameraLookAtUpOffsetMultiplier = 1.5f;
    public float CameraLookAtForwardOffsetMultiplier = 2;

    [SerializeField] SimpleNewspaperThrower newspaperThrower;

    private Vector3 positionBefore;
    private Vector3 forwardBefore;

    private void Start()
    {
        positionBefore = transform.position;
    }

    void FixedUpdate()
    {
        //Define a target position
        Vector3 targetCameraPosition = target.position - target.forward * 3 + Vector3.up;

        //Smoothly move the camera towards that target position
        transform.position = Vector3.Lerp(positionBefore, targetCameraPosition, positionSmoothAmount);

        //Define a terget look at
        Vector3 targetCameraLookAt = target.position + Vector3.up * CameraLookAtUpOffsetMultiplier + target.forward * CameraLookAtForwardOffsetMultiplier;
        
        if (newspaperThrower != null && newspaperThrower.state != newspaperThrowState.NONE)
        {
            targetCameraLookAt = Vector3.Lerp(newspaperThrower.CalculateThrowTarget(), targetCameraLookAt, 0.5f);
        }

        //Lerp to the new calculated vector
        transform.forward = Vector3.Lerp(forwardBefore, (targetCameraLookAt - transform.position), rotationSmoothAmount);

        //store current psotion and rotation by forward vector
        positionBefore = transform.position;
        forwardBefore = transform.forward;
    }
}

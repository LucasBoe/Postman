using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine.Animations.Rigging;
using System;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
    public bool canBreak;
}

public class NewBikeController : MonoBehaviour
{
    [SerializeField] List<AxleInfo> axleInfos;

    [SerializeField] Transform visualBackWheel, visualFrontWheel, poleFront;

    [SerializeField] Animator animator;

    [Header("Settings")]
    [SerializeField] float maxMotorTorque;
    [SerializeField] float maxSteeringAngle;
    [SerializeField] float maxBreakTorque;
    [SerializeField] float animationSpeedMultiplier = 0.25f;

    [OnValueChanged("OnStabilityChanged")]
    [Range(0, 0.4f)] [SerializeField] float wheelWidth = 0.15f;

    [OnValueChanged("OnCenterOfMassOffsetChanged")]
    [SerializeField] Vector3 centerOfMassOffset;

    [Tooltip("0 = ground, 2xradius = top. Should be around center of mass.")]
    [OnValueChanged("OnWheelsApplyForceHeightChanged")]
    [SerializeField] float wheelsApplyForceHeight = 0.45f;

    new Rigidbody rigidbody;

    private bool inCorrection;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        UpdateWheels();

        ApplyAxleToVisuals(axleInfos[0], visualFrontWheel);
        ApplyAxleToVisuals(axleInfos[1], visualBackWheel);
        poleFront.localRotation = Quaternion.AngleAxis(axleInfos[0].leftWheel.steerAngle, Vector3.up);

        animator.speed = animationSpeedMultiplier * rigidbody.velocity.WithYZero().magnitude;

        UpdateRotationFromBody();

        CheckForCorrection();
    }

    private void CheckForCorrection()
    {
        if (Input.GetButtonDown("Upright") && !inCorrection)
        {
            StartCoroutine(CorrectionRoutine());
        }
    }

    private IEnumerator CorrectionRoutine()
    {
        inCorrection = true;
        Vector3 prevOffset = centerOfMassOffset;
        float prevWidth = wheelWidth;

        centerOfMassOffset = new Vector3(0, -2, 0);
        wheelWidth = 0.5f;
        OnStabilityChanged();
        OnCenterOfMassOffsetChanged();

        yield return new WaitForSeconds(1f);

        centerOfMassOffset = prevOffset;
        wheelWidth = prevWidth;
        OnStabilityChanged();
        OnCenterOfMassOffsetChanged();

        inCorrection = false;
    }

    private void UpdateRotationFromBody()
    {
        float sideSpeed = Vector3.Dot(rigidbody.velocity, transform.right);
        float forwardSpeed = Vector3.Dot(rigidbody.velocity, transform.forward);
        //todo bend forward and to sides
    }

    public void ApplyAxleToVisuals(AxleInfo axle, Transform visual)
    {

        axle.leftWheel.GetWorldPose(out Vector3 pLeft, out Quaternion rotLeft);
        axle.rightWheel.GetWorldPose(out Vector3 pRight, out Quaternion rotRight);

        visual.position = Vector3.Lerp(pLeft, pRight, 0.5f);
        visual.rotation = Quaternion.Lerp(rotLeft, rotRight, 0.5f);
    }

    private void UpdateWheels()
    {

        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        float breaks = maxBreakTorque * Input.GetAxis("Breaks");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            if (axleInfo.canBreak)
            {
                axleInfo.leftWheel.brakeTorque = breaks;
                axleInfo.rightWheel.brakeTorque = breaks;
            }
        }
    }

    private void OnStabilityChanged()
    {
        foreach (var axle in axleInfos)
        {
            Vector3 pos = axle.leftWheel.transform.localPosition;

            axle.leftWheel.transform.localPosition = new Vector3(-wheelWidth, pos.y, pos.z);
            axle.rightWheel.transform.localPosition = new Vector3(wheelWidth, pos.y, pos.z);
        }
    }

    private void OnCenterOfMassOffsetChanged()
    {
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();

        rigidbody.ResetCenterOfMass();
        rigidbody.centerOfMass += centerOfMassOffset;
    }

    private void OnWheelsApplyForceHeightChanged()
    {
        foreach (var axle in axleInfos)
        {
            Vector3 pos = axle.leftWheel.transform.localPosition;

            axle.leftWheel.forceAppPointDistance = wheelsApplyForceHeight;
            axle.rightWheel.forceAppPointDistance = wheelsApplyForceHeight;
        }
    }

    private Vector3 CalculateSideVelocity()
    {
       return transform.right * Vector3.Dot(transform.right, rigidbody.velocity);
    }

    private void OnDrawGizmos()
    {
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + transform.rotation * rigidbody.centerOfMass, Vector3.one * 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + rigidbody.velocity);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + CalculateSideVelocity());
    }

}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;


[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class NewBikeController : MonoBehaviour
{
    [SerializeField] List<AxleInfo> axleInfos;
    [SerializeField] float maxMotorTorque;
    [SerializeField] float maxSteeringAngle;

    [SerializeField] Transform visualBackWheel, visualFrontWheel, poleFront;

    [SerializeField] Animator animator;
    [SerializeField] float animationSpeedMultiplier = 0.25f;

    [NaughtyAttributes.OnValueChanged("OnStabilityChanged")]
    [Range(0, 0.4f)][SerializeField] float stability = 0.15f;

    new Rigidbody rigidbody;

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
        }
    }

    private void OnStabilityChanged()
    {
        foreach (var axle in axleInfos)
        {
            Vector3 pos = axle.leftWheel.transform.localPosition;

            axle.leftWheel.transform.localPosition = new Vector3(-stability, pos.y, pos.z);
            axle.rightWheel.transform.localPosition = new Vector3(stability, pos.y, pos.z);
        }
    }
}
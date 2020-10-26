using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;

public class SimpleBicycleController : MonoBehaviour
{
    public Transform poleFront;
    public Rigidbody myRigidbody;
    public Animator myAnimator;

    public Transform spineForward;
    public float leanAmount = 0.8f;
    public float animationSpeedMultiplier = 0.25f;

    public float speed;
    public float steeringSpeed;
    public float maxSpeed;
    public AnimationCurve acceleration;

    float steeringAmount = 0;

    private void Update()
    {
        if (myRigidbody != null)
        {
            UpdateSpeedAndRotation2();

            float vel = myRigidbody.velocity.magnitude;
            steeringAmount += Input.GetAxis("Horizontal") * Time.deltaTime * steeringSpeed;
            steeringAmount /= (1 + Time.deltaTime);

            poleFront.localRotation = Quaternion.Euler(0, steeringAmount, 0);
            spineForward.localPosition = Vector3.up * 2 + Vector3.back * vel * leanAmount;

            if (myAnimator != null)
                myAnimator.speed = animationSpeedMultiplier * vel;
        }
    }

    private void UpdateSpeedAndRotation()
    {
        float vel = myRigidbody.velocity.magnitude;
        myRigidbody.velocity = vel * GetSteeringVector(addOwnRotation: true);

        myRigidbody.AddForce(GetSteeringVector() * Time.deltaTime * Input.GetAxis("Vertical") * speed * GetAccelorationFactor());

        float rotation = transform.rotation.eulerAngles.y + steeringAmount * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, rotation, steeringAmount * 0.1f);
    }

    private void UpdateSpeedAndRotation2()
    {
        float vel = myRigidbody.velocity.magnitude;
        float input = Input.GetAxis("Vertical");
        myRigidbody.AddForce(GetSteeringVector() * Time.deltaTime * input * speed * GetAccelorationFactor());

        //myRigidbody.velocity = vel * GetSteeringVector(addOwnRotation: true);
        //float rotation = transform.rotation.eulerAngles.y + steeringAmount * Time.deltaTime;
        //transform.rotation = Quaternion.Euler(0, rotation, steeringAmount * 0.1f);

        Vector3 forward = new Vector3(-myRigidbody.velocity.x, 0, -myRigidbody.velocity.z);

  
            transform.forward = Vector3.Lerp(transform.forward, forward, vel*Time.deltaTime);
    }

    private Vector3 GetSteeringVector(bool addOwnRotation = false)
    {
        if (poleFront != null)
        {
            Vector3 steering = new Vector3(poleFront.forward.x, 0, poleFront.forward.z).normalized;
            Vector3 forward = transform.forward.normalized;

            if (addOwnRotation)
            {
                return ((steering + forward) / 2).normalized * -1;
            }
            else
            {
                return (steering) * -1;
            }
        }

        return Vector3.forward;
    }

    private float GetAccelorationFactor()
    {
        if (myRigidbody != null && acceleration != null)
        {
            return acceleration.Evaluate(myRigidbody.velocity.magnitude / maxSpeed);
        }

        return 1;
    }

    private void OnGUI()
    {
        string str = "";
        str += myRigidbody.velocity.magnitude.ToString() + "\n";

        if (myRigidbody != null && acceleration != null)
        {
            float positionOnCurve = myRigidbody.velocity.magnitude / maxSpeed;
            str += "pos: " + positionOnCurve + " acel: " + acceleration.Evaluate(myRigidbody.velocity.magnitude / maxSpeed);
        }

        GUI.TextArea(new Rect(10, 10, 100, 100), str);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + GetSteeringVector(), 0.1f);
    }
}

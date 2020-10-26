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

    public Transform spineForward;
    public float leanAmount = 0.8f;

    public float speed;
    public float steeringSpeed;
    public float maxSpeed;
    public AnimationCurve curve;

    float steeringAmount = 0;

    private void Update()
    {
        if (myRigidbody != null)
        {
            float vel = myRigidbody.velocity.magnitude;
            myRigidbody.velocity = vel * GetSteeringVector(addOwnRotation: true);

            steeringAmount += Input.GetAxis("Horizontal") * Time.deltaTime * steeringSpeed;
            poleFront.localRotation = Quaternion.Euler(0, steeringAmount, 0);

            myRigidbody.AddForce(GetSteeringVector() * Time.deltaTime * Input.GetAxis("Vertical") * speed * GetAccelorationFactor());

            float rotation = transform.rotation.eulerAngles.y + steeringAmount * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, rotation, steeringAmount * 0.1f);
            steeringAmount /= (1 + Time.deltaTime);

            spineForward.localPosition = Vector3.up * 2 + Vector3.back * vel * leanAmount;
        }
    }
    private Vector3 GetSteeringVector (bool addOwnRotation = false)
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

    private float GetAccelorationFactor ()
    {
        if (myRigidbody != null && curve != null)
        {
            return curve.Evaluate(myRigidbody.velocity.magnitude / maxSpeed);
        }

        return 1;
    }

    private void OnGUI()
    {
        string str = "";
        str += myRigidbody.velocity.magnitude.ToString() + "\n";

        if (myRigidbody != null && curve != null)
        {
            float positionOnCurve = myRigidbody.velocity.magnitude / maxSpeed;
            str += "pos: " + positionOnCurve + " acel: " + curve.Evaluate(myRigidbody.velocity.magnitude / maxSpeed);
        }

        GUI.TextArea(new Rect(10,10,100,100),str);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + GetSteeringVector(),0.1f);
    }
}

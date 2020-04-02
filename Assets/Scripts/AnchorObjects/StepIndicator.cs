using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Microsoft.MixedReality.Toolkit.Utilities;

public class StepIndicator : MonoBehaviour
{
    public float stability = 0.3f;
    public float speed = 2.0f;
    Rigidbody rigidBody;

    public GameObject target;
    public GameObject indicator;

    void Start()
    {
        rigidBody = indicator.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        /*
        Vector3 predictedUp = Quaternion.AngleAxis(
            rigidBody.angularVelocity.magnitude * Mathf.Rad2Deg * stability / speed,
            rigidBody.angularVelocity
        ) * transform.up;

        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        torqueVector = Vector3.Project(torqueVector, transform.forward);
        rigidBody.AddTorque(torqueVector * speed * speed);*/
        KeepFacingTarget();
    }


    private void KeepFacingTarget()
    {
        if (target != null)
        {
            Vector3 direction = target.transform.position - indicator.transform.position;
            indicator.transform.rotation = Quaternion.Slerp(indicator.transform.rotation, Quaternion.LookRotation(direction), speed * Time.deltaTime);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.SpatialAwareness
{
    public class RotateTest : MonoBehaviour
    {
        public Transform parent, target;
        float progress;
        public float rotationSpeed = 0.5f, xAngle = 15, yAngle = 30, zAngle = 12.5f, limit = 0.1f;

        public GameObject[] axesRotatePoints = new GameObject[3];
        public float intX, intY, intZ;
        GameObject pointToRotateFrom;

        Quaternion q, qx, qy, qz;
        void Start()
        {
            q = parent.rotation;

            Vector3 vectAngles = q.eulerAngles;
            xAngle = intX = -vectAngles.x;
            yAngle = intY = -vectAngles.y;
            zAngle = intZ = -vectAngles.z;

            vectAngles += new Vector3(intX, intY, intZ);
            q = Quaternion.Euler(vectAngles);
        }

        void CheckAndSet()
        {
            if (Mathf.Abs(xAngle - intX) > limit || Mathf.Abs(yAngle - intY) > limit || Mathf.Abs(zAngle - intZ) > limit)
            {
                intX = xAngle;
                intY = yAngle;
                intZ = zAngle;
                q = Quaternion.Euler(new Vector3(intX, intY, intZ));
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (target == null)
                return;
            Vector3 dir = target.position - parent.position;

            Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            if (Mathf.Abs(parent.transform.rotation.eulerAngles.x - targetRotation.eulerAngles.x) > 0
               ||
               Mathf.Abs(parent.transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y) > limit
               ||
               Mathf.Abs(parent.transform.rotation.eulerAngles.z - targetRotation.eulerAngles.z) > limit
               )
                parent.transform.rotation = Quaternion.RotateTowards(parent.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            //parent.rotation = targetRotation;

            return;
            //progress += rotationSpeed * Time.deltaTime;
            //Vector3 dirToFace = target.transform.position - parent.transform.position;
            CheckAndSet();
            if (Mathf.Abs(parent.transform.rotation.eulerAngles.x - q.eulerAngles.x) > 0 
                || 
                Mathf.Abs(parent.transform.rotation.eulerAngles.y - q.eulerAngles.y) > limit
                ||
                Mathf.Abs(parent.transform.rotation.eulerAngles.z - q.eulerAngles.z) > limit
                )
            {
                Debug.Log($"X: {parent.transform.rotation.eulerAngles.x - q.eulerAngles.x}");
                Debug.Log($"y: {parent.transform.rotation.eulerAngles.y - q.eulerAngles.y}");
                Debug.Log($"z: {parent.transform.rotation.eulerAngles.y - q.eulerAngles.z}");

                parent.transform.rotation = Quaternion.RotateTowards(parent.transform.rotation, q, rotationSpeed * Time.deltaTime);

            }
            /*else
            {
                Vector3 dir = target.position - parent.position;

                Quaternion targetRotation = Quaternion.LookRotation(-dir, Vector3.up);
                parent.rotation = targetRotation;
            }*/
            /*
            if (Mathf.Abs(parent.transform.rotation.eulerAngles.x - qx.eulerAngles.x) > 0)
            {
                Debug.Log($"X: {parent.transform.rotation.eulerAngles.x - qx.eulerAngles.x}");
                parent.transform.rotation = Quaternion.RotateTowards(parent.transform.rotation, qx, rotationSpeed * Time.deltaTime);
            }
            else if (Mathf.Abs(parent.transform.rotation.eulerAngles.y - qy.eulerAngles.y) > 0)
            {
                Debug.Log($"y: {parent.transform.rotation.eulerAngles.y - qy.eulerAngles.y}");
                parent.transform.rotation = Quaternion.RotateTowards(parent.transform.rotation, qy, rotationSpeed * Time.deltaTime);
            }
            else if (Mathf.Abs(parent.transform.rotation.eulerAngles.z - qz.eulerAngles.z) > 0)
            {
                Debug.Log($"z: {parent.transform.rotation.eulerAngles.z - qz.eulerAngles.z}");
                parent.transform.rotation = Quaternion.RotateTowards(parent.transform.rotation, qz, rotationSpeed * Time.deltaTime);
            }

            Vector3 axis;
            float angle;
            Quaternion q = parent.transform.rotation;
            q.ToAngleAxis(out angle, out axis);
            Debug.Log(axis);
            //parent.transform.RotateAround(parent.transform.position, axis, angle);

            //Vector3 dir = (target.position - parent.position).normalized;
            //Quaternion q = Quaternion.FromToRotation(parent.up, Vector3.up) * parent.rotation;
            //parent.rotation = Quaternion.Slerp(parent.rotation, q, Time.deltaTime * rotationSpeed);

            //q = Quaternion.FromToRotation(parent.forward, dir) * parent.rotation;
            //parent.rotation = Quaternion.Slerp(parent.rotation, q, Time.deltaTime * rotationSpeed);

            bool doRate = false;
            Vector3 currentRotationAxis = new Vector3(0, 0, 0);

            if (Input.GetKeyDown(KeyCode.X))
            {
                doRate = true;
                currentRotationAxis = new Vector3(-1, 0, 0);
                pointToRotateFrom = axesRotatePoints[(int)SmartMarkerController.AXES.X];
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                doRate = true;
                currentRotationAxis = new Vector3(0, 1, 0);
                pointToRotateFrom = axesRotatePoints[(int)SmartMarkerController.AXES.X];
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                doRate = true;
                currentRotationAxis = new Vector3(0, 0, 1);
                pointToRotateFrom = axesRotatePoints[(int)SmartMarkerController.AXES.X];
            }

            //if (doRate)
            //{
            Vector3 prevDir = Vector3.ProjectOnPlane(parent.transform.position, currentRotationAxis).normalized;
            Vector3 currentDir = Vector3.ProjectOnPlane(parent.transform.position, currentRotationAxis).normalized;

            Quaternion q = Quaternion.FromToRotation(prevDir, currentDir);
            //Quaternion q = Quaternion.FromToRotation(parent.transform.forward, currentRotationAxis);
            Vector3 axis;
            float angle;
            q.ToAngleAxis(out angle, out axis);
            Debug.Log(axis);
            parent.transform.RotateAround(parent.transform.position, axis, angle);
            //}

            //Vector3 relativePos = target.position - parent.position;

            //Quaternion standUpRotation = Quaternion.FromToRotation(parent.up, Vector3.up);
            //parent.rotation = standUpRotation * parent.rotation;

            //parent.transform.LookAt(target);

            //Quaternion headingRotation = Quaternion.LookRotation(planetTangent * heading, -towardsPlanetCenter);
            //transform.rotation = Quaternion.Slerp(transform.rotation, headingRotation, Time.deltaTime * turnSpeed);
            */
        }
    }
}
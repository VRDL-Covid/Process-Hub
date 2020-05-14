using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Scripts.TestStuff
{
    public class RotateTest : MonoBehaviour
    {
        public enum LOCATESTATES
        {
            INITIALIZE,
            INITIAL_ROTATION,
            LOCK_TO_ANGLES,
            LOCK_TO_POSITION,
            READY_TO_ANCHOR
        }

        public enum AXES
        {
            X = 0, Y, Z
        }
        AXES enmAxis;
        public List<TestRayStuff> axes = new List<TestRayStuff>(3);

        public bool isValidConfiguration = false;
        public bool isReady = false;
        public bool isInAngleTolerance = false;

        public LOCATESTATES locState = LOCATESTATES.INITIALIZE;
        public Transform parent, target;
        float progress, axisAngleToleranceDeg = 1.5f;
        public float rotationSpeed = 0.5f, xAngle = 15, yAngle = 30, zAngle = 12.5f, limit = 0.1f;

        public GameObject[] axesRotatePoints = new GameObject[3];
        public float intX, intY, intZ;
        public GameObject pointToRotateFrom;
        public Queue errorStack = new Queue();
        public int stackLimit = 3;

        public Vector3 parentDestination = Vector3.zero, hitNormal = Vector3.zero;

        [Tooltip("This sets the number of times to allow sign change before increasing the tolerance by the set amount (axis based)")]
        public int signChangeMax = 5;

        Quaternion q, qx, qy, qz;

        Vector3 yDir = Vector3.zero, zDir = Vector3.zero;

        bool movingToPosition = false;
        void Start()
        {
            // Should not be set, visible for debugging...
            target = null;

            q = parent.rotation;

            Vector3 vectAngles = q.eulerAngles;
            xAngle = intX = -vectAngles.x;
            yAngle = intY = -vectAngles.y;
            zAngle = intZ = -vectAngles.z;

            vectAngles += new Vector3(intX, intY, intZ);
            q = Quaternion.Euler(vectAngles);

            // order the axes ascending...
            axes = axes.OrderBy(axis => axis.dimChooser).ToList();
            int sum = axes.Sum(axis => axis.dimChooser);

            isValidConfiguration = axes.Count == 3 && sum == 3 && axes.Select(axis => axis != null).Count() == 3;

            // set the angle tolerance in each axis...
            axes.ForEach(axis => axis.axisAngleTolerance = axisAngleToleranceDeg);
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
            if (!isValidConfiguration)
                return;

            isReady = axes.All(axis => axis.isRecognising);
            if (!movingToPosition && !isReady)
                return;

            Vector3 dir = Vector3.zero;
            Quaternion targetRotation = Quaternion.identity;

            if (target != null)
            {
                // get its position and rotations for the state machine below...
                dir = target.position - parent.position;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            }

            switch (locState)
            {
                case LOCATESTATES.INITIALIZE:
                    isInAngleTolerance = axes.All(axis => axis.isInAngleTolerance);

                    GameObject zHitObject = axes.Single(axis => axis.dimChooser == (int)AXES.Z).hitObject;
                    if (zHitObject == null)
                    {
                        axes.Single(axis => axis.dimChooser == (int)AXES.Z).ForceCreateObject = true;
                    }
                    else
                    {
                        target = axes.Single(axis => axis.dimChooser == (int)AXES.Z).hitObject.transform;

                        hitNormal = axes.Single(axis => axis.dimChooser == (int)AXES.Z).hitNormal;

                        // move it all along...
                        if (!isInAngleTolerance)
                            locState = LOCATESTATES.INITIAL_ROTATION;
                        else
                            locState = LOCATESTATES.LOCK_TO_ANGLES;
                    }

                    break;
                case LOCATESTATES.INITIAL_ROTATION:
                    if (Mathf.Abs(parent.transform.rotation.eulerAngles.x - targetRotation.eulerAngles.x) > limit ||
                        Mathf.Abs(parent.transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y) > limit ||
                        Mathf.Abs(parent.transform.rotation.eulerAngles.z - targetRotation.eulerAngles.z) > limit)
                    {

                        parent.transform.rotation = Quaternion.RotateTowards(parent.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    }
                    else
                    {
                        // time to move the wall thing....
                        locState = LOCATESTATES.LOCK_TO_ANGLES;

                        // reset the sign change counter on the z-axis...
                        axes.Single(axis => axis.dimChooser == (int)AXES.Z).signChangedCount = 0;
                        // setup y axis direction here....
                        if (target.transform.position.y < parent.transform.position.y)
                            yDir = Vector3.down;
                        else
                            yDir = Vector3.up;

                        // setup zDir...
                        SetZDir(hitNormal);
                    }
                    break;

                case LOCATESTATES.LOCK_TO_ANGLES:
                    // reduce y and z error angles settings...

                    if (RotateTarget(targetRotation, yDir, (int)AXES.Y) && RotateTarget(targetRotation, zDir, (int)AXES.Z))
                    {
                        // time to lock rotations and move to position...
                        // get the target position for each axis...
                        float distanceX = axes.Single(axis => axis.dimChooser == (int)AXES.X).distance;
                        float distanceY = axes.Single(axis => axis.dimChooser == (int)AXES.Y).distance;
                        float distanceZ = axes.Single(axis => axis.dimChooser == (int)AXES.Z).distance;

                        parentDestination = parent.transform.position + new Vector3(distanceX, distanceY, distanceZ);
                        movingToPosition = true;
                        locState = LOCATESTATES.LOCK_TO_POSITION;
                    }
                    break;

                case LOCATESTATES.LOCK_TO_POSITION:

                    if (MoveTarget(parentDestination))// && MoveTarget(Vector3.up, (int)AXES.Y))
                    {
                        // time to lock rotations and move to position...
                        locState = LOCATESTATES.READY_TO_ANCHOR;
                    }
                    break;
            }
        }

        private void SetZDir(Vector3 targetNormal)
        {
            if (targetNormal.x == 1)
                zDir = Vector3.forward;
            else if (targetNormal.x == -1)
                zDir = Vector3.back;
            else if (targetNormal.z == -1)
                zDir = Vector3.right;
            else if (targetNormal.z == 1)
                zDir = Vector3.left;
        }

        private bool MoveTarget(Vector3 parentDestination)
        {
            //float distanceError = axes.Single(axis => axis.dimChooser == axis2Move).distance;
            if (!axes.Single(axis => axis.dimChooser == (int)AXES.X).isInDistanceTolerance || 
                !axes.Single(axis => axis.dimChooser == (int)AXES.Y).isInDistanceTolerance || 
                !axes.Single(axis => axis.dimChooser == (int)AXES.Z).isInDistanceTolerance)
            {

                // move along axis to reduce distance...
                parent.transform.position = Vector3.Lerp(parent.transform.position, parentDestination, 0.5f * Time.deltaTime);
                return false;

            }
            return true;
        }

        private bool RotateTarget(Quaternion targetRotation, Vector3 targetMotion, int axis2Move)
        {
            float angleError = axes.Single(axis => axis.dimChooser == axis2Move).AngleError;
            if (!axes.Single(axis => axis.dimChooser == axis2Move).isInAngleTolerance)
            {
                Queue errQ = axes.Single(axis => axis.dimChooser == axis2Move).angleErrorStack;
                if (errQ.Count > 0)
                {
                    errQ.Dequeue();
                }
                errQ.Enqueue(angleError);

                // calculate error rms
                float rms = RMSCalc(errQ);

                //  move the object in x to resolve the rotation in z...
                float movement = 0.005f * rms * (int)axes.Single(axis => axis.dimChooser == axis2Move).enmAngSign;
                // check if error larger than previous and flp direction if needed....

                target.transform.position += targetMotion * movement;
                parent.transform.rotation = Quaternion.RotateTowards(parent.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // get the number of times the sign has changed for this axis and if it has reached the preset limit...
                if ((int)axes.Single(axis => axis.dimChooser == axis2Move).signChangedCount >= signChangeMax)
                {
                    //   force a tolerance increment
                    axes.Single(axis => axis.dimChooser == axis2Move).enableTolIncrement = true;
                }
            }
            return axes.Single(axis => axis.dimChooser == axis2Move).isInAngleTolerance;
        }

        private float RMSCalc(Queue errors, bool normalise = true)
        {
            float value = 0;
            foreach(float error in errors)
            {
                value += error * error;
            }
            value = Mathf.Sqrt(value / errors.Count);
            if (normalise)
                value /= 100;
            return value;
        }
    }
}
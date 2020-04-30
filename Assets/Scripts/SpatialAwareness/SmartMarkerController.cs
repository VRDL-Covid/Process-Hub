using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Scripts.SpatialAwareness
{
    public class SmartMarkerController : MonoBehaviour
    {
        public enum AXES
        {
            X=0,Y,Z
        }

        AXES enmAxis;

        public List<Dimensionizer> axes = new List<Dimensionizer>(3);

        public bool isValidConfiguration = false;
        public bool isReady = false;
        public bool isInAngleTolerance = false;

        public float axisAngleToleranceDeg = 1.5f;

        public Transform targetTransform;
        public float rotationSpeed = 0.1f;

        public bool enableSmartPositioning = false;

        GameObject zHitObject = null;
        public Material meshMat;
        // Start is called before the first frame update
        void Start()
        {
            if (null == targetTransform)
                targetTransform = transform;
            
            // order the axes ascending...
            axes = axes.OrderBy(axis => axis.dimChooser).ToList();
            int sum = axes.Sum(axis => axis.dimChooser);

            isValidConfiguration = axes.Count == 3 && sum == 3 && axes.Select(axis => axis != null).Count() == 3;

            // set the angle tolerance in each axis...
            axes.ForEach(axis => axis.axisAngleTolerance = axisAngleToleranceDeg);
        }

        // Update is called once per frame
        void Update()
        {
            if (!enableSmartPositioning)
                return;

            if (!isValidConfiguration)
                return;

            isReady = axes.All(axis => axis.isRecognising);
            if (!isReady)
                return;

            // get the z axis to create an object at the hit point...
            if (axes.First(axis => axis.dimChooser == (int)AXES.Z))
            // don't move onto direction if angles not in tolerance...
            isInAngleTolerance = axes.All(axis => axis.isInAngleTolerance);
            if (!isInAngleTolerance)
            {
                zHitObject = axes.Single(axis => axis.dimChooser == (int)AXES.Z).hitObject;
                if (zHitObject == null)
                {
                    axes.Single(axis => axis.dimChooser == (int)AXES.Z).ForceCreateObject = true;
                    return;
                }

                bool isX_AngleInTol = axes.Single(axis => axis.dimChooser == (int)AXES.X).isInAngleTolerance;
                bool isY_AngleInTol = axes.Single(axis => axis.dimChooser == (int)AXES.Y).isInAngleTolerance;
                bool isZ_AngleInTol = axes.Single(axis => axis.dimChooser == (int)AXES.Z).isInAngleTolerance;
                    
                Vector3 dir2ZObj = zHitObject.transform.position - targetTransform.position;

                Quaternion targetRotation = Quaternion.LookRotation(dir2ZObj, Vector3.up);
                targetTransform.rotation = Quaternion.RotateTowards(targetTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


                /*if (!isY_AngleInTol)
                {
                    // sort out the z axis first...
                    Quaternion q = Quaternion.FromToRotation(targetTransform.up, Vector3.up) * targetTransform.rotation;
                    targetTransform.rotation = Quaternion.Slerp(targetTransform.rotation, q, Time.deltaTime * rotationSpeed);
                    return;
                }*/

                //isY_AngleInTol = axes.Single(axis => axis.dimChooser == (int)AXES.Y).isInAngleTolerance;

                //Debug.Log($"yAxis in tol: {isY_AngleInTol}");

                //if (!isY_AngleInTol)
                //    return;

                // Need to get direction to target in x direction.
                // Note: this might be a pro`blem as we move the parent as this will mean that the target will change so 
                // this will be iterative, i.e. as we move the object will attempt to rotate toward a (now) non-orthogonal target (in the spatial mesh)
                // so we this will mean that the objects will need to reorient toward a new object in the mesh and then realign the rotation before
                // commencing/recommencing translation.  This way alignment will be iterative.
                //bool isX_AngleInTol = axes.Single(axis => axis.dimChooser == (int)AXES.Y).isInAngleTolerance;

                //Debug.Log($"ZAxis in tol: {isZ_AngleInTol}");

                //else if (!isZ_AngleInTol)
                //{
                // need to get the direction to the object hit in the z axis, this object is non-null if a hit has occured...
                //if (null == zHitObject)
                //    zHitObject = axes.Single(axis => axis.dimChooser == (int)AXES.Z).currentHitObject;

                // now calculate the direction to this object...
                //if (null != zHitObject)
                /*{
                    //zHitObject.GetComponent<Renderer>().material = meshMat;
                    Vector3 dir = (zHitObject.transform.position - targetTransform.position);

                    // calculate the delta in x...
                    dir.z = dir.y = 0;
                    // instead of rotating here, going to try something else...
                    // going to move orthogonal to the selected mesh item and then rotate toward it...
                    // need to move target in x axis and then rotate toward it, this should result in the correct angle...
                    //targetTransform.position = new Vector3(targetTransform.position.x + dir.x, targetTransform.position.y, targetTransform.position.z);
                    Quaternion q = Quaternion.FromToRotation(targetTransform.forward, dir) * targetTransform.rotation;
                    targetTransform.rotation = Quaternion.Slerp(targetTransform.rotation, q, Time.deltaTime * rotationSpeed);

                    //targetTransform.position = new Vector3(targetTransform.position.x + dir.x, targetTransform.position.y, targetTransform.position.z);
                    //Quaternion q = Quaternion.FromToRotation(targetTransform.forward, dir) * targetTransform.rotation;
                    //targetTransform.rotation = Quaternion.Slerp(targetTransform.rotation, q, Time.deltaTime * rotationSpeed);
                }*/
                //}

                // exit this update now as reassessed on next frame...
                //return;
            }
        }

        public void ToggleSmartPositioning()
        {
            enableSmartPositioning = !enableSmartPositioning;
        }
    }
}

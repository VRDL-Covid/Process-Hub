using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using Microsoft.MixedReality.Toolkit.SpatialAwareness;

namespace Scripts.SpatialAwareness
{
    public class Dimensionizer_V1 : MonoBehaviour
    {
        [Range(0, 2)]
        [Tooltip("Select the dimension this is assigned to, x == 0, y == 1, z==2")]
        public int dimChooser;
        public GameObject dimension, midpoint;
        public Color lengthTextFaceColor = new Color(1, 1, 1);
        public string shaderColProperty = "_FaceColor";

        [Header("Tooltip Text boxes")]
        public TMPro.TextMeshPro lengthTextBox;
        public TMPro.TextMeshPro lengthRayCastTextBox;
        public TMPro.TextMeshPro angleRayCastTextBox;
        //public Microsoft.MixedReality.Toolkit.SpatialAwareness.BaseSpatialObserver observer;
        
        [Tooltip("Used to multiply the length of the axis line, only select one! (1 == 1m)")]
        public Vector3 scalerSource = new Vector3(0, 0, 0);

        public float axisAngleTolerance = 0.5f;

        [Tooltip("Current raycast length in metres long primary axis")]
        public float rayCastLengthMetres = 5;

        public Vector3 directionToCast = new Vector3(0, 0, 0);

        public float angleFound = 0f;
        public float angleRequired = 90f;

        public float updateIntervalSecs = 2.5f;

        public bool ForceCreateObject = false;

        public int missedCountMax = 3;

        [HideInInspector]
        public GameObject currentHitObject = null;

        public GameObject hitObject;

        //[HideInInspector]
        [Header("State Attributes")]
        public bool isRecognising = false;
        public bool isInAngleTolerance = false;
        public bool isInDistanceTolerance = false;

        //public Lin

        int layerMask = 1;
        int missedCount = 0;
        public Vector3 dirToCast = new Vector3(0, 0, 0);
        private Vector3[] castdirs = new Vector3[3];

        public float AngleError
        {
            get { return angleRequired - angleFound; }
        }

        void Start()
        {
            if (null == dimension)
                dimension = gameObject.transform.GetChild(0).gameObject;
            if (null == midpoint)
                midpoint = dimension.transform.GetChild(0).gameObject;

            dimension.transform.localScale = scalerSource;

            lengthTextBox.text = string.Format("{0:0.00}m", scalerSource[dimChooser]);
            lengthTextBox.renderer.material.SetColor(shaderColProperty, lengthTextFaceColor);

            lengthRayCastTextBox.text = string.Format("Looking for {0:0.00}m", scalerSource[dimChooser]);
            lengthRayCastTextBox.renderer.material.SetColor(shaderColProperty, lengthTextFaceColor);

            angleRayCastTextBox.text = string.Format("Looking for {0:0.00}°", angleRequired);
            angleRayCastTextBox.renderer.material.SetColor(shaderColProperty, lengthTextFaceColor);

            // Raycast against all game objects that are on either the
            // spatial surface or UI layers.
            layerMask = 1 << LayerMask.NameToLayer("SpatialSurface");

            StartCoroutine(CheckCollision());
        }

        private void Update()
        {
            if (directionToCast.x == 1)
                dirToCast = dimension.transform.right * (scalerSource[dimChooser] >= 0 ? 1 : -1);
            else if (directionToCast.y == 1)
                dirToCast = dimension.transform.up;
            else
                dirToCast = dimension.transform.forward;
        }

        IEnumerator CheckCollision()
        {
            var wait = new WaitForSeconds(updateIntervalSecs);
            
            while (true)
            {
                // do raycast to hit mesh....
                RaycastHit hit;
                if (Physics.Raycast(dimension.transform.position, dirToCast, out hit, rayCastLengthMetres, layerMask))
                {
                    if (ForceCreateObject)
                    {
                        ForceCreateObject = false;
                        if (null != hitObject)
                            GameObject.Destroy(hitObject);
                        GameObject bullseye = Resources.Load("ProcessPrefabs/SupportingPrefabs/SpatialMarkers/bullseye") as GameObject;
                        Quaternion rotPos = Quaternion.Euler(hit.normal);
                        hitObject = GameObject.Instantiate(bullseye);
                        hitObject.transform.position = hit.point;// + (hit.normal * 0.1f);
                        hitObject.transform.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
                        DrawLine(hit.point, hit.point + hit.normal, Color.white, 60);
                    }

                    Vector3 hitNorm = hit.normal;
                    currentHitObject = hit.transform.gameObject;
                    isRecognising = true;
                    lengthRayCastTextBox.text = string.Format("Hit at: {0:0.00}m", hit.distance);
                    missedCount = 0;
                    Debug.DrawRay(dimension.transform.position, dirToCast, lengthTextFaceColor, updateIntervalSecs, true);

                    float cosine = Vector3.Dot(dirToCast, hit.normal);
                    angleFound = Mathf.Rad2Deg * Mathf.Acos(cosine) - 90f;
                    angleRayCastTextBox.text = string.Format("Hit °: {0:0.00}° (diff: {1:0.00}° ", angleFound, angleRequired - angleFound);

                    isInAngleTolerance = Mathf.Abs(angleRequired - angleFound) <= axisAngleTolerance;

                    DrawLine(hit.point, hit.point + hitNorm/*directionToCast*/, lengthTextFaceColor, 5);
                }
                else
                {
                    currentHitObject = null;
                    isRecognising = false;
                    isInAngleTolerance = false;
                    isInDistanceTolerance = false;
                }
                
                /*if (missedCount++ < missedCountMax)
                    missedCount++;
                else if((missedCount >= missedCountMax))
                    lengthRayCastTextBox.text = string.Format("Looking for {0:0.00}m", scalerSource[dimChooser]);*/



                // Calculate the normal angle to the collided surface...
                /*Vector3 outGoingVec = dimension.transform.position - hit.point;
                Vector3 reflectedVec = Vector3.Reflect(outGoingVec, hit.normal);

                Debug.DrawRay(dimension.transform.position, hit.point, Color.white, updateIntervalSecs, true);
                Debug.DrawRay(hit.point, reflectedVec, Color.yellow, updateIntervalSecs, true);
                */
                yield return wait;
            }
        }

        void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
        {
            //Material material = new Material(Shader.Find("Standard"));
            //material.color = color;

            GameObject myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.widthMultiplier = 0.02f;
            lr.startColor = color ;
            lr.endColor = color ;
            lr.positionCount = 2;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            GameObject.Destroy(myLine, duration);
        }
    }
}

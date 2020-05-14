using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Scripts.TestStuff
{
    public class TestRayStuff : MonoBehaviour
    {
        public enum ERROR_ANGLE_SIGN
        {
            POSITIVE= -1,
            NOT_SET = 0,
            NEGATIVE = 1
        }

        [Range(0, 2)]
        [Tooltip("Select the dimension this is assigned to, x == 0, y == 1, z==2")]
        public int dimChooser;
        public GameObject dimension;
        public Color lengthTextFaceColor = new Color(1, 1, 1);
        public string shaderColProperty = "_FaceColor";

        [Tooltip("Used to multiply the length of the axis line, only select one! (1 == 1m)")]
        public Vector3 scalerSource = new Vector3(0, 0, 0);

        public float axisAngleTolerance = 0.5f;
        public float axisDistanceTolerance = 0.1f;

        [Tooltip("Current raycast length in metres along primary axis")]
        public float rayCastLengthMetres = 50;

        public Vector3 directionToCast = new Vector3(0, 0, 0);

        public float angleFound = 0f;
        public float angleRequired = 90f;

        public float distance = 0;

        public float updateIntervalSecs = 2.5f;

        public bool ForceCreateObject = false;


        public int signChangedCount = 0;
        public float tolInc = 0.1f;
        public bool enableTolIncrement = false;

        public int missedCountMax = 3;
        public ERROR_ANGLE_SIGN enmAngSign, enmPrevAngSign = ERROR_ANGLE_SIGN.NOT_SET;
        public Queue angleErrorStack = new Queue();
        public Queue distanceErrorStack = new Queue();


        public GameObject hitObject;
        public Vector3 hitNormal = Vector3.zero;

        Quaternion lastRot;

        [Header("State Attributes")]
        public bool isRecognising = false;
        public bool isInAngleTolerance = false;
        public bool isInDistanceTolerance = false;
        int missedCount = 0;
        public Vector3 dirToCast = new Vector3(0, 0, 0);
        private Vector3[] castdirs = new Vector3[3];
        int layerMask = 0;
        public float AngleError
        {
            get { return angleRequired - angleFound; }
        }

        void Start()
        {
            if (null == dimension)
                dimension = gameObject.transform.GetChild(0).gameObject;

            dimension.transform.localScale = scalerSource;
            layerMask = 1 << LayerMask.NameToLayer("Wall");
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
                if (Physics.Raycast(dimension.transform.position, dirToCast, out hit,200, layerMask))
                {
                    if (ForceCreateObject)
                    {
                        ForceCreateObject = false;
                        if (null != hitObject)
                            GameObject.Destroy(hitObject);
                        GameObject bullseye = Resources.Load("ProcessPrefabs/SupportingPrefabs/SpatialMarkers/bullseye") as GameObject;
                        Quaternion rotPos = Quaternion.Euler(hit.normal);
                        hitObject = GameObject.Instantiate(bullseye);
                        hitObject.transform.position = hit.point;
                        hitObject.transform.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
                        DrawLine(hit.point, hit.point + hit.normal, Color.white, 60, $"normal for bullseye");

                        // need to return hit normal as this is used to calculate te direction to move the target in...
                        hitNormal = hit.normal;
                    }

                    Vector3 hitNorm = hit.normal;
                    isRecognising = true;
                    missedCount = 0;
                    Debug.DrawRay(dimension.transform.position, dirToCast, lengthTextFaceColor, updateIntervalSecs, true);

                    float signedAngle = Vector3.SignedAngle(dirToCast, hit.normal, Vector3.up);

                    enmAngSign = signedAngle > 0 ? ERROR_ANGLE_SIGN.POSITIVE : ERROR_ANGLE_SIGN.NEGATIVE;
                    signChangedCount += enmAngSign != enmPrevAngSign ? 1 : 0;
                    enmPrevAngSign = enmAngSign;
                    if (enableTolIncrement)
                    {
                        axisAngleTolerance += tolInc;
                        enableTolIncrement = false;
                        signChangedCount = 0;
                    }

                    lastRot = dimension.transform.rotation;

                    // seems there is an error in Vector3.Dot it messes up the arc cos calc...
                    float cosine = Mathf.Clamp(Vector3.Dot(dirToCast, hit.normal), -1, 1);

                    angleFound = Mathf.Rad2Deg * Mathf.Acos(cosine) - 90f;
                    isInAngleTolerance = Mathf.Abs(angleRequired - angleFound) <= axisAngleTolerance;

                    distance = hit.distance - scalerSource[dimChooser];
                    isInDistanceTolerance = Mathf.Abs(distance) <= axisDistanceTolerance;

                    DrawLine(hit.point, hit.point + (hitNorm * 20), lengthTextFaceColor, 5, $"Normal for dim {dimChooser}");
                }
                else
                {
                    isRecognising = false;
                    isInAngleTolerance = false;
                    isInDistanceTolerance = false;
                }
                
                yield return wait;
            }
        }

        void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f, string lineName = "a line...")
        {
            //Material material = new Material(Shader.Find("Standard"));
            //material.color = color;

            GameObject line = new GameObject();
            line.name = lineName;
            line.transform.position = start;
            line.AddComponent<LineRenderer>();
            LineRenderer lr = line.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.widthMultiplier = 0.02f;
            lr.startColor = color ;
            lr.endColor = color ;
            lr.positionCount = 2;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            GameObject.Destroy(line, duration);
        }
    }
}

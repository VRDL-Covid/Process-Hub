using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Microsoft.MixedReality.Toolkit.SpatialAwareness;

namespace Scripts.SpatialAwareness
{
    public class Dimensionizer : MonoBehaviour
    {
        [Range(0, 2)]
        [Tooltip("Select thedimension this is assigned to, x == 0, y == 1, z==2")]
        public int dimChooser;
        public GameObject dimension, midpoint;
        public Color lengthTextFaceColor = new Color(1, 1, 1);
        public string shaderColProperty = "_FaceColor";
        public TMPro.TextMeshPro lengthTextBox;
        public TMPro.TextMeshPro lengthRayCastTextBox;
        //public Microsoft.MixedReality.Toolkit.SpatialAwareness.BaseSpatialObserver observer;

        [Tooltip("Used to multiply the length of the axis line, only select one! (1 == 1m)")]
        public Vector3 scalerSource = new Vector3(0, 0, 0);

        [Tooltip("Current raycast length in metres long primary axis")]
        public float rayCastLengthMetres = 5;

        public Vector3 directionToCast = new Vector3(0, 0, 0);

        private Vector3[] castdirs = new Vector3[3];

        public float updateIntervalSecs = 2.5f;
        public int missedCountMax = 3;

        int layerMask = 1;
        int missedCount = 0;
        Vector3 dirToCast = new Vector3(0, 0, 0);
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
                    lengthRayCastTextBox.text = string.Format("Hit at: {0:0.00}m", hit.distance);
                    missedCount = 0;
                }
                /*else if (missedCount++ < missedCountMax)
                    missedCount++;
                else if((missedCount >= missedCountMax))
                    lengthRayCastTextBox.text = string.Format("Looking for {0:0.00}m", scalerSource[dimChooser]);*/

                Debug.DrawRay(dimension.transform.position, dirToCast, lengthTextFaceColor, updateIntervalSecs, true);
                yield return wait;
            }
        }
    }
}

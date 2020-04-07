using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.SpatialAwareness
{
    public class SnapToMe : MonoBehaviour
    {
        public GameObject Snap2Me;

        public GameObject marker;

        Color[] axisColours = new Color [3];

        string[] axisNames = new string[3] { "_xlength", "_ylength", "_zlength" };

        public Vector3 lengths = new Vector3(1, 1, 1);

        public void SnapMarker()
        {
            // instantiate colors now to keep memory usage down,,,
            axisColours[0] = Color.red;
            axisColours[1] = Color.green;
            axisColours[2] = Color.blue;

            // create the marker object....
            marker = GameObject.Instantiate(Snap2Me);
            int axisPos = 0;

            // set up the prefab....
            foreach (string axisName in axisNames)
            {
                Vector3 scalerSource = new Vector3(1, 1, 1);
                scalerSource[axisPos] = lengths[axisPos];

                Vector3 directionToCast = new Vector3(0, 0, 0);
                directionToCast[axisPos] = 1;

                // now set the properties...
                GameObject goAxis = marker.transform.Find(axisName).gameObject;
                Dimensionizer odzr = goAxis.GetComponent<Dimensionizer>();

                odzr.scalerSource = scalerSource;
                odzr.directionToCast = directionToCast;
                odzr.lengthTextFaceColor = axisColours[axisPos];
                axisPos++;
            }

            // snap it to me...
            marker.transform.position = gameObject.transform.position;
        }

    }
}

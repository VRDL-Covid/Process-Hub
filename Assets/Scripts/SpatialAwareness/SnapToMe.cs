using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Json;
using Newtonsoft.Json;
using System.Text;

namespace Scripts.SpatialAwareness
{
    public class SnapToMe : MonoBehaviour
    {
        public GameObject markerPrefab;
        public string axisParentName;

        public GameObject marker;

        public Color[] axisColours = new Color [3];

        public Vector3 lengths = new Vector3(1, 1, 1);

        public string coordJSON = "coordinates";

        string[] axisNames = new string[3] { "_xlength", "_ylength", "_zlength" };
        SpatialCoordinates spcoord;

        List<GameObject> markers = new List<GameObject>();

        public void SnapMarker()
        {
            // instantiate colors now to keep memory usage down,,,
            axisColours[0] = Color.red;
            axisColours[1] = Color.green;
            axisColours[2] = Color.blue;

            // create the marker object....
            marker = GameObject.Instantiate(markerPrefab);

            SetUpMarker(marker, lengths);

            // snap it to me...
            marker.transform.position = gameObject.transform.position;
        }

        public void GetCoordinateData()
        {
            spcoord = ReadCoordinateData(coordJSON);

            foreach (SpatialCoordinates.Marker marker in spcoord.markers)
            {
                GameObject omrkr = GameObject.Instantiate(markerPrefab, marker.position, marker.Rotation);
                markers.Add(omrkr);
                SetUpMarker(omrkr, marker.lengths);
            }
        }

        private void SetUpMarker(GameObject marker, Vector3 lengths)
        {


            // Get axis parent...
            GameObject axisParent = null;
            for(int i = 0; i < marker.transform.childCount;i++)
            {
                GameObject parent = marker.transform.GetChild(i).gameObject;
                if (parent.transform.Find(axisNames[0]))
                {
                    axisParent = parent;
                    break;
                }
            }
            if (null == axisParent)
            {
                Debug.LogError("No Axis Parent found");
            }
            // set up the prefab....
            int axisPos = 0;
            foreach (string axisName in axisNames)
            {
                Vector3 scalerSource = new Vector3(1, 1, 1);
                scalerSource[axisPos] = lengths[axisPos];

                Vector3 directionToCast = new Vector3(0, 0, 0);
                directionToCast[axisPos] = 1;

                // now set the properties...
                GameObject goAxis = axisParent.transform.Find(axisName).gameObject;
                Dimensionizer odzr = goAxis.GetComponent<Dimensionizer>();

                odzr.scalerSource = scalerSource;
                odzr.directionToCast = directionToCast;
                odzr.lengthTextFaceColor = axisColours[axisPos];
                axisPos++;
            }
        }

        private void Start()
        {
            GetCoordinateData();
        }

        public SpatialCoordinates ReadCoordinateData(string fileName)
        {
            SpatialCoordinates spcoords = new SpatialCoordinates();
            string json = string.Empty;

            string path = string.Format("{0}/{1}.json", Application.persistentDataPath, fileName);
            if (UnityEngine.Windows.File.Exists(path))
            {
                byte[] data = UnityEngine.Windows.File.ReadAllBytes(path);
                json = Encoding.ASCII.GetString(data);

                spcoords = JsonConvert.DeserializeObject<SpatialCoordinates>(json);
            }

            return spcoords;
        }

    }
}

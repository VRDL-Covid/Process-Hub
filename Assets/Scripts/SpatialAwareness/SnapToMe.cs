using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Json;
using Newtonsoft.Json;
using System.Text;
using System.Linq;

namespace Scripts.SpatialAwareness
{
    public class SnapToMe : MonoBehaviour
    {
        public GameObject markerPrefab;
        public string axisParentName;

        public GameObject marker;

        public Material[] coneMaterials = new Material[3];

        public GameObject MarkerSpawnPoint;

        public Color[] axisColours = new Color [3];

        public Vector3 lengths = new Vector3(1, 1, 1);

        public string coordJSON = "coordinates";

        string[] axisNames = new string[3] { "_xlength", "_ylength", "_zlength" };
        SpatialCoordinates spcoord;

        public List<GameObject> markers = new List<GameObject>();

        public void SnapMarker()
        {
            // instantiate colors now to keep memory usage down,,,
            axisColours[0] = Color.red;
            axisColours[1] = Color.green;
            axisColours[2] = Color.blue;

            // create the marker object....
            marker = GameObject.Instantiate(markerPrefab);

            SetUpMarker(marker, lengths, 0);

            // snap it to me...
            marker.transform.position = gameObject.transform.position;
        }

        #region App Bar button event handlers...
        public void SpawnMarkers()
        {
            spcoord = ReadCoordinateData(coordJSON);

            int mrkrIdx = 0;
            foreach (SpatialCoordinates.Marker marker in spcoord.markers)
            {
                GameObject omrkr = markers.FirstOrDefault(m => m.name == marker.Name);
                if (null == omrkr)
                {
                    omrkr = GameObject.Instantiate(markerPrefab, MarkerSpawnPoint.transform.position + marker.position, marker.Rotation);
                    omrkr.name = marker.Name;
                    markers.Add(omrkr);
                    omrkr.transform.SetParent(MarkerSpawnPoint.transform);
                }
                SetUpMarker(omrkr, marker.lengths, mrkrIdx++);
            }
        }
        public void SeekMarkers()
        {

        }

        public void ResetMarkers()
        {
            SpawnMarkers();
        }

        public void DeleteMarkers()
        {
            foreach (GameObject marker in markers)
                GameObject.Destroy(marker);
            markers.Clear();
        }

        #endregion button handlers

        private void SetUpMarker(GameObject marker, Vector3 lengths, int mrkrIdx)
        {

            // set the cone material
            SetConeMaterial scm = marker.GetComponent<SetConeMaterial>();
            if (null != scm)
                scm.ConeMaterial = coneMaterials[mrkrIdx];
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
               // odzr.lengthTextFaceColor = axisColours[axisPos];
                axisPos++;
            }
        }

        private void _Start()
        {
            SpawnMarkers();
        }

        private SpatialCoordinates ReadCoordinateData(string fileName)
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

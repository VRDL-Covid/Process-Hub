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

        public GameObject markerPositionsPrefab;

        public Material[] coneMaterials = new Material[3];

        public GameObject MarkerSpawnPoint;

        public Color[] axisColours = new Color [3];

        public Vector3 lengths = new Vector3(1, 1, 1);
        
        public Vector3 offset = new Vector3(0.2f, -0.2f, 0.2f);

        public string coordJSON = "coordinates";

        string[] axisNames = new string[3] { "_xlength", "_ylength", "_zlength" };
        SpatialCoordinates spcoord;

        public List<GameObject> markers = new List<GameObject>();

        #region App Bar button event handlers...
        public void SpawnMarkers()
        {
            // read json pos information...
            spcoord = ReadCoordinateData(coordJSON);

            int mrkrIdx = 0;

            // create instance of object (same one used to generate Json above)...
            GameObject markerPositions = GameObject.Instantiate(markerPositionsPrefab);
            //reset position to the marker used to move all the markers...
            markerPositions.transform.position = MarkerSpawnPoint.transform.position;

            //get the mean of the marker positions...
            Vector3 centroid = CalculateCentroid();

            // now create a SmartMarker and replace all the child markers...
            foreach (SpatialCoordinates.Marker marker in spcoord.markers)
            {
                GameObject omrkr = markers.FirstOrDefault(m => m.name == marker.Name);
                if (null == omrkr)
                {
                    GameObject posToDelete = markerPositions.transform.Find(marker.Name).gameObject;
                    if (null != posToDelete)
                    {
                        omrkr = GameObject.Instantiate(markerPrefab, posToDelete.transform.position, marker.Rotation);
                        omrkr.transform.SetParent(markerPositions.transform);
                        GameObject.Destroy(posToDelete);
                    }

                    // calc position...
                    //Vector3 markerPos = new Vector3(markerParent.transform.position.x - centroid.x - marker.position.x, marker.position.y, markerParent.transform.position.z - centroid.z - marker.position.z);
                    omrkr.name = marker.Name;
                    markers.Add(omrkr);
                    markerPositions.transform.SetParent(MarkerSpawnPoint.transform);
                }
                SetUpMarker(omrkr, marker.lengths, mrkrIdx++);
            }
            //GetClosestObject(markerParent.transform);
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

        Transform GetClosestObject(Transform referenceObject)
        {
            Transform tMin = null;
            int posIdx = -1;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = referenceObject.position;

            for (int i = 0; i < referenceObject.childCount; i++)
            {
                float dist = Vector3.Distance(referenceObject.GetChild(i).transform.position, currentPos);
                if (dist < minDist)
                {
                    posIdx = i;
                    tMin = referenceObject.GetChild(i).transform;
                    minDist = dist;
                }
            }

            Vector3 offset = referenceObject.position - tMin.position;
            // now move offset the children by the difference between the reference objects position and the closest object...
            for (int i = 0; i < referenceObject.childCount; i++)
            {
                referenceObject.GetChild(i).transform.position -= offset;
            }

            return tMin;
        }

        public Vector3 CalculateCentroid()
        {
            Vector3 centroid = Vector3.zero;
            List<Vector3> centroids = spcoord.markers.Select(m => m.position).ToList<Vector3>();
            // get all child gameobjects of type Centroid and calculate their barycentric value...
            foreach(Vector3 pos in centroids)
                centroid += pos;

            centroid /= centroids.Count;

            return centroid;
        }

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

        private void Start()
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

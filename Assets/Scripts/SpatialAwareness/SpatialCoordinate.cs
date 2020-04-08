using UnityEngine;
using Scripts.Json;
using Newtonsoft.Json;
using System.Text;

namespace Scripts.SpatialAwareness
{
    public enum JSONSTATE
    {
        NOT_WRITTEN,
        WRITTEN
    }
    [ExecuteInEditMode]
    public class SpatialCoordinate : MonoBehaviour
    {
        public JSONSTATE jsonState = JSONSTATE.NOT_WRITTEN;

        public string coordinateJson = "coordinates";

#if UNITY_EDITOR  
        void Update()
        {
            if (jsonState == JSONSTATE.NOT_WRITTEN)
                GenerateJSONResource();          
        }

        void GenerateJSONResource()
        {
            SpatialCoordinates spcoords = new SpatialCoordinates();
            spcoords.me.Name = gameObject.name;
            spcoords.me.position = gameObject.transform.position;
            spcoords.me.scale = gameObject.transform.localScale;
            spcoords.me.SetRotation(gameObject.transform.rotation);

            for (int i= 0;i < gameObject.transform.childCount;i++)
            {
                GameObject marker = gameObject.transform.GetChild(i).gameObject;
                SpatialCoordinates.Marker omk = new SpatialCoordinates.Marker();
                spcoords.markers.Add(omk);
                omk.Name = marker.name;
                omk.position = marker.transform.position;
                omk.scale = marker.transform.localScale;
                omk.SetRotation(marker.transform.rotation);
            }
            SaveDataData(spcoords);
            jsonState = JSONSTATE.WRITTEN;
            Debug.Log(jsonState);
        }

        public void SaveDataData(SpatialCoordinates spcoords)
        {
            string path = string.Format("{0}/{1}.json", Application.persistentDataPath, coordinateJson);

            string json = JsonConvert.SerializeObject(spcoords, Formatting.Indented);
            byte[] data = Encoding.ASCII.GetBytes(json);

            UnityEngine.Windows.File.WriteAllBytes(path, data);
        }

#endif
    }
}

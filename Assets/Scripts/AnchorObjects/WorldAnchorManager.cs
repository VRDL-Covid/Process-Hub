using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;

//#if WINDOWS_UWP
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
using MST = Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
//#endif
namespace Scripts.AnchorObjects
{
    public class WorldAnchorManager : ManagerBase
    {
        public Dictionary<string, AnchoredGameObject> gameObjectsToSerialize = new Dictionary<string, AnchoredGameObject>();
        public Dictionary<string, GameObject> gos = new Dictionary<string, GameObject>();

        public string fileName = "";

        public Mode worldMode = Mode.SetUp;

        bool loadedFromHub = false;
        bool deletingAnchors = false;

        List<string> ids;
        // Start is called before the first frame update
        void Start()
        {
            if (worldMode == Mode.Operate)
            {
                LoadExistingAnchors();
           }

        }

        public void LoadExistingAnchors()
        {
            AnchoredGameObjects agos = ReadData();

            int id = 0;
            foreach (AnchoredGameObject ago in agos.anchorObjects)
            {
                // get object from json...

                // get parent in scene...
                GameObject parent = GameObject.Find(ago.ParentName);
                if (null == parent)
                    return;
                // look up the type in the store
                //create the object
                // set pos, rot and scale...
                // GameObject prefab =  UnityEditor.AssetDatabase.LoadAssetAtPath(ago.PrefabSource, typeof(GameObject)) as GameObject;
                GameObject prefab = Resources.Load(ago.PrefabSource, typeof(GameObject)) as GameObject;
                GameObject go = GameObject.Instantiate(prefab);
                go.transform.position = parent.transform.position;
                go.transform.SetParent(parent.transform);
                go.name += "_" + id++;
                go.transform.localScale = ago.Scale;
                ago.SetScale(go);
                ago.SetToolTip(go);

                AnchoredGameObject newAgo = new AnchoredGameObject(go, ago.PrefabSource);
                newAgo.RunOrder = ago.RunOrder;
                newAgo.Description = ago.Description;
                //gameObjectsToSerialize.Add(id, newAgo);
                gameObjectsToSerialize.Add(go.name, ago);
                gos.Add(go.name, go);

                // Show on load only if we are setup mode only...
                if (this.worldMode == Mode.Operate)
                {
                    Destroy(go.transform.Find("AppBar").gameObject);
                    //go.SetActive(false);
                }
            }
        }

        public GameObject SetActivateStateForObject(string id2Activate, bool state)
        {
            gos[id2Activate].SetActive(true);
            GameObject target = gos[id2Activate];

#if WINDOWS_UWP
        StepIndicator stepIndicator = CameraCache.Main.transform.GetComponentInChildren<StepIndicator>();
#else
            Camera camera = (Camera)GameObject.FindObjectOfType(typeof(Camera));
            StepIndicator stepIndicator = camera.transform.GetComponentInChildren<StepIndicator>();
#endif
            stepIndicator.target = target;
            return target;
        }

        public bool AnchorExists(string id)
        {
            return ids.Contains(id);
        }

        public AnchoredGameObjects ReadData()
        {
            AnchoredGameObjects agos = new AnchoredGameObjects();
            string json = string.Empty;

            string path = string.Format("{0}/{1}.json", Application.persistentDataPath, this.fileName);
            if (UnityEngine.Windows.File.Exists(path))
            {
                byte[] data = UnityEngine.Windows.File.ReadAllBytes(path);
                json = Encoding.ASCII.GetString(data);

                agos = JsonConvert.DeserializeObject<AnchoredGameObjects>(json);
            }

            return agos;
        }

        public void SaveData()
        {
#if WINDOWS_UWP

            // determine if this is being called from the hub...
            GameObject goIdx = GameObject.Find("IndexManager");
            if (goIdx != null)
            {
                IndexManager idxMgr = goIdx.GetComponent<IndexManager>();
                idxMgr.AddIndex(this.fileName, "<New Process>", this.instanceID);
                idxMgr.SaveIndexData();
            }
            var anchorsToDelete = gameObjectsToSerialize.Where(pair => (GameObject.Find(pair.Key) == null)).Select(pair => new { pair.Key, pair.Value }).ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (string id in anchorsToDelete.Keys)
            {
                was.Delete(id);
                gameObjectsToSerialize.Remove(id);
            }
            var agos = gameObjectsToSerialize.Where(pair => (GameObject.Find(pair.Key) != null)).Select(pair => pair.Value).ToList();

            AnchoredGameObjects agoParent = new AnchoredGameObjects();

            foreach (AnchoredGameObject ago in agos)
                agoParent.anchorObjects.Add(ago);

            string path = string.Format("{0}/{1}.json", Application.persistentDataPath, this.fileName);

            string json = JsonConvert.SerializeObject(agoParent, Formatting.Indented);
            byte[] data = Encoding.ASCII.GetBytes(json);

            UnityEngine.Windows.File.WriteAllBytes(path, data);

            // string assetPath = string.Format("{0}/_{1}.json", Application.persistentDataPath, this.fileName);
            //TextAsset jsonAsset = new TextAsset(json);
            // UnityEditor.AssetDatabase.CreateAsset(jsonAsset, assetPath);
#endif
        }

        public void SaveModelData()
        {
            var agos = gameObjectsToSerialize.Where(pair => (GameObject.Find(pair.Key) != null)).Select(pair => pair.Value).ToList();

            AnchoredGameObjects agoParent = new AnchoredGameObjects();

            foreach (AnchoredGameObject ago in agos)
                agoParent.anchorObjects.Add(ago);

            string path = string.Format("{0}/{1}.json", Application.persistentDataPath, this.fileName);

            string json = JsonConvert.SerializeObject(agoParent, Formatting.Indented);
            byte[] data = Encoding.ASCII.GetBytes(json);

            UnityEngine.Windows.File.WriteAllBytes(path, data);
        }


    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Status
    {
        Incomplete = 0,
        Complete = 1
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Mode
    {
        SetUp = 0,
        Operate = 1
    }
}
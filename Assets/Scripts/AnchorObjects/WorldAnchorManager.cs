using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;

#if WINDOWS_UWP
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
using MST = Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
#endif
namespace Scripts.AnchorObjects
{
    public class WorldAnchorManager : ManagerBase
    {
        public Dictionary<string, AnchoredGameObject> gameObjectsToSerialize = new Dictionary<string, AnchoredGameObject>();
        public Dictionary<string, GameObject> gos = new Dictionary<string, GameObject>();

        public string fileName = "anchordata";

        public Mode worldMode = Mode.SetUp;

#if WINDOWS_UWP
        WorldAnchorStore was;
#endif
        bool loadedFromHub = false;
        bool deletingAnchors = false;

        List<string> ids;
        // Start is called before the first frame update
        void Start()
        {
            if (worldMode == Mode.SetUp)
            {
#if WINDOWS_UWP               
                //WorldAnchorStore.GetAsync(StoreLoaded);
#endif
            }
        }
#if WINDOWS_UWP
        public void InitiateAnchorLoad()
        {
            WorldAnchorStore.GetAsync(StoreLoaded);
        }

        private void StoreLoaded(WorldAnchorStore was)
        {
            this.was = was;
            if (worldMode == Mode.SetUp)
                LoadExistingAnchors();
        }

        public void LoadExistingAnchors()
        {
            if (deletingAnchors)
            {
                ids = this.was.GetAllIds().ToList<string>();

                foreach (string id in ids)
                {
                    DeleteWorldAnchor(id);
                }
            }
            ids = this.was.GetAllIds().ToList<string>();

            AnchoredGameObjects agos = this.ReadData();

            foreach (string id in ids)
            {
                // get object from json...
                AnchoredGameObject ago = agos.anchorObjects.FirstOrDefault(item => item.Name == id);
                if (ago != null)
                {

                    //DeleteWorldAnchor(id);
                    // look up the type in the store
                    //create the object
                    // set pos, rot and scale...
                    // GameObject prefab =  UnityEditor.AssetDatabase.LoadAssetAtPath(ago.PrefabSource, typeof(GameObject)) as GameObject;
                    GameObject prefab = Resources.Load(ago.PrefabSource, typeof(GameObject)) as GameObject;
                    GameObject go = GameObject.Instantiate(prefab);
                    go.name = id;
                    go.transform.localScale = ago.Scale;
                    ago.SetScale(go);
                    ago.SetToolTip(go);

                    AnchoredGameObject newAgo = new AnchoredGameObject(go, ago.PrefabSource);
                    newAgo.RunOrder = ago.RunOrder;
                    newAgo.Description = ago.Description;
                    //gameObjectsToSerialize.Add(id, newAgo);
                    gameObjectsToSerialize.Add(id, ago);
                    gos.Add(id, go);
                    WorldAnchor wa = was.Load(id, go);
                    Debug.Log(id);

                    // Show on load only if we are setup mode only...
                    if (this.worldMode == Mode.Operate)
                    {
                        // hide the app bar, not needed in operate mode...
                        Destroy(go.transform.Find("AppBar").gameObject);
                        go.SetActive(false);
                    }
                }
            }
        }
#endif
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

#if WINDOWS_UWP
        public void CreateWorldAnchor(GameObject objToAnchor)
        {
            WorldAnchor anchor = objToAnchor.GetComponent<WorldAnchor>();
            if (anchor == null)
                anchor = objToAnchor.AddComponent<WorldAnchor>();
            anchor.name = objToAnchor.name;
            if (anchor.isLocated)
                this.DoLocalSave(objToAnchor.name, anchor);
            else
            {
                // not enough track data (probably!)....
                anchor.OnTrackingChanged += OnTrackingChange;
            }
        }
#endif

#if WINDOWS_UWP
        public void DeleteWorldAnchor(string anchorName)
        {
            was.Delete(anchorName);
        }
#endif

#if WINDOWS_UWP
        public bool DoLocalSave(string anchorName, WorldAnchor anchor)
        {
            if (AnchorExists(anchorName))
                was.Delete(anchorName);
            return was.Save(anchorName, anchor);
        }
#endif

#if WINDOWS_UWP
        private void OnTrackingChange(WorldAnchor anchor, bool isLocated)
        {
            if (isLocated)
            {
                this.DoLocalSave(anchor.name, anchor);
                anchor.OnTrackingChanged -= OnTrackingChange;
            }
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
#endif

#if WINDOWS_UWP
        public void SaveData()
        {

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
        }
#endif
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
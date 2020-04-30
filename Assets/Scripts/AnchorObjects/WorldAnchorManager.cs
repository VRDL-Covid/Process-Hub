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

        public Scripts.LineObjects.CurvedLineRenderer curvedLineRender;

        public string anchorsFileName = "";
        public string ModelsFileName = "";
        public Mode worldMode = Mode.SetUp;

        WorldAnchorStore was;
        bool loadedFromHub = false;
        bool deletingAnchors = false;
        GameObject fullModel;

        List<string> ids;
        // Start is called before the first frame update
        void Start()
        {
            WorldAnchorStore.GetAsync(StoreLoaded);
        }

        private void StoreLoaded(WorldAnchorStore was)
        {
            // clear line data
            curvedLineRender.DoNavigationLine(this, new Vector3[0]);
            this.was = was;

            LoadModels();
            if (worldMode == Mode.Operate)
            {
                //Load Anchors...
                LoadExistingAnchors();
            }
        }

        private void OnTrackingChange(WorldAnchor anchor, bool isLocated)
        {
            if (isLocated)
            {
                this.DoLocalSave(anchor.name, anchor);
                anchor.OnTrackingChanged -= OnTrackingChange;
            }
        }
        public void InitiateAnchorLoad()
        {
            WorldAnchorStore.GetAsync(StoreLoaded);
        }

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

        public bool DoLocalSave(string anchorName, WorldAnchor anchor)
        {
            if (AnchorExists(anchorName))
                was.Delete(anchorName);
            return was.Save(anchorName, anchor);
        }

        public void DeleteWorldAnchor(string anchorName)
        {
            was.Delete(anchorName);
            if (this.gameObjectsToSerialize.ContainsKey(anchorName))
                gameObjectsToSerialize.Remove(anchorName);
        }

        /// <summary>
        /// Load Models instantiates the models defined in the json file [ModelsFileName]
        /// One of these models is defined as the FULL model and constitutes the parent for the anchors
        /// Hence that is why the models are loaded prior to loading the anchors...
        /// </summary>
        public void LoadModels()
        {

            //if WINDOWS_UWP      
            ids = this.was.GetAllIds().ToList<string>();
            if (deletingAnchors)
            {

                foreach (string aid in ids)
                {
                    DeleteWorldAnchor(aid);
                }
                //re get...
                ids = this.was.GetAllIds().ToList<string>();
            }
            //#endif

            AnchoredGameObjects agos = ReadData(this.ModelsFileName);

            int id = 0;
            // set up line points vector...
            foreach (AnchoredGameObject ago in agos.anchorObjects)
            {

                // look up the type in the store
                //create the object
                // set pos, rot and scale...
                GameObject prefab = Resources.Load(ago.PrefabSource, typeof(GameObject)) as GameObject;
                GameObject go = GameObject.Instantiate(prefab);
                
                go.name = ago.Name;
                // switched off as sorted by anchor...
                
                go.transform.position = ago.Position;
                go.transform.localScale = ago.Scale;
                go.transform.rotation = new Quaternion(ago.RotateX, ago.RotateY, ago.RotateZ, ago.RotateW);
                ago.SetScale(go);

                // set the transform for speech control...
                GameObject aspectTxfr = GameObject.Find("AspectTransformer");
                if (null != aspectTxfr)
                {
                    AspectTransformer compAspectTxformer = aspectTxfr.GetComponent<AspectTransformer>();
                    if (null != compAspectTxformer)
                    {
                        compAspectTxformer.targetTransform = go.transform;
                    }
                }
                
                ago.SetToolTip(go);

                // time to anchor this object if one exists...
                was.Load(go.name, go);
                // Set the target for the finite state machine...

                // TODO: sort this real hack..
                if (ago.Name.ToUpper().Contains("FULL"))
                {
                    // find the Finite State Machine and set it's global state target...
                    GameObject fsm = GameObject.Find("FSMHost");
                    if (null != fsm)
                    {
                        GlobalState gs = fsm.GetComponent<GlobalState>();
                        if (null != gs)
                        {
                            fullModel = go;
                            gs.target = go;
                        }
                    }
                }

                // Load anchor if running on UWP device and anchor exists...
                // TODO Add anchor script, NOTE need to set position of anchored object to zero and rotate...

                AnchoredGameObject newAgo = new AnchoredGameObject(go, ago.PrefabSource);
                newAgo.RunOrder = ago.RunOrder;
                newAgo.Description = ago.Description;
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

        public static GameObject GetNestedChild(Transform parentTransform, string childName)
        {

            GameObject nestedChild = null;
            int childCount = parentTransform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                if (parentTransform.GetChild(i).gameObject.name == childName)
                {
                    nestedChild = parentTransform.GetChild(i).gameObject;
                    break;
                }
                if (null == nestedChild)
                    nestedChild = GetNestedChild(parentTransform.GetChild(i), childName);
            }
            return nestedChild;
        }

        public void LoadExistingAnchors()
        {
            // need full model to load into
            if (null == fullModel)
                return;
//if WINDOWS_UWP      
            ids = this.was.GetAllIds().ToList<string>();
            if (deletingAnchors)
            {

                foreach (string aid in ids)
                {
                    DeleteWorldAnchor(aid);
                }
                //re get...
                ids = this.was.GetAllIds().ToList<string>();
            }
            //#endif

            AnchoredGameObjects agos = WorldAnchorManager.ReadData(this.anchorsFileName);

            int id = 0;
            // set up line points vector...
            Vector3[] linePoints = new Vector3[agos.anchorObjects.Count];
            foreach (AnchoredGameObject ago in agos.anchorObjects)
            {
                // get object from json...

                // get parent in scene...
                //GameObject parent = GameObject.Find(ago.ParentName);
                GameObject parent = WorldAnchorManager.GetNestedChild(fullModel.transform, ago.ParentName);

                if (null == parent)
                    return;
                // look up the type in the store
                //create the object
                // set pos, rot and scale...
                GameObject prefab = Resources.Load(ago.PrefabSource, typeof(GameObject)) as GameObject;
                GameObject go = GameObject.Instantiate(prefab);
                go.name += "_" + id++;
                
                go.transform.position = parent.transform.position;
                go.transform.SetParent(parent.transform);
                go.transform.localScale = ago.Scale;
                ago.SetScale(go);

                ago.SetToolTip(go);

                // Load anchor if running on UWP device and anchor exists...
                // TODO Add anchor script, NOTE need to set position of anchored object to zero and rotate...

                AnchoredGameObject newAgo = new AnchoredGameObject(go, ago.PrefabSource);
                newAgo.RunOrder = ago.RunOrder;
                newAgo.Description = ago.Description;
                gameObjectsToSerialize.Add(go.name, ago);
                gos.Add(go.name, go);

                // add line point at right location...
                linePoints[ago.RunOrder] = go.transform.position;

                // Show on load only if we are setup mode only...
                if (this.worldMode == Mode.Operate)
                {
                    Destroy(go.transform.Find("AppBar").gameObject);
                    //go.SetActive(false);
                }
            }

            // need to reload navlines...
            curvedLineRender.DoNavigationLine(this, linePoints);
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

        public static AnchoredGameObjects ReadData(string jsonFileRoot)
        {
            AnchoredGameObjects agos = new AnchoredGameObjects();
            string json = string.Empty;

            string path = string.Format("{0}/{1}.json", Application.persistentDataPath, jsonFileRoot);
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
                idxMgr.AddIndex(this.anchorsFileName, "<New Process>", this.instanceID);
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

            string path = string.Format("{0}/{1}.json", Application.persistentDataPath, this.anchorsFileName);

            string json = JsonConvert.SerializeObject(agoParent, Formatting.Indented);
            byte[] data = Encoding.ASCII.GetBytes(json);

            UnityEngine.Windows.File.WriteAllBytes(path, data);

#endif
        }

        public void SaveModelData()
        {
            var agos = gameObjectsToSerialize.Where(pair => (GameObject.Find(pair.Key) != null)).Select(pair => pair.Value).ToList();

            AnchoredGameObjects agoParent = new AnchoredGameObjects();

            foreach (AnchoredGameObject ago in agos)
                agoParent.anchorObjects.Add(ago);

            string path = string.Format("{0}/{1}.json", Application.persistentDataPath, this.ModelsFileName);

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
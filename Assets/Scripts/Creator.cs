using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
using Newtonsoft.Json;
using System.Text;
using Microsoft.MixedReality.Toolkit.Utilities;
using MST = Microsoft.MixedReality.Toolkit.UI;
using Scripts.AnchorObjects;
using Scripts.Json;
using System.Linq;

public class Creator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prefabObject;
    public Vector3 startPosition;
    public float scale = 1.0f;
    public int selectedModelIndex = 0;
    public ManagerBase wam;
   [HideInInspector]
    public enum ModelToLoad
    {
        NONE,
        TRAINING,
        FULL
    }

    public ModelToLoad modelType;
    public Mode mode;

    ProcessModels models = null;

    public void Start()
    {
        // TBD Remove...
        //if (modelType.Equals(ModelToLoad.TRAINING))
        //    CreateModelPrefab();
    }

    public Dictionary<string, AnchoredGameObject> gameObjectsToSerialize = new Dictionary<string, AnchoredGameObject>();

    //public ObjectLoader objectLoader;

    public string rootName = "Object";

    int index = 1;

    public void CreateModelPrefab()
    {
        // Deserialise the model data for the process...
        models = ObjectLoader.ReadProcessData();

        IndexManager idxMgr = GameObject.Find("GlobalManager").GetComponent<IndexManager>();

        int selectedModelIndex = (int)idxMgr.GetValue("processIndex");

        // query the model being asked for. 
        // select the main model and then which type to instantiate...
        ProcessModels.ProcessModel.Model model = models.processmodels.FirstOrDefault(ProcMod => ProcMod.Index == selectedModelIndex).models.FirstOrDefault(mod => mod.type.Equals(modelType.ToString()));

        // now get the type position...
        if (model != null)
        {
            Vector3 startPos = model.positions.FirstOrDefault(pos => pos.type.Equals(mode.ToString())).values;
            // get the process models...
            GameObject prefab = Resources.Load(model.prefabPath, typeof(GameObject)) as GameObject;
            Vector3 thisPosition = CameraCache.Main.transform.position + startPos;
            GameObject go = GameObject.Instantiate(prefab, thisPosition, Quaternion.identity) as GameObject;
            MST.ToolTip ttip = go.GetComponentInChildren<MST.ToolTip>();
            if (ttip != null)
                ttip.ToolTipText = model.tooltip;
        }
    }

    public void CreatePrefabInstance()
    {
        if (prefabObject != null)
        {
            PrefabData pd = prefabObject.GetComponent<PrefabData>();
            string prefPath = pd.prefabPath;
            // get the camera object and locate to the right...
            // rotate toward the camera....
            //transform.LookAt(transform.position);
            Vector3 thisPosition = CameraCache.Main.transform.position + startPosition;
            GameObject go = GameObject.Instantiate(prefabObject, thisPosition, Quaternion.identity) as GameObject;
            go.transform.localScale = go.transform.localScale * scale;
            string name = string.Empty;
            /*do
            {
                name = string.Format("{0}_{1}_{2}", rootName, wam.instanceID, index++);
            }
            while (GameObject.Find(name) != null);*/
            go.name = name;
            MST.ToolTip ttip = go.GetComponentInChildren<MST.ToolTip>();
            if (ttip != null)
                ttip.ToolTipText = string.Format("Temp Tooltip:\n{0}", name);
            if (wam != null)
                wam.gameObjectsToSerialize.Add(go.name, new AnchoredGameObject(go, prefPath));
        }
    }

    public void OnReturnToHub()
    {
        // remove all game objects...
        foreach(string key in wam.gameObjectsToSerialize.Keys)
        {
            GameObject clone = GameObject.Find(key);
            if (clone != null)
                Destroy(clone);
        }
        GameObject goIdxMgr = GameObject.Find("IndexManager");
        if (goIdxMgr != null)
        {
            IndexManager idxMgr = goIdxMgr.GetComponent<IndexManager>();
            idxMgr.MainPanelVisibility = true;
            idxMgr.Reload();
        }
        SceneManager.UnloadSceneAsync(1);
    }
}



using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
using MST = Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Scripts.Json;
using Scripts.AnchorObjects;

public class IndexManager : ManagerBase
{
    [Tooltip("Container for processes")]
    public GameObject processContainer;

    [Tooltip("Main panel for index manager")]
    public GameObject MainPanel;

    [Tooltip("Step Locator (used when Running process")]
    public GameObject stepLocator;

    public Dictionary<string, Index> indices = new Dictionary<string, Index>();
    public string fileName = "index";
    public int nextIndex = 1;

    public ObjectLoader idxMgr;

    const string IndexTemplate = "<TEMPLATE>";
    Index idxTemplate;

    Indices inds;

    Dictionary<string, object> globals = new Dictionary<string, object>();

    [Tooltip("DO NOT ADD TO THIS LIST IN THE INSPECTOR")]
    public List<string> currentKeys;

    void Start()
    {
        this.SetValue("processIndex", -1, true);

        currentKeys = globals.Keys.ToList();

        // switch off step locator...
        this.stepLocator.SetActive(false);
        LoadExistingIndices();
    }

    public void SetValue(string key, object value, bool replace)
    {
        if (!replace && globals.ContainsKey(key))
            throw new GlobalItemAlreadyExistsException(key);
        else
        {
            if (globals.ContainsKey(key))
                globals[key] = value;
            else
                globals.Add(key, value);
        }
    }

    public object GetValue(string key)
    {
        object value = null;
        if (!globals.ContainsKey(key))
            throw new GlobalItemDoesNotExistException(key);
        else
            value = globals[key];
            return value;
    }

    public void LoadExistingIndices()
    {
        inds = this.ReadData(this.fileName);

        float yoffset = 0.02f, height = 0.04f;
        // delete existing process objects
        foreach (Transform child in processContainer.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Index idx in inds.indexList)
        {
            if (idx.Name.Equals(IndexTemplate))
            {
                idxTemplate = idx;
                continue;
            }

            // work around to ensure index is unique...
            if (idx.InstanceID.Equals(-1))
                idx.InstanceID = nextIndex;
            //DeleteWorldAnchor(id);
            // look up the type in the store
            //create the object
            // set pos, rot and scale...
            // create prefab for preview...
            
            //GameObject previewPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath(idx.PrefabModel, typeof(GameObject)) as GameObject;
            GameObject previewPrefab = Resources.Load(idx.PrefabModel, typeof(GameObject)) as GameObject;

            // instantiate a new process prefab...
            //GameObject processPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath(idx.PrefabSource, typeof(GameObject)) as GameObject;
            GameObject processPrefab = Resources.Load(idx.PrefabSource, typeof(GameObject)) as GameObject;
            GameObject goProcess = GameObject.Instantiate(processPrefab, processContainer.gameObject.transform, false);
            //float height = goProcess.GetComponent<Collider>().bounds.size.y;
            //goProcess.transform.position = new Vector3(0.05f, yoffset * -1f * goProcess.transform.localScale.y, -0.002f);
            //goProcess.transform.position = new Vector3(goProcess.transform.position.x, yoffset * -1f, goProcess.transform.position.z);
            //yoffset += height;
            goProcess.transform.localScale = idx.Scale;
            //goProcess.transform.position = new Vector3(goProcess.transform.position.x, goProcess.transform.position.y - yoffset, goProcess.transform.position.z);
            goProcess.transform.localPosition = new Vector3(0f, -yoffset, -0.002f);
            yoffset += height;


            // set the filename to launch run scene...
            goProcess.GetComponent<ProcessLauncher>().processPath = idx.FilePath;
            goProcess.GetComponent<ProcessLauncher>().instanceID = idx.InstanceID;
            // set name and description...
            PanelData pd = goProcess.GetComponent<PanelData>();
            pd.Title.text = idx.Name;
            pd.Description.text = idx.Description;

            // instantiate a new 
            //GameObject goPreview = GameObject.Instantiate(prefab);
            //goPreview.transform.localScale = idx.Scale;
            //idx.SetToolTip(goPreview);
            nextIndex++;
        }
        this.instanceID = nextIndex;
    }

    public Indices ReadData(string fileName)
    {
        Indices inds = new Indices();
        string json = string.Empty;

        string path = string.Format("{0}/{1}.json", Application.persistentDataPath, fileName);
        if (UnityEngine.Windows.File.Exists(path))
        {
            byte[] data = UnityEngine.Windows.File.ReadAllBytes(path);
            json = Encoding.ASCII.GetString(data);

            inds = JsonConvert.DeserializeObject<Indices>(json);
        }

        return inds;
    }

    public void AddIndex(string filePath, string name, int instanceID)
    {
        Index existingIndex = inds.indexList.FirstOrDefault(idx => idx.FilePath == filePath);
        if (existingIndex == null)
        {
            Index newIdx = idxTemplate.Clone();
            newIdx.FilePath = filePath;
            newIdx.InstanceID = instanceID;
            newIdx.Name = name;
            inds.indexList.Add(newIdx);
        }
    }

    public bool MainPanelVisibility
    {
        get
        {
            bool isVisible = false;
            if (this.MainPanel != null)
            {
                isVisible = this.MainPanel.activeInHierarchy;
            }
            return isVisible;
        }
        set
        {
            if (this.MainPanel != null)
            {
                this.MainPanel.SetActive(value);
            }
        }
    }

    public void SaveIndexData()
    {
        string path = string.Format("{0}/{1}.json", Application.persistentDataPath, "index");

        string json = JsonConvert.SerializeObject(inds, Formatting.Indented);
        byte[] data = Encoding.ASCII.GetBytes(json);

        UnityEngine.Windows.File.WriteAllBytes(path, data);
    }

    public void Reload()
    {
        for( int i= 0;i < processContainer.transform.childCount; i++)
        {
            Destroy(processContainer.transform.GetChild(i).gameObject);
        }
        LoadExistingIndices();
    }
}

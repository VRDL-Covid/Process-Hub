using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Scripts.Json;

namespace Scripts.AnchorObjects
{
    public class ObjectLoader : MonoBehaviour
    {
        public string fileName = "anchordata";
        public const string processFileName = "processmodels";
        public int index = 1;
        public int processIndex = 0;
        private void Start()
        {
            // prevent erroneous process being loaded...
            PlayerPrefs.SetInt("processIndex", -1);

            if (PlayerPrefs.HasKey("jsonfilename"))
            {
                fileName = PlayerPrefs.GetString("jsonfilename");

                //find the WAM object and set the InstanceID
                ManagerBase wam = null;

                if (null != GameObject.Find("WAM"))
                {
                    wam = GameObject.Find("WAM").GetComponent<ManagerBase>();
                    wam.instanceID = PlayerPrefs.GetInt("instanceid");
                    wam.anchorsFileName = PlayerPrefs.GetString("jsonfilename");
                }
            }
        }

        public AnchoredGameObjects AnchorData
        {
            get
            {
                AnchoredGameObjects agos = new AnchoredGameObjects();
                string path = string.Format("{0}/{1}.json", Application.persistentDataPath, this.fileName);
                agos = ReadAnchorData(path);
                return agos;
            }
        }

        /// <summary>
        /// Reads the JSON from application folder on VR and HoloLens
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ProcessModels ReadProcessData()
        {
            string path = string.Format("{0}/{1}.json", Application.persistentDataPath, processFileName);

            ProcessModels models = new ProcessModels();

            if (File.Exists(path))
            {
                byte[] data = File.ReadAllBytes(path);
                string json = Encoding.ASCII.GetString(data);

                models = JsonConvert.DeserializeObject<ProcessModels>(json);
            }
            else
                models = null;
            return models;
        }

        /// <summary>
        /// Reads the JSON from application folder on VR and HoloLens
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static AnchoredGameObjects ReadAnchorData(string fileName)
        {
            AnchoredGameObjects agos = new AnchoredGameObjects();
            if (File.Exists(fileName))
            {
                byte[] data = File.ReadAllBytes(fileName);
                string json = Encoding.ASCII.GetString(data);

                agos = JsonConvert.DeserializeObject<AnchoredGameObjects>(json);
            }

            return agos;
        }

        public void SaveData(AnchoredGameObjects agos)
        {
            string path = string.Format("{0}/{1}.json", Application.persistentDataPath, this.fileName);

            string json = JsonConvert.SerializeObject(agos, Formatting.Indented);
            byte[] data = Encoding.ASCII.GetBytes(json);

            File.WriteAllBytes(path, data);
        }

        public void CreateNewProcess()
        {
            IndexManager idxMgr = GameObject.Find("GlobalManager").GetComponent<IndexManager>();

            string newProcessName = string.Format("{0}{1}", this.fileName, idxMgr.instanceID);
            PlayerPrefs.SetString("jsonfilename", newProcessName);
            PlayerPrefs.SetInt("instanceid", idxMgr.instanceID);
            idxMgr.MainPanelVisibility = false;
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        }
    }
}
   






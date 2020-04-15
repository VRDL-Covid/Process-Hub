using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Scripts.AnchorObjects;
using Scripts.Json;

namespace Scripts.Process
{
    public class ProcessLoader : MonoBehaviour
    {
        public GameObject buttonPrefab;
        public TMPro.TextMeshPro processNameTextBox;
        public TMPro.TextMeshPro processTypeTextBox;
        public AspectTransformer aspectTransformer;

        [Tooltip("Used to select a process from list.")]
        public int processIndex = 0;


        void Start()
        {
            // load process models...
            ProcessModels.ProcessModel processmodels = ObjectLoader.ReadProcessData().processmodels[processIndex];
            
            // get processes
            processNameTextBox.text = processmodels.Name;
            foreach (ProcessModels.ProcessModel.Model model in processmodels.models)
            {
                // create a process button and add to this object...
                GameObject go = GameObject.Instantiate(buttonPrefab, transform.position, Quaternion.identity) as GameObject;

                ProcessData pd = go.GetComponent<ProcessData>();
                
                // set the index and the title...
                pd.title.text = model.type;
                pd.prefabToLoad = model.prefabPath;
                pd.parentOfinstance = aspectTransformer.gameObject;
                // and parent to the collection...
                go.transform.parent = gameObject.transform;
                pd.ProcessTextBox = this.processTypeTextBox;

                // tell the button which aspect transformer to load when clicked...
                pd.aspectTransformer = this.aspectTransformer;
                pd.toolTipText = model.tooltip;

            }

            // finally sort into the grid...
            GridObjectCollection goc = gameObject.GetComponent<GridObjectCollection>();
            goc.UpdateCollection();
        }
    }
}

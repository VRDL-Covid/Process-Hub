using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Scripts.AnchorObjects;
using Scripts.Json;

namespace Scripts.Process
{
    public class ProcessScripts : MonoBehaviour
    {
        public GameObject processButtonPrefab;
        public TMPro.TextMeshPro processTextBox;

        void Start()
        {
            // read the process json from ObjectLoader..
            ProcessModels models = ObjectLoader.ReadProcessData();
            foreach (ProcessModels.ProcessModel model in models.processmodels)
            {
                // create a process button and add to this object...
                GameObject go = GameObject.Instantiate(processButtonPrefab, transform.position, Quaternion.identity) as GameObject;

                ProcessData pd = go.GetComponent<ProcessData>();
                
                // set the index and the title....
                pd.index = model.Index;
                pd.title.text = model.Name;
                // and parent to the collection...
                go.transform.parent = gameObject.transform;
                pd.ProcessTextBox = this.processTextBox;
            }

            // finally sort into the grid...
            GridObjectCollection goc = gameObject.GetComponent<GridObjectCollection>();
            goc.UpdateCollection();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

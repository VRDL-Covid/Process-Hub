using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;


namespace Scripts.Process
{
    public class ButtonGridLoader : MonoBehaviour
    {
        public GameObject buttonPrefab;
        public TMPro.TextMeshPro phraseTextBox;
        public AspectTransformer aspectTransformer;

        public string[] list = new string[0];

        void Start()
        {

            foreach (string item in list)
            {
                // create a process button and add to this object...
                GameObject go = GameObject.Instantiate(buttonPrefab, transform.position, Quaternion.identity) as GameObject;

                ProcessData pd = go.GetComponent<ProcessData>();
                
                // set the index and the title...
                pd.title.text = item;
                // and parent to the collection...
                go.transform.parent = gameObject.transform;
                pd.ProcessTextBox = this.phraseTextBox;
            }

            // finally sort into the grid...
            GridObjectCollection goc = gameObject.GetComponent<GridObjectCollection>();
            goc.UpdateCollection();
        }
    }
}

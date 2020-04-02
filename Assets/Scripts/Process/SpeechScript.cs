using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;


namespace Scripts.Process
{
    public class SpeechScript : MonoBehaviour
    {
        public GameObject speechPrefab;
        public TMPro.TextMeshPro phraseTextBox;
        public AspectTransformer aspectTransformer;

        void Start()
        {

            foreach (string phrase in aspectTransformer.keywords)
            {
                // create a process button and add to this object...
                GameObject go = GameObject.Instantiate(speechPrefab, transform.position, Quaternion.identity) as GameObject;

                ProcessData pd = go.GetComponent<ProcessData>();
                
                // set the index and the title....
                pd.title.text = phrase;
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

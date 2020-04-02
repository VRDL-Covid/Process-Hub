using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProcessData : MonoBehaviour
{
    public int index;
    public TextMeshPro title;
    public TMPro.TextMeshPro ProcessTextBox;

    public void OnSelectProcess()
    {

        IndexManager idxManager = GameObject.Find("GlobalManager").GetComponent<IndexManager>();
        ProcessTextBox.text = title.text;
        idxManager.SetValue("processIndex", index, true);
        //PlayerPrefs.SetInt("processIndex", index);
    }
}

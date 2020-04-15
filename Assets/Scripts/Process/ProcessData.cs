using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MST = Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;

public class ProcessData : MonoBehaviour
{
    public int index;
    public TextMeshPro title;
    public TMPro.TextMeshPro ProcessTextBox;
    public GameObject parentOfinstance;
    public AspectTransformer aspectTransformer;
    public string toolTipText = string.Empty;

    public string prefabToLoad;

    public void OnSelectProcess()
    {

        IndexManager idxManager = GameObject.Find("GlobalManager").GetComponent<IndexManager>();
        ProcessTextBox.text = title.text;
        idxManager.SetValue("processIndex", index, true);
        //PlayerPrefs.SetInt("processIndex", index);
    }

    public void OnSelectProcessModel()
    {
        if (null != parentOfinstance)
        {
            GameObject model = Resources.Load(this.prefabToLoad) as GameObject;
            if (model)
            {
                GameObject modelInstance = GameObject.Instantiate(model, CameraCache.Main.transform.position + new Vector3(0, 0, 1.5f), Quaternion.identity);

                PrefabData pd = modelInstance.GetComponent<PrefabData>();
                pd.prefabPath = this.prefabToLoad;
                aspectTransformer.targetTransform = modelInstance.transform;
                MST.ToolTip ttip = modelInstance.GetComponentInChildren<MST.ToolTip>();
                if (null != ttip)
                {
                    ttip.ToolTipText = toolTipText;
                }
            }
        }

    }

}

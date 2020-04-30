using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.AnchorObjects;
using Microsoft.MixedReality.Toolkit.UI;

[ExecuteInEditMode]
public class PrefabData : MonoBehaviour
{
    [Tooltip("Set this to the relative path to the prefab in the resources folder omit the extension.")]
    public string prefabPath = "";

    [Tooltip("Set to the preview Model that is viewed in the hub.")]
    public string modelPrefabPath = "";
    [Tooltip("Set to anchor data file for this process...")]
    public string anchorsFileName;
    [Tooltip("Tool tip for the model")]
    public string toolTipText;
    [Tooltip("Vector offset from main camera")]
    public Vector3 modelOffset = new Vector3(0, -0.5f, 0);

    GameObject modelPrefab;

    public void OnShowTrainingModel()
    {
        if (modelPrefabPath != string.Empty)
        {
            if (null == modelPrefab)
            {
                modelPrefab = Resources.Load(modelPrefabPath, typeof(GameObject)) as GameObject;
                var worldPos = Camera.main.transform.TransformPoint(modelOffset);
                GameObject model = GameObject.Instantiate(modelPrefab);
                model.transform.position = worldPos;
                LoadExistingAnchors(model.transform);

                ToolTip ttip = model.GetComponentInChildren<ToolTip>();
                if (ttip != null)
                    ttip.ToolTipText = toolTipText;
            }
        }
    }

    public void LoadExistingAnchors(Transform trans)
    {

        AnchoredGameObjects agos = Scripts.AnchorObjects.WorldAnchorManager.ReadData(this.anchorsFileName);

        int id = 0;
        // set up line points vector...
        Vector3[] linePoints = new Vector3[agos.anchorObjects.Count];
        foreach (AnchoredGameObject ago in agos.anchorObjects)
        {
            // get object from json...

            // get parent in scene...
            //GameObject parent = GameObject.Find(ago.ParentName);
            GameObject parent = Scripts.AnchorObjects.WorldAnchorManager.GetNestedChild(trans, ago.ParentName);

            if (null == parent)
                return;
            // look up the type in the store
            //create the object
            // set pos, rot and scale...
            GameObject prefab = Resources.Load(ago.PrefabSource, typeof(GameObject)) as GameObject;
            GameObject go = GameObject.Instantiate(prefab);
            go.transform.position = parent.transform.position;
            go.transform.SetParent(parent.transform);
            go.name += "_" + id++;
            go.transform.localScale = ago.Scale;
            ago.SetScale(go);
            ago.SetToolTip(go);

            // Load anchor if running on UWP device and anchor exists...
            // TODO Add anchor script, NOTE need to set position of anchored object to zero and rotate...

            AnchoredGameObject newAgo = new AnchoredGameObject(go, ago.PrefabSource);
            newAgo.RunOrder = ago.RunOrder;
            newAgo.Description = ago.Description;


        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.WSA;
using Scripts.AnchorObjects;

public class AnchorClient : MonoBehaviour
{
    WorldAnchorManager wam;

    public GameObject objToAnchor;
    // Start is called before the first frame update
    void Start()
    {
        wam = GameObject.Find("WAM").GetComponent<WorldAnchorManager>();
    }

    public void CreateWorldAnchor()
    {
        // need to update the object here....
        string prefabPath = string.Empty;
        if (wam.gameObjectsToSerialize.ContainsKey(objToAnchor.name))
        {
            prefabPath = wam.gameObjectsToSerialize[objToAnchor.name].PrefabSource;
            wam.gameObjectsToSerialize.Remove(objToAnchor.name);
        }

        wam.gameObjectsToSerialize.Add(objToAnchor.name, new AnchoredGameObject(objToAnchor, prefabPath));
        //wam.CreateWorldAnchor(objToAnchor);
    }

    public void DeleteWorldAnchor()
    {
        //wam.DeleteWorldAnchor(objToAnchor.name);
        Destroy(objToAnchor.GetComponent<WorldAnchor>());
    }

}

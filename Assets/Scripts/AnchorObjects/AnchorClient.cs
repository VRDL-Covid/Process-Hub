using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.WSA;
using Scripts.AnchorObjects;

namespace Scripts.AnchorObjects
{
    public class AnchorClient : MonoBehaviour
    {
        WorldAnchorManagerBlender wam;

        public GameObject objToAnchor;

        void Start()
        {
            GameObject oWAM = GameObject.Find("WAM");
            if (null != oWAM)
                wam = oWAM.GetComponent<WorldAnchorManagerBlender>();
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
            else
            {
                PrefabData pd = objToAnchor.GetComponent<PrefabData>();
                prefabPath = pd.prefabPath;
            }

            wam.gameObjectsToSerialize.Add(objToAnchor.name, new AnchoredGameObject(objToAnchor, prefabPath));
            wam.CreateWorldAnchor(objToAnchor);
        }

        public void DeleteWorldAnchor()
        {
            wam.DeleteWorldAnchor(objToAnchor.name);
            Destroy(objToAnchor.GetComponent<WorldAnchor>());
        }
    }
}

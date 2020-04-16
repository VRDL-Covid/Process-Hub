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
            else
            {
                PrefabData pd = objToAnchor.GetComponent<PrefabData>();
                prefabPath = pd.prefabPath;
            }

            wam.gameObjectsToSerialize.Add(objToAnchor.name, new AnchoredGameObject(objToAnchor, prefabPath));

        }

        public void DeleteWorldAnchor()
        {

        }
    }
}

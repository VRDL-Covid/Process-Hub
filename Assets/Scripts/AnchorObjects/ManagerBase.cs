using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scripts.AnchorObjects
{
    public class ManagerBase : MonoBehaviour
    {

        public Dictionary<string, AnchoredGameObject> gameObjectsToSerialize = new Dictionary<string, AnchoredGameObject>();
        public Dictionary<string, GameObject> gos = new Dictionary<string, GameObject>();

        public Scripts.LineObjects.CurvedLineRenderer curvedLineRender;

        public string anchorsFileName = "";
        public string ModelsFileName = "";
        public Mode worldMode = Mode.SetUp;

        [Tooltip("Instance ID is used to inject a unique ID to delineate each created object within a process")]
        public int instanceID = 0;

        [System.Serializable]
        public class GlobalItemAlreadyExistsException : System.Exception
        {
            public GlobalItemAlreadyExistsException(string key) : base(System.String.Format("key: '{0}' already exists", key)) { }
        }

        [System.Serializable]
        public class GlobalItemDoesNotExistException : System.Exception
        {
            public GlobalItemDoesNotExistException(string key) : base(System.String.Format("key: '{0}' does not exist", key)) { }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerBase : MonoBehaviour
{
    [Tooltip("Instance ID is used to inject a unique ID to delineate each created object within a process")]
    public int instanceID = 0;

    [System.Serializable]
    public class GlobalItemAlreadyExistsException : System.Exception
    {
        public GlobalItemAlreadyExistsException(string key) : base(System.String.Format("key: '{0}' already exists", key)) {}
    }

    [System.Serializable]
    public class GlobalItemDoesNotExistException : System.Exception
    {
        public GlobalItemDoesNotExistException(string key) : base(System.String.Format("key: '{0}' does not exist", key)) { }
    }
}

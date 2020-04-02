using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Scripts.AnchorObjects
{
    [ExecuteInEditMode]
    public class ObjectEvents : MonoBehaviour
    {
        public bool RaiseEvents { set; get; }
        public delegate void delDestroyHandler(object sender, ObjectEventArgs e);
        public event delDestroyHandler OnObjectDestroy;

        public string anchorName = string.Empty;
        public void OnDestroy()
        {
            ObjectEventArgs e = new ObjectEventArgs();
            e.AnchorName = string.Empty;

            // ensure we are attached to a game object and at least one client is listening to this event...
            if (gameObject != null && OnObjectDestroy != null)
            {
                e.AnchorName = anchorName;
                OnObjectDestroy(this, e);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Scripts.Json;

//#if WINDOWS_UWP
    using MST = Microsoft.MixedReality.Toolkit.UI;
    using Microsoft.MixedReality.Toolkit.Utilities;
//#endif

namespace Scripts.AnchorObjects
{
    public class AnchoredGameObject : IPosition
    {
        private string m_tooltipText = string.Empty;
        public Status StepStatus { get; set; } = Status.Incomplete;
        public Mode StepMode { get; set; } = Mode.SetUp;
        public string Name { get; set; }
        public string ParentName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int RunOrder { get; set; }
        public string PrefabSource { get; set; }
        public Vector3 Position { get; set; }
        public float RotateW { get; set; }
        public float RotateX { get; set; }
        public float RotateY { get; set; }
        public float RotateZ { get; set; }
        public string type { get; set; }
        public string ToolTipText { get; set; }
        public Vector3 Scale { get; set; }

        public List<AnchoredGameObject> Children = new List<AnchoredGameObject>();
        public List<string> MediaURLs = new List<string>();

        public AnchoredGameObject() { }

        public AnchoredGameObject(GameObject objToAnchor, string prefabPath) : this(objToAnchor, prefabPath, true) { }
  

        public AnchoredGameObject(GameObject objToAnchor, string prefabPath, bool addChildren)
        {
            this.PrefabSource = prefabPath;
            this.Name = objToAnchor.name;
            if (null != objToAnchor.transform.parent)
                this.ParentName = objToAnchor.transform.parent.name;
            this.Position = objToAnchor.transform.localPosition;
            this.RotateW = objToAnchor.transform.rotation.w;
            this.RotateX = objToAnchor.transform.rotation.x;
            this.RotateY = objToAnchor.transform.rotation.y;
            this.RotateZ = objToAnchor.transform.rotation.z;
            this.Scale = objToAnchor.transform.localScale;
            if (addChildren)
                this.AddChildren(objToAnchor);

//#if WINDOWS_UWP
            MST.ToolTip ttip = objToAnchor.GetComponentInChildren<MST.ToolTip>();
            if (ttip != null)
                ToolTipText = ttip.ToolTipText;
//#endif
        }

        public void StoreGameObjectParameters()
        {

        }

        public void SetScale(GameObject go)
        {
            foreach (AnchoredGameObject oChild in this.Children)
            {
                foreach (Transform t in go.transform)
                {
                    if (t.transform.gameObject.name == oChild.Name)
                    {
                        t.transform.gameObject.transform.localScale = oChild.Scale;
                        break;
                    }
                }
            }
        }


        public void SetToolTip(GameObject go)
        {
//#if WINDOWS_UWP
            MST.ToolTip ttip = go.GetComponentInChildren<MST.ToolTip>();
            if (ttip != null)
                ttip.ToolTipText = this.ToolTipText;
//#endif
        }


            public void AddWaypoint(GameObject parent, Holograms.Hologram holDef)
        {
            string[] waypointobjects = holDef.WayPointObject.Split('/');
            
            foreach (Transform t in parent.transform)
            {
                //if (t.gameObject.name in )
                if (t.gameObject.tag != "NotSerialised" && t.gameObject.name != "rigRoot")
                    Children.Add(new AnchoredGameObject(t.gameObject, ""));
            }
        }

        public void AddChildRecursive(GameObject parent, List<string> children)
        {
            string child = string.Empty;
            if (children.Count == 0)
                return;
            child = children[0];// hildren.Select(item => item.First()).;
            children.Remove(child);
            foreach (Transform t in parent.transform)
            {
                if (t.gameObject.name == child)
                {
                    //AddChildren(parent);
                    AnchoredGameObject childAgo = new AnchoredGameObject(t.gameObject, "", false);
                    Children.Add(childAgo);
                    childAgo.AddChildRecursive(t.gameObject, children);
                    break;
                }
            }
        }

        public void AddChildren(GameObject parent)
        {
            foreach (Transform t in parent.transform)
            {
                if (t.gameObject.tag != "NotSerialised" && t.gameObject.name != "rigRoot")
                    Children.Add(new AnchoredGameObject(t.gameObject, ""));
            }
        }
    }
}

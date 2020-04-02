using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MST = Microsoft.MixedReality.Toolkit.UI;
//using ExtensionMethods;

namespace Scripts.Json
{
    [System.Serializable]
    public class Index
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public int InstanceID { get; set; }
        public int Order { get; set; }
        public string PrefabModel { get; set; }
        public string PrefabSource { get; set; }
        public Vector3 Scale { get; set; }
        public double RotateW { get; set; }
        public double RotateX { get; set; }
        public double RotateY { get; set; }
        public double RotateZ { get; set; }
        public string ToolTipText { get; set; }
        public Vector3 Position { get; set; }

        //public void SetToolTip(GameObject go)
        //{
        //    MST.ToolTip ttip = go.GetComponentInChildren<MST.ToolTip>();
        //    if (ttip != null)
        //        ttip.ToolTipText = this.ToolTipText;
        //}

        public Index Clone()
        {
            Index idx = new Index
            {
                InstanceID = InstanceID,
                Name = Name,
                Description = Description,
                FilePath = FilePath,
                Order = Order,
                Scale = Scale,
                PrefabModel = PrefabModel,
                PrefabSource = PrefabSource,
                RotateW = RotateW,
                RotateX = RotateX,
                RotateY = RotateY,
                RotateZ = RotateZ,
                ToolTipText = ToolTipText,
                Position = Position
            };
            return idx;

        }
    }

    public class Indices
    {
        private string name = string.Empty;
        private string description = string.Empty;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public List<Index> indexList = new List<Index>();
    }
}

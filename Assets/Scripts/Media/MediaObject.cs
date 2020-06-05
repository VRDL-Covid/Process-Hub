using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Scripts.AnchorObjects;
namespace Scripts.Media
{
    public enum enmMediaType
    {
        INVALID = -1,
        BLEND,
        FBX,
        TMPRO,
        MP4
    }

    public struct MediaSettings
    {
        public enmMediaType Mediatype { get; set; }
        public string PrefabPath { get; set; }
        public string ContainerObjectName { get; set; }
    }

    public class MediaObject
    {
        private string m_tooltipText = string.Empty;
        private System.Uri MediaLocation { get; set; }
        public string Name { get; set; }
        public int RunOrder { get; set; }
        public string PrefabSource { get; set; }
        public Vector3 Position { get; set; }
        public float RotateW { get; set; }
        public float RotateX { get; set; }
        public float RotateY { get; set; }
        public float RotateZ { get; set; }
        //public enmMediaType Mediatype { get; set; }
        public string ToolTipText { get; set; }
        public Vector3 Scale { get; set; }
        public AnchoredGameObject MediaContainer { get; set; }
        public MediaSettings mediaSettings = new MediaSettings();

        public void SetMediaSettingsForExtension(string ext)
        {
            this.mediaSettings.Mediatype = enmMediaType.INVALID;
            this.mediaSettings.PrefabPath = string.Empty;
            switch (ext.ToLower())
            {
                case ".fbx":
                    this.mediaSettings.Mediatype = enmMediaType.FBX;
                    this.mediaSettings.PrefabPath = "ProcessPrefabs/MediaContainer";
                    this.mediaSettings.ContainerObjectName = "Container";
                    break;
                case ".blend":
                    this.mediaSettings.Mediatype = enmMediaType.BLEND;
                    this.mediaSettings.PrefabPath = "ProcessPrefabs/MediaContainer";
                    this.mediaSettings.ContainerObjectName = "Container";
                    break;
                case ".mp4":
                    this.mediaSettings.Mediatype = enmMediaType.MP4;
                    break;
                case ".txt":
                case ".xml": // asume this is properly transformed...
                    this.mediaSettings.Mediatype = enmMediaType.TMPRO;
                    break;
            }
        }
    }
}

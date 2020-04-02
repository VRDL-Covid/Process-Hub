using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

namespace Scripts.AnchorObjects
{
    public class AnchoredGameObjects// : MonoBehaviour
    {
        [JsonIgnore]
        public AnchorNames anchorNames { get; set; }
        //public AnchorObjectCommons anchorCommons = new AnchorObjectCommons();

        private string description = string.Empty;

        public string Name { get; set; }


        public string Description { get; set; }


        public List<AnchoredGameObject> anchorObjects = new List<AnchoredGameObject>();

        [JsonIgnore]
        public AnchorNames AnchorNames
        {
            get
            {
                return anchorNames;
            }
            set
            {
                anchorNames = value;
            }
        }

        public void AddAnchor(AnchoredGameObject ago)
        {
            if (!anchorObjects.Contains(ago))
            {
                anchorObjects.Add(ago);
               //foreach(GameObject in ago.Children)
                anchorNames.Names = anchorObjects.Select(item => item.Name).ToArray();
            }
        }

        public void AddAnchor(AnchoredGameObject ago, GameObject child)
        {
            if (!anchorObjects.Contains(ago))
            {
                ago.Children.Add(new AnchoredGameObject(child, ""));
                anchorObjects.Add(ago);

                anchorNames.Names = anchorObjects.Select(item => item.Name).ToArray();
            }
        }
    }
}
            


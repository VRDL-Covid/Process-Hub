using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Scripts.Json
{
    [System.Serializable]
    public class SpatialCoordinates
    {
        public Marker me = new Marker();
        public List<Marker> markers = new List<Marker>();

        [System.Serializable]
        public class Marker
        {
            public string Name;
            public Vector3 position;
            public Vector3 scale;
            public Vector3 lengths = new Vector3(1, 1, 1);
            public Vect4 rotation = new Vect4();

            public void SetRotation(Quaternion rot)
            {
               this.rotation.values.w = rot.w;
               this.rotation.values.x = rot.x;
               this.rotation.values.y = rot.y;
               this.rotation.values.z = rot.z;
            }

            [JsonIgnore]
            public Quaternion Rotation
            {
                get
                {
                    Quaternion qtn = new Quaternion();
                    qtn.w = this.rotation.values.w;
                    qtn.x = this.rotation.values.x;
                    qtn.y = this.rotation.values.y;
                    qtn.z = this.rotation.values.z;
                    return qtn;
                }
            }

        }
        [System.Serializable]
        public class Vect4
        {
            public Vector4 values;
        }

        [System.Serializable]
        public class Vect3
        {
            public string type;
            public bool worldspace;
            public Vector3 values;
        }
    }
}
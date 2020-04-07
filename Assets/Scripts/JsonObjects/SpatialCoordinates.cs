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
               this.rotation.values.x = rot.y;
               this.rotation.values.y = rot.y;
               this.rotation.values.z = rot.z;
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
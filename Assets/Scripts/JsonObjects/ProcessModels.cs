using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Scripts.Json
{

    [System.Serializable]
    public class ProcessModels
    {
        public ProcessModel[] processmodels;

        [System.Serializable]
        public class ProcessModel
        {
            public Model[] models;
            public string Name;
            public string RootName;
            public int Index;

            [System.Serializable]
            public class Model
            {
                public string type;
                public string prefabPath;
                public string tooltip;
                public Vector3 initialPositionOffset;
                public Vector3 initialScale;
                public Vect3[] positions;
                public Vect3[] scales;
                public Vect4[] rotations;
            }

            [System.Serializable]
            public class Vect4
            {
                public string type;
                public bool worldspace;
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
}
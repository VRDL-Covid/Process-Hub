using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Scripts.Json
{
    [System.Serializable]
    public class Categories
    {
        public Category[] categories;

        [System.Serializable]
        public class Category
        {
            public string Name;
            public int Index;
        }
    }

    [System.Serializable]
    public class Holograms
    {
        public Hologram[] holograms;

        [System.Serializable]
        public class Hologram
        {
            public string Name;
            public int Index;
            public string ResourcePath;
            public string RuntimeObject;
            public string WayPointObject;
        }
    }
}


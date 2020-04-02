using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Scripts.AnchorObjects
{
    public class AnchorObjectCommons
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
            get { return name; }
            set { name = value; }
        }
    }

    public class DataSetDirectory
    {
        public List<DataSet> dataSets = new List<DataSet>();
    }

    public class DataSet
    {
        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}

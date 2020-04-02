using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonScreenObjects
{
    [Serializable]
    public class ScreenObjects
    {
        public ScreenObject[] screenObjects;

        [Serializable]
        public class ScreenObject
        {
            public string ScreenName;
            public string IP;
            public int Port;
            public int ScreenPosition;
            public bool Enabled = false;
            public int ConnectionIntms;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Scripts.AnchorObjects
{
    public class ObjectEventArgs : EventArgs
    {
        public string AnchorName { set; get; }
    }
}

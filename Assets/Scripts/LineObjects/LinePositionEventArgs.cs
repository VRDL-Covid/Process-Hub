using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Scripts.LineObjects
{
    public class LinePositionEventArgs : EventArgs
    {
        public Vector3 Position { get; set; }
        public int Order { set; get; }
    }
}

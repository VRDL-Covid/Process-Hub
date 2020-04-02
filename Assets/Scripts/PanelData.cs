using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class PanelData : MonoBehaviour
{
    public TextMeshPro Title;
    public TextMeshPro Description;
    public TextMeshPro Status;
    [Tooltip("Used to enable camera tracking")]
    public Orbital orbital;
    public Vector3 PanelOffsetTarget = new Vector3(0,0,0);
}

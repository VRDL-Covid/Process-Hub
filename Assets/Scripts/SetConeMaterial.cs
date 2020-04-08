using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetConeMaterial : MonoBehaviour
{
    public GameObject markerCone;

    public Material ConeMaterial
    {
        set
        {
            if (null != value && null != markerCone)
            {
                markerCone.GetComponent<Renderer>().material = value;

            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

public class Facer : MonoBehaviour
{
    // Start is called before the first frame update
    public bool keepFacingCamera;

    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (keepFacingCamera)
            KeepFacingTheCamera();
    }

    private void KeepFacingTheCamera()
    {
        transform.LookAt(CameraCache.Main.transform.position);
    }
}

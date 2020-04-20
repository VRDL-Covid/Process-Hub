using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

public class Facer : MonoBehaviour
{
    // Start is called before the first frame update
    public bool keepFacingCamera;

    //public Vector3 rotationOffset = new Vector3(0, 180, 0);

    void Start()
    {
        //this.transform.eulerAngles = rotationOffset;
    }

    // Update is called once per frame
    private void Update()
    {
        if (keepFacingCamera)
            KeepFacingTheCamera();
    }

    private void KeepFacingTheCamera()
    {
        transform.LookAt(2 * transform.position - CameraCache.Main.transform.position);
    }
}

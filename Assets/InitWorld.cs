using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitWorld : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        // remove player settings...
        PlayerPrefs.DeleteKey("instanceid");
        PlayerPrefs.DeleteKey("jsonfilename");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

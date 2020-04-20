using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    System.Uri uri;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LaunchURI()
    {
        string uriToLaunch = @"http://www.bing.com";

        // Create a Uri object from a URI string 
        uri = new System.Uri(uriToLaunch);
    }


    // Launch the URI
    async void DefaultLaunch()
    {
        // Launch the URI
        Application.OpenURL(uri.ToString());
    }

}

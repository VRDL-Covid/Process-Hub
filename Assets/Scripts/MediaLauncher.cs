using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediaLauncher : MonoBehaviour
{

   public UnityEngine.Video.VideoPlayer videoPlayer;

    public string url = string.Empty;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayMP4()
    {
        videoPlayer.Stop();
        videoPlayer.source = UnityEngine.Video.VideoSource.Url;
        videoPlayer.isLooping = false;
        videoPlayer.url = url;
        videoPlayer.Play();
    }

    public void LaunchURI()
    {
        string uriToLaunch = @"http://www.bing.com";

        // Create a Uri object from a URI string 
        //uri = new System.Uri(uriToLaunch);
        DefaultLaunch();
    }


    // Launch the URI
    void DefaultLaunch()
    {
        // Launch the URI
        //Application.OpenURL(uri.ToString());
    }

}

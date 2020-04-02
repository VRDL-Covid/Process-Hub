﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scripts.AnchorObjects;

public class ProcessLauncher : MonoBehaviour
{
    [Tooltip("The name of the json file in the persistent data folder, usually set by script")]
    public string processPath;

    [Tooltip("The unique ID of this process")]
    public int instanceID;

    public void LaunchProcess()
    {   
        IndexManager idxMgr = GameObject.Find("IndexManager").GetComponent<IndexManager>();

        PlayerPrefs.SetString("jsonfilename", processPath);
        PlayerPrefs.SetInt("instanceid", instanceID);
        idxMgr.MainPanelVisibility = false;
        idxMgr.stepLocator.SetActive(true);
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }

    public void EditProcess()
    {
        IndexManager idxMgr = GameObject.Find("IndexManager").GetComponent<IndexManager>();

        PlayerPrefs.SetString("jsonfilename", processPath);
        PlayerPrefs.SetInt("instanceid", instanceID);
        idxMgr.MainPanelVisibility = false;
        idxMgr.stepLocator.SetActive(false);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }
}

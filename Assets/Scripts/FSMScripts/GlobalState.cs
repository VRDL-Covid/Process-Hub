using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Scripts.AnchorObjects;

#if WINDOWS_UWP
    using Microsoft.MixedReality.Toolkit.Utilities;
#endif

public class GlobalState : MonoBehaviour
{
    // used to store the state for the entire process...
    public WorldAnchorManagerBlender wam;
    [HideInInspector]
    public List<AnchoredGameObject> orderedList;
    [HideInInspector]
    public int currentProcessStep = 0;
    [HideInInspector]
    public int stepIndex;
    [HideInInspector]
    public int[] orderToInspect;

    [Tooltip("References the main panel used to set the status for the current step")]
    public GameObject MainPanel;
    [Tooltip("References the current for the state machine normally set in the FSM")]
    public GameObject target;
    [Tooltip("References the Step Locator used to guide the user to the target")]
    public GameObject stepLocator;
    [Tooltip("The prefab to place above a locatable item")]
    public GameObject spinnerPrefab;

    [Tooltip("References tshe data object used to setup the panel")]
    public PanelData panelData;

    [HideInInspector]
    public AnchoredGameObject dataForStep;

    [HideInInspector]
    public bool closeStepEvent = false;

    [HideInInspector]
    public GameObject spinnerInstance;

    private StepIndicator stepIndicator = null;

    /// <summary>
    /// This method should only be called at the begining of the process once
    /// </summary>
    public void Initialise()
    {
         if (wam == null)
            wam = GameObject.Find("WAM").GetComponent<WorldAnchorManagerBlender>();
#if WINDOWS_UWP
         stepIndicator = CameraCache.Main.transform.GetComponentInChildren<StepIndicator>();
#else
        Camera camera = (Camera)GameObject.FindObjectOfType(typeof(Camera));
        stepIndicator = camera.transform.GetComponentInChildren<StepIndicator>();
#endif

        wam.anchorsFileName = PlayerPrefs.GetString("jsonfilename");
        wam.instanceID = PlayerPrefs.GetInt("instanceid");
#if WINDOWS_UWP
        wam.InitiateAnchorLoad();
        wam.LoadExistingAnchors();
#endif
        // sort the list in the designated order to review in the world...
        orderedList = wam.gameObjectsToSerialize.Values.ToList<AnchoredGameObject>();
        
        // sort the list on ascending run order...
        orderedList.Sort((a, b) => (a.RunOrder.CompareTo(b.RunOrder)));
        
        // get the array of run order into a standard list of ints
        // this is used to allow us to simply index into the list of objects
        // using a simple incremented value stored in stepIndex (see below for implementation in Linq)...
        orderToInspect = orderedList.Select(x => x.RunOrder).ToArray();
    }

    /// <summary>
    // this is an indirect look up that takes the orderto inspect as a list of values and uses
    // stepIndex to get the runOrder No and uses this No to query the orderedList collection.
    // it uses FirstOrDefault so as not to error here, but should be checked for null by an
    // object that calls it...
    /// </summary>
    public AnchoredGameObject DataForStep
    {
        get
        {
            dataForStep = orderedList.Where(ol => orderToInspect.Any(oti => ol.RunOrder == oti && orderToInspect[stepIndex] == oti)).FirstOrDefault();
            return dataForStep;
        }
    }

    public GameObject SetActivateStateForObject(string name, bool state)
    {
        GameObject target = wam.SetActivateStateForObject(name, state);
        return target;
    }

    public void OnCloseEvent()
    {
        closeStepEvent = true;
    }

    public GameObject TargetMainItem
    {
        // gets the central component of the target instance...
        get
        {
            GameObject targetMI = null;
            if (this.target != null)
                targetMI = this.target.GetComponent<MainItem>().Item;
            return targetMI;

        }
    }
}
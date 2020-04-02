using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Scripts.Json;
using Scripts.AnchorObjects;

public class SetStatus : MonoBehaviour
{
    public Status selectedStatus = Status.Incomplete;
    public TextMeshPro statusField;
    public GlobalState globalState;
    // Start is called before the first frame update

    public void OnSetStatus()
    {
        // raise an event or something...
        if (statusField != null && globalState != null)
        {
            statusField.text = selectedStatus.ToString();
            globalState.dataForStep.StepStatus = selectedStatus;
        }
    }

}
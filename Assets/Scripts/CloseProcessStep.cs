using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CloseProcessStep : MonoBehaviour
{
    [Header("Event to handle Close out.")]
    public UnityEvent closeEvent;

    // Start is called before the first frame update
    public void OnClose()
    {
        if (closeEvent != null)
            closeEvent.Invoke();
    }
}

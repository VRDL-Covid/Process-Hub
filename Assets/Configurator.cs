using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.AnchorObjects;
using System.Reflection;

// class used to configure the object in different mode.


public class Configurator : MonoBehaviour
{

    public Mode modeToRemoveObjectsIn = Mode.Operate;

    [Tooltip("Check this if you need the configurator to turn items off or remove compenents when a scene has no World Anchor Manager")]
    public bool overrideMode = false;

    [Tooltip("Items in this list will be destroyed")]
    public GameObject[] removeGameObjectInMode;
    [Tooltip("Components in this list will be removed")]
    public Component[] disableComponentInMode;


    void Start()
    {
        WorldAnchorManager wam = null;
        GameObject oWAM = GameObject.Find("WAM");
        if (null != oWAM)
            wam = oWAM.GetComponent<WorldAnchorManager>();

        if (overrideMode || wam != null && modeToRemoveObjectsIn == wam.worldMode)
        {
            foreach(GameObject go in removeGameObjectInMode)
            {
                GameObject.Destroy(go);
            }

            foreach (Component comp in disableComponentInMode)
            {
                System.Type typ = comp.GetType();
                PropertyInfo enabledInf = typ.GetProperty("enabled");
                if (enabledInf != null)
                {
                    enabledInf.SetValue(comp, false);
                }
            }
        }
    }

    void Update()
    {
        
    }
}

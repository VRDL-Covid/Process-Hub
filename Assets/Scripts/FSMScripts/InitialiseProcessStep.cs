using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.AnchorObjects;

#if WINDOWS_UWP
	using MST = Microsoft.MixedReality.Toolkit.UI;
	using Microsoft.MixedReality.Toolkit.Utilities;
#endif

public class InitialiseProcessStep : ProcessBase
{
    bool processStarted = false;
    // initialise all the references to the process
    // need to get a reference to the world anchor manager that holds all the 
    // anchors and objects that are related to them.
    // each object is hidden until required for use...

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("startProcess", false);
        animator.SetBool("stepComplete", false);
        animator.SetBool("endProcess", false);
        animator.SetBool("leaveProcess", false);
        base.OnStateEnter(animator, stateInfo, layerIndex);

        gs.stepIndex++;
        if (gs.stepIndex < gs.orderToInspect.Length)
        {
            AnchoredGameObject ago = gs.DataForStep;
            gs.MainPanel.SetActive(false);
            gs.stepLocator.SetActive(true);

            if (ago != null)
            {
                // get the target for this step...
                gs.target = gs.SetActivateStateForObject(ago.Name, true);

                // set the panel to the offset from the camera...
#if WINDOWS_UWP
                gs.MainPanel.transform.position = CameraCache.Main.transform.position + gs.panelData.PanelOffsetTarget;
#else
                // get the camera position...
                Camera camera = (Camera)GameObject.FindObjectOfType(typeof(Camera));
                gs.MainPanel.transform.position = camera.transform.position + gs.panelData.PanelOffsetTarget;
#endif
                //gs.MainPanel.transform.position = gs.target.transform.position + gs.panelData.PanelOffsetTarget;
                //gs.MainPanel.transform.LookAt(CameraCache.Main.transform.position);

                // move state machine to state for proximity detection...
                animator.SetBool("moveToNextProcessStep", true);
            }
        }
        else
        {
            // end process...
            animator.SetBool("endProcess", true);
        }

    }
}





using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConductStep : ProcessBase
{

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool("atProcessStepLocation", false);

        //we're here until the user clicks Close on the dialog, 
        //switch off the position indicator and switch on the information panel...
        gs.stepLocator.SetActive(false);
        gs.MainPanel.SetActive(true);

        //enable camera tracking...
#if WINDOWS_UWP
        gs.panelData.orbital.enabled = true;
#endif  
        gs.panelData.Title.text = gs.dataForStep.Name;
        gs.panelData.Description.text = gs.dataForStep.Description;
        gs.panelData.Status.text = gs.dataForStep.StepStatus.ToString();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (gs.closeStepEvent)
        {
            gs.closeStepEvent = false;
            animator.SetBool("stepComplete", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("stepComplete", false);
    }

}

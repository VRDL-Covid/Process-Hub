using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scripts.AnchorObjects;

public class EndProcess : ProcessBase
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("endProcess", false);
        base.OnStateEnter(animator, stateInfo, layerIndex);

        gs.panelData.Description.text = "";
        gs.panelData.Status.text = "You have Completed the Process, click Continue to return to Hub";
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (gs.closeStepEvent)
        {
            gs.closeStepEvent = false;
            OnReturnToHub();

            animator.SetBool("leaveProcess", true);
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    public void OnReturnToHub()
    {
        // remove all game objects...
        foreach (AnchoredGameObject ago in gs.orderedList)
        {

            GameObject clone = GameObject.Find(ago.Name);
            if (clone != null)
                Destroy(clone);
        }
        GameObject goIdxMgr = GameObject.Find("IndexManager");
        gs.MainPanel.SetActive(false);
        gs.stepLocator.SetActive(false);
        if (gs.spinnerInstance != null)
            Destroy(gs.spinnerInstance);
        if (goIdxMgr != null)
        {
            IndexManager idxMgr = goIdxMgr.GetComponent<IndexManager>();
            idxMgr.MainPanelVisibility = true;
            idxMgr.Reload();
        }
        SceneManager.UnloadSceneAsync(2);
    }
}

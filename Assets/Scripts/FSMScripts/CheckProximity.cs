using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if WINDOWS_UWP
    using Microsoft.MixedReality.Toolkit.Utilities;
#endif
public class CheckProximity : ProcessBase
{
    public float maxRange = 1f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool("moveToNextProcessStep", false);

        // create a spinner prefab and locate above target, if set...
        if (gs.TargetMainItem != null)
        {
            // destroy previous instance...
            if (gs.spinnerInstance != null)
                Destroy(gs.spinnerInstance);
            Transform spinTx = gs.TargetMainItem.transform;
            //spinTx.position = new Vector3(0, 0.2f, 0);
            //gs.spinnerInstance = GameObject.Instantiate(gs.spinnerPrefab, spinTx, true);
            gs.spinnerInstance = GameObject.Instantiate(gs.spinnerPrefab);//, new Vector3(0, 1f, 0), gs.TargetMainItem.transform.rotation);
            gs.spinnerInstance.transform.position = new Vector3(spinTx.position.x, spinTx.position.y + 0.5f, spinTx.position.z);

        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit
   override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
#if WINDOWS_UWP
        var heading = CameraCache.Main.transform.position - gs.target.transform.position;
#else
        var heading = GameObject.Find("Camera").transform.position - gs.target.transform.position;
#endif
        var distance = heading.magnitude;
        // are we within limits of target?
        Debug.Log(heading.magnitude);
        if (heading.magnitude < maxRange)
        {
            // inside the area so we need to transit to conduct the process step, where we will switch off the position indicator and switch on the information panel...
            animator.SetBool("atProcessStepLocation", true);
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
}

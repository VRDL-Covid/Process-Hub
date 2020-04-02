using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.AnchorObjects;

public class InitialiseProcess : ProcessBase
{
    bool processStarted = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gs = animator.GetComponentInChildren<GlobalState>();
        gs.Initialise();
        // get the first item in the list of items to inspect/visit...
        AnchoredGameObject ago = gs.orderedList.FirstOrDefault();
        if (ago != null)
        {
            Debug.Log("======STARTING PROCESS =========");
            // store current run item
            // start at -1 as next state increments and needs
            // to be correct for first state...
            gs.stepIndex = -1;

            if (gs.orderToInspect.Count() > 0)
            {
                // move to the next state...
                animator.SetBool("startProcess", true);
            }
            else
            {
                // no steps to undertake so exit...
                animator.SetBool("endProcess", true);
            }

        }
    }

}


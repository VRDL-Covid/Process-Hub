using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProcessBase : StateMachineBehaviour
{
    protected GlobalState gs;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        gs = animator.transform.GetComponent<GlobalState>();
    }

}



using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

namespace Scripts
{
    public class FollowMeSolverToggle : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("An opional object for visualizing the carry mode state")]
        private GameObject visualizationObject = null;

        public Microsoft.MixedReality.Toolkit.Utilities.Solvers.Solver solver = null;

        private void Start()
        {


        }

        public void ToggleFollowMeBehavior()
        {
            if (solver != null)
            {
                solver.enabled = !solver.enabled;

                if(visualizationObject != null)
                {
                    visualizationObject.SetActive(solver.enabled);
                }
            }

        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class ControlManipulation : MonoBehaviour
{

    public bool showBoundingBox;

    public Solver solver;

    public GameObject target;
    // Start is called before the first frame update
    private BoundingBoxHelper helper = new BoundingBoxHelper();
    private List<Vector3> boundsPoints = new List<Vector3>();

    #region Private Serialized Fields
    [Header("Target Bounding Box")]
    [SerializeField]
    private BoundingBox boundingBox = null;
    #endregion

    private void Start()
    {
        if (null == target)
            return;
        if (null == boundingBox)
        {
            boundingBox = target.GetComponent<BoundingBox>();

            // switch off the solver and never switch back (for now)...
            if (null == solver)
            {
                solver = target.GetComponent<Solver>();
            }
        }
    }

    public void ToggleBoundingBox()
    {
        // BoundingBox can't update in editor mode
        if (!Application.isPlaying)
            return;

        if (boundingBox == null)
            return;
        showBoundingBox = !showBoundingBox;
        boundingBox.Active = showBoundingBox;
        if (null != solver && solver.enabled)
        {
            solver.enabled = false;
        }
    }
}

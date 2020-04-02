using UnityEngine;
using System.Collections;

namespace Scripts.LineObjects
{
    public class CurvedLinePoint : MonoBehaviour
    {
        [HideInInspector] public bool showGizmo = true;
        [HideInInspector] public float gizmoSize = 0.1f;
        [HideInInspector] public Color gizmoColor = new Color(1, 0, 0, 0.5f);
        [HideInInspector] public int order = 0;

        void OnDrawGizmos()
        {
            if (showGizmo == true)
            {
                Gizmos.color = gizmoColor;

                Gizmos.DrawSphere(this.transform.position, gizmoSize);
            }
        }

        //update parent line when this point moved
        void OnDrawGizmosSelected()
        {
            CurvedLineRenderer curvedLine = this.transform.parent.GetComponent<CurvedLineRenderer>();

            if (curvedLine != null)
            {
                curvedLine.Update();
            }
        }
    }  
}

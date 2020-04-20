﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scripts.AnchorObjects;

namespace Scripts.LineObjects
{
    [RequireComponent(typeof(LineRenderer))]
    public class CurvedLineRenderer : MonoBehaviour
    {
        //PUBLIC
        public float lineSegmentSize = 0.15f;
        public float lineWidth = 0.1f;
        [Header("Gizmos")]
        public bool showGizmos = true;
        public float gizmoSize = 0.1f;
        public Color gizmoColor = new Color(1, 0, 0, 0.5f);
        //PRIVATE
        private CurvedLinePoint[] linePoints = new CurvedLinePoint[0];
        private Vector3[] linePositions = new Vector3[0];
        private Vector3[] linePositionsOld = new Vector3[0];

        [Header("When Set to true, requires AnchorObjects ")]
        public bool LinePointsFromAnchors = false;

        public List<Vector3> AnchorPositions;
        public List<AnchoredGameObjects> Anchors;

        public void DoNavigationLine(object sender, Vector3[] linePositions)
        {

            // redo order...
            this.linePositions = linePositions;
            SetPointsToLine(true);
            Update();
        }

        // Update is called once per frame
        public void Update()
        {
            //GetPoints();
            SetPointsToLine();
        }

        void GetPoints()
        {
            //add positions
            linePositions = new Vector3[linePoints.Length];
            for (int i = 0; i < linePoints.Length; i++)
            {
                linePositions[i] = linePoints[i].transform.position;
            }
        }
        void SetPointsToLine()
        {
            SetPointsToLine(false);
        }
        void SetPointsToLine(bool forceUpdate)
        {
            bool moved = false;

            //override if requested...
            moved = forceUpdate;
            //create old positions if they dont match
            if (linePositionsOld.Length != linePositions.Length)
            {
                linePositionsOld = new Vector3[linePositions.Length];
            }

            //check if line points have moved
            for (int i = 0; i < linePositions.Length; i++)
            {
                //compare
                if (linePositions[i] != linePositionsOld[i])
                {
                    moved = true;
                }
            }

            //update if moved
            if (moved == true)
            {
                LineRenderer line = this.GetComponent<LineRenderer>();

                //get smoothed values
                Vector3[] smoothedPoints = LineSmoother.SmoothLine(linePositions, lineSegmentSize);

                //set line settings
                line.positionCount = smoothedPoints.Length;
                line.SetPositions(smoothedPoints);
                line.startWidth = line.endWidth = lineWidth;
            }
        }

        void OnDrawGizmosSelected()
        {
            Update();
        }

        void OnDrawGizmos()
        {
            /*if (linePoints.Length == 0)
            {
                GetPoints();
            }*/

            //settings for gizmos
            foreach (CurvedLinePoint linePoint in linePoints)
            {
                linePoint.showGizmo = showGizmos;
                linePoint.gizmoSize = gizmoSize;
                linePoint.gizmoColor = gizmoColor;
            }
        }
    }
}

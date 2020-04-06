using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dimensionizer : MonoBehaviour
{
    [Range(0,2)]
    [Tooltip("Select thedimension this is assigned to, x == 0, y == 1, z==2")]
    public int dimChooser;

    public GameObject dimension, midpoint;

    public Color lengthTextFaceColor = new Color(1, 1, 1);
    public string shaderColProperty = "_FaceColor";

    public TMPro.TextMeshPro lengthTextBox;
    public TMPro.TextMeshPro lengthRayCastTextBox;

    [Tooltip("Used to multiply the length of the axis line, only select one! (1 == 1m)")]
    public Vector3 scalerSource = new Vector3(0,0,0);

    [Tooltip("Current raycast length in metres long primary axis")]
    public float rayCastLengthMetres = 5;

    public Vector3 directionToCast = new Vector3(0, 0, 0);

    public float updateIntervalSecs = 2.5f;

    int layerMask = 1;

    void Start()
    {
        if (null == dimension)
            dimension = gameObject.transform.GetChild(0).gameObject;
        if (null == midpoint)
            midpoint = dimension.transform.GetChild(0).gameObject;

        dimension.transform.localScale = scalerSource;

        lengthTextBox.text = string.Format("{0:0.0}m", scalerSource[dimChooser]);
        lengthTextBox.renderer.material.SetColor(shaderColProperty, lengthTextFaceColor);

        lengthRayCastTextBox.text = string.Format("Looking for {0:0.0}m", scalerSource[dimChooser]);
        lengthRayCastTextBox.renderer.material.SetColor(shaderColProperty, lengthTextFaceColor);

        // Raycast against all game objects that are on either the
        // spatial surface or UI layers.
        layerMask = 1 << LayerMask.NameToLayer("SpatialSurface");

        StartCoroutine(CheckCollision());
    }

    IEnumerator CheckCollision()
    {
        var wait = new WaitForSeconds(updateIntervalSecs);
        while (true)
        {
            // do raycast to hit mesh....
            RaycastHit hit;
            if (Physics.Raycast(dimension.transform.position, directionToCast, out hit, rayCastLengthMetres, layerMask))
            {
                // get length to hit origin...
                float distance = Vector3.Distance(dimension.transform.position, hit.transform.position);
                lengthRayCastTextBox.text = string.Format("Hit at: {0:0.00}m", distance);
            }

            GameObject tester = GameObject.Find("marker01");
            Debug.DrawRay(dimension.transform.position, directionToCast, lengthTextFaceColor, updateIntervalSecs, true); 
            yield return wait;
        }
    }
}

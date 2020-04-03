using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dimensionizer : MonoBehaviour
{
    [Range(0,2)]
    public int dimChooser;

    public GameObject dimension;

    public Color lengthTextFaceColor = new Color(1, 1, 1);
    public string shaderColProperty = "_FaceColor";

    public TMPro.TextMeshPro lengthTextBox, lengthRayCastTextBox;

    public Vector3 scalerSource = new Vector3(0,0,0);

    void Start()
    {
        if (null == dimension)
            dimension = gameObject.transform.GetChild(0).gameObject;
        if (null == lengthTextBox)
        {
            lengthTextBox = gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
            lengthRayCastTextBox = gameObject.transform.GetChild(1).gameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshPro>();
        }
        dimension.transform.localScale = scalerSource;

        lengthTextBox.text = string.Format("{0:0.0}m", scalerSource[dimChooser]);
        lengthTextBox.renderer.material.SetColor(shaderColProperty, lengthTextFaceColor);
    }

        void Update()
    {
        // do raycast to hit mesh....

    }
}

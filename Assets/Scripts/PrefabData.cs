using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabData : MonoBehaviour
{
    [Tooltip("Set this to the relative path to the prefab in the resources folder omit the extension.")]
    public string prefabPath = "";
}

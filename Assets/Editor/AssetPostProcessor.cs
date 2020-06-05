using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetPostProcessor : AssetPostprocessor
{
    public static string message { get; set; }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        Debug.Log("OnPostprocessAllAssets");

        foreach (var imported in importedAssets)
        {
            message = "Imported: " + imported;
            Debug.Log("Imported: " + imported);
        }

        foreach (var deleted in deletedAssets)
            Debug.Log("Deleted: " + deleted);

        foreach (var moved in movedAssets)
            Debug.Log("Moved: " + moved);

        foreach (var movedFromAsset in movedFromAssetPaths)
            Debug.Log("Moved from Asset: " + movedFromAsset);
    }
}
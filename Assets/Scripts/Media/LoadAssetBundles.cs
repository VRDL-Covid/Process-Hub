using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAssetBundles : MonoBehaviour
{
    public string bundleDirectory = "";
    public string[] assetBundleNames;
    public Dictionary<string, AnimationClip> animationClips = new Dictionary<string, AnimationClip>();
    public Object[] materials;
    public Object[] gameobjectAssets;
    public bool instantiateImmediate = false;

    public void LoadBundlesForScene()
    {
        if (bundleDirectory.Equals(string.Empty))
            bundleDirectory = AssetBundles.Utility.GetPlatformName();

        string assetBundleFullPath = System.IO.Path.Combine(Application.persistentDataPath, bundleDirectory);

        foreach (string assetBundleName in assetBundleNames)
        {
            var assetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(assetBundleFullPath, assetBundleName));
            if (assetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }
            materials = assetBundle.LoadAllAssets(typeof(Material));
            gameobjectAssets = assetBundle.LoadAllAssets(typeof(GameObject));

            foreach (object o in materials)
            {
                var ot = o as Material;
                if (ot != null && instantiateImmediate)
                    Instantiate(ot);
            }

            foreach
                (object o in gameobjectAssets)
            {
                var ot = o as GameObject;
                if (ot == null)
                    return;
                if(instantiateImmediate)
                    Instantiate(ot);
                    // get anim clips and owners...
                Transform[] trans = ot.GetComponentsInChildren<Transform>();
                foreach (Transform tr in trans)
                {
                    Animation animation = tr.gameObject.GetComponent<Animation>();
                    if (null == animation)
                        continue;
                    foreach (AnimationState animState in animation)
                    {
                        AnimationClip clip = animState.clip;
                        if(clip.length > 1)
                            animationClips.Add($"{tr.gameObject.name}__{clip.name}", clip);
                    }
                }
            }


            /*
            string[] assets = assetBundle.GetAllAssetNames();
            string ext = System.IO.Path.GetExtension(assets[0]);
            switch (ext.ToLower())
            {
                case ".mat":
                    var mat = assetBundle.LoadAsset<Material>($"{assets[0]}");
                    Instantiate(mat);
                    break;
                case ".blend":
                case ".fbx":
                    var prefab = assetBundle.LoadAsset<GameObject>($"{assets[0]}");
                    Instantiate(prefab);
                    break;
            } */
            assetBundle.Unload(false);
        }

    }
}

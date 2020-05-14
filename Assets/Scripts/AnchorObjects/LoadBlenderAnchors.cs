//#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scripts.Json;
using System.Linq;
using Scripts.AnchorObjects;
using Scripts.LineObjects;
using UnityEditorInternal;
using Newtonsoft.Json;
using System.Text;
using System.IO;

namespace Scripts.AnchorObjects
{
    public class LoadBlenderAnchors : MonoBehaviour
    {
        Dictionary<int, GameObject> processObjects = new Dictionary<int, GameObject>();

        public void Awake()
        {

            LoadAnchors();
        }


 

        private void LoadAnchors()
        {
            /*
            // Read in existing JSON script and bind it to the serialised list to show in dialog...
            jsonPath = EditorUtility.OpenFilePanel("Select JSON catalogue for process", Application.persistentDataPath, "json");

            AnchoredGameObjects agos = ObjectLoader.ReadAnchorData(jsonPath);
            agos.anchorObjects.Sort(delegate (AnchoredGameObject a, AnchoredGameObject b)
            {
                return a.RunOrder.CompareTo(b.RunOrder);
            });

            // now add to member list that binds to UI...
            foreach (AnchoredGameObject ago in agos.anchorObjects)
            {
                anchors.AddAnchor(ago);
                InstantiateHologram(ago, true);
            }*/
        }

        void UpdateHologram()
        {
            /*
            if (GUILayout.Button("Update"))
            {
                string selName = Selection.activeGameObject.name;
                AnchoredGameObject ago = anchors.anchorObjects.SingleOrDefault(anch => anch.Name == selName);
                if (null != ago)
                {
                    ago.Description = anchorDescription;
                    ago.Scale = anchorScale;
                    ago.ToolTipText = anchorToolTip;
                }
            }*/
        }

        /// <summary>
        /// Instantiates the hologram for each test point in question...
        /// </summary>
        /*GameObject InstantiateHologram(AnchoredGameObject ago, bool useAgo)
        {
            
            GameObject runtime = null;
            // Check that the required prefab  is a valid hologram...

            GameObject parent = GameObject.Find(ago.ParentName);
            if (parent != null && (useAgo || (!useAgo && selectedCategoryIdx > 0 && selectedHologramIdx > 0)))
            {
                Holograms.Hologram holoGramDef = null;
                if (useAgo)
                    holoGramDef = holograms.holograms.Where(hol => hol.ResourcePath == ago.PrefabSource).Select(hol => hol).FirstOrDefault();
                else
                    holoGramDef = holograms.holograms.Where(hol => hol.Index == selectedHologramIdx).Select(hol => hol).FirstOrDefault();

                Categories.Category category = categories.categories.Where(cat => cat.Index == selectedCategoryIdx).Select(cat => cat).FirstOrDefault();

                Selection.activeGameObject = parent;
                GameObject objHolo = (GameObject)Instantiate(Resources.Load(holoGramDef.ResourcePath), Selection.activeGameObject.transform);

                ObjectEvents oEvts = objHolo.AddComponent<ObjectEvents>();
                oEvts.RaiseEvents = true;
                oEvts.OnObjectDestroy += this.DoHierarchyObjectDestroyed;

                runtime = objHolo.transform.Find(holoGramDef.RuntimeObject).gameObject;
                objHolo.name = string.Format("{0}_anchor", Selection.activeGameObject.name);
                runtime.name += string.Format("_{0}", Selection.activeGameObject.name);
                oEvts.anchorName = runtime.name;

                ago.Name = runtime.name;
                ago.ParentName = Selection.activeGameObject.name;
                if (useAgo)
                {
                    objHolo.transform.localScale = ago.Scale;
                    objHolo.transform.position = ago.Position;
                    Quaternion rotation = new Quaternion(ago.RotateX, ago.RotateY, ago.RotateZ, ago.RotateW);
                    objHolo.transform.rotation = rotation;
                }
                else
                {
                    ago.Position = Selection.activeGameObject.transform.position;
                    ago.Scale = Selection.activeGameObject.transform.localScale;
                    ago.Category = category.Name;
                    ago.Description = anchorDescription;
                    ago.ToolTipText = anchorToolTip;
                }
                ago.PrefabSource = holoGramDef.ResourcePath;


#if WINDOWS_UWP
                        MST.ToolTip ttip = objToAnchor.GetComponentInChildren<MST.ToolTip>();
                        if (ttip != null)
                            ToolTipText = ttip.ToolTipText;
#else
                //ago.ToolTipText = "";
#endif

                anchors.AddAnchor(ago, runtime);

                List<string> waypoint = holoGramDef.WayPointObject.Split('/').ToList();
                ago.AddChildRecursive(objHolo, waypoint);
            }
            return runtime;
        }
        */
    }
    
}
//#endif
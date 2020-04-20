#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
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

namespace Scripts.ProcessEditorObjects
{
    public class ProcessEditor : EditorWindow
    {

        string strRegexSearch = "_holopoint";
        public string anchorFileName = "anchordata";

        int[] strSelectionIDs;
        int selectedHologramIdx, selectedCategoryIdx = 0;
        GameObject selectedParent = null;
        bool enabledAddition = false;
        bool enableUpdate = false;
        const string conSelValidObject = "<SELECT VALID OBJECT>";
        string strSelectedObjectName = conSelValidObject;
        Dictionary<int, GameObject> processObjects = new Dictionary<int, GameObject>();
        ProcessMetaData pmd = new ProcessMetaData();
        string json = string.Empty;
        string[] cats = { "" };
        string[] hols = { "" };
        string jsonPath = string.Empty;
        const int TOP_PADDING = 2;
        Vector2 textScroll;

        // stores the data of the current anchor..
        Vector3 anchorScale = new Vector3(1, 1, 1);
        string anchorDescription, anchorToolTip = string.Empty;

        // holds categories that can be assigned to a hologram....
        Categories categories = new Categories();

        // specifies the hologram to be set at an anchor...
        //
        Holograms holograms = new Holograms();

        // stores the list of holograms that is used to generate the JSON...
        AnchoredGameObjects anchors = new AnchoredGameObjects();
        AnchorNames anchorNames = null;

        // used list the existing waypoints....
        SerializedObject anchorsSerObj = null;
        ReorderableList anchorsReorderableList = null;

        // used list the media for a step....
        SerializedObject mediaSerObj = null;
        ReorderableList mediaReorderableList = null;

        // used to attach script for storing anchor script...
        GameObject ago = null;

        // setup to allow recording the change in order to redisplay 
        // and edit an existing waypoint hologram...
        ReorderableList.ChangedCallbackDelegate cbAnchorsOnChange;
        ReorderableList.SelectCallbackDelegate cbAnchorsOnSelect;


        // setup to allow recording the change in order to redisplay 
        // and edit an existing waypoint hologram...
        ReorderableList.SelectCallbackDelegate cbMediaOnSelect;

        // Drop list callbacks...
        delegate void DelMenuItem();
        DelMenuItem cbHologramSelect, cbCategorySelect;


        [MenuItem("Window/Process Editor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ProcessEditor));
        }

        public void Awake()
        {
            // assign delegates for droplists...
            cbHologramSelect = new DelMenuItem(DoHoloGrams);
            cbCategorySelect = new DelMenuItem(DoCategorySelect);

            //Assign the list delegates...
            cbAnchorsOnChange = OnListOrderChanged;
            cbAnchorsOnSelect = OnListItemSelect;

            // load in the process...
            string jsonFilePath = string.Format("Json/{0}", ReadMetaData.fileName);
            json = Resources.Load(jsonFilePath).ToString();
            if (json != null && json != string.Empty)
            {
                categories = JsonUtility.FromJson<Categories>(json);
                holograms = JsonUtility.FromJson<Holograms>(json);
                cats = categories.categories.OrderBy(cat => cat.Index).Select(cat => cat.Name).ToArray();
                hols = holograms.holograms.OrderBy(hol => hol.Index).Select(hol => hol.Name).ToArray();
            }

            // now remove and add a placeholder object for the anchor list...
            while (GameObject.Find("AnchorContainer"))
            {
                ago = GameObject.Find("AnchorContainer");
                if (ago)
                    GameObject.DestroyImmediate(ago);
            }

            // create a placeholder for the anchor names...
            ago = new GameObject("AnchorContainer");
            ago.AddComponent<MeshFilter>();
            ago.AddComponent<MeshRenderer>();
            ago.AddComponent<AnchorNames>();
            anchorNames = ago.GetComponent<AnchorNames>();
            ago.GetComponent<AnchorNames>().hideFlags = HideFlags.HideInInspector;
            anchors.AnchorNames = anchorNames;

            LoadAnchors();
        }

        private void OnGUI()
        {
            // search bar to select objects in scene that BEGIN with regexp...
            GUILayout.Label("Search for Process location objects", EditorStyles.boldLabel);
            strRegexSearch = EditorGUILayout.TextField("Search Pattern", strRegexSearch);

            GUILayout.Label(string.Format("Set up Hologram for {0}", strSelectedObjectName), EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            DrawAndHandleMenu("Select a Category", cats, cbCategorySelect, ref selectedCategoryIdx);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DrawAndHandleMenu("Select a Hologram", hols, cbHologramSelect, ref selectedHologramIdx);
            GUILayout.EndHorizontal();

            // sliders for scaling...
            DoObjectProperties();

            // Update the list containing the generated items...
            UpdateAnchorList();

            // Handle serialisation back to json...
            // if the user hasn't selected a json file then search the persistent data store and find a new file name...
            // This step takes the added objects and gets the order from the list
            GUILayout.BeginHorizontal();
            GUILayout.Label("UploadMedia to HoloLens...", EditorStyles.boldLabel);
            CheckFinalise();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Serialise Holograms and reset scene...", EditorStyles.boldLabel);
            CheckFinalise();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Create the list of current anchors defined in the process
        /// and set up the callbacks to redo the order...
        /// </summary>
        private void DoAnchorsList()
        {

            if (anchorNames)
            {
                anchorsSerObj = new SerializedObject(anchorNames);
                //SerializedProperty namesPropArr = anchorNames.Names;
                anchorsReorderableList = new ReorderableList(anchorsSerObj, anchorsSerObj.FindProperty("Names"), true, true, false, false);

                anchorsReorderableList.onChangedCallback = cbAnchorsOnChange;
                anchorsReorderableList.onSelectCallback = cbAnchorsOnSelect;
                anchorsReorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Holographic Way Points");
                anchorsReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    rect.y += TOP_PADDING;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, anchorsReorderableList.serializedProperty.GetArrayElementAtIndex(index));
                };
            }
        }

        /// <summary>
        /// Clean up...
        /// </summary>
        private void OnDisable()
        {
            // TODO
        }

        /// <summary>
        ///  called to set the focus to the item in the list...
        /// </summary>
        /// <param name="list">Contains anchors in the order</param>
        private void OnListItemSelect(ReorderableList list)
        {
            string selName = list.serializedProperty.GetArrayElementAtIndex(list.index).stringValue;
            AnchoredGameObject ago = anchors.anchorObjects.SingleOrDefault(anch => anch.Name == selName);
            if (null != ago)
            {
                anchorDescription = ago.Description;
                anchorToolTip = ago.ToolTipText;
                anchorScale = ago.Scale;

                //selectedHologramIdx = System.Array.FindIndex(hols, h => h == ago.PrefabSource);
                //selectedCategoryIdx = System.Array.FindIndex(cats, c => c == ago.Category);
            }
            GameObject selObj = GameObject.Find(selName);
            if (selObj)
            {
                Selection.activeGameObject = selObj;
                CheckObjectAndFocus();
                enableUpdate = true;
            }
        }

        /// <summary>
        ///  called twhen the list is reordered, and updates the path in the scene...
        /// </summary>
        /// <param name="list">Contains anchors in the current order</param>
        private void OnListOrderChanged(ReorderableList list)
        {
            GameObject navLine = GameObject.Find("NavLine");
            Vector3[] linePoints = new Vector3[list.count];
            if (navLine)
            {
                //clear current linepoints...
                for (int i = 0; i < navLine.transform.childCount; i++)
                {
                    GameObject.DestroyImmediate(navLine.transform.GetChild(i));
                }
                for (int i = 0; i < list.count; i++)
                {
                    // get the anchor...
                    AnchoredGameObject ago = anchors.anchorObjects.SingleOrDefault(anchor => anchor.Name == list.serializedProperty.GetArrayElementAtIndex(i).stringValue);

                    // store the position of this object...
                    if (ago != null)
                        linePoints[i] = ago.Position;
                }

                // set the line renderer to show the updated order for the list...
                CurvedLineRenderer clr = navLine.GetComponent<CurvedLineRenderer>();
                clr.DoNavigationLine(this, linePoints);
            }
        }

        private void LoadAnchors()
        {
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
            }
        }

        private void OnEnable()
        {
            // Render the anchors list...
            DoAnchorsList();
            OnListOrderChanged(anchorsReorderableList);
        }

        void DoObjectProperties()
        {
            // check that the selected object is parented to a valid gameobject also that it is a valid "RunTime" object...
            if (CheckValidRuntimeObjectSelected())
            {
                strSelectedObjectName = Selection.activeGameObject.name;
                AnchoredGameObject ago = anchors.anchorObjects.SingleOrDefault(anch => anch.Name == Selection.activeGameObject.name);
                // setup the scaling sliders...
                anchorScale = Selection.activeGameObject.transform.localScale;
                GUILayout.Label("Scale axes", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                GUILayout.Label(" x axis", EditorStyles.boldLabel);
                anchorScale.x = EditorGUILayout.Slider(anchorScale.x, -10, 10);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("y axis", EditorStyles.boldLabel);
                anchorScale.y = EditorGUILayout.Slider(anchorScale.y, -10, 10);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("z axis", EditorStyles.boldLabel);
                anchorScale.z = EditorGUILayout.Slider(anchorScale.z, -10, 10);
                Selection.activeGameObject.transform.localScale = anchorScale;
                GUILayout.EndHorizontal();

                // set up the Tooltip to be displayed to the end user...
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                anchorToolTip = EditorGUILayout.TextField("ToolTip: ", anchorToolTip, GUILayout.ExpandHeight(false));
                GUILayout.EndHorizontal();

                // set up the Description to be displayed to the end user...
                GUILayout.Label("Description", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                textScroll = EditorGUILayout.BeginScrollView(textScroll, GUILayout.Height(50));
                anchorDescription = EditorGUILayout.TextArea(anchorDescription, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Update the selected hologram", EditorStyles.boldLabel);
                UpdateHologram();
                GUILayout.EndHorizontal();
            }
            else
                strSelectedObjectName = conSelValidObject;

        }

        void DrawAndHandleMenu(string title, string[] options, DelMenuItem DoSelectCallback, ref int idx)
        {
            if (Selection.gameObjects.Length == 1 && Selection.activeGameObject.name.StartsWith(strRegexSearch))
            {
                strSelectedObjectName = Selection.activeGameObject.name;

                EditorGUI.BeginChangeCheck();
                idx = EditorGUILayout.Popup(title, idx, options);
                DoSelectCallback?.Invoke();
            }
            else
                strSelectedObjectName = "<SELECT VALID OBJECT>";
        }

        void AddHoloGramAsChildOfSelected(int selIdx, GameObject selected)
        {
            // empty has a single child gameobject which is the prefab...
            if (Selection.gameObjects.Length == 1)
            {
                enabledAddition = true;

            }
        }

        /// <summary>
        /// ind the object and remove it from the lists...
        /// </summary>
        /// <param name="sender">Object raising the event</param>
        /// <param name="e">Contains the name of the hologram to remove from the lists</param>
        void DoHierarchyObjectDestroyed(object sender, ObjectEventArgs e)
        {
            // remove from list...
            if (anchorNames.Names.Contains(e.AnchorName))
            {
                List<string> temp = anchorNames.Names.ToList();
                temp.Remove(e.AnchorName);
                anchorNames.Names = temp.ToArray();
                anchors.anchorObjects.RemoveAll(anch => anch.Name == e.AnchorName);
                anchorsSerObj.Update();
            }
        }

        void DoHoloGrams()
        {
            CheckObject();
            if (EditorGUI.EndChangeCheck())
            {
                selectedParent = Selection.activeGameObject;
                enabledAddition = true;
            }
        }

        void DoCategorySelect()
        {

        }

        void UpdateAnchorList()
        {
            anchorsSerObj.Update();
            anchorsReorderableList.DoLayoutList();
            anchorsSerObj.ApplyModifiedProperties();
        }

        void UpdateHologram()
        {
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
            }
        }

        void CheckObject()
        {

            if (GUILayout.Button("Add"))
            {
                if (enabledAddition && selectedHologramIdx > 0 && selectedCategoryIdx > 0)
                {
                    // retrieve the object to load from the json object using the following LinQ query...
                    // Get name of child "RunTime" object to select from prefab that scaled to "bound" part of the mesh to be inspected etc..
                    // Scalings, rotations, id and other attributes are stored in the json script as part of the definition that describes the process to the HoloLens


                    // NOTE: on the HoloLens the process step state machine will instantiate the object at each step and transform the object 
                    // based upon the settings stored in the Process Json...

                    // Now add the object to the anchors collection...
                    AnchoredGameObject ago = new AnchoredGameObject
                    {
                        ParentName = Selection.activeGameObject.name
                    };

                    GameObject runtime = InstantiateHologram(ago, false);

                    if (runtime)
                    {
                        Selection.activeGameObject = runtime;
                        bool isValidChild = CheckValidRuntimeObjectSelected();
                    }

                    // prevent any further clicks to add the same object, as Why Would you???
                    enabledAddition = false;
                    selectedHologramIdx = selectedCategoryIdx = 0;
                }
            }
        }

        /// <summary>
        /// Instantiates the hologram for each test point in question...
        /// </summary>
        GameObject InstantiateHologram(AnchoredGameObject ago, bool useAgo)
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

        /// <summary>
        /// Closes out the editor and serialises the object to json...
        /// </summary>
        void UploadMediaToHoloLens()
        {
            DevicePortalWrapper.ConnectInfo conInfo = new DevicePortalWrapper.ConnectInfo("127.0.0.1:10080", "holovrdl", "mrRobot2020");

            //DevicePortalWrapper.PutFile(conInfo, "LocalAppData", "textUpLoad.txt")
        }
        
        /// <summary>
         /// Closes out the editor and serialises the object to json...
         /// </summary>
        void CheckFinalise()
        {
            // get the order and store...
            SetAnchorOrder();

            if (GUILayout.Button("Finalize and Close"))
            {
                if (!EditorUtility.DisplayDialog("Confirmation", "Are you sure that you have finished setting up the holograms as they will now be deleted?", "Finished", "Cancel"))
                    return;
                AnchoredGameObjects agoParent = new AnchoredGameObjects();

                foreach (AnchoredGameObject ago in anchors.anchorObjects)
                    if (GameObject.Find(ago.Name) != null)
                    {
                        //AnchoredGameObject agoNew = new AnchoredGameObject(GameObject.Find(ago.Name), ago.PrefabSource);
                        agoParent.anchorObjects.Add(ago);
                    }

                jsonPath = EditorUtility.OpenFilePanel("Create/Select json file to save", Application.persistentDataPath, "json");

                // get the objects and serialise them to the selected json script...
                string json = JsonConvert.SerializeObject(agoParent, Formatting.Indented);
                byte[] data = Encoding.ASCII.GetBytes(json);

                UnityEngine.Windows.File.WriteAllBytes(jsonPath, data);

                // now delete all holographic objects....
                foreach (AnchoredGameObject ago in anchors.anchorObjects)
                {
                    GameObject go = GameObject.Find(ago.Name).transform.parent.gameObject;
                    if (go != null)
                    {
                        ObjectEvents evts = go.GetComponent<ObjectEvents>();
                        if (evts != null)
                        {
                            evts.RaiseEvents = false;
                            evts.OnObjectDestroy -= this.DoHierarchyObjectDestroyed;
                        }
                        GameObject.DestroyImmediate(go);

                    }
                }

                EditorWindow.GetWindow(typeof(ProcessEditor)).Close();
            }
        }

        /// <summary>
        /// Checks whether the selected object is a child of (or actually is) 
        /// a current valid hologram...
        /// </summary>
        /// <returns></returns>
        bool CheckValidRuntimeObjectSelected()
        {
            GameObject objSelected = Selection.activeGameObject;
            if (objSelected == null)
                return false;
            if (objSelected.name == "__Orientor")
                return false;
            GameObject level = objSelected;
            bool found = false;
            while (level != null && !found)
            {
                found = level.name.StartsWith(strRegexSearch);
                if (!found)
                    if (level.transform.parent != null)
                        level = level.transform.parent.gameObject;
                    else
                        break;
            }
            return found;
        }

        private void OnSelectionChange()
        {
            CheckObjectAndFocus();
        }

        void CheckObjectAndFocus()
        {
            if (CheckValidRuntimeObjectSelected())
                SceneView.FrameLastActiveSceneView();
        }

        void SetAnchorOrder()
        {
            int size = anchorsReorderableList.serializedProperty.arraySize;
            for (int i = 0; i < size; i++)
            {
                string name = anchorsReorderableList.serializedProperty.GetArrayElementAtIndex(i).stringValue;
                AnchoredGameObject ago = anchors.anchorObjects.SingleOrDefault(anch => anch.Name == name);
                if (null != ago)
                {
                    ago.RunOrder = i;
                }
            }
        }
    }
}
#endif
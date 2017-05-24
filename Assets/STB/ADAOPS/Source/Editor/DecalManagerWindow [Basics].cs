#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;

namespace STB.ADAOPS
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: DecalManagerWindow
    /// # Main window class to handle all decal and object painter system
    /// </summary>
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class DecalManagerWindow : EditorWindow
    {
        // public static 
        public static bool autoChangeToFirstElementInList = false;
        public static cInsertMode insertMode = cInsertMode.controlAndMouse;

        // private -- enum
        enum cSystemMode
        {
            meshDecals,
            projectedDecals,
            objects,
            edition
        }
        ;

        // public -- enum
        public enum cInsertMode
        {
            shiftAndMouse,
            controlAndMouse,
            justMouse,
            disabled
        }

        // private -- control
        GameObject actualObjectToForceSelect = null;
        double actualObjectToForceSelectCounter = 0;
        double previousEditorTimeToForceSelect = 0;
        double previousEditorTime = 0;
        cSystemMode systemMode = cSystemMode.edition;
        bool deleteAllConfirmationMode = false;
        bool cleanAlSceneScriptsConfirmationMode = false;

        // private -- prefabs
        string seedForInstancies = BasicDefines.NOT_DEFINED;
        GenericMeshDecal meshDecalPrefab = null;
        GenericProjectorDecal projectedDecalPrefab = null;
        GameObject genericObject = null;

        // private -- config saver
        ConfigSaver configSaver = null;

        // private -- window
        Vector2 windowPosition = Vector2.zero;

        // private -- selection changes
        Object lastSelectedActiveObject = null;

        // private -- static
        static Vector3 lastMeshDecalsHitPoint = Vector3.zero;
        static Vector3 lastPrefabHitPoint = Vector3.zero;
        static Vector3 lastProjectedDecalsHitPoint = Vector3.zero;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetSeedForInstancies
        /// # Get the actual seed (it will be generated if it is the first time)
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        string GetSeedForInstancies()
        {
            //Debug.Log("GetSeedForInstancies A");

            if (seedForInstancies == BasicDefines.NOT_DEFINED)
            {
                seedForInstancies = "";

                for (int i = 0; i < 9; i++)
                {
                    seedForInstancies += Random.Range(0, 9).ToString();
                }

                //Debug.Log("GetSeedForInstancies B -> seedForInstancies: "+seedForInstancies);
            }

            return seedForInstancies;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetEditorTimeDiff
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        double GetEditorTimeDiff()
        {
            return (EditorApplication.timeSinceStartup - previousEditorTime);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnEnable
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnEnable()
        {
            LoadBasics();

            SceneView.onSceneGUIDelegate += SceneGUI;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawBasicButtons
        /// # Draw all basic buttons
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void DrawBasicButtons()
        {
            bool drawThis = true;

            if (!configSaver.parameters.showBasicActionsAlways && (systemMode != cSystemMode.edition))
            {
                drawThis = false;
            }

            if (drawThis)
            {
                configSaver.parameters.showBasicActions = EditorGUILayout.Foldout(configSaver.parameters.showBasicActions, new GUIContent("Basic actions", "Show basic actions"));

                if (configSaver.parameters.showBasicActions)
                {
                    EditorBasicFunctions.DrawEditorBox("Basic actions", Color.yellow, position);

                    float buttonsScale = position.width / 5;

                    if (deleteAllConfirmationMode)
                    {
                        EditorBasicFunctions.DrawEditorBox("Do you really want to delete all?", Color.white, position);

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();

                        if (EditorBasicFunctions.GetEditorButton("NoButton", "Don't delete", new Vector2(buttonsScale, buttonsScale), true, false, false, true))
                        {
                            deleteAllConfirmationMode = false;
                        }

                        GUILayout.FlexibleSpace();

                        if (EditorBasicFunctions.GetEditorButton("YesButton", "Continue deleting", new Vector2(buttonsScale, buttonsScale), true, false, false, true))
                        {
                            DeleteAllAction();
                            deleteAllConfirmationMode = false;
                        }

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.Separator();

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();

                        if (EditorBasicFunctions.GetEditorButton("DeleteAllButton", "Delete all created objects of every mode, in edit mode delete all created objects of all modes", new Vector2(buttonsScale, buttonsScale), true, false, false, true))
                        {
                            deleteAllConfirmationMode = true;
                        }

                        GUILayout.FlexibleSpace();

                        if (EditorBasicFunctions.GetEditorButton("DeleteLastButton", "Delete last created object of every mode, in edit mode delete last create object of all modes", new Vector2(buttonsScale, buttonsScale), true, false, false, true))
                        {
                            DeleteLastAction();
                        }

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                    EditorBasicFunctions.DrawLineSeparator();
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawAdvancedButtons
        /// # Draw all advanced buttons
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void DrawAdvancedButtons()
        {
            bool drawThis = true;

            if (!configSaver.parameters.showAdvancedActionsAlways && (systemMode != cSystemMode.edition))
            {
                drawThis = false;
            }

            if (drawThis)
            {
                configSaver.parameters.showAdvancedActions = EditorGUILayout.Foldout(configSaver.parameters.showAdvancedActions, new GUIContent("Advanced actions", "Show advanced actions"));

                if (configSaver.parameters.showAdvancedActions)
                {
                    EditorBasicFunctions.DrawEditorBox("Advanced actions", Color.yellow, position);

                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();

                    float buttonsScale = position.width / 6;

                    if (cleanAlSceneScriptsConfirmationMode)
                    {
                        EditorBasicFunctions.DrawEditorBox("Do you really want to clean all the scripts?", Color.white, position);

                        // clean all scripts			
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();

                        if (EditorBasicFunctions.GetEditorButton("NoButton", "Don't clean", new Vector2(buttonsScale, buttonsScale), true, false, false, true))
                        {
                            cleanAlSceneScriptsConfirmationMode = false;
                        }

                        EditorBasicFunctions.GetEditorButton("EmptyButton", "", new Vector2(buttonsScale, buttonsScale), false, true, true, true);
                        EditorBasicFunctions.GetEditorButton("EmptyButton", "", new Vector2(buttonsScale, buttonsScale), false, true, true, true);

                        if (EditorBasicFunctions.GetEditorButton("YesButton", "Continue cleaning", new Vector2(buttonsScale, buttonsScale), true, false, false, true))
                        {
                            CleanAllSceneScriptsAction();
                            cleanAlSceneScriptsConfirmationMode = false;
                        }

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        // clean all scripts			
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();

                        if (EditorBasicFunctions.GetEditorTextButton("CLEAN SCRIPTS", "Clean all the scripts in the scene, for example to use without this editor extension in the system", position))
                        {
                            cleanAlSceneScriptsConfirmationMode = true;
                        }

                        // create new decal				
                        if (EditorBasicFunctions.GetEditorTextButton("CREATE DECAL", "Create a new decal just selecting textures, type and folder", position))
                        {
                            CreateDecalWindow window = ScriptableObject.CreateInstance<CreateDecalWindow>();
                            window.name = "Create new decal";
#if UNITY_5_0
                            window.title = window.name;
#else
                            window.titleContent = new GUIContent(window.name);
#endif
                            window.Show();
                        }

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        EditorGUILayout.Separator();

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();


                        // lock all future time lockable decals		
                        if (EditorBasicFunctions.GetEditorTextButton("LOCK DECALS", "Mark all 'futureTimeLockableShape' decals as locked", position))
                        {
                            foreach (GenericMeshDecal actualDecal in GameObject.FindObjectsOfType<GenericMeshDecal>())
                            {
                                if (actualDecal.futureTimeLockableShape)
                                {
                                    actualDecal.lockedShapeAlways = true;
                                }
                            }
                        }

                        // merge decals
                        if (EditorBasicFunctions.GetEditorTextButton("MERGE DECALS", "Merge all selected decals into single one mesh", position))
                        {
                            MergeDecalWindow window = ScriptableObject.CreateInstance<MergeDecalWindow>();
                            window.name = "Merge new decal";
#if UNITY_5_0
                            window.title = window.name;
#else
                            window.titleContent = new GUIContent(window.name);
#endif
                            window.Show();
                        }

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                    EditorBasicFunctions.DrawLineSeparator();
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SaveAllGenericDecalMeshesDoDisk
        /// # Save all generic decal meshes to disk
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void SaveAllGenericDecalMeshesDoDisk()
        {
            string savingPath = BasicDefines.MAIN_PATH + "Saved/GenericDecalMeshes/";

            Directory.CreateDirectory(savingPath);

            List<Object> actualMeshesInSavingFolderList = EditorBasicFunctions.GetObjectListFromDirectory(savingPath, ".asset");


            string savingBaseName = "GenericMeshDecal_savedMesh_";

            int lastDetectedIndex = 1;

            for (int i = 0; i < actualMeshesInSavingFolderList.Count; i++)
            {
                string actualName = actualMeshesInSavingFolderList[i].name;
                string actualIndexString = "";

                for (int j = actualName.Length - 1; j >= 0; j--)
                {
                    string actualSubString = actualName.Substring(j, 1);

                    if (actualSubString == "_")
                    {
                        break;
                    }
                    else
                    {
                        actualIndexString = actualSubString + actualIndexString;
                    }
                }

                if (actualName.Length > 0)
                {
                    //Debug.Log ("actualIndexString: " + actualIndexString);

                    int actualIndex = -1;

                    int.TryParse(actualIndexString, out actualIndex);

                    if (actualIndex > lastDetectedIndex)
                    {
                        lastDetectedIndex = actualIndex;
                    }
                }
            }

            //Debug.Log ("lastDetectedIndex: " + lastDetectedIndex);

            foreach (GenericMeshDecal decal in GameObject.FindObjectsOfType<GenericMeshDecal>())
            {
                lastDetectedIndex++;

                Mesh actualMesh = decal.GetComponent<MeshFilter>().sharedMesh;
                AssetDatabase.CreateAsset(actualMesh, savingPath + savingBaseName + lastDetectedIndex + ".asset"); // saves to "assets/"
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CleanScriptsAction
        /// # Clean base scripts of all decals and objects in the scene
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void CleanAllSceneScriptsAction()
        {
            //Debug.Log ("CleanAllSceneScriptsAction");

            // Generic mesh decals
            SaveAllGenericDecalMeshesDoDisk();

            foreach (GenericMeshDecal decal in GameObject.FindObjectsOfType<GenericMeshDecal>())
            {
                DestroyImmediate(decal.GetComponent<GenericMeshDecal>());
            }

            // Generic projector decals
            foreach (GenericProjectorDecal decal in GameObject.FindObjectsOfType<GenericProjectorDecal>())
            {
                DestroyImmediate(decal.GetComponent<GenericProjectorDecal>());
            }

            // Generic objects
            foreach (GenericObject obj in GameObject.FindObjectsOfType<GenericObject>())
            {
                DestroyImmediate(obj.GetComponent<GenericObject>());
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UndoAction
        /// # Handle "undo" action
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void UndoAction()
        {
            Debug.Log("Undo");
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DeleteAllAction
        /// # Handle "delete all" action
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void DeleteAllAction()
        {
            //Debug.Log("Delete All");

            bool deleteMeshDecals = false;
            bool deleteProjectedDecals = false;
            bool deleteObjects = false;
            bool doItByBaseName = false;
            string baseName = "";

            switch (systemMode)
            {
                case cSystemMode.edition:
                    {
                        deleteMeshDecals = true;
                        deleteProjectedDecals = true;
                        deleteObjects = true;
                    }
                    break;

                case cSystemMode.meshDecals:
                    {
                        deleteMeshDecals = true;
                        doItByBaseName = true;
                        baseName = BasicDefines.MESH_DECAL_BASE_NAME;
                    }
                    break;

                case cSystemMode.projectedDecals:
                    {
                        deleteProjectedDecals = true;
                        doItByBaseName = true;
                        baseName = BasicDefines.PROJECTED_DECAL_BASE_NAME;
                    }
                    break;

                case cSystemMode.objects:
                    {
                        deleteObjects = true;
                        doItByBaseName = true;
                        baseName = BasicDefines.OBJECT_BASE_NAME;
                    }
                    break;
            }

            if (deleteMeshDecals)
            {
                GenericDestroyable.DestroyAll(doItByBaseName, baseName);
                BasicFunctions.DestroyGameObjectByName(baseName);
            }

            if (deleteProjectedDecals)
            {
                GenericDestroyable.DestroyAll(doItByBaseName, baseName);
                BasicFunctions.DestroyGameObjectByName(baseName);
            }

            if (deleteObjects)
            {
                GenericDestroyable.DestroyAll(doItByBaseName, baseName);
                BasicFunctions.DestroyGameObjectByName(baseName);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DeleteLastAction
        /// # Handle "delete last" action
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void DeleteLastAction()
        {
            //Debug.Log("Delete Last");

            bool doItByName = false;
            string baseName = "";

            switch (systemMode)
            {
                case cSystemMode.edition:
                    {
                    }
                    break;

                case cSystemMode.meshDecals:
                    {
                        doItByName = true;
                        baseName = BasicDefines.MESH_DECAL_BASE_NAME;
                    }
                    break;

                case cSystemMode.projectedDecals:
                    {
                        doItByName = true;
                        baseName = BasicDefines.PROJECTED_DECAL_BASE_NAME;
                    }
                    break;

                case cSystemMode.objects:
                    {
                        doItByName = true;
                        baseName = BasicDefines.OBJECT_BASE_NAME;
                    }
                    break;
            }


            GenericDestroyable.DestroyLast(GetSeedForInstancies(), doItByName, baseName);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// LoadBasics
        /// # Load basics
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void LoadBasics()
        {
            configSaver = new ConfigSaver();


            meshDecalPrefab = AssetDatabase.LoadAssetAtPath(BasicDefines.MESH_DECAL_PREFAB_PATH, typeof(GenericMeshDecal)) as GenericMeshDecal;

            if (!meshDecalPrefab)
            {
                Debug.Log("NOTE -> no meshDecalPrefab, verify folder: " + BasicDefines.MESH_DECAL_PREFAB_PATH);
            }

            projectedDecalPrefab = AssetDatabase.LoadAssetAtPath(BasicDefines.PROJECTED_DECAL_PREFAB_PATH, typeof(GenericProjectorDecal)) as GenericProjectorDecal;

            if (!projectedDecalPrefab)
            {
                Debug.Log("NOTE -> no projectedDecalPrefab, verify folder: " + BasicDefines.MESH_DECAL_PREFAB_PATH);
            }

            if (EditorBasicFunctions.GetPrefabList().Count > 0)
            {
                genericObject = EditorBasicFunctions.GetPrefabList()[0];
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawBasics
        /// # Draw basics 
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void DrawBasics()
        {
            EditorGUILayout.Separator();

            EditorBasicFunctions.DrawActualVersion(position);

            EditorGUILayout.Separator();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawModeButtons
        /// # Draw editor mode buttons
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void DrawModeButtons()
        {
            // create mode buttons
            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            float buttonsScale = position.width / 6;

            // separator			
            EditorBasicFunctions.GetEditorButton("EmptyButton", "", new Vector2(0.1f * buttonsScale, buttonsScale), false, true, true, true);

            if (EditorBasicFunctions.GetEditorButton("EditionModeButton", "Select 'Edit Mode'", new Vector2(buttonsScale, buttonsScale), (systemMode == cSystemMode.edition), true, false, false))
            {
                systemMode = cSystemMode.edition;
            }

            // separator			
            EditorBasicFunctions.GetEditorButton("EmptyButton", "", new Vector2(0.1f * buttonsScale, buttonsScale), false, true, true, true);

            if (EditorBasicFunctions.GetEditorButton("PutMeshDecalsButton", "Select 'Mesh Decals Mode'", new Vector2(buttonsScale, buttonsScale), (systemMode == cSystemMode.meshDecals), true, false, false))
            {
                systemMode = cSystemMode.meshDecals;
            }

            // separator			
            EditorBasicFunctions.GetEditorButton("EmptyButton", "", new Vector2(0.1f * buttonsScale, buttonsScale), false, true, true, true);

            if (EditorBasicFunctions.GetEditorButton("PutObjectsButton", "Select 'Insert Objects Mode'", new Vector2(buttonsScale, buttonsScale), (systemMode == cSystemMode.objects), true, false, false))
            {
                systemMode = cSystemMode.objects;
            }

            // separator			
            EditorBasicFunctions.GetEditorButton("EmptyButton", "", new Vector2(0.1f * buttonsScale, buttonsScale), false, true, true, true);

            if (EditorBasicFunctions.GetEditorButton("PutProjectedDecalsButton", "Select 'Insert Projected Decals Mode'", new Vector2(buttonsScale, buttonsScale), (systemMode == cSystemMode.projectedDecals), true, false, false))
            {
                systemMode = cSystemMode.projectedDecals;
            }

            GUILayout.FlexibleSpace();
            EditorBasicFunctions.GetEditorButton("EmptyButton", "", new Vector2(0.1f * buttonsScale, buttonsScale), false, true, true, true);


            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawGeneralOptions
        /// # Draw the basic general options
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void DrawGeneralOptions()
        {
            if (systemMode == cSystemMode.edition)
            {
                configSaver.parameters.showBasicGeneralOptions = EditorGUILayout.Foldout(configSaver.parameters.showBasicGeneralOptions, new GUIContent("General options", "Show general options"));

                if (configSaver.parameters.showBasicGeneralOptions)
                {
                    EditorBasicFunctions.DrawEditorBox("General options", Color.yellow, position);

                    // basic actions only in edition mode?
                    configSaver.parameters.showBasicActionsAlways = EditorGUILayout.Toggle(new GUIContent("Basic actions always", "Attach created decal to hit object"), configSaver.parameters.showBasicActionsAlways);

                    // advanced actions only in edition mode?
                    configSaver.parameters.showAdvancedActionsAlways = EditorGUILayout.Toggle(new GUIContent("Advanced actions always", "Attach created decal to hit object"), configSaver.parameters.showAdvancedActionsAlways);

                    // show help?
                    configSaver.parameters.hideBasicHelp = EditorGUILayout.Toggle(new GUIContent("Hide basic help", "Hide basic help"), configSaver.parameters.hideBasicHelp);

                    // insert mode
                    EditorGUILayout.Separator();
                    insertMode = (cInsertMode)EditorGUILayout.EnumPopup(new GUIContent("Insert mode", "Select the way to insert decals or prefabs using keyboard and mouse"), insertMode, new GUILayoutOption[] { GUILayout.Width(0.81f * position.width) });


                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                    EditorBasicFunctions.DrawLineSeparator();
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnGUI
        /// # Handle OnGUI
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0.0f * position.width, 0, 1f * position.width, position.height));


            DrawBasics();
            DrawModeButtons();

            EditorBasicFunctions.DrawLineSeparator();

            windowPosition = EditorGUILayout.BeginScrollView(windowPosition, false, false);

            DrawGeneralOptions();
            DrawBasicButtons();
            DrawAdvancedButtons();
            DrawExtraOptions();


            // draw specifics modes
            switch (systemMode)
            {
                case cSystemMode.edition:
                    {
                        DrawEditionMode();
                    }
                    break;

                case cSystemMode.meshDecals:
                    {
                        DrawMeshDecalsMode();
                    }
                    break;

                case cSystemMode.projectedDecals:
                    {
                        DrawProjectedDecalsMode();
                    }
                    break;

                case cSystemMode.objects:
                    {
                        DrawObjectsMode();
                    }
                    break;
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();

            // save config
            if (configSaver != null)
            {
                configSaver.SaveConfig();
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetDoingSomethingSpecial
        /// # Return true if we are doing something special with the input
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        bool GetDoingSomethingSpecial()
        {
            switch (DecalManagerWindow.insertMode)
            {
                case DecalManagerWindow.cInsertMode.controlAndMouse:
                    {
                        return ((Event.current.button == 1) || (Event.current.button == 2) || (Event.current.shift) || (Event.current.alt));
                    }

                case DecalManagerWindow.cInsertMode.shiftAndMouse:
                    {
                        return ((Event.current.button == 1) || (Event.current.button == 2) || (Event.current.control) || (Event.current.alt));
                    }

                case DecalManagerWindow.cInsertMode.justMouse:
                    {
                        return ((Event.current.button == 1) || (Event.current.button == 2) || (Event.current.control) || (Event.current.shift) || (Event.current.alt));
                    }

                case DecalManagerWindow.cInsertMode.disabled:
                    {
                        return true;
                    }
            }

            return false;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// # Update editor window
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            double editorDeltaTimeToForceSelect = EditorApplication.timeSinceStartup - previousEditorTimeToForceSelect;
            previousEditorTimeToForceSelect = EditorApplication.timeSinceStartup;

            if (actualObjectToForceSelect)
            {
                if (actualObjectToForceSelect)
                {
                    //Debug.Log ("actualObjectToForceSelect: " + actualObjectToForceSelect.name);
                    Selection.activeObject = actualObjectToForceSelect;
                }

                actualObjectToForceSelectCounter += editorDeltaTimeToForceSelect;

                if (actualObjectToForceSelectCounter > 0.25f)
                {
                    //Debug.Log ("actualObjectToForceSelectCounter: " + actualObjectToForceSelectCounter);

                    actualObjectToForceSelectCounter = 0;
                    actualObjectToForceSelect = null;
                }
            }


            // handle selection changes
            if ((lastSelectedActiveObject != null) && (Selection.activeObject != lastSelectedActiveObject))
            {
                //Debug.Log ("Selection changes");

                lastSelectedActiveObject = Selection.activeObject;

                lastSelectedActiveObject = null;

                Repaint();
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SceneGUI
        /// # Handle SceneGUI
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void SceneGUI(SceneView sceneView)
        {
            UpdateManager();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdateManager
        /// # Update this manager
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void UpdateManager()
        {
            HandleAllElementsEdition();

            // handle general
            if (!GetDoingSomethingSpecial())
            {
                // handle modes in normal state
                switch (systemMode)
                {
                    case cSystemMode.edition:
                        {
                            HandleExtraOptions();
                        }
                        break;

                    case cSystemMode.meshDecals:
                        {
                            HandleMeshDecalsMode();
                        }
                        break;

                    case cSystemMode.projectedDecals:
                        {
                            HandleProjectedDecalsMode();
                        }
                        break;

                    case cSystemMode.objects:
                        {
                            HandleObjectsMode();
                        }
                        break;
                }
            }
        }
    }
}

#endif
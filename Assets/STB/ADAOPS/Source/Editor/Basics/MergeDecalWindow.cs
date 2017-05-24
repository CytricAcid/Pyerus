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
	/// Class: MergeDecalWindow
	/// # Main window class to merge decals
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public partial class MergeDecalWindow : EditorWindow
	{			
		// private -- static
		static string mergedObjectName = "NewMergedDecals";
		static bool deleteOldDecals = true;
		static bool resizeIfNeeded = true;


		// Add shortcut in Window menu
		[MenuItem ("Tools/STB/ADAOPS/Merge decals")]


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Init
		/// # Initialise the window and show it
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		static void Init ()
		{
			// Get existing open window or if none, make a new one:
			MergeDecalWindow window = (MergeDecalWindow)EditorWindow.GetWindow (typeof(MergeDecalWindow));

			window.Show ();
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// OnDestroy
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void OnDestroy ()
		{
			resizeIfNeeded = true;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// MergeAllDecals
		/// # Merge all decals in scene
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void MergeAllDecals ()
		{
			GenericMeshDecal[] actualDecalsInSceneList = (GenericMeshDecal[])GameObject.FindObjectsOfType (typeof(GenericMeshDecal));

			List<GameObject> actualDecalsInSceneGameObjectList = new List<GameObject> ();

			for (int i = 0; i < actualDecalsInSceneList.Count(); i++)
			{
				Undo.RegisterFullObjectHierarchyUndo (actualDecalsInSceneList [i], "Merge decals: delete originals");

				GenericMeshDecal actualDecal = actualDecalsInSceneList [i].gameObject.GetComponent<GenericMeshDecal> ();
				
				if (actualDecal)
				{
					actualDecalsInSceneGameObjectList.Add (actualDecal.gameObject);
				}
			}
			
			if (actualDecalsInSceneGameObjectList.Count > 0)
			{
				MeshMergerer.Merge (actualDecalsInSceneGameObjectList, mergedObjectName, deleteOldDecals);
			}
			else
			{	
				EditorUtility.DisplayDialog ("ERROR", "There are not decals in scene", "OK");
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// MergeActualSelectedDecals
		/// # Merge actual selected decals into one
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void MergeActualSelectedDecals ()
		{
			List<GameObject> selectedMeshDecalsList = new List<GameObject> ();
			
			for (int i = 0; i < Selection.gameObjects.Count(); i++)
			{
				GenericMeshDecal actualSelectedDecal = Selection.gameObjects [i].GetComponent<GenericMeshDecal> ();
				
				if (actualSelectedDecal)
				{
					selectedMeshDecalsList.Add (actualSelectedDecal.gameObject);
				}
			}
			
			if (selectedMeshDecalsList.Count > 0)
			{
				MeshMergerer.Merge (selectedMeshDecalsList, mergedObjectName, deleteOldDecals);
			}
			else
			{	
				EditorUtility.DisplayDialog ("ERROR", "You need a texture to create a decal", "OK");
			}
		} 
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// OnGUI
		/// # Handle OnGUI
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void OnGUI ()
		{	
			// resize
			if (resizeIfNeeded && ((position.width != 320) || (position.height != 280)))
			{
				resizeIfNeeded = false;
				
				Rect actualPosition = position;
				
				actualPosition.width = 320;
				actualPosition.height = 280;
				
				position = actualPosition;
			}

			GUILayout.BeginArea (new Rect (0.0f * position.width, 0, position.width, position.height));
			
			// title
			EditorGUILayout.Separator ();
			EditorBasicFunctions.DrawEditorBox ("Merge decals", Color.yellow, position);
			EditorGUILayout.Separator ();

			deleteOldDecals = EditorGUILayout.Toggle (new GUIContent ("Delete old decals", "Delete selected decals once the new gameobject is created"), deleteOldDecals);

		
			// decal name
			EditorGUILayout.Separator ();
			
			GUILayout.BeginHorizontal ();
			
			EditorGUILayout.LabelField (new GUIContent ("Merged name", "Once merge the name of the create gameobject"), new GUILayoutOption[]{GUILayout.Width (100)});
			mergedObjectName = EditorGUILayout.TextArea (mergedObjectName, new GUILayoutOption[]{GUILayout.Width (200)});
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();		


			// create button
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

			// merge selected
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();

			if (GUILayout.Button (new GUIContent ("MERGE SELECTED", "Merge actual selected decals"), new GUILayoutOption[] {GUILayout.Height (32)}))
			{			
				MergeActualSelectedDecals ();
				Close ();
			}

			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();	


			// merge all
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();

			if (GUILayout.Button (new GUIContent ("MERGE ALL", "Merge all decals in the scene. Use with cauttion!"), new GUILayoutOption[] {GUILayout.Height (32)}))
			{			
				MergeAllDecals ();
				Close ();
			}

			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();		


			GUILayout.EndArea ();
		}
	}
}


#endif
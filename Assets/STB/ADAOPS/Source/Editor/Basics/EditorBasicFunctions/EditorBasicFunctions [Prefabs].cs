#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: EditorBasicFunctions
	/// # Compilation on functions used by editor
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public partial class EditorBasicFunctions : MonoBehaviour
	{	
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetObjectList
		/// # Return a prefab list from a folder
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static List<GameObject> GetPrefabList ()
		{
			List<Object> objectList = GetObjectListFromDirectory (BasicDefines.MAIN_PATH + "Paintable/Objects/Prefabs/", ".prefab");
			List<GameObject> prefabList = new List<GameObject> ();
			
			foreach (Object obj in objectList)
			{
				prefabList.Add (obj as GameObject);
			}
			
			return prefabList;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DrawObjectlList
		/// # Draw actual selectable prefab list (looking inside it's folder) and handle dev's selections
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static GameObject DrawPrefabList (GameObject actualGameObject, Rect position)
		{
			// save actual gui color
			Color actualGuiColor = GUI.color;
			Color bgColor = GUI.backgroundColor;
			
			GameObject selectedGameObject = actualGameObject;
			
			EditorGUILayout.Separator ();

			// get actual prefab list			
			bool redoTheList = false;
			
			for (int i = 0; i  < prefabList.Count; i++)
			{
				if (AssetDatabase.GetAssetPath (prefabList [i]).Length < 2)
				{
					redoTheList = true;
				}
			}
			
			if (redoTheList)
			{				
				Debug.Log ("Prefab deleted, redo prefab decal list");
			}

			if ((prefabList.Count <= 0) || redoTheList)
			{
				prefabList = EditorBasicFunctions.GetPrefabList ();
			}
			
			// calculate paths
			List<string> localTotalGameObjectPathList = new List<string> ();
			
			for (int i = 0; i < prefabList.Count; i++)
			{
				string totalPath = AssetDatabase.GetAssetPath (prefabList [i]);
				string actualPath = Path.GetDirectoryName (totalPath);
				
				int index = 0;
				
				for (int j = 0; j < actualPath.Length; j++)
				{
					if (actualPath [j] == '/')
					{
						index = j;
					}
				}
				
				string finalPath = actualPath.Substring (index + 1, actualPath.Length - index - 1);
				
				localTotalGameObjectPathList.Add (finalPath);
			}
			
			List<string> localGameObjectPathList = localTotalGameObjectPathList.Distinct ().ToList ();

			if (!showSystemFolders)
			{
				localGameObjectPathList.Remove ("System");
			}

			//print ("------------------------------------------");
			//for (int i = 0; i < localGameObjectPathList.Count; i++)
			//{
			//print (localGameObjectPathList [i]);
			//}
			
			EditorGUILayout.Separator ();
			GenericObject.actualFolderIndex = EditorGUILayout.Popup (GenericObject.actualFolderIndex, localGameObjectPathList.ToArray (), new GUILayoutOption[] {GUILayout.Width (0.81f * position.width)});

			EditorGUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();

			if (EditorBasicFunctions.GetEditorTextButton ("Refresh", "Refresh the list, just in case", position))
			{
				RefreshLists ();
			}

			GUILayout.FlexibleSpace ();
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			
			string selectedGameObjectPath = BasicDefines.NOT_DEFINED;
			
			if (localGameObjectPathList.Count > 0)
			{
				selectedGameObjectPath = localGameObjectPathList [GenericObject.actualFolderIndex];
			}
			
			
			// test some things
			int numberOfObjectsPerLine = 4;
			int cont = 0;		
			bool begin = false;
			bool actualSelectedElementIsInSelectedPathList = false;
			GameObject firstElementInActualPath = null;


			// draw objects in editor window
			for (int i = 0; i < Mathf.Ceil((float)prefabList.Count); i++)
			{			
				if (localTotalGameObjectPathList [i] == selectedGameObjectPath)
				{		
					if (firstElementInActualPath == null)
					{
						firstElementInActualPath = prefabList [i];
					}
					
					if ((selectedGameObject == prefabList [i]))
					{
						actualSelectedElementIsInSelectedPathList = true;
					}

					if (cont % numberOfObjectsPerLine == 0)
					{
						GUILayout.BeginHorizontal ();
						begin = true;
					}	

					Texture previsualization = BasicFunctions.GetThumbnail (prefabList [i]);

					Texture unityPreview = AssetPreview.GetAssetPreview (actualGameObject);

					if (unityPreview)
					{
						//previsualization = unityPreview;
					}

					if (actualGameObject == prefabList [i])
					{
						GUI.color = new Color (1, 1, 1, 1);
						GUI.backgroundColor = new Color (0.6f, 0.0f, 0.6f, 1f);
					}
					else
					{
						GUI.color = new Color (1, 1, 1, 1f);
						GUI.backgroundColor = new Color (1, 1, 1, 0.3f);
					}
					
					float buttonsScale = 0.94f * position.width / (numberOfObjectsPerLine + 0.3f);
					
					bool selected = GUILayout.Button (new GUIContent (previsualization, prefabList [i].name), new GUILayoutOption[] {
						GUILayout.Width (buttonsScale),
						GUILayout.Height (buttonsScale)});
					
					
					if (selected)
					{
						selectedGameObject = prefabList [i];
					}		 
					
					if (cont % numberOfObjectsPerLine == numberOfObjectsPerLine - 1)
					{
						GUILayout.EndHorizontal ();
						begin = false;
					}	
					
					cont++;
				}
			}

			if (!actualSelectedElementIsInSelectedPathList)
			{
				//print ("Change to firstElementInActualPath: " + firstElementInActualPath.name);
				selectedGameObject = firstElementInActualPath;
			}
			
			if (begin)
			{
				EditorGUILayout.EndHorizontal ();
			}
			
			// restore Gui Color
			GUI.color = actualGuiColor;
			GUI.backgroundColor = bgColor;
			
			
			//return selected game object
			return selectedGameObject;
		}
	}
}

#endif
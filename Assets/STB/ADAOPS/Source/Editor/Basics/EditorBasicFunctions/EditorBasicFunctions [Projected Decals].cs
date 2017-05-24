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
		/// GetProjectedDecalMaterialList
		/// # Return a projected decal material list from a folder
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static List<Material> GetProjectedDecalMaterialList ()
		{			
			List<Object> objectList = GetObjectListFromDirectory (BasicDefines.MAIN_PATH + "Paintable/ProjectedDecals/Materials/", ".mat");
			List<Material> materialList = new List<Material> ();
			
			foreach (Object obj in objectList)
			{
				materialList.Add (obj as Material);
			}

			//Debug.Log ("materialList count: " + materialList.Count);
			
			return materialList;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DrawProjectedDecalElements
		/// # Draw all projected decal elements
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Material DrawProjectedDecalElements (GenericProjectorDecal decal, bool comeFromEditor, Rect position, ConfigSaver configSaver)
		{			
			if (decal)
			{
				EditorBasicFunctions.DrawEditorBox ("Insert projected decals: Options", Color.white, position);		

				if (!configSaver.parameters.hideBasicHelp)
				{
					EditorBasicFunctions.DrawEditorBox ("NOTE: Projected decals are experimental, they work but not perfectly!\n" + GetInsertModeHelpString () + " them", Color.yellow, position);
				}

				EditorGUILayout.Separator ();

				configSaver.parameters.showProjectedDecalsConfigOptions = EditorGUILayout.Foldout (configSaver.parameters.showProjectedDecalsConfigOptions, new GUIContent ("Show projected decals configuration options", "Show projected decals configuration options"));
				
				if (!configSaver.parameters.showProjectedDecalsConfigOptions)
				{
					EditorGUILayout.Separator ();
				}
				else
				{			
					EditorBasicFunctions.DrawEditorBox ("Configuration", Color.yellow, position);

					EditorGUILayout.Separator ();

					decal.attachToCollisionObject = EditorGUILayout.Toggle (new GUIContent ("Attach to father", "Attach created decal to hit object"), decal.attachToCollisionObject);

					if (comeFromEditor)
					{												
						EditorGUILayout.Separator ();
				
						decal.scaleRange = EditorGUILayout.Vector2Field (new GUIContent ("Scale range", "Randomize decal scale between 2 values"), decal.scaleRange, new GUILayoutOption[]{GUILayout.Width (0.5f * position.width)});
						decal.scaleRange.x = Mathf.Clamp (decal.scaleRange.x, 0.01f, 10);
						decal.scaleRange.y = Mathf.Clamp (decal.scaleRange.y, 0.01f, 10);
				
						EditorGUILayout.Separator ();
				
						decal.rotationRange = EditorGUILayout.Vector2Field (new GUIContent ("Rotation range", "Randomize decal rotation between 2 values"), decal.rotationRange, new GUILayoutOption[]{GUILayout.Width (0.5f * position.width)});
						decal.rotationRange.x = Mathf.Clamp (decal.rotationRange.x, 0, 360);
						decal.rotationRange.y = Mathf.Clamp (decal.rotationRange.y, 0, 360);
					}
						
					EditorGUILayout.Separator ();
					EditorGUILayout.Separator ();
				}
			
				EditorBasicFunctions.DrawEditorBox ("Choose projected decal!", Color.white, position);	
				EditorGUILayout.Separator ();
				EditorGUILayout.Separator ();
			
				decal.material = EditorBasicFunctions.ShowObjectField<Material> ("Actual selected material ", decal.material);
			
				decal.material = EditorBasicFunctions.DrawProjectedDecalMaterialList (decal.material, Screen.width);
			
				EditorGUILayout.Separator ();
				EditorGUILayout.Separator ();
			
				if (decal.material)
				{
					List<Sprite> spriteListFromTexture = EditorBasicFunctions.GetSpriteListFromTexture (decal.material.mainTexture);
			
					if (spriteListFromTexture.Count > 0)
					{
						decal.sprite = spriteListFromTexture [0];
					}
			
					if (!comeFromEditor)
					{
				
						EditorBasicFunctions.DrawEditorBox ("Info", Color.white, position);								
				
						EditorGUILayout.Separator ();
					}

					return decal.material;
				}
			}


			return null;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DrawProjectedDecalMaterialList
		/// Draw actual selectable material (looking inside it's folder) list and handle dev's selections
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Material DrawProjectedDecalMaterialList (Material actualMaterial, float areaWidth)
		{
			// save actual gui color
			Color actualGuiColor = GUI.color;
			Color bgColor = GUI.backgroundColor;
			
			// initialise selected material to actual material
			Material selectedMaterial = actualMaterial;
			
			EditorGUILayout.Separator ();
			
			// get actual material list from decal's material folder
			bool redoTheList = false;
			
			for (int i = 0; i  < projectedDecalMaterialsList.Count; i++)
			{
				if (AssetDatabase.GetAssetPath (projectedDecalMaterialsList [i]).Length < 2)
				{
					redoTheList = true;
				}
			}
			
			if (redoTheList)
			{				
				Debug.Log ("Material deleted, redo projected decal list");
			}

			if ((projectedDecalMaterialsList.Count <= 0) || redoTheList)
			{
				projectedDecalMaterialsList = EditorBasicFunctions.GetProjectedDecalMaterialList ();			
			}
			
			// calculate paths
			List<string> localTotalMaterialPathList = new List<string> ();
			
			for (int i = 0; i < projectedDecalMaterialsList.Count; i++)
			{
				string totalPath = AssetDatabase.GetAssetPath (projectedDecalMaterialsList [i]);
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
				
				localTotalMaterialPathList.Add (finalPath);
			}
			
			List<string> localMaterialPathList = localTotalMaterialPathList.Distinct ().ToList ();

			if (!showSystemFolders)
			{
				localMaterialPathList.Remove ("System");
			}
			
			//print ("------------------------------------------");
			//for (int i = 0; i < localMaterialPathList.Count; i++)
			//{
			//print (localMaterialPathList [i]);
			//}
			
			EditorGUILayout.Separator ();
			GenericProjectorDecal.actualFolderIndex = EditorGUILayout.Popup (GenericProjectorDecal.actualFolderIndex, localMaterialPathList.ToArray (), new GUILayoutOption[] {GUILayout.Width (0.81f * areaWidth)});
			EditorGUILayout.Separator ();
			
			string selectedMaterialPath = BasicDefines.NOT_DEFINED;
			
			if (localMaterialPathList.Count > 0)
			{
				selectedMaterialPath = localMaterialPathList [GenericProjectorDecal.actualFolderIndex];
			}
			
			// test some things
			int numberOfMaterialsPerLine = 4;			
			bool begin = false;
			int cont = 0;
			bool actualSelectedElementIsInSelectedPathList = false;			
			Material firstElementInActualPath = null;


			// draw materials in editor window
			for (int i = 0; i < Mathf.Ceil((float)projectedDecalMaterialsList.Count); i++)
			{			
				if (localTotalMaterialPathList [i] == selectedMaterialPath)
				{
					if (firstElementInActualPath == null)
					{ 
						firstElementInActualPath = projectedDecalMaterialsList [i];
					}

					if ((selectedMaterial == projectedDecalMaterialsList [i]))
					{
						actualSelectedElementIsInSelectedPathList = true;
					}

					if (cont % numberOfMaterialsPerLine == 0)
					{
						GUILayout.BeginHorizontal ();
						begin = true;
					}	
					
					if (actualMaterial == projectedDecalMaterialsList [i])
					{
						GUI.color = new Color (1, 1, 1, 1);
						GUI.backgroundColor = new Color (0.6f, 0.0f, 0.6f, 1f);
					}
					else
					{
						GUI.color = new Color (1, 1, 1, 1f);
						GUI.backgroundColor = new Color (1, 1, 1, 0.3f);
					}
					
					float buttonsScale = 0.94f * areaWidth / (numberOfMaterialsPerLine + 0.3f);
					
					// test if one material is selected
					bool selected = GUILayout.Button (new GUIContent (projectedDecalMaterialsList [i].mainTexture, projectedDecalMaterialsList [i].name), new GUILayoutOption[] {
						GUILayout.Width (buttonsScale),
						GUILayout.Height (buttonsScale)});
					
					// if selected then is the actual selected material
					if (selected)
					{
						selectedMaterial = projectedDecalMaterialsList [i];
					}		
					
					if (cont % numberOfMaterialsPerLine == numberOfMaterialsPerLine - 1)
					{
						GUILayout.EndHorizontal ();
						begin = false;
					}	
					
					cont++;
				}
			}

			if (!actualSelectedElementIsInSelectedPathList && DecalManagerWindow.autoChangeToFirstElementInList)
			{
				//print ("Change to firstElementInActualPath: " + firstElementInActualPath.name);
				selectedMaterial = firstElementInActualPath;
			} 
			
			if (begin)
			{
				EditorGUILayout.EndHorizontal ();
			}
			
			// restore Gui Color
			GUI.color = actualGuiColor;
			GUI.backgroundColor = bgColor;
			
			
			// return actual selected material
			return selectedMaterial;
		}
	}
}

#endif
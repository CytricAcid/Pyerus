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
		/// GetMeshDecalMaterialList 
		/// # Return a mesh decal material list from a folder
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static List<Material> GetMeshDecalMaterialList ()
		{			
			List<Object> objectList = GetObjectListFromDirectory (BasicDefines.MAIN_PATH + "Paintable/MeshDecals/Materials/", ".mat");
			List<Material> materialList = new List<Material> ();
			
			foreach (Object obj in objectList)
			{
				materialList.Add (obj as Material);
			}
			
			return materialList;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetInsertModeHelpString
		/// # Returns a string with insert mode help
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static string GetInsertModeHelpString ()
		{
			switch (DecalManagerWindow.insertMode)
			{
			case DecalManagerWindow.cInsertMode.controlAndMouse:
				{
					return "Use 'Control+Left Mouse Button' to insert";
				}

			case DecalManagerWindow.cInsertMode.shiftAndMouse:
				{
					return "Use 'Shift+Left Mouse Button' to insert";
				}
				
			case DecalManagerWindow.cInsertMode.justMouse:
				{
					return "Use 'Left Mouse Button' to insert";
				}
				
			case DecalManagerWindow.cInsertMode.disabled:
				{
					return "Insert mode disabled, select another mode in advanced options to insert";
				}
			}

			return "Unkown insert mode";
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DrawMeshDecalElements
		/// # Draw all mesh decal elements
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Material DrawMeshDecalElements (GenericMeshDecal decal, bool comeFromEditor, Rect position, ConfigSaver configSaver)
		{			
			if (decal)
			{				
				EditorBasicFunctions.DrawEditorBox ("Insert mesh decals: Options", Color.white, position);

				if ((configSaver != null) && !configSaver.parameters.hideBasicHelp)
				{
					EditorBasicFunctions.DrawEditorBox (GetInsertModeHelpString () + " mesh decals", Color.yellow, position);
				}
				
				EditorGUILayout.Separator ();

				bool showConfigOptions = true;

				if (configSaver != null)
				{
					configSaver.parameters.showMeshDecalsConfigOptions = EditorGUILayout.Foldout (configSaver.parameters.showMeshDecalsConfigOptions, new GUIContent ("Show mesh decals configuration options", "Show mesh decals configuration options"));

					showConfigOptions = configSaver.parameters.showMeshDecalsConfigOptions;
				}
				
				if (!showConfigOptions)
				{
				}
				else
				{			
					EditorBasicFunctions.DrawEditorBox ("Configuration", Color.yellow, position);
					
					EditorGUILayout.Separator ();
					
					decal.attachToCollisionObject = EditorGUILayout.Toggle (new GUIContent ("Attach to father", "Attach created decal to hit object"), decal.attachToCollisionObject);
					
					if (comeFromEditor)
					{						
						decal.futureTimeLockableShape = EditorGUILayout.Toggle (new GUIContent ("Future time lockable shape", "It this is checked you can use the button called 'Lock all future time lockable decals' in advance options to lock all 'futureTimeLockableShape' checked on decals, Use with caution"), decal.futureTimeLockableShape);
						decal.addCollider = EditorGUILayout.Toggle (new GUIContent ("Add collider", "Add a collider to be detected for the next decals, Use with caution"), decal.addCollider);
						planarMeshDecals = EditorGUILayout.Toggle (new GUIContent ("Simple planar", "Do the decal to be a simple plane, it's performance friendly"), planarMeshDecals);
						EditorGUILayout.Separator ();
												
						decal.scaleRange = EditorGUILayout.Vector2Field (new GUIContent ("Scale range", "Randomize decal scale between 2 values"), decal.scaleRange, new GUILayoutOption[]{GUILayout.Width (0.5f * position.width)});
						decal.scaleRange.x = Mathf.Clamp (decal.scaleRange.x, 0.01f, 10);
						decal.scaleRange.y = Mathf.Clamp (decal.scaleRange.y, 0.01f, 10);
						
						EditorGUILayout.Separator ();
						
						decal.rotationRange = EditorGUILayout.Vector2Field (new GUIContent ("Rotation range", "Randomize decal rotation between 2 values"), decal.rotationRange, new GUILayoutOption[]{GUILayout.Width (0.5f * position.width)});
						decal.rotationRange.x = Mathf.Clamp (decal.rotationRange.x, 0, 360);
						decal.rotationRange.y = Mathf.Clamp (decal.rotationRange.y, 0, 360);
					}
					else
					{	
						decal.lockedShapeAlways = EditorGUILayout.Toggle (new GUIContent ("Lock shape always", "Lock shape always"), decal.lockedShapeAlways);

						decal.lockedShapeInRuntime = EditorGUILayout.Toggle (new GUIContent ("Lock shape in runtime", "Lock shape in runtime"), decal.lockedShapeInRuntime);

						decal.planarDecal = EditorGUILayout.Toggle (new GUIContent ("A Simple planar", "Do the decal to be a simple plane, it's performance friendly"), decal.planarDecal);
						EditorGUILayout.Separator ();
					}
					
					EditorGUILayout.Separator ();
					
					decal.angleLimit = EditorGUILayout.Slider (new GUIContent ("Collision angle limit", "For not planar decals, max angle between hit objects"), decal.angleLimit, 1, 180);
					decal.angleLimit = Mathf.Clamp (decal.angleLimit, 1, 180);
					
					decal.distanceFromHit = EditorGUILayout.Slider (new GUIContent ("Distance from hit", "Distance in the hit normal from hit object"), decal.distanceFromHit, 0.001f, 1);
					decal.distanceFromHit = Mathf.Clamp (decal.distanceFromHit, 0.001f, 1);
					
					EditorGUILayout.Separator ();
					EditorGUILayout.Separator ();
				}
				
				EditorBasicFunctions.DrawEditorBox ("Choose mesh decal!", Color.white, position);	
				EditorGUILayout.Separator ();
				EditorGUILayout.Separator ();
				
				decal.material = EditorBasicFunctions.ShowObjectField<Material> ("Actual selected material ", decal.material);
				
				decal.material = EditorBasicFunctions.DrawMeshDecalMaterialList (decal.material, Screen.width);
				
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
						//EditorGUILayout.Separator ();	
						//EditorGUILayout.Separator ();
						
						EditorBasicFunctions.DrawEditorBox ("Info", Color.white, position);	
						
						//decal.affectedLayers = EditorBasicFunctions.LayerMaskField ("Affected Layers", decal.affectedLayers);
						
						decal.showAffectedObject = EditorGUILayout.Foldout (decal.showAffectedObject, "Affected Objects");
						
						if (decal.showAffectedObject && (decal.affectedObjects != null))
						{
							GUILayout.BeginHorizontal ();
							GUILayout.Space (15);
							GUILayout.BeginVertical ();
							
							foreach (GameObject go in decal.affectedObjects)
							{
								EditorGUILayout.ObjectField (go, typeof(GameObject), true);
							}
							GUILayout.EndVertical ();
							GUILayout.EndHorizontal ();
						}
						
						EditorGUILayout.Separator ();
					}
					
					
					return decal.material;
				}
			}
			
			
			return null;
		}	
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DrawMeshDecalMaterialList
		/// Draw actual selectable material (looking inside it's folder) list and handle dev's selections
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Material DrawMeshDecalMaterialList (Material actualMaterial, float areaWidth)
		{
			// save actual gui color
			Color actualGuiColor = GUI.color;
			Color bgColor = GUI.backgroundColor;
			
			// initialise selected material to actual material
			Material selectedMaterial = actualMaterial;

			EditorGUILayout.Separator ();

			// get actual material list from decal's material folder
			bool redoTheList = false;

			for (int i = 0; i  < meshDecalMaterialsList.Count; i++)
			{
				if (AssetDatabase.GetAssetPath (meshDecalMaterialsList [i]).Length < 2)
				{
					redoTheList = true;
				}
			}

			if (redoTheList)
			{				
				Debug.Log ("Material deleted, redo mesh decal list");
			}

			if ((meshDecalMaterialsList.Count <= 0) || redoTheList)
			{				
				meshDecalMaterialsList = EditorBasicFunctions.GetMeshDecalMaterialList ();	
			}
			
			// calculate paths
			List<string> localTotalMaterialPathList = new List<string> ();
			
			for (int i = 0; i < meshDecalMaterialsList.Count; i++)
			{
				string totalPath = AssetDatabase.GetAssetPath (meshDecalMaterialsList [i]);
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
			GenericMeshDecal.actualFolderIndex = EditorGUILayout.Popup (GenericMeshDecal.actualFolderIndex, localMaterialPathList.ToArray (), new GUILayoutOption[] {GUILayout.Width (0.81f * areaWidth)});
			EditorGUILayout.Separator ();
			
			string selectedMaterialPath = BasicDefines.NOT_DEFINED;
			
			if (localMaterialPathList.Count > 0)
			{
				selectedMaterialPath = localMaterialPathList [GenericMeshDecal.actualFolderIndex];
			}
			
			// test some things
			int numberOfMaterialsPerLine = 4;			
			bool begin = false;
			int cont = 0;
			bool actualSelectedElementIsInSelectedPathList = false;			
			Material firstElementInActualPath = null;


			// draw materials in editor window
			for (int i = 0; i < Mathf.Ceil((float)meshDecalMaterialsList.Count); i++)
			{			
				if (localTotalMaterialPathList [i] == selectedMaterialPath)
				{
					if (firstElementInActualPath == null)
					{ 
						firstElementInActualPath = meshDecalMaterialsList [i];
					}

					if ((selectedMaterial == meshDecalMaterialsList [i]))
					{
						actualSelectedElementIsInSelectedPathList = true;
					}

					if (cont % numberOfMaterialsPerLine == 0)
					{
						GUILayout.BeginHorizontal ();
						begin = true;
					}	
					
					if (actualMaterial == meshDecalMaterialsList [i])
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
					bool selected = GUILayout.Button (new GUIContent (meshDecalMaterialsList [i].mainTexture, meshDecalMaterialsList [i].name), new GUILayoutOption[] {
						GUILayout.Width (buttonsScale),
						GUILayout.Height (buttonsScale)});
					 
					// if selected then is the actual selected material
					if (selected)
					{
						selectedMaterial = meshDecalMaterialsList [i];
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
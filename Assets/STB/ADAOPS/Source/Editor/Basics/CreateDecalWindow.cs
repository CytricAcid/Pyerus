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
	/// Class: CreateDecalWindow
	/// # Main window class to create a new decal
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public partial class CreateDecalWindow : EditorWindow
	{			
		// private
		enum cCreateDecalType
		{
			fade,
			cutout}
		;

		// private static
		static cCreateDecalType createDecalType = cCreateDecalType.cutout;
		static Texture createDecalTexture_albedo = null;
		static Texture createDecalTexture_normal = null;
		static Texture createDecalTexture_emission = null;
		static string createDecalName = "NewDecal";
		static int actualCreateDecalFadeFolderIndex = 0;
		static int actualCreateDecalCutOutFolderIndex = 0;
		static List<string> fadeFolderNameList = new List<string> ();
		static List<string> cutOutFolderNameList = new List<string> ();
		static bool resizeIfNeeded = true;


		// Add shortcut in Window menu
		[MenuItem ("Tools/STB/ADAOPS/Create new decal")]


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Init
		/// # Initialise the window and show it
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		static void Init ()
		{
			// Get existing open window or if none, make a new one:
			CreateDecalWindow window = (CreateDecalWindow)EditorWindow.GetWindow (typeof(CreateDecalWindow));

			window.Show ();
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// OnDestroy
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void OnDestroy ()
		{
			fadeFolderNameList.Clear ();
			cutOutFolderNameList.Clear ();
			resizeIfNeeded = true;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// CreateNewDecal
		/// # Create new decal
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void CreateNewDecal ()
		{					
			string actualFolderName = "";
			Material originalMaterial = null;

			switch (createDecalType)
			{
			case cCreateDecalType.fade:
				{
					actualFolderName = fadeFolderNameList [actualCreateDecalFadeFolderIndex];				
					originalMaterial = AssetDatabase.LoadAssetAtPath (BasicDefines.MAIN_PATH + "Paintable/MeshDecals/Materials/System/GenericFadeMaterial.mat", typeof(Material)) as Material;
				}
				break;
				
			case cCreateDecalType.cutout:
				{
					actualFolderName = cutOutFolderNameList [actualCreateDecalCutOutFolderIndex];	
					originalMaterial = AssetDatabase.LoadAssetAtPath (BasicDefines.MAIN_PATH + "Paintable/MeshDecals/Materials/System/GenericCutOutMaterial.mat", typeof(Material)) as Material;
				}
				break;
			}
			
			Material materialCopy = new Material (originalMaterial);

			AssetDatabase.CreateAsset (materialCopy, BasicDefines.MAIN_PATH + "Paintable/MeshDecals/Materials/" + actualFolderName + "/" + createDecalName + ".mat");
						
			materialCopy.SetColor ("_Color", Color.white);
			materialCopy.SetTexture ("_MainTex", createDecalTexture_albedo);
			materialCopy.SetTexture ("_BumpMap", createDecalTexture_normal);
			materialCopy.SetTexture ("_EmissionMap", createDecalTexture_emission);


			EditorBasicFunctions.RefreshLists ();
			AssetDatabase.Refresh ();


			// the material has changed
			MaterialChangeManager.MaterialChanged (materialCopy);
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


			DirectoryInfo actualDirInfo = new DirectoryInfo (BasicDefines.MAIN_PATH + "Paintable/MeshDecals/Materials/");

			if ((fadeFolderNameList.Count <= 0) || (cutOutFolderNameList.Count <= 0))
			{
				for (int i = 0; i < actualDirInfo.GetDirectories().Count(); i++)
				{
					string actualName = actualDirInfo.GetDirectories () [i].Name;
					//Debug.Log(actualName);

					if (actualName.Length > 1)
					{
						if ((actualName.Length > 7) && actualName.Substring (0, 7) == "[Fade] ")
						{
							fadeFolderNameList.Add (actualDirInfo.GetDirectories () [i].Name);
						}
						else
						{
							cutOutFolderNameList.Add (actualDirInfo.GetDirectories () [i].Name);
						}
					}
				}
			}

			GUILayout.BeginArea (new Rect (0.0f * position.width, 0, position.width, position.height));

			// title
			EditorGUILayout.Separator ();
			EditorBasicFunctions.DrawEditorBox ("Create a new decal", Color.yellow, position);
			EditorGUILayout.Separator ();


			// type		
			GUILayout.BeginHorizontal ();
			
			GUILayout.Label ("Mode");
			createDecalType = (cCreateDecalType)EditorGUILayout.EnumPopup (createDecalType);
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();


			// texture albedo
			EditorGUILayout.Separator ();
			
			GUILayout.BeginHorizontal ();
			
			EditorGUILayout.LabelField ("Albedo", new GUILayoutOption[]{GUILayout.Width (100)});
			createDecalTexture_albedo = EditorGUILayout.ObjectField (createDecalTexture_albedo, typeof(Texture), true, new GUILayoutOption[]{GUILayout.Width (200)}) as Texture;
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();	


			// texture normal
			EditorGUILayout.Separator ();
			
			GUILayout.BeginHorizontal ();
			
			EditorGUILayout.LabelField ("Normal", new GUILayoutOption[]{GUILayout.Width (100)});
			createDecalTexture_normal = EditorGUILayout.ObjectField (createDecalTexture_normal, typeof(Texture), true, new GUILayoutOption[]{GUILayout.Width (200)}) as Texture;
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();	



			// texture albedo
			EditorGUILayout.Separator ();
			
			GUILayout.BeginHorizontal ();
			
			EditorGUILayout.LabelField ("Emission", new GUILayoutOption[]{GUILayout.Width (100)});
			createDecalTexture_emission = EditorGUILayout.ObjectField (createDecalTexture_emission, typeof(Texture), true, new GUILayoutOption[]{GUILayout.Width (200)}) as Texture;
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();	



			// decal name
			EditorGUILayout.Separator ();

			GUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("Decal name", new GUILayoutOption[]{GUILayout.Width (100)});
			createDecalName = EditorGUILayout.TextArea (createDecalName, new GUILayoutOption[]{GUILayout.Width (200)});
			
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();		


			// folder
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

			GUILayout.BeginHorizontal ();
			
			GUILayout.Label ("Folder");

			switch (createDecalType)
			{
			case cCreateDecalType.fade:
				{
					actualCreateDecalFadeFolderIndex = EditorGUILayout.Popup (actualCreateDecalFadeFolderIndex, fadeFolderNameList.ToArray (), new GUILayoutOption[] {GUILayout.Width (200)});
				}
				break;

			case cCreateDecalType.cutout:
				{
					actualCreateDecalCutOutFolderIndex = EditorGUILayout.Popup (actualCreateDecalCutOutFolderIndex, cutOutFolderNameList.ToArray (), new GUILayoutOption[] {GUILayout.Width (200)});
				}
				break;
			}

			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();


			// create button
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();

			if (GUILayout.Button ("CREATE", new GUILayoutOption[] {GUILayout.Height (32)}))
			{			 
				if (createDecalTexture_albedo)
				{
					CreateNewDecal ();
					Close ();
				}
				else
				{
					EditorUtility.DisplayDialog ("ERROR", "You need a texture to create a decal", "OK");
				}
			}

			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();		


			GUILayout.EndArea ();
		}
	}
}


#endif
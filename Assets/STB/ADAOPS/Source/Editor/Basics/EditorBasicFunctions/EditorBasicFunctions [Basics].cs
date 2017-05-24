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
		// public -- mesh decals
		public static bool planarMeshDecals = true;

		// private -- system
		static bool showSystemFolders = false;

		// private static -- basic lists		
		static List<Material> meshDecalMaterialsList = new List<Material> ();
		static List<Material> projectedDecalMaterialsList = new List<Material> ();
		static List<GameObject> prefabList = new List<GameObject> ();


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// OnDestroy
		/// # To do on destroy
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void OnDestroy ()
		{
			RefreshLists ();
		}		
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// RefreshLists
		/// # Read again from disk prefabs and decal lists
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void RefreshLists ()
		{
			meshDecalMaterialsList.Clear ();
			projectedDecalMaterialsList.Clear ();
			prefabList.Clear ();
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetMouseButtonDown
		/// # Return true if mouse button is down (uses event current, not input)
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static bool GetMouseButtonDown (int buttonIndex)
		{
			switch (DecalManagerWindow.insertMode)
			{
			case DecalManagerWindow.cInsertMode.shiftAndMouse:
			case DecalManagerWindow.cInsertMode.controlAndMouse:	
			case DecalManagerWindow.cInsertMode.justMouse:
				{
					return ((Event.current.type == EventType.MouseDown) && (Event.current.button == buttonIndex));
				}
								
			case DecalManagerWindow.cInsertMode.disabled:
				{
					return false;
				}
			}

			return false;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Class: GetSpriteListFromTexture
		/// # Return a sprite list from a texture
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static List<Sprite> GetSpriteListFromTexture (Texture texture)
		{
			string actualPath = AssetDatabase.GetAssetPath (texture);
			Object[] objectList = AssetDatabase.LoadAllAssetsAtPath (actualPath);
			List<Sprite> spriteList = new List<Sprite> ();
			
			foreach (Object actualObject in objectList)
			{
				if (actualObject is Sprite)
				{
					spriteList.Add ((Sprite)actualObject);
				}
			}
			
			return spriteList;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// ShowObjectField
		/// # Show some object field
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static T ShowObjectField<T> (string label, T obj) where T : Object
		{
			return (T)EditorGUILayout.ObjectField (label, (T)obj, typeof(T), false);
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// LayerMaskField
		/// # Return a layer mask field
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static LayerMask LayerMaskField (string label, LayerMask mask)
		{
			List<string> layers = new List<string> ();
			
			for (int i = 0; i < 32; i++)
			{
				string name = LayerMask.LayerToName (i);

				if (name != "")
				{
					layers.Add (name);
				}
			}
			
			return EditorGUILayout.MaskField (label, mask, layers.ToArray ());
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetDecalGetObjectList
		/// # Return a prefab list from a folder
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		static List<Object> GetObjectListFromDirectory (string basePath, DirectoryInfo directoryInfo, string fileExtension)
		{
			List<Object> materialList = new List<Object> ();
			
			List<FileInfo> fileList = directoryInfo.GetFiles ("*.*").ToList ();
			
			for (int i = 0; i < fileList.Count; i++)
			{				
				if (fileList [i].Extension.ToLower () == fileExtension)
				{					
					string actualGetObjectPath = basePath + fileList [i].Name;
					
					Object actualGetObject = AssetDatabase.LoadAssetAtPath (actualGetObjectPath, typeof(Object)) as Object;
					
					if (actualGetObject)
					{
						materialList.Add (actualGetObject);
					}
					else
					{
						Debug.Log (actualGetObjectPath + " not a object of extension: " + fileExtension);
					}
				}
			}
			
			return materialList;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetObjectListFromDirectory
		/// # Return a prefab list from folder
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static List<Object> GetObjectListFromDirectory (string basePath, string fileExtension)
		{
			List<Object> materialList = new List<Object> ();
			
			DirectoryInfo actualDirInfo = new DirectoryInfo (basePath);
			
			List<DirectoryInfo> allDirInfoList = new List<DirectoryInfo> ();
			
			allDirInfoList.Add (actualDirInfo);
			
			foreach (DirectoryInfo dirInfo in actualDirInfo.GetDirectories())
			{
				allDirInfoList.Add (dirInfo);
			}
			
			foreach (DirectoryInfo dirInfo in allDirInfoList)
			{
				string finalbasePah = basePath;
				
				if (dirInfo != actualDirInfo)
				{
					finalbasePah += dirInfo.Name + "/";
				}
				
				List<Object> actualSubDirList = GetObjectListFromDirectory (finalbasePah, dirInfo, fileExtension);				
				
				for (int i = 0; i < actualSubDirList.Count; i++)
				{
					materialList.Add (actualSubDirList [i]);
				}
			}
			
			return materialList;
		}
	}
}


#endif
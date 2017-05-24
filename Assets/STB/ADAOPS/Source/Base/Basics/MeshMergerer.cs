using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: MeshMergerer
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class MeshMergerer : MonoBehaviour
	{ 	
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetMaterialListFromAGameObjectList
		/// # Merge mesh
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		static List<Material> GetMaterialListFromAGameObjectList (List<GameObject> gameObjectList)
		{
			// get mesh fiters
			List<MeshFilter> meshFilters = new List<MeshFilter> ();
			
			for (int i = 0; i < gameObjectList.Count; i++)
			{
				meshFilters.Add (BasicFunctions.GetMeshFilterInChilds (gameObjectList [i].transform));
			}	
			
			// get material list
			List<Material> materials = new List<Material> ();
			
			foreach (MeshFilter mf in meshFilters)
			{	
				MeshRenderer mr = mf.gameObject.GetComponent <MeshRenderer> ();		
				
				materials.Add (mr.gameObject.GetComponent<Renderer> ().sharedMaterial);
			}

			return materials;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Merge
		/// # Merge mesh
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void  Merge (List<GameObject> gameObjectListOriginal, string mergedObjectName, bool deleteOldObjects)
		{ 		
			List<Material> distinctMaterialList = GetMaterialListFromAGameObjectList (gameObjectListOriginal).Distinct ().ToList ();


			List<GameObject> finalGameObjectList = new List<GameObject> ();

			//Debug.Log ("----------------------------------------------------");

			for (int i = 0; i < distinctMaterialList.Count; i++)
			{
				//Debug.Log (i + " -> distinctMaterialList -> " + distinctMaterialList [i].name);

				List<GameObject> subGameObjectList = new List<GameObject> ();

				for (int j = 0; j < gameObjectListOriginal.Count; j++)
				{
					Material actualGameObjectMainMaterial = BasicFunctions.GetMainMaterial (gameObjectListOriginal [j].transform);

					if (!actualGameObjectMainMaterial)
					{
						//Debug.Log ("Null material: " + gameObjectListOriginal [j].name);
					}
					else
					{
						if (distinctMaterialList [i] == actualGameObjectMainMaterial)
						{
							//Debug.Log (j + " -> merge: " + gameObjectListOriginal [j].name);
							subGameObjectList.Add (gameObjectListOriginal [j]);
						}
						else
						{
							//Debug.Log (j + " -> not same material: " + actualGameObjectMainMaterial.name + " and distinct: " + distinctMaterialList [i]);
						}
					}
				}

				finalGameObjectList.Add (SubMerge (subGameObjectList, "sub_" + mergedObjectName, deleteOldObjects, true));
			}

			SubMerge (finalGameObjectList, mergedObjectName, deleteOldObjects, false);


			// delete olds			
			foreach (GameObject go in finalGameObjectList)
			{							
				GameObject.DestroyImmediate (go);
			}


			if (deleteOldObjects)
			{
				foreach (GameObject go in gameObjectListOriginal)
				{							
					GameObject.DestroyImmediate (go);
				}
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Merge
		/// # Merge mesh
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		static GameObject SubMerge (List<GameObject> gameObjectList, string mergedObjectName, bool deleteOldObjects, bool colapse)
		{ 			
			// create merged game object
			GameObject newGo = new GameObject (mergedObjectName);

			// get mesh fiters
			List<MeshFilter> meshFilters = new List<MeshFilter> ();

			for (int i = 0; i < gameObjectList.Count; i++)
			{
				meshFilters.Add (BasicFunctions.GetMeshFilterInChilds (gameObjectList [i].transform));
			}	

			// get material list
			List<Material> materials = new List<Material> ();
			
			foreach (MeshFilter mf in meshFilters)
			{	
				MeshRenderer mr = mf.gameObject.GetComponent <MeshRenderer> ();		
				
				materials.Add (mr.gameObject.GetComponent<Renderer> ().sharedMaterial);
			}
				

			CombineInstance[] combine = new CombineInstance[meshFilters.Count];
			
			int x = 0;
			
			while (x < meshFilters.Count)
			{
				combine [x].mesh = meshFilters [x].sharedMesh;
				combine [x].transform = meshFilters [x].transform.localToWorldMatrix;

				x++;
			}

			newGo.AddComponent<MeshFilter> ();
			newGo.GetComponent<MeshFilter> ().mesh = new Mesh ();
			newGo.GetComponent<MeshFilter> ().sharedMesh.name = "NewMesh";
			newGo.GetComponent<MeshFilter> ().sharedMesh.CombineMeshes (combine, colapse);


			// For MeshRenderer
			// Get / Create mesh renderer
			MeshRenderer meshRendererCombine = newGo.GetComponent<MeshRenderer> ();

			if (!meshRendererCombine)
			{
				meshRendererCombine = newGo.AddComponent<MeshRenderer> ();    
			}
				
			// Assign materials
			meshRendererCombine.materials = materials.ToArray ();


			// delete olds
			/*if (deleteOldObjects)
			{
				foreach (GameObject go in gameObjectList)
				{							
					GameObject.DestroyImmediate (go);
				}
			}*/

			return newGo;
		}
	}
}
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
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DrawMeshDecalsMode
		/// # Draw al gui buttons, checboxes, ... to handle and insert mesh decals in scene
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void DrawMeshDecalsMode ()
		{		
			Material actualMaterial = EditorBasicFunctions.DrawMeshDecalElements (meshDecalPrefab, true, position, configSaver);

			if (actualMaterial)
			{
				EditorBasicFunctions.DrawEditorBox (actualMaterial.name, Color.yellow, position);	
			}
			else
			{
				EditorBasicFunctions.DrawEditorBox ("No decal selected to put!", Color.gray, position);	
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// HandleMeshDecalsMode
		/// # To insert mesh decals in scene using mouse
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void HandleMeshDecalsMode ()
		{
			if ((GetEditorTimeDiff () > 0.1f) && EditorBasicFunctions.GetMouseButtonDown (0) && EditorBasicFunctions.GetInsertModeKeyPressed () && !GetDoingSomethingSpecial ())
			{			
				previousEditorTime = EditorApplication.timeSinceStartup;

				//Debug.Log ("Event.current.mousePosition: " + Event.current.mousePosition);
				
				Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
				RaycastHit hit;

				//Debug.Log ("-----------------------------------------------------------");

				if (Physics.Raycast (ray, out hit))
				{			
					if (lastMeshDecalsHitPoint.ToString () == hit.point.ToString ())
					{
						//Debug.Log ("NOTE: same point duplicate -> lastMeshDecalsHitPoint: " + lastMeshDecalsHitPoint);
					}
					else
					{
						//Debug.Log ("lastMeshDecalsHitPoint A: " + lastMeshDecalsHitPoint);

						lastMeshDecalsHitPoint = hit.point;

						//Debug.Log ("hit.point: " + hit.point);
						//Debug.Log ("lastMeshDecalsHitPoint B: " + lastMeshDecalsHitPoint);

						//Debug.Log ("New Decal");
						//Debug.Log ("Hit position: " + hit.point);
						//Debug.Log ("Collider Name: " + hit.collider.name);

						bool setPlanarDecal = EditorBasicFunctions.planarMeshDecals;

						if (hit.collider.GetComponent<Terrain> ())
						{
							//Debug.Log ("It's a terrain, set the decal as planar");
							setPlanarDecal = true;
						}
						
						meshDecalPrefab.planarDecal = setPlanarDecal;
						meshDecalPrefab.comeFromEditor = setPlanarDecal;

						GenericMeshDecal actualDecal = Instantiate (meshDecalPrefab.gameObject).GetComponent<GenericMeshDecal> ();
						actualDecal.transform.position = hit.point;

						meshDecalPrefab.comeFromEditor = false;

						Vector3 finalScale = Random.Range (actualDecal.scaleRange.x, actualDecal.scaleRange.y) * actualDecal.transform.localScale;

						float textureAspectRatio = (float)actualDecal.material.mainTexture.width / (float)actualDecal.material.mainTexture.height;

						if (actualDecal.GetPlanar ())
						{												
							finalScale.x = 0.5f * finalScale.x;
							finalScale.y = textureAspectRatio * finalScale.x;
						}
						else
						{						
							finalScale.x = textureAspectRatio * finalScale.x;
						}
					
						//finalScale.y = finalScale.x;
						//finalScale.z = finalScale.x;


						actualDecal.transform.localScale = finalScale;

						actualDecal.transform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);		
					
						actualDecal.name = actualDecal.Generate (BasicDefines.MESH_DECAL_BASE_NAME, GetSeedForInstancies (), true, actualDecal.material.name);														

						actualDecal.UpdateDecallShape (true, false);								

						//Debug.Log ("actualDecal.attachToCollisionObject: " + actualDecal.attachToCollisionObject);

						if (actualDecal.attachToCollisionObject)
						{
							//Debug.Log ("Parent name: " + hit.collider.name);
							actualDecal.transform.parent = hit.collider.transform;
						}
						else
						{
							GameObject decalsContainer = BasicFunctions.CreateContainerIfNotExists (BasicDefines.MESH_DECAL_CONTAINER_NAME);
							actualDecal.transform.parent = decalsContainer.transform;
						}								

						if (actualDecal.addCollider)
						{
							actualDecal.gameObject.AddComponent <MeshCollider> ();
							//actualDecal.gameObject.GetComponent<MeshCollider> ().convex = true;
						}

						actualObjectToForceSelect = actualDecal.gameObject;
					}
				}
			}
		}	
	}
}


#endif
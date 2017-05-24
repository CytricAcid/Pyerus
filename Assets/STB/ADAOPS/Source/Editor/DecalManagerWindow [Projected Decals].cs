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
		/// DrawProjectedDecalsMode
		/// # Draw al gui buttons, checboxes, ... to handle and insert projected decals in scene
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void DrawProjectedDecalsMode ()
		{		
			Material actualMaterial = EditorBasicFunctions.DrawProjectedDecalElements (projectedDecalPrefab, true, position, configSaver);

			if (actualMaterial)
			{
				EditorBasicFunctions.DrawEditorBox (actualMaterial.name, Color.yellow, position);	
			}
			else
			{
				EditorBasicFunctions.DrawEditorBox ("No projected decal selected to put!", Color.gray, position);	
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// HandleProjectedDecalsMode
		/// # To insert projected decals in scene using mouse
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void HandleProjectedDecalsMode ()
		{
			if ((GetEditorTimeDiff () > 0.1f) && EditorBasicFunctions.GetMouseButtonDown (0) && EditorBasicFunctions.GetInsertModeKeyPressed () && !GetDoingSomethingSpecial ())
			{			
				previousEditorTime = EditorApplication.timeSinceStartup;

				//Debug.Log ("Event.current.mousePosition: " + Event.current.mousePosition);
				
				Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
				RaycastHit hit;
				
				if (Physics.Raycast (ray, out hit))
				{		
					if (lastProjectedDecalsHitPoint.ToString () == hit.point.ToString ())
					{
						//Debug.Log ("NOTE: same point duplicate -> lastProjectedDecalsHitPoint: " + lastProjectedDecalsHitPoint);
					}
					else
					{
						//Debug.Log ("lastProjectedDecalsHitPoint A: " + lastProjectedDecalsHitPoint);
						
						lastProjectedDecalsHitPoint = hit.point;

						//Debug.Log ("New Decal");
						//Debug.Log ("Hit position: " + hit.point);
						//Debug.Log ("Collider Name: " + hit.collider.name);
					
						GenericProjectorDecal actualProjectedDecal = Instantiate (projectedDecalPrefab.gameObject).GetComponent<GenericProjectorDecal> () as GenericProjectorDecal;
						actualProjectedDecal.SetOldParameters (projectedDecalPrefab.transform.localScale, projectedDecalPrefab.GetComponent<Projector> ().orthographicSize, projectedDecalPrefab.GetComponent<Projector> ().aspectRatio);

						//Debug.Log (actualProjectedDecal.material.mainTexture.name);

						actualProjectedDecal.transform.position = hit.point + 0.3f * hit.normal;

						Vector3 finalScale = Random.Range (actualProjectedDecal.scaleRange.x, actualProjectedDecal.scaleRange.y) * actualProjectedDecal.transform.localScale;

						float textureAspectRatio = (float)actualProjectedDecal.material.mainTexture.width / (float)actualProjectedDecal.material.mainTexture.height;

						finalScale.x = textureAspectRatio * finalScale.x;

						actualProjectedDecal.transform.localScale = finalScale;

						actualProjectedDecal.transform.LookAt (hit.point);

						actualProjectedDecal.transform.Rotate (new Vector3 (0, 0, Random.Range (actualProjectedDecal.rotationRange.x, actualProjectedDecal.rotationRange.y)));	

						actualProjectedDecal.name = actualProjectedDecal.Generate (BasicDefines.PROJECTED_DECAL_BASE_NAME, GetSeedForInstancies (), true, actualProjectedDecal.material.name);														

						//Debug.Log ("actualProjectedDecal.attachToCollisionObject: " + actualProjectedDecal.attachToCollisionObject);

						if (actualProjectedDecal.attachToCollisionObject)
						{
							//Debug.Log ("Parent name: " + hit.collider.name);
							actualProjectedDecal.transform.parent = hit.collider.transform;
						}
						else
						{
							GameObject decalsContainer = BasicFunctions.CreateContainerIfNotExists (BasicDefines.PROJECTED_DECAL_CONTAINER_NAME);
							actualProjectedDecal.transform.parent = decalsContainer.transform;
						}			
					
						actualProjectedDecal.UpdateShape ();
						
						actualObjectToForceSelect = actualProjectedDecal.gameObject;
					}
				}
			}
		}		
	}
}

#endif
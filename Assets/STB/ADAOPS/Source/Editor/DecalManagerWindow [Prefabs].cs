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
		// private -- enum
		enum cPivotMode
		{
			useOriginalPivot,
			autoCalculate
		}
		;

		// private
		float extraNormalOffset = 0;
		cPivotMode pivotMode = cPivotMode.useOriginalPivot;
		bool attachObjectToCollisionObject = false;
		Vector2 objectScaleRange = Vector2.one;
		Vector2 objectRotationRangeX = Vector2.zero;
		Vector2 objectRotationRangeY = Vector2.zero;
		Vector2 objectRotationRangeZ = Vector2.zero;


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DrawObjectsMode
		/// # Draw al gui buttons, checboxes, ... to handle and insert objects in scene
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void DrawObjectsMode ()
		{					
			EditorBasicFunctions.DrawEditorBox ("Insert objects: Options", Color.white, position);	
			
			if (!configSaver.parameters.hideBasicHelp)
			{
				EditorBasicFunctions.DrawEditorBox (EditorBasicFunctions.GetInsertModeHelpString () + " objects", Color.yellow, position);
			}
			
			EditorGUILayout.Separator ();

			configSaver.parameters.showPrefabConfigOptions = EditorGUILayout.Foldout (configSaver.parameters.showPrefabConfigOptions, new GUIContent ("Show prefab configuration options", "Show prefab configuration options"));
			
			if (!configSaver.parameters.showPrefabConfigOptions)
			{
			}
			else
			{
				EditorBasicFunctions.DrawEditorBox ("Configuration", Color.yellow, position);

				EditorGUILayout.Separator ();
			
				attachObjectToCollisionObject = EditorGUILayout.Toggle (new GUIContent ("Attach to father", "Attach created object to hit object"), attachObjectToCollisionObject);
				EditorGUILayout.Separator ();
				
				extraNormalOffset = EditorGUILayout.Slider (new GUIContent ("Extra normal offset", "Set the extra offset using hit normal"), extraNormalOffset, -10, 10, new GUILayoutOption[] {GUILayout.Width (0.81f * position.width)});
				extraNormalOffset = Mathf.Clamp (extraNormalOffset, -10, 10);

				pivotMode = (cPivotMode)EditorGUILayout.EnumPopup (new GUIContent ("Pivot mode", "Select the pivot mode to positionate prefab using hit normal"), pivotMode, new GUILayoutOption[] {GUILayout.Width (0.81f * position.width)});
				EditorGUILayout.Separator ();
				EditorGUILayout.Separator ();

				objectScaleRange = EditorGUILayout.Vector2Field (new GUIContent ("Scale range", "Randomize object scale between 2 values"), objectScaleRange, new GUILayoutOption[]{GUILayout.Width (0.5f * position.width)});
				objectScaleRange.x = Mathf.Clamp (objectScaleRange.x, 0.01f, 10);
				objectScaleRange.y = Mathf.Clamp (objectScaleRange.y, 0.01f, 10);
			
				EditorGUILayout.Separator ();

				objectRotationRangeX = EditorGUILayout.Vector2Field (new GUIContent ("Rotation range X", "Randomize object rotation in X axis between 2 values"), objectRotationRangeX, new GUILayoutOption[]{GUILayout.Width (0.5f * position.width)});
				objectRotationRangeX.x = Mathf.Clamp (objectRotationRangeX.x, 0, 360);
				objectRotationRangeX.y = Mathf.Clamp (objectRotationRangeX.y, 0, 360);

				objectRotationRangeY = EditorGUILayout.Vector2Field (new GUIContent ("Rotation range Y", "Randomize object rotation in Y axis between 2 values"), objectRotationRangeY, new GUILayoutOption[]{GUILayout.Width (0.5f * position.width)});
				objectRotationRangeY.x = Mathf.Clamp (objectRotationRangeY.x, 0, 360);
				objectRotationRangeY.y = Mathf.Clamp (objectRotationRangeY.y, 0, 360);

				objectRotationRangeZ = EditorGUILayout.Vector2Field (new GUIContent ("Rotation range Z", "Randomize object rotation in Z axis between 2 values"), objectRotationRangeZ, new GUILayoutOption[]{GUILayout.Width (0.5f * position.width)});
				objectRotationRangeZ.x = Mathf.Clamp (objectRotationRangeZ.x, 0, 360);
				objectRotationRangeZ.y = Mathf.Clamp (objectRotationRangeZ.y, 0, 360);
						
				EditorGUILayout.Separator ();
				EditorGUILayout.Separator ();
			}
			
			EditorBasicFunctions.DrawEditorBox ("Choose prefab!", Color.white, position);	

			genericObject = EditorBasicFunctions.DrawPrefabList (genericObject, position);
									
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

			if (genericObject)
			{
				EditorBasicFunctions.DrawEditorBox (genericObject.name, Color.yellow, position);	
			}
			else
			{
				EditorBasicFunctions.DrawEditorBox ("No object selected to put!", Color.gray, position);	
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// HandleObjectsMode
		/// # To insert objects in scene using mouse
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void HandleObjectsMode ()
		{
			if ((GetEditorTimeDiff () > 0.1f) && EditorBasicFunctions.GetMouseButtonDown (0) && EditorBasicFunctions.GetInsertModeKeyPressed () && !GetDoingSomethingSpecial ())
			{
				previousEditorTime = EditorApplication.timeSinceStartup;

				Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
				RaycastHit hit;
				
				if (Physics.Raycast (ray, out hit))
				{				
					if (lastPrefabHitPoint.ToString () == hit.point.ToString ())
					{
						//Debug.Log ("NOTE: same point duplicate -> lastPrefabHitPoint: " + lastPrefabHitPoint);
					}
					else
					{
						//Debug.Log ("lastPrefabHitPoint A: " + lastPrefabHitPoint);

						lastPrefabHitPoint = hit.point;	

						//Debug.Log ("New Object");
						//Debug.Log ("Hit position: " + hit.point);
						//Debug.Log ("Collider Name: " + hit.collider.name);
					
						GameObject actualObject = Instantiate (genericObject);

						switch (pivotMode)
						{
						case cPivotMode.useOriginalPivot:
							{
								actualObject.transform.position = hit.point + extraNormalOffset * hit.normal;
							}
							break;

						case cPivotMode.autoCalculate:
							{
								MeshFilter actualMeshFilter = BasicFunctions.GetMeshFilterInChilds (actualObject.transform);
							
								if (actualMeshFilter == null)
								{
									actualObject.transform.position = hit.point + extraNormalOffset * hit.normal;
								}
								else
								{
									float pivotOffset = extraNormalOffset + 0.5f * (Mathf.Abs (actualMeshFilter.sharedMesh.bounds.max.y) + Mathf.Abs (actualMeshFilter.sharedMesh.bounds.min.y));
								
									//Debug.Log ("pivotOffset: " + pivotOffset);
									//Debug.Log ("max: " + actualMeshFilter.sharedMesh.bounds.max);
									//Debug.Log ("min: " + actualMeshFilter.sharedMesh.bounds.min);
									//Debug.Log ("Center: " + actualMeshFilter.sharedMesh.bounds.center);
								
									actualObject.transform.position = hit.point + pivotOffset * hit.normal;
								}
							}
							break;
						}

						actualObject.transform.localScale = Random.Range (objectScaleRange.x, objectScaleRange.y) * actualObject.transform.localScale;
						actualObject.transform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);
						actualObject.transform.Rotate (genericObject.transform.rotation.eulerAngles);
						actualObject.transform.Rotate (Random.Range (objectRotationRangeX.x, objectRotationRangeX.y), Random.Range (objectRotationRangeY.x, objectRotationRangeY.y), Random.Range (objectRotationRangeZ.x, objectRotationRangeZ.y));
					
						if (attachObjectToCollisionObject)
						{
							actualObject.transform.parent = hit.collider.transform;
						}
						else
						{
							GameObject objectsContainer = BasicFunctions.CreateContainerIfNotExists (BasicDefines.OBJECT_CONTAINER_NAME);
							actualObject.transform.parent = objectsContainer.transform;
						}

						actualObject.AddComponent<GenericObject> ();
										
						actualObject.name = actualObject.GetComponent<GenericObject> ().Generate (BasicDefines.OBJECT_BASE_NAME, GetSeedForInstancies (), true, genericObject.name);

						actualObjectToForceSelect = actualObject.gameObject;
					}
				}
			}
		}
	}
}

#endif
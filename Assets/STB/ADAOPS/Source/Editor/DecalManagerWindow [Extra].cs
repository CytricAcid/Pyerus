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
		// private
		bool splineDecalModeEnabled = false;
		bool closedWaypointSpline = true;
		bool loopWaypointSpline = true;


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DrawEditionMode
		/// # Draw al gui buttons, checboxes, ... to handle edition mode
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void DrawExtraOptions ()
		{		
			bool drawThis = true;
			
			if (systemMode != cSystemMode.edition)
			{
				drawThis = false;
			}
			
			if (drawThis)
			{
				configSaver.parameters.showExtraOptions = EditorGUILayout.Foldout (configSaver.parameters.showExtraOptions, new GUIContent ("Extra options", "Show extra options"));

				if (configSaver.parameters.showExtraOptions)
				{
					EditorBasicFunctions.DrawEditorBox ("Extra options", Color.yellow, position);

					if (splineDecalModeEnabled)
					{
						if (!configSaver.parameters.hideBasicHelp)
						{
							EditorGUILayout.HelpBox ("You can insert new waypoints using actual selected insert mode. Once you create waypoints just select one decal to make it to follow a path", MessageType.Info);
						}

						EditorGUILayout.Separator ();

						closedWaypointSpline = EditorGUILayout.Toggle (new GUIContent ("Closed spline", "Close the spline to finish in the initial point"), closedWaypointSpline);
						loopWaypointSpline = EditorGUILayout.Toggle (new GUIContent ("Loop spline", "Update position in a cotinuous loop"), loopWaypointSpline);

						EditorGUILayout.Separator ();

						// build spline decal
						GUILayout.BeginHorizontal ();	
						GUILayout.FlexibleSpace ();

						if (EditorBasicFunctions.GetEditorTextButton ("BUILD SPLINE DECAL", "Create a spline decal using actual waypoints", position))
						{
							List<WayPoint> wayPointList = new List<WayPoint> ();
							List<Object> objectList = GameObject.FindObjectsOfType (typeof(WayPoint)).ToList ();
							
							for (int i = 0; i < objectList.Count; i++)
							{
								wayPointList.Add (objectList [i] as WayPoint);
							}

							wayPointList = wayPointList.OrderBy (x => x.index).ToList ();

							if (wayPointList.Count >= 2)
							{
								if (Selection.gameObjects.Count () == 1)
								{
									GenericPathFollower actualPathFollower = Selection.gameObjects [0].GetComponent<GenericPathFollower> ();

									if (!actualPathFollower)
									{
										actualPathFollower = Selection.gameObjects [0].gameObject.AddComponent<GenericPathFollower> ();
									}

									actualPathFollower.Create (wayPointList, closedWaypointSpline, loopWaypointSpline);								
								}
								else
								{
									EditorUtility.DisplayDialog ("WARNING", "Please, select one object only", "OK");
								}
							}
							else
							{
								EditorUtility.DisplayDialog ("WARNING", "Please, insert at least two waypoints", "OK");
							}
						}			
												
						// remove waypoints
						GUILayout.BeginHorizontal ();	
						GUILayout.FlexibleSpace ();
						
						if (EditorBasicFunctions.GetEditorTextButton ("REMOVE ALL WAYPOINTS", "Remove all waypoins in scene", position))
						{
							GameObject wpContainer = GameObject.Find ("_DECAL_WAYPOINTS");
							DestroyImmediate (wpContainer);

							List<WayPoint> wayPointList = new List<WayPoint> ();
							List<Object> objectList = GameObject.FindObjectsOfType (typeof(WayPoint)).ToList ();
							
							for (int i = 0; i < objectList.Count; i++)
							{
								wayPointList.Add (objectList [i] as WayPoint);
							}
							
							foreach (WayPoint wp in wayPointList)
							{
								DestroyImmediate (wp.gameObject);
							}
						}					
						
						GUILayout.FlexibleSpace ();
						GUILayout.EndHorizontal ();	

						GUILayout.FlexibleSpace ();
						GUILayout.EndHorizontal ();	
						
						EditorGUILayout.Separator ();


						// leave spline decal mode
						GUILayout.BeginHorizontal ();	
						GUILayout.FlexibleSpace ();
						
						if (EditorBasicFunctions.GetEditorTextButton ("LEAVE", "Leave spline decal mode", position))
						{
							splineDecalModeEnabled = false;
						}					
						
						GUILayout.FlexibleSpace ();
						GUILayout.EndHorizontal ();	
					}
					else
					{
						EditorGUILayout.Separator ();

						GUILayout.BeginHorizontal ();	
						GUILayout.FlexibleSpace ();
					
						if (EditorBasicFunctions.GetEditorTextButton ("SPLINE DECAL MODE", "Go to spline decal mode", position))
						{
							splineDecalModeEnabled = true;
						}					
					
						GUILayout.FlexibleSpace ();
						GUILayout.EndHorizontal ();	
					}
					
					
					EditorGUILayout.Separator ();

					EditorBasicFunctions.DrawLineSeparator ();
				}
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// HandleAllElementsEdition
		/// # To edit objects in scene using Unity controls
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void HandleExtraOptions ()
		{
			if (splineDecalModeEnabled && (systemMode == cSystemMode.edition) && (GetEditorTimeDiff () > 0.1f) && EditorBasicFunctions.GetMouseButtonDown (0) && EditorBasicFunctions.GetInsertModeKeyPressed () && !GetDoingSomethingSpecial ())
			{			
				previousEditorTime = EditorApplication.timeSinceStartup;

				AddAWayPointWithMouse ();
			}
		}		
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// AddAWayPointWithMouse
		/// # Add a wayPoint using mouse
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void AddAWayPointWithMouse ()
		{								
			Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
			RaycastHit hit;
					
			if (Physics.Raycast (ray, out hit))
			{	
				AddNewWayPoint (true, hit);
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// AddNewWayPoint
		/// # Add a new part to car
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void AddNewWayPoint (bool useHit, RaycastHit hit)
		{			
			//Debug.Log ("Add new wayPoint");		

			GameObject wayPointsContainer = BasicFunctions.CreateContainerIfNotExists ("_DECAL_WAYPOINTS");
			
			GameObject newWayPoint = GameObject.CreatePrimitive (PrimitiveType.Capsule);
			newWayPoint.transform.parent = wayPointsContainer.transform;
			newWayPoint.transform.localScale = 0.2f * Vector3.one;
			newWayPoint.GetComponent<Renderer> ().sharedMaterial = new Material (newWayPoint.GetComponent<Renderer> ().sharedMaterial);
			newWayPoint.GetComponent<Renderer> ().sharedMaterial.color = Color.yellow;

			newWayPoint.AddComponent<WayPoint> ();
			newWayPoint.GetComponent<WayPoint> ().index = GetWayPointLogicalIndex ();

			newWayPoint.name = "Waypoint_" + newWayPoint.GetComponent<WayPoint> ().index;
			
			if (useHit)
			{
				newWayPoint.transform.position = hit.point;
				newWayPoint.transform.rotation = Quaternion.FromToRotation (Vector3.up, hit.normal);
				newWayPoint.GetComponent<WayPoint> ().hitNormal = hit.normal;
			}
			
			Selection.activeObject = newWayPoint;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetWayPointLogicalIndex
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		int GetWayPointLogicalIndex ()
		{
			// find all waypoints in the scene
			List<WayPoint> wayPointList = new List<WayPoint> ();
			List<Object> objectList = GameObject.FindObjectsOfType (typeof(WayPoint)).ToList ();
			
			for (int i = 0; i < objectList.Count; i++)
			{
				wayPointList.Add (objectList [i] as WayPoint);
			}
			
			wayPointList = wayPointList.OrderBy (x => x.index).ToList ();
			
			//print ("wayPointList count: " + wayPointList.Count);
			
			
			return wayPointList [wayPointList.Count - 1].index + 1;
		}
	}
}

#endif
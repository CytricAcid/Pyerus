﻿#if UNITY_EDITOR
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
		/// DrawEditionMode
		/// # Draw al gui buttons, checboxes, ... to handle edition mode
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void DrawEditionMode ()
		{				
			if (!configSaver.parameters.hideBasicHelp)
			{
				EditorBasicFunctions.DrawEditorBox ("Use 'Edit Mode' to edit decals or objects, to clean scripts in the scene or to create new decals", Color.yellow, position);
			}

			EditorGUILayout.Separator ();
						
					
			EditorBasicFunctions.DrawEditorBox ("Edit the scene!", Color.white, position);	
			
			EditorGUILayout.Separator ();

			GameObject actualSelectedObject = Selection.activeGameObject;
			
			
			if (actualSelectedObject)
			{														
				GenericMeshDecal actualDecal = actualSelectedObject.GetComponent ("GenericMeshDecal") as GenericMeshDecal;
				GenericObject actualObject = actualSelectedObject.GetComponent ("GenericObject") as GenericObject;
				
				if (actualDecal)
				{
					EditorBasicFunctions.DrawEditorBox ("Selected decal: " + actualSelectedObject.name, Color.yellow, position);
										
					var editor = Editor.CreateEditor (actualDecal);
					editor.OnInspectorGUI ();            
				} 
				else if (actualObject)
				{
					EditorBasicFunctions.DrawEditorBox ("Selected object: " + actualSelectedObject.name, Color.yellow, position);	
				}
				else
				{
					EditorBasicFunctions.DrawEditorBox ("Not editable object: " + actualSelectedObject.name, Color.gray, position);	
				}
			}
			else
			{
				EditorBasicFunctions.DrawEditorBox ("Nothing selected!", Color.black, position);	
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// HandleAllElementsEdition
		/// # To edit objects in scene using Unity controls
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void HandleAllElementsEdition ()
		{
			if (Event.current.type == EventType.MouseDrag)
			{
				//Debug.Log ("HandleAllElementsEdition -> MouseDrag");

				if (Selection.activeGameObject)
				{
					GenericMeshDecal actualDecal = Selection.activeGameObject.GetComponent ("GenericMeshDecal") as GenericMeshDecal;
					
					if (actualDecal)
					{
						actualDecal.UpdateDecallShape (false, false);
					}		
				}					
			}
		}
	}
}

#endif
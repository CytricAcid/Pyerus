#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace STB.ADAOPS
{
	[CustomEditor(typeof(GenericMeshDecal))]
	///////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// Class: GenericMeshDecalEditor
/// # Create an editor for generic decal
/// </summary>
///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class GenericMeshDecalEditor : Editor
	{	
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// OnInspectorGUI
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		public override void OnInspectorGUI ()
		{
			GenericMeshDecal decal = (GenericMeshDecal)target;
			
			bool updateShape = false;

			if (decal.material)
			{
				string actualMaterialName = decal.material.name;

				decal.material = EditorBasicFunctions.DrawMeshDecalElements (decal, false, new Rect (0, 0, EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight), null);

				if (decal.material.name != actualMaterialName)
				{
					updateShape = true;

					//Debug.Log ("Material Change: "+decal.material);
					decal.GetComponent<Renderer>().sharedMaterial = decal.material;
				}
			}
				
			if (GUI.changed || updateShape)
			{
				decal.UpdateDecallShape (false, false);
			}
		}
	}
}

#endif
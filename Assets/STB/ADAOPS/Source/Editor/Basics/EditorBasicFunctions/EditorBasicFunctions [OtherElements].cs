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
		/// DrawLineSeparator
		/// # Draw a line as separator
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void DrawLineSeparator ()
		{
			GUILayout.Box ("", new GUILayoutOption[] {
				GUILayout.ExpandWidth (true),
				GUILayout.Height (1)
			});
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetInsertModeKeyPressed
		/// # Return true if selected insert mode key is pressed
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static bool GetInsertModeKeyPressed ()
		{
			switch (DecalManagerWindow.insertMode)
			{
			case DecalManagerWindow.cInsertMode.controlAndMouse:
				{
					//Debug.Log("controlAndMouse");
					return Event.current.control;
				}

			case DecalManagerWindow.cInsertMode.shiftAndMouse:
				{
					//Debug.Log("shiftAndMouse");
					return Event.current.shift;
				}
				
			case DecalManagerWindow.cInsertMode.justMouse:
				{
					//Debug.Log("justMouse");
					return true;
				}
				
			case DecalManagerWindow.cInsertMode.disabled:
				{
					//Debug.Log("disabled");
					return false;
				}
			}
			
			//Debug.Log("false");
			return false;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DrawEditorBox
		/// # Draw an editor box given text, color and position
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void DrawEditorBox (string text, Color color, Rect position)
		{
			GUIStyle boxStyle = new GUIStyle (GUI.skin.box);

			if (!UnityEditorInternal.InternalEditorUtility.HasPro ())
			{
				Color actualGuiColor = GUI.color;

				GUI.color = Color.white;

				boxStyle.normal.textColor = Color.black;			
				GUILayout.Box (text, boxStyle, new GUILayoutOption[]{GUILayout.Width (0.92f * position.width)});

				GUI.color = actualGuiColor;
			}
			else
			{
				boxStyle.normal.textColor = color;			
				GUILayout.Box (text, boxStyle, new GUILayoutOption[]{GUILayout.Width (0.92f * position.width)});
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DrawActualVersion
		/// # Draw actual ADAOPS version
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void DrawActualVersion (Rect position)
		{
			EditorBasicFunctions.DrawEditorBox ("ADAOPS [v" + BasicDefines.VERSION + "]", Color.white, position);	
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetEditorTextButton
		/// # Return true if created text button is pressed
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static bool GetEditorTextButton (string name, string helpText, Rect position)
		{
			return GUILayout.Button (new GUIContent (name, helpText), new GUILayoutOption[] {GUILayout.Height (32)});
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetEditorButton
		/// # Create an editor button and return true if it is pressed
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static bool GetEditorButton (string name, string helpTex, Vector2 size, bool enabled, bool applyColorChange, bool justSeparator, bool fixedSize)
		{	
			Texture texture = AssetDatabase.LoadAssetAtPath (BasicDefines.MAIN_PATH + "Basics/Textures/Editor/" + name + ".png", typeof(Texture)) as Texture;
	
			Color guiColor = GUI.color;
			Color bgColor = GUI.backgroundColor;

			if (justSeparator)
			{
				GUI.color = new Color (1, 1, 1, 0.0f);
				GUI.backgroundColor = new Color (1, 1, 1, 0.0f);
			}
			else if (applyColorChange)
			{
				if (enabled)
				{
					GUI.color = new Color (1, 1, 1, 1);
					GUI.backgroundColor = new Color (0.6f, 0.0f, 0.6f, 1f);
				}
				else
				{
					GUI.color = new Color (1, 1, 1, 0.3f);
					GUI.backgroundColor = new Color (1, 1, 1, 0.3f);
				}
			}

			bool r = false;

			if (!fixedSize)
			{
				GUIStyle newStyle = new GUIStyle ();

				newStyle.fixedHeight = 0;
				newStyle.fixedWidth = 0;
			
				r = GUILayout.Button (new GUIContent (texture, helpTex), newStyle, new GUILayoutOption[] {
				GUILayout.Width (size.x),
				GUILayout.Height (size.y),
			});
			}
			else
			{
				r = GUILayout.Button (new GUIContent (texture, helpTex), new GUILayoutOption[] {
					GUILayout.Width (size.x),
					GUILayout.Height (size.y),
				});
			}

			
			GUI.color = guiColor;
			GUI.backgroundColor = bgColor;
			
			return r;
		}	
	}
}

#endif
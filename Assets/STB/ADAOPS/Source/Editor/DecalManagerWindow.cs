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
        // Add shortcut in Window menu
        [MenuItem("Tools/STB/ADAOPS/Main Manager")]

        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Init
        /// # Initialise the window and show it
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            DecalManagerWindow window = (DecalManagerWindow)EditorWindow.GetWindow(typeof(DecalManagerWindow));

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0
            window.title = "ADAOPS";
#else
            window.titleContent = new GUIContent("ADAOPS");
#endif

            window.Show();
        }
    }
}
#endif
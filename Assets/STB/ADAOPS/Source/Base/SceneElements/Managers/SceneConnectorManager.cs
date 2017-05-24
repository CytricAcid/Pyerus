using UnityEngine;
#if !UNITY_5_0
using UnityEngine.SceneManagement;
#endif
using System.Collections;
using System.Collections.Generic;

namespace STB.ADAOPS
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: SceneConnectorManager
    /// # Connect all scenes
    /// </summary>
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    public class SceneConnectorManager : MonoBehaviour
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Start
        /// # Start the class
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void Start()
        {
            DontDestroyOnLoad(transform.gameObject);

#if UNITY_5_0
            Application.LoadLevel(1);
#else
            SceneManager.LoadScene(1);
#endif
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// # Update the class
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            DontDestroyOnLoad(transform.gameObject);

            for (int i = 0; i < 2; i++)
            {
                if (Input.GetKey(KeyCode.Alpha1 + i))
                {
#if UNITY_5_0
                    Application.LoadLevel(i + 1);
#else
                    SceneManager.LoadScene(i + 1);
#endif
                }
            }
        }
    }
}

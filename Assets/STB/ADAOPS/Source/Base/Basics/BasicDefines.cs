using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace STB.ADAOPS
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// BasicDefines
    /// # Recopilation of some needed defines
    /// </summary>
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    public class BasicDefines
    {
        // public -- version
        public static string VERSION = "1.9.2.6";

        // public -- paths
        public static string MAIN_PATH = "Assets/STB/ADAOPS/";
        public static string GENERIC_DECAL_PATH = MAIN_PATH + "Objects/Prefabs/System/GenericMeshDecal.prefab";
        public static string MESH_DECAL_PREFAB_PATH = MAIN_PATH + "Paintable/Objects/Prefabs/System/MeshDecalPrefab.prefab";
        public static string PROJECTED_DECAL_PREFAB_PATH = MAIN_PATH + "Paintable/Objects/Prefabs/System/ProjectedDecalPrefab.prefab";

        // public -- names
        public static string MESH_DECAL_CONTAINER_NAME = "_DECAL_CONTAINER";
        public static string PROJECTED_DECAL_CONTAINER_NAME = "_PROJECTED_DECAL_CONTAINER";
        public static string OBJECT_CONTAINER_NAME = "_OBJECT_CONTAINER";
        public static string BASENAME_NOT_DEFINED = "BASENAME_NOT_DEFINED";
        public static string SEED_NOT_DEFINED = "SEED_NOT_DEFINED";
        public static string MESH_DECAL_BASE_NAME = "MeshDecal";
        public static string PROJECTED_DECAL_BASE_NAME = "ProjectedDecal";
        public static string OBJECT_BASE_NAME = "Object";
        public static string NOT_DEFINED = "NOT_DEFINED";

        // others
        public static Vector3 TOO_FAR_POSITION = 999999 * Vector3.one;
    }
}
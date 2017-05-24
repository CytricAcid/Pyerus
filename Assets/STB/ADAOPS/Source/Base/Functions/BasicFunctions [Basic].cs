using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace STB.ADAOPS
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// BasicFunctions
    /// # Compilation of some needed fuctions
    /// </summary>
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class BasicFunctions
    {
        // private static
        static List<Texture> thumbTextureList = new List<Texture>();


        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// RTImage
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////		
        public static Texture2D RTImage(Camera cam)
        {
            Texture2D image = null;

            if (cam.targetTexture == null)
            {
                cam.targetTexture = new RenderTexture(64, 64, 0);
            }

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = cam.targetTexture;

            cam.Render();

            image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
            image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
            image.Apply();
            RenderTexture.active = currentRT;

            return image;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnDestroy
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////	
        void OnDestroy()
        {
            thumbTextureList.Clear();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetThumbnail
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////			
        public static Texture GetThumbnail(GameObject obj)
        {
            string actualTextureName = obj.name + "_" + "thumbnailforeditorwindow";

            // fist, look in the list
            for (int i = 0; i < thumbTextureList.Count; i++)
            {
                if (thumbTextureList[i] && (thumbTextureList[i].name == actualTextureName))
                {
                    //Debug.Log ("Is in the list, actualTextureName: " + actualTextureName);
                    return thumbTextureList[i];
                }
            }

            // if it is not in the list
            GameObject container = new GameObject();

            GameObject clone = GameObject.Instantiate(obj);
            clone.transform.position = 99999 * Vector3.one;

            Vector3 lookAtTarget = clone.transform.position;

            float offset = 0.3f * obj.transform.localScale.magnitude;

            MeshFilter actualMeshFilter = BasicFunctions.GetMeshFilterInChilds(clone.transform);

            if ((actualMeshFilter != null) && (actualMeshFilter.sharedMesh != null))
            {
                lookAtTarget = clone.transform.position + actualMeshFilter.sharedMesh.bounds.center;
                offset = 0.75f * Vector3.Distance(actualMeshFilter.sharedMesh.bounds.max, actualMeshFilter.sharedMesh.bounds.min);
            }

            Camera actualCamera = container.gameObject.AddComponent<Camera>();
            actualCamera.farClipPlane = offset + 10;
            actualCamera.nearClipPlane = 0.1f;
            actualCamera.transform.position = 99999 * Vector3.one + (new Vector3(0, 0, offset));
            actualCamera.transform.LookAt(lookAtTarget);
            actualCamera.clearFlags = CameraClearFlags.SolidColor;
            actualCamera.backgroundColor = Color.clear;

            Light actualLight = container.gameObject.AddComponent<Light>();
            actualLight.type = LightType.Directional;
            actualLight.transform.position = 99999 * Vector3.one + (new Vector3(0, 0, offset));
            actualLight.transform.LookAt(lookAtTarget);

            clone.transform.LookAt(actualCamera.transform);

            Texture actualThumb = RTImage(actualCamera);

            GameObject.DestroyImmediate(container, true);
            GameObject.DestroyImmediate(clone, true);


            if (actualThumb != null)
            {
                // put in the list
                actualThumb.name = actualTextureName;
                //Debug.Log ("Add new thumb to the list: " + actualThumb.name);
                thumbTextureList.Add(actualThumb);
            }


            return actualThumb;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CreateContainerIfNotExists
        /// # Creates a new empty gameobject to contain others
        /// # Return created or founded container
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public static GameObject CreateContainerIfNotExists(string name)
        {
            GameObject container = GameObject.Find(name);

            if (!container)
            {
                container = new GameObject(name);
            }

            return container;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetTransformBounds
        /// # Return the bounds of a transform
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Bounds GetTransformBounds(Transform transform)
        {
            Vector3 size = transform.lossyScale;
            Vector3 min = -size / 2f;
            Vector3 max = size / 2f;

            Vector3[] vts = new Vector3[]
            {
                new Vector3 (min.x, min.y, min.z),
                new Vector3 (max.x, min.y, min.z),
                new Vector3 (min.x, max.y, min.z),
                new Vector3 (max.x, max.y, min.z),

                new Vector3 (min.x, min.y, max.z),
                new Vector3 (max.x, min.y, max.z),
                new Vector3 (min.x, max.y, max.z),
                new Vector3 (max.x, max.y, max.z),
            };

            for (int i = 0; i < 8; i++)
            {
                vts[i] = transform.TransformDirection(vts[i]);
            }

            min = max = vts[0];

            foreach (Vector3 v in vts)
            {
                min = Vector3.Min(min, v);
                max = Vector3.Max(max, v);
            }

            return new Bounds(transform.position, max - min);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// IsLayerContains
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        private static bool IsLayerContains(LayerMask mask, int layer)
        {
            return (mask.value & 1 << layer) != 0;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetMainMaterial
        /// # Return first material found for this gameobject
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Material GetMainMaterial(Transform mainTransform)
        {
            MeshRenderer actualMeshRenderer = mainTransform.GetComponent<MeshRenderer>();

            if (actualMeshRenderer != null)
            {
                return actualMeshRenderer.sharedMaterial;
            }

            foreach (Transform child in mainTransform)
            {
                Material t = GetMainMaterial(child);

                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetMeshFilterInChilds
        /// # Return a mesh filter inside a transform using a name
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public static MeshFilter GetMeshFilterInChilds(Transform mainTransform)
        {
            MeshFilter actualMeshFilter = mainTransform.GetComponent<MeshFilter>();

            if (actualMeshFilter != null)
            {
                return actualMeshFilter;
            }

            foreach (Transform child in mainTransform)
            {
                MeshFilter t = GetMeshFilterInChilds(child);

                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetTransformInChildsByName
        /// # Return a child transform inside another transform using a name
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Transform GetTransformInChildsByName(Transform mainTransform, string name)
        {
            if (mainTransform.name == name)
            {
                return mainTransform;
            }

            foreach (Transform child in mainTransform)
            {
                Transform t = GetTransformInChildsByName(child, name);

                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetAllAffectedObjects
        /// # Return all objects affected inside some bounds
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public static GameObject[] GetAllAffectedObjects(Bounds bounds, LayerMask affectedLayers)
        {
            MeshRenderer[] renderers = (MeshRenderer[])GameObject.FindObjectsOfType<MeshRenderer>();
            List<GameObject> objects = new List<GameObject>();

            foreach (Renderer r in renderers)
            {
                bool validObject = true;

                if (r.gameObject.GetComponent<NotStainableObject>())
                {
                    validObject = false;
                }

                if (validObject)
                {
                    if (!r.enabled)
                    {
                        continue;
                    }

                    if (!IsLayerContains(affectedLayers, r.gameObject.layer))
                    {
                        continue;
                    }

                    if (r.GetComponent<GenericMeshDecal>() != null)
                    {
                        continue;
                    }

                    if (bounds.Intersects(r.bounds))
                    {
                        objects.Add(r.gameObject);
                    }
                }
            }

            return objects.ToArray();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DestroyGameObjectByName
        /// # Destroy all gameobjects with "name" as name
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DestroyGameObjectByName(string name)
        {
            GameObject actualGameObject = GameObject.Find(name);

            Object.DestroyImmediate(actualGameObject);
        }
    }
}
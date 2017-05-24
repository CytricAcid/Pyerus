using UnityEngine;
using System.Collections.Generic;

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: BasicDecalFunctions
	/// # Compilation of some needed fuctions
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public partial class BasicFunctions
	{
		// private static
		private static List<Vector3> vertexBufferList = new List<Vector3> ();
		private static List<Vector3> normalsBufferList = new List<Vector3> ();
		private static List<Vector2> textCoordsBufferList = new List<Vector2> ();
		private static List<int> indexBufferList = new List<int> ();


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// CreateMesh
		/// # To create a new mesh using given vertex and index
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Mesh CreateMesh ()
		{
			//Debug.Log ("CreateMesh");
			
			if (indexBufferList.Count == 0)
			{
				return null;
			}
			
			// create a new mesh
			Mesh mesh = new Mesh ();
			
			mesh.vertices = vertexBufferList.ToArray ();
			mesh.normals = normalsBufferList.ToArray ();
			mesh.uv = textCoordsBufferList.ToArray ();
			mesh.uv2 = textCoordsBufferList.ToArray ();
			mesh.triangles = indexBufferList.ToArray ();
			
			vertexBufferList.Clear ();
			normalsBufferList.Clear ();
			textCoordsBufferList.Clear ();
			indexBufferList.Clear ();
			
			
			return mesh;
		}		
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// AddVertex
		/// # Add a new vertex
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		private static int AddVertex (Vector3 vertex, Vector3 normal)
		{
			int index = FindVertex (vertex);
			
			if (index == -1)
			{
				vertexBufferList.Add (vertex);
				normalsBufferList.Add (normal);
				index = vertexBufferList.Count - 1;
			}
			else
			{
				Vector3 t = normalsBufferList [index] + normal;
				normalsBufferList [index] = t.normalized;
			}
			
			return (int)index;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Push
		/// # Push object into a distance
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void Push (float distance)
		{
			for (int i = 0; i < vertexBufferList.Count; i++)
			{
				Vector3 normal = normalsBufferList [i];
				vertexBufferList [i] += normal * distance;
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// FindVertex
		/// # Find some vertex
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		private static int FindVertex (Vector3 vertex)
		{
			for (int i = 0; i < vertexBufferList.Count; i++)
			{
				if (Vector3.Distance (vertexBufferList [i], vertex) < 0.0001f)
				{
					return i;
				}
			}
			return -1;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// BuildDecalForObject
		/// # Build a decal for one of the affected objects
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void BuildDecalForObject (GenericMeshDecal decal, GameObject affectedObject)
		{
			if (affectedObject.isStatic)
			{
				Debug.Log ("NOTE: static objects are not supported by realtime mesh decals, we are trying to make it possible soon");
			}
			else
			{
				Mesh affectedMesh = affectedObject.GetComponent<MeshFilter> ().sharedMesh;

				if (affectedMesh == null)
				{
					return;
				}

				float angleLimit = decal.angleLimit;

				Plane right = new Plane (Vector3.right, Vector3.right / 2f);
				Plane left = new Plane (-Vector3.right, -Vector3.right / 2f);

				Plane top = new Plane (Vector3.up, Vector3.up / 2f);
				Plane bottom = new Plane (-Vector3.up, -Vector3.up / 2f);

				Plane front = new Plane (Vector3.forward, Vector3.forward / 2f);
				Plane back = new Plane (-Vector3.forward, -Vector3.forward / 2f);

				Vector3[] vertices = affectedMesh.vertices;
				int[] triangles = affectedMesh.triangles;
				int startVertexCount = vertexBufferList.Count;

				Matrix4x4 matrix = decal.transform.worldToLocalMatrix * affectedObject.transform.localToWorldMatrix;

				for (int i = 0; i < triangles.Length; i+=3)
				{
					int i1 = triangles [i];
					int i2 = triangles [i + 1];
					int i3 = triangles [i + 2];
			
					Vector3 v1 = matrix.MultiplyPoint (vertices [i1]);
					Vector3 v2 = matrix.MultiplyPoint (vertices [i2]);
					Vector3 v3 = matrix.MultiplyPoint (vertices [i3]);

					Vector3 side1 = v2 - v1;
					Vector3 side2 = v3 - v1;
					Vector3 normal = Vector3.Cross (side1, side2).normalized;

					if (Vector3.Angle (-Vector3.forward, normal) >= angleLimit)
					{
						continue;
					}

					GenericPoly poly = new GenericPoly (v1, v2, v3);

					poly = GenericPoly.ClipPoly (poly, right);

					if (poly == null)
					{
						continue;
					}

					poly = GenericPoly.ClipPoly (poly, left);
				
					if (poly == null)
					{
						continue;
					}

					poly = GenericPoly.ClipPoly (poly, top);
				
					if (poly == null)
					{
						continue;
					}

					poly = GenericPoly.ClipPoly (poly, bottom);
				
					if (poly == null)
					{
						continue;
					}

					poly = GenericPoly.ClipPoly (poly, front);
				
					if (poly == null)
					{
						continue;
					}

					poly = GenericPoly.ClipPoly (poly, back);
				
					if (poly == null)
					{
						continue;
					}

					AddPoly (poly, normal);
				}

				GenerateTexCoords (startVertexCount, decal.sprite);
			}
		}	
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// LineCast
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Vector3 LineCast (Plane plane, Vector3 a, Vector3 b)
		{
			float dis;
			Ray ray = new Ray (a, b - a);
			plane.Raycast (ray, out dis);
			return ray.GetPoint (dis);
		}	
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// AddPoly
		/// # Add a poly
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		private static void AddPoly (GenericPoly poly, Vector3 normal)
		{
			int ind1 = AddVertex (poly.vertexList [0], normal);
			for (int i = 1; i < poly.vertexList.Count-1; i++)
			{
				int ind2 = AddVertex (poly.vertexList [i], normal);
				int ind3 = AddVertex (poly.vertexList [i + 1], normal);

				indexBufferList.Add (ind1);
				indexBufferList.Add (ind2);
				indexBufferList.Add (ind3);
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GenerateTexCoords
		/// # Generate texture coords given a sprite
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		private static void GenerateTexCoords (int start, Sprite sprite)
		{
			Rect rect = sprite.rect;

			rect.x /= sprite.texture.width;
			rect.y /= sprite.texture.height;
			rect.width /= sprite.texture.width;
			rect.height /= sprite.texture.height;
		
			//Debug.Log ("---------------------------------");

			for (int i=start; i<vertexBufferList.Count; i++)
			{
				Vector3 vertex = vertexBufferList [i];
			
				Vector2 uv = new Vector2 (vertex.x + 0.5f, vertex.y + 0.5f);
				uv.x = Mathf.Lerp (rect.xMin, rect.xMax, uv.x);
				uv.y = Mathf.Lerp (rect.yMin, rect.yMax, uv.y);

				//if (uv.x > 1)
				//uv.x = 1;
				//if (uv.y > 1)
				//uv.y = 1;

				//Debug.Log (i + " -> " + uv.x.ToString () + " " + uv.y.ToString ());
			
				textCoordsBufferList.Add (uv);
			}
		}
	}
}
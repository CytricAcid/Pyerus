using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: GenericMeshDecal
	/// # To create and handle a generic decal
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	[ExecuteInEditMode]
	public partial class GenericMeshDecal : GenericDestroyable
	{
		// public static
		public static int actualFolderIndex = 0;

		// public
		public Material material;
		public Sprite sprite;
		public bool attachToCollisionObject = false;
		public Vector2 scaleRange = Vector2.one;
		public Vector2 rotationRange = Vector2.zero;
		public float angleLimit = 90.0f;
		public float distanceFromHit = 0.001f;
		public LayerMask affectedLayers = -1;
		public bool showAffectedObject = false;
		public GameObject[] affectedObjects;
		public bool addCollider = false;
		public bool planarDecal = false;
		public bool lockedShapeAlways = false;
		public bool lockedShapeInRuntime = false;
		public bool futureTimeLockableShape = false;
		public bool comeFromEditor = false;
		 
		// private -- control
		bool autoDestroyable = false;
		float destroyTime = 0;
		float destroySpeed = 1;
		bool firstTime = true;

		// private -- transform
		Matrix4x4 oldMatrix;
		Vector3 oldScale;

		
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// SetPlanar
		/// # Set decal to be a simple plane
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Awake ()
		{
			bool hasToBuild = true;

			if (planarDecal && comeFromEditor)
			{
				comeFromEditor = false;
				hasToBuild = false;
			}

			if (hasToBuild)
			{
				MeshFilter meshFilter = GetComponent<MeshFilter> ();

				if (meshFilter && !meshFilter.sharedMesh)
				{
					BuildMesh (this);			
				}
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetPlanar
		/// # Return planarDecal
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool GetPlanar ()
		{
			return planarDecal;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// SetDestroyable
		/// # Set decal to be destroyable or not
		/// # destroyTime defines the time before it will be destroyed
		/// # destroySpeed sets the speed to smoothly dissapear once it have to be destroyed 
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public void SetDestroyable (bool autoDestroyable, float destroyTime, float destroySpeed)
		{
			this.autoDestroyable = autoDestroyable;
			this.destroyTime = destroyTime;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Update
		/// # Update the decal
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Update ()
		{
			if (autoDestroyable)
			{
				destroyTime -= destroySpeed * Time.deltaTime;

				if (destroyTime <= 0)
				{
					destroyTime = 0;

					Color actualColor = this.GetComponent<Renderer> ().material.color;

					actualColor.a -= 2 * Time.deltaTime;

					this.GetComponent<Renderer> ().material.color = actualColor;

					if (actualColor.a <= 0)
					{
						Destroy (this.gameObject);
					}
				}
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// CreatePlanarMesh
		/// # Creates a new planar mesh
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		Mesh CreatePlanarMesh (float width, float height)
		{
			float zFactor = 0.01f;

			Mesh m = new Mesh ();
			m.name = "ScriptedMesh";
			m.vertices = new Vector3[] {
				new Vector3 (-width, -height, zFactor),
				new Vector3 (width, -height, zFactor),
				new Vector3 (width, height, zFactor),
				new Vector3 (-width, height, zFactor)
			};
			m.uv = new Vector2[] {
				new Vector2 (0, 0),
				new Vector2 (0, 1),
				new Vector2 (1, 1),
				new Vector2 (1, 0)
			};
			m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
			m.RecalculateNormals ();
			 
			return m;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// BuildMesh
		/// # Creates decal's mesh and applys new texture coords
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////	
		public void BuildMesh (GenericMeshDecal decal)
		{
			MeshFilter filter = decal.GetComponent<MeshFilter> ();
			
			if (filter == null)
			{
				filter = decal.gameObject.AddComponent<MeshFilter> ();
			}
			
			if (decal.GetComponent<Renderer> () == null)
			{
				decal.gameObject.AddComponent<MeshRenderer> ();
			}
			
			decal.GetComponent<Renderer> ().material = decal.material;
			
			if (decal.material == null || decal.sprite == null)
			{
				filter.mesh = null;
				return;
			}
			
			affectedObjects = BasicFunctions.GetAllAffectedObjects (BasicFunctions.GetTransformBounds (decal.transform), decal.affectedLayers);

			foreach (GameObject go in affectedObjects)
			{
				if (!go.GetComponent<WayPoint> () && !go.GetComponent<GenericMeshDecal> ())
				{
					BasicFunctions.BuildDecalForObject (decal, go);
				}
			}
			
			BasicFunctions.Push (decal.distanceFromHit);
			
			Mesh mesh = null;

			if (planarDecal)
			{
				mesh = CreatePlanarMesh (1, 1);
			}
			else
			{
				mesh = BasicFunctions.CreateMesh ();
				//ModifyPlanarMesh (ref mesh, 1, 1);
			}
			
			if (mesh != null)
			{
				mesh.name = "GenericPoly";
				filter.mesh = mesh;
			}
		}		
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DoFinalTransform
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public void DoFinalTransform (bool randomizeNow)
		{
			Vector3 scale = this.transform.localScale;
			
			float ratio = 1;
			
			if (this.sprite)
			{
				ratio = (float)this.sprite.rect.width / this.sprite.rect.height;
			}
			
			//print ("ratio A: "+ratio);		
			if (oldScale.x != scale.x)
			{
				scale.y = scale.x / ratio;
			}
			else if (oldScale.y != scale.y)
			{
				scale.x = scale.y * ratio;
			}
			else if (scale.x != scale.y * ratio)
			{
				scale.x = scale.y * ratio;
			}
			
			if (firstTime)
			{
				firstTime = false;
				
				if (randomizeNow)
				{
					this.transform.Rotate (new Vector3 (0, 0, Random.Range (this.rotationRange.x, this.rotationRange.y)));
				}
				
				if (planarDecal)
				{
					this.transform.Rotate (new Vector3 (180, 0, -90));
				}
			}
			
			// verify if decal has changed to rebuild
			bool hasChanged = oldMatrix != this.transform.localToWorldMatrix;
			
			oldMatrix = this.transform.localToWorldMatrix;
			oldScale = this.transform.localScale;
			
			if (hasChanged)
			{
				BuildMesh (this);
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// UpdateDecallShape
		/// # Update decal's shape (for example when decals is scaled, rotated or moved)
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public void UpdateDecallShape (bool randomizeNow, bool forceShapeUpdate)
		{			
			//print ("UpdateDecallShape A: " + this.transform.position);

			bool lockedShape = lockedShapeAlways;

			if (lockedShapeInRuntime && Application.isPlaying)
			{
				lockedShape = true;
			}

			if (!lockedShape || forceShapeUpdate)
			{
				bool recalculateAllowed = true;

				if (planarDecal && (this.GetComponent<MeshFilter> ().sharedMesh != null))
				{
					recalculateAllowed = false;
				}

				if (recalculateAllowed)
				{
					//print ("UpdateDecallShape B: " + this.transform.position);

					if (Application.isPlaying)
					{
						//print ("UpdateDecallShape C: " + this.transform.position);

						//Ray ray = new Ray (this.transform.position, -this.transform.up);				
						Ray ray = new Ray (this.transform.position, -this.transform.up);
						RaycastHit hit = new RaycastHit ();
						if (Physics.Raycast (ray, out hit, 50))
						{
							this.transform.position = hit.point - 0.00f * hit.normal;
							this.transform.forward = -hit.normal;
						}
					}
					else
					{
#if UNITY_EDITOR
				HandleUtility.AddDefaultControl (GUIUtility.GetControlID (FocusType.Passive));
				
				if (Event.current.type == EventType.MouseDown)
				{
					Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
					RaycastHit hit = new RaycastHit ();
					if (Physics.Raycast (ray, out hit, 50))
					{
						this.transform.position = hit.point;
						this.transform.forward = -hit.normal;
					}
				}
#endif
					}
		
					DoFinalTransform (randomizeNow);
				}
			}
		}
	}
}
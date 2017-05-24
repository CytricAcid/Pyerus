using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: GenericProjectorDecal
	/// # To create and handle a generic projector decal
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	[ExecuteInEditMode]
	public partial class GenericProjectorDecal : GenericDestroyable
	{
		// public static
		public static int actualFolderIndex = 0;

		// public
		public Sprite sprite;
		public bool attachToCollisionObject = false;
		public Vector2 scaleRange = Vector2.one;
		public Vector2 rotationRange = Vector2.zero;
		public float angleLimit = 90.0f;
		public float distanceFromHit = 0.001f;

		// private -- control
		bool autoDestroyable = false;
		float destroyTime = 0;
		float destroySpeed = 1;

		// private -- base
		Vector3 oldScale = Vector3.zero;
		float oldOrthographicSize = -9999;
		float oldAspectRatio = -9999;

		// public functions		
		public Material material
		{
			set { this.GetComponent<Projector> ().material = value;}
			get { return this.GetComponent<Projector> ().material;}
		}

		
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Start
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Start ()
		{
			oldScale = this.transform.localScale;
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
		/// OnDrawGizmos
		/// # Draw custom gizmo
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void OnDrawGizmos ()
		{
			/*Texture gizmoTexture = AssetDatabase.LoadAssetAtPath (BasicDefines.MAIN_PATH + "Basics/Textures/Editor/" + "LogoDecalsProyector.png", typeof(Texture)) as Texture;

			Gizmos.DrawIcon (transform.position, gizmoTexture.name, true);*/
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// SetOldParameters
		/// # Set the old parameters for the decal
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public void SetOldParameters (Vector3 scale, float orthographicSize, float aspectRatio)
		{
			//print ("scale: " + scale);
			//print ("orthographicSize: " + orthographicSize);
			//print ("aspectRatio: " + aspectRatio);

			oldScale = scale;
			oldOrthographicSize = orthographicSize;
			oldAspectRatio = aspectRatio;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// UpdateShape
		/// # Update the decal's shape
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public void UpdateShape ()
		{	
			Projector actualProjector = this.GetComponent<Projector> ();
			
			if (oldScale == Vector3.zero)
			{
				oldScale = this.transform.localScale;
			}
			
			if (oldOrthographicSize == -9999)
			{
				oldOrthographicSize = actualProjector.orthographicSize;
			}
			
			if (oldAspectRatio == -9999)
			{
				oldAspectRatio = actualProjector.aspectRatio;
			}
			
			// modify scale
			if (this.transform.localScale != oldScale)
			{
				//print ("Scale has changed from oldScale: " + oldScale.ToString ());
				
				float aspectRatioX = this.transform.localScale.y / oldScale.y;
				float aspectRatioY = this.transform.localScale.x / oldScale.x;
				
				actualProjector.orthographicSize = oldOrthographicSize * aspectRatioX;
				actualProjector.aspectRatio = oldAspectRatio * aspectRatioY / aspectRatioX;
				
				//print ("aspectRatioX: " + aspectRatioX);
				//print ("actualProjector.orthographicSize: " + actualProjector.orthographicSize);
			}
			
			//print ("Pass ambient light color to shader: " + RenderSettings.ambientLight); 
			GetComponent<Projector> ().material.SetVector ("_AmbientColor", RenderSettings.ambientLight);
			
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
		/// Update
		/// # Update the decal
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Update ()
		{	
			UpdateShape ();
		}
	}
}
using UnityEngine;
using System.Collections;

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: DirtyObject
	/// # An object which can stain others
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class DirtyObject : NotStainableObject
	{			
		public Material decalMaterial = null;

		// protected
		protected bool continuousMode = false;
		protected float destroyTime = 9;
		protected float destroySpeed = 0.2f;
		protected Vector2 scaleMultiplierRange = new Vector2 (2, 4);
		protected Vector2 rotationRange = Vector2.zero;
		protected float timeBetweenDecalCreations = 0.25f;
		protected int maxDecalsCreatedPerCollision = 1;

		// private
		float counterBetweenDecalCreations = 0;


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// OnCollisionStay
		/// # Get collisions with other objects (everytime there is a one)
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void OnCollisionStay (Collision col)
		{
			if (continuousMode)
			{
				CreateDecal (col);

				OnCollisionStayExtended(col);
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// OnCollisionStayExtended -- VIRTUAL
		/// Extends OnCollisionStay
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		protected virtual void OnCollisionStayExtended (Collision col)
		{
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// OnCollisionEnter
		/// # Get collisions with other objects (in a new collision)
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void OnCollisionEnter (Collision col)
		{
			//print ("New collision: " + col.collider.name);
			CreateDecal (col);

			OnCollisionEnterExtended (col);
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// OnCollisionEnterExtended -- VIRTUAL
		/// Extends OnCollisionEnter
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		protected virtual void OnCollisionEnterExtended (Collision col)
		{
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// CreateDecal
		/// # Create a decal for a collision
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void CreateDecal (Collision col)
		{
			if (counterBetweenDecalCreations <= 0)
			{
				counterBetweenDecalCreations = timeBetweenDecalCreations;

				int numberOfCreatedDecals = 0;

				foreach (ContactPoint contact in col.contacts)
				{
					float actualScaleMultiplier = Random.Range (scaleMultiplierRange.x, scaleMultiplierRange.y);

					if (!contact.otherCollider.gameObject.GetComponent<NotStainableObject> ())
					{
						if (DecalInGameManager.DECAL_INGAME_MANAGER)
						{
							GenericMeshDecal actualDecal = DecalInGameManager.DECAL_INGAME_MANAGER.CreateNewMeshDecal (decalMaterial, contact.otherCollider.transform, contact.point, contact.normal, actualScaleMultiplier, rotationRange, false);

							actualDecal.SetDestroyable (true, destroyTime, destroySpeed);

							numberOfCreatedDecals++; 
						}
					}

					if (numberOfCreatedDecals >= maxDecalsCreatedPerCollision)
					{
						break;
					}
				}
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Update
		/// # Update the class
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Update ()
		{
			//print ("counterBetweenDecalCreations: " + counterBetweenDecalCreations);

			counterBetweenDecalCreations -= Time.deltaTime;

			if (counterBetweenDecalCreations <= 0)
			{
				counterBetweenDecalCreations = 0;
			}
		}
	}
}

using UnityEngine;
using System.Collections;

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: DirtyParticle
	/// # Similar to objects buy applied to particles, a particle which stain objects
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class DirtyParticle : MonoBehaviour
	{
		// public
		public Material decalMaterial = null;
		public float decalScale = .01f;

		// private
		ParticleCollisionEvent[] collisionEvents;
		ParticleSystem part;
		float timeBewteenDecals = 0;


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Start
		/// # Start the class
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Start ()
		{
			part = GetComponent<ParticleSystem> ();
			collisionEvents = new ParticleCollisionEvent[16];
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// OnParticleCollision
		/// # Hhandle collisions
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void OnParticleCollision (GameObject other)
		{
			//print ("other name: " + other.name);
			
			int safeLength = part.GetSafeCollisionEventSize ();
			if (collisionEvents.Length < safeLength)
				collisionEvents = new ParticleCollisionEvent[safeLength];
			
			int numCollisionEvents = part.GetCollisionEvents (other, collisionEvents);
			
			if (DecalInGameManager.DECAL_INGAME_MANAGER)
			{
				for (int i = 0; i < numCollisionEvents; i++)
				{
					if (timeBewteenDecals <= 0)
					{
						timeBewteenDecals = 0.1f;

                        //print ("Create decal");
#if UNITY_5_0
                        GenericMeshDecal actualDecal = DecalInGameManager.DECAL_INGAME_MANAGER.CreateNewMeshDecal(decalMaterial, collisionEvents[i].collider.transform, collisionEvents[i].intersection, collisionEvents[i].normal, 4, Vector2.zero, false);
#else
                        GenericMeshDecal actualDecal = DecalInGameManager.DECAL_INGAME_MANAGER.CreateNewMeshDecal (decalMaterial, collisionEvents [i].colliderComponent.transform, collisionEvents [i].intersection, collisionEvents [i].normal, 4, Vector2.zero, false);
#endif

						actualDecal.transform.localScale = decalScale * actualDecal.transform.localScale;
						actualDecal.SetDestroyable (true, 2, 0.2f);
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
			timeBewteenDecals -= Time.deltaTime;

			if (timeBewteenDecals <= 0)
			{
				timeBewteenDecals = 0;
			}
		}
	}
}
